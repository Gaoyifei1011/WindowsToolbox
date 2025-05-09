﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using WindowsTools.Services.Root;
using WindowsTools.WindowsAPI.ComTypes;
using WindowsTools.WindowsAPI.PInvoke.Ole32;

// 抑制 CA1806 警告
#pragma warning disable CA1806

namespace WindowsTools.Services.Controls.Download
{
    /// <summary>
    /// 下载服务
    /// </summary>
    public static class DeliveryOptimizationService
    {
        private static readonly string displayName = "WindowsTools";
        private static readonly object deliveryOptimizationLock = new();
        private static Guid CLSID_DeliveryOptimization = new("5B99FA76-721C-423C-ADAC-56D03C8A8007");

        private static Dictionary<Guid, (IDODownload doDownload, DODownloadStatusCallback doDownloadStatusCallback)> DeliveryOptimizationDict { get; } = [];

        public static event Action<Guid, string, string, string, double> DownloadCreated;

        public static event Action<Guid> DownloadContinued;

        public static event Action<Guid> DownloadPaused;

        public static event Action<Guid> DownloadDeleted;

        public static event Action<Guid, DO_DOWNLOAD_STATUS> DownloadProgressing;

        public static event Action<Guid, DO_DOWNLOAD_STATUS> DownloadCompleted;

        /// <summary>
        /// 获取下载任务的数量
        /// </summary>
        public static int GetDownloadCount()
        {
            int count = 0;
            lock (deliveryOptimizationLock)
            {
                count = DeliveryOptimizationDict.Count;
            }
            return count;
        }

        /// <summary>
        /// 使用下载链接创建下载
        /// </summary>
        public static void CreateDownload(string url, string saveFilePath)
        {
            Task.Factory.StartNew((param) =>
            {
                string downloadID = string.Empty;

                try
                {
                    IDOManager doManager = null;

                    // 创建 IDoManager
                    doManager = (IDOManager)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_DeliveryOptimization));
                    doManager.CreateDownload(out IDODownload doDownload);
                    IntPtr pInterface = Marshal.GetComInterfaceForObject<object, IDODownload>(doDownload);
                    Ole32Library.CoSetProxyBlanket(pInterface, uint.MaxValue, uint.MaxValue, unchecked((IntPtr)ulong.MaxValue), 0, 3, IntPtr.Zero, 32);
                    Marshal.Release(pInterface);
                    Marshal.FinalReleaseComObject(doManager);

                    // 添加下载信息
                    doDownload.SetProperty(DODownloadProperty.DODownloadProperty_DisplayName, displayName);
                    doDownload.SetProperty(DODownloadProperty.DODownloadProperty_Uri, url);
                    doDownload.SetProperty(DODownloadProperty.DODownloadProperty_LocalPath, saveFilePath);

                    DODownloadStatusCallback doDownloadStatusCallback = new();
                    doDownloadStatusCallback.StatusChanged += OnStatusChanged;
                    doDownload.SetProperty(DODownloadProperty.DODownloadProperty_CallbackInterface, new UnknownWrapper(doDownloadStatusCallback));
                    doDownload.SetProperty(DODownloadProperty.DODownloadProperty_ForegroundPriority, true);

                    doDownload.GetProperty(DODownloadProperty.DODownloadProperty_Id, out object id);
                    doDownload.GetProperty(DODownloadProperty.DODownloadProperty_TotalSizeBytes, out object size);
                    doDownloadStatusCallback.DownloadID = new(Convert.ToString(id));
                    DownloadCreated?.Invoke(doDownloadStatusCallback.DownloadID, Path.GetFileName(saveFilePath), saveFilePath, url, Convert.ToDouble(size));

                    lock (deliveryOptimizationLock)
                    {
                        if (!DeliveryOptimizationDict.ContainsKey(doDownloadStatusCallback.DownloadID))
                        {
                            DeliveryOptimizationDict.Add(doDownloadStatusCallback.DownloadID, ValueTuple.Create(doDownload, doDownloadStatusCallback));
                        }
                    }

                    doDownload.Start(IntPtr.Zero);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Create delivery optimization download failed", e);
                }
            }, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        /// <summary>
        /// 继续下载
        /// </summary>
        public static void ContinueDownload(Guid downloadID)
        {
            Task.Factory.StartNew((param) =>
            {
                try
                {
                    lock (deliveryOptimizationLock)
                    {
                        if (DeliveryOptimizationDict.TryGetValue(downloadID, out (IDODownload doDownload, DODownloadStatusCallback doDownloadStatusCallback) downloadValue))
                        {
                            int continueResult = downloadValue.doDownload.Start(IntPtr.Zero);

                            if (continueResult is 0)
                            {
                                DownloadContinued?.Invoke(downloadID);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Continue delivery optimization download failed", e);
                }
            }, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        /// <summary>
        /// 暂停下载
        /// </summary>
        public static void PauseDownload(Guid downloadID)
        {
            Task.Factory.StartNew((param) =>
            {
                try
                {
                    lock (deliveryOptimizationLock)
                    {
                        if (DeliveryOptimizationDict.TryGetValue(downloadID, out (IDODownload doDownload, DODownloadStatusCallback doDownloadStatusCallback) downloadValue))
                        {
                            int pauseResult = downloadValue.doDownload.Pause();

                            if (pauseResult is 0)
                            {
                                DownloadPaused?.Invoke(downloadID);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Pause delivery optimization download failed", e);
                }
            }, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        /// <summary>
        /// 删除下载
        /// </summary>
        public static void DeleteDownload(Guid downloadID)
        {
            Task.Factory.StartNew((param) =>
            {
                try
                {
                    lock (deliveryOptimizationLock)
                    {
                        if (DeliveryOptimizationDict.TryGetValue(downloadID, out (IDODownload doDownload, DODownloadStatusCallback doDownloadStatusCallback) downloadValue))
                        {
                            int deleteResult = downloadValue.doDownload.Abort();

                            if (deleteResult is 0)
                            {
                                downloadValue.doDownloadStatusCallback.StatusChanged -= OnStatusChanged;
                                DownloadDeleted?.Invoke(downloadID);
                                DeliveryOptimizationDict.Remove(downloadID);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Delete delivery optimization download failed", e);
                }
            }, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        /// <summary>
        /// 终止所有下载任务，仅用于应用关闭时
        /// </summary>
        public static void TerminateDownload()
        {
            Task.Factory.StartNew((param) =>
            {
                if (GetDownloadCount() > 0)
                {
                    lock (deliveryOptimizationLock)
                    {
                        try
                        {
                            foreach (KeyValuePair<Guid, (IDODownload doDownload, DODownloadStatusCallback doDownloadStatusCallback)> deliveryOptimizationKeyValue in DeliveryOptimizationDict)
                            {
                                deliveryOptimizationKeyValue.Value.doDownload.Abort();
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, "Abort all task failed", e);
                        }
                    }
                }
            }, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        /// <summary>
        /// 下载状态发生变化触发的事件
        /// </summary>
        private static void OnStatusChanged(DODownloadStatusCallback callback, IDODownload doDownload, DO_DOWNLOAD_STATUS status)
        {
            if (status.State is DODownloadState.DODownloadState_Transferring)
            {
                DownloadProgressing?.Invoke(callback.DownloadID, status);
            }
            else if (status.State is DODownloadState.DODownloadState_Transferred)
            {
                DownloadCompleted?.Invoke(callback.DownloadID, status);
                try
                {
                    callback.StatusChanged -= OnStatusChanged;
                    doDownload.Finalize();

                    lock (deliveryOptimizationLock)
                    {
                        if (DeliveryOptimizationDict.ContainsKey(callback.DownloadID))
                        {
                            DeliveryOptimizationDict.Remove(callback.DownloadID);
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Warning, "Finalize download task failed", e);
                }
            }
            else if (status.State is DODownloadState.DODownloadState_Paused)
            {
                DownloadPaused?.Invoke(callback.DownloadID);
            }
        }
    }
}
