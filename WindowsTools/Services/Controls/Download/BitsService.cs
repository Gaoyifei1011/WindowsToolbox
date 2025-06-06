﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using WindowsTools.Services.Root;
using WindowsTools.WindowsAPI.ComTypes;

namespace WindowsTools.Services.Controls.Download
{
    /// <summary>
    /// 后台智能传输服务
    /// </summary>
    public static class BitsService
    {
        private static readonly string displayName = "WindowsTools";
        private static readonly object bitsLock = new();
        private static Guid CLSID_BackgroundCopyManager = new("4991D34B-80A1-4291-83B6-3328366B9097");

        private static IBackgroundCopyManager backgroundCopyManager;

        private static Dictionary<Guid, (IBackgroundCopyJob backgroundCopyJob, BackgroundCopyCallback backgroundCopyCallback)> BitsDict { get; } = [];

        public static event Action<Guid, string, string, string, double> DownloadCreated;

        public static event Action<Guid> DownloadContinued;

        public static event Action<Guid> DownloadPaused;

        public static event Action<Guid> DownloadDeleted;

        public static event Action<Guid, BG_JOB_PROGRESS> DownloadProgressing;

        public static event Action<Guid, BG_JOB_PROGRESS> DownloadCompleted;

        /// <summary>
        /// 初始化后台智能传输服务
        /// </summary>
        public static void Initialize()
        {
            if (backgroundCopyManager is null)
            {
                Task.Factory.StartNew((param) =>
                {
                    try
                    {
                        backgroundCopyManager = (IBackgroundCopyManager)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_BackgroundCopyManager));
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, "Initialize background intelligent transfer service failed", e);
                    }
                }, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
            }
        }

        /// <summary>
        /// 获取下载任务的数量
        /// </summary>
        public static int GetDownloadCount()
        {
            int count = 0;
            lock (bitsLock)
            {
                count = BitsDict.Count;
            }
            return count;
        }

        /// <summary>
        /// 终止所有下载任务，仅用于应用关闭时
        /// </summary>
        public static void TerminateDownload()
        {
            Task.Factory.StartNew((param) =>
            {
                try
                {
                    if (GetDownloadCount() > 0)
                    {
                        lock (bitsLock)
                        {
                            try
                            {
                                foreach (KeyValuePair<Guid, (IBackgroundCopyJob backgroundCopyJob, BackgroundCopyCallback backgroundCopyCallback)> bitsKeyValue in BitsDict)
                                {
                                    bitsKeyValue.Value.backgroundCopyJob.Cancel();
                                }
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(EventLevel.Error, "Terminate all task failed", e);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Terminate background transfer intelligent service download all task failed", e);
                }
            }, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        /// <summary>
        /// 使用下载链接创建下载
        /// </summary>
        public static void CreateDownload(string url, string saveFilePath)
        {
            Task.Factory.StartNew((param) =>
            {
                try
                {
                    if (backgroundCopyManager is not null)
                    {
                        backgroundCopyManager.CreateJob(displayName, BG_JOB_TYPE.BG_JOB_TYPE_DOWNLOAD, out Guid downloadID, out IBackgroundCopyJob downloadJob);
                        downloadJob.AddFile(url, saveFilePath);
                        downloadJob.SetNotifyFlags(BG_JOB_NOTIFICATION_TYPE.BG_NOTIFY_FILE_RANGES_TRANSFERRED | BG_JOB_NOTIFICATION_TYPE.BG_NOTIFY_JOB_ERROR | BG_JOB_NOTIFICATION_TYPE.BG_NOTIFY_JOB_MODIFICATION);
                        BackgroundCopyCallback backgroundCopyCallback = new()
                        {
                            DownloadID = downloadID
                        };
                        backgroundCopyCallback.StatusChanged += OnStatusChanged;
                        downloadJob.SetNotifyInterface(new UnknownWrapper(backgroundCopyCallback).WrappedObject);

                        downloadJob.GetProgress(out BG_JOB_PROGRESS progress);
                        DownloadCreated?.Invoke(backgroundCopyCallback.DownloadID, Path.GetFileName(saveFilePath), saveFilePath, url, progress.BytesTotal is ulong.MaxValue ? 0 : progress.BytesTotal);

                        lock (bitsLock)
                        {
                            if (!BitsDict.ContainsKey(downloadID))
                            {
                                BitsDict.Add(downloadID, ValueTuple.Create(downloadJob, backgroundCopyCallback));
                            }
                        }

                        downloadJob.Resume();
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Create background intelligent transfer service download failed", e);
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
                    lock (bitsLock)
                    {
                        if (BitsDict.TryGetValue(downloadID, out (IBackgroundCopyJob backgroundCopyJob, BackgroundCopyCallback backgroundCopyCallback) downloadValue))
                        {
                            int continueResult = downloadValue.backgroundCopyJob.Resume();

                            if (continueResult is 0)
                            {
                                DownloadContinued?.Invoke(downloadID);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Continue background intelligent transfer service download failed", e);
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
                    lock (bitsLock)
                    {
                        if (BitsDict.TryGetValue(downloadID, out (IBackgroundCopyJob backgroundCopyJob, BackgroundCopyCallback backgroundCopyCallback) downloadValue))
                        {
                            int pauseResult = downloadValue.backgroundCopyJob.Suspend();

                            if (pauseResult is 0)
                            {
                                DownloadPaused?.Invoke(downloadID);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Pause background intelligent transfer service download failed", e);
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
                    lock (bitsLock)
                    {
                        if (BitsDict.TryGetValue(downloadID, out (IBackgroundCopyJob backgroundCopyJob, BackgroundCopyCallback backgroundCopyCallback) downloadValue))
                        {
                            int deleteResult = downloadValue.backgroundCopyJob.Cancel();

                            if (deleteResult is 0)
                            {
                                downloadValue.backgroundCopyCallback.StatusChanged -= OnStatusChanged;
                                DownloadDeleted?.Invoke(downloadID);
                                BitsDict.Remove(downloadID);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Delete background intelligent transfer service download failed", e);
                }
            }, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        /// <summary>
        /// 下载状态发生变化触发的事件
        /// </summary>
        private static void OnStatusChanged(BackgroundCopyCallback callback, IBackgroundCopyJob downloadJob, BG_JOB_STATE state)
        {
            if (state is BG_JOB_STATE.BG_JOB_STATE_TRANSFERRING)
            {
                downloadJob.GetProgress(out BG_JOB_PROGRESS progress);
                DownloadProgressing?.Invoke(callback.DownloadID, progress);
            }
            else if (state is BG_JOB_STATE.BG_JOB_STATE_TRANSFERRED)
            {
                downloadJob.GetProgress(out BG_JOB_PROGRESS progress);
                DownloadCompleted?.Invoke(callback.DownloadID, progress);

                try
                {
                    callback.StatusChanged -= OnStatusChanged;
                    downloadJob.Complete();

                    lock (bitsLock)
                    {
                        if (BitsDict.ContainsKey(callback.DownloadID))
                        {
                            BitsDict.Remove(callback.DownloadID);
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Warning, "Finalize background intelligent transfer service download task failed", e);
                }
            }
            else if (state is BG_JOB_STATE.BG_JOB_STATE_SUSPENDED)
            {
                DownloadPaused?.Invoke(callback.DownloadID);
            }
        }
    }
}
