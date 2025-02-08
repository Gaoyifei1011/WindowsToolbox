using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using WindowsTools.Extensions.DataType.Class;
using WindowsTools.Extensions.DataType.Enums;
using WindowsTools.Helpers.Controls;
using WindowsTools.Helpers.Root;
using WindowsTools.Models;
using WindowsTools.Services.Root;
using WindowsTools.UI.TeachingTips;
using WindowsTools.WindowsAPI.ComTypes;
using WUApiLib;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace WindowsTools.Views.Pages
{
    /// <summary>
    /// Windows 更新管理页面
    /// </summary>
    public sealed partial class UpdateManagerPage : Page, INotifyPropertyChanged
    {
        private readonly SynchronizationContext synchronizationContext = SynchronizationContext.Current;
        private readonly UpdateSession updateSession = new();
        private readonly UpdateServiceManager updateServiceManager = new();
        private readonly Dictionary<string, IDownloadJob> downloadJobDict = [];
        private readonly Dictionary<string, IInstallationJob> installationJobDict = [];
        private readonly Dictionary<string, IInstallationJob> uninstallationJobDict = [];
        private readonly Regex updateNumberRegex = new(@"KB(\d)*", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private bool isLoaded;
        private UpdateSearcher updateSearcher;

        private Microsoft.UI.Xaml.Controls.NavigationViewItem _selectedItem;

        public Microsoft.UI.Xaml.Controls.NavigationViewItem SelectedItem
        {
            get { return _selectedItem; }

            set
            {
                if (!Equals(_selectedItem, value))
                {
                    _selectedItem = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedItem)));
                }
            }
        }

        private string _windowsUpdateVersion;

        public string WindowsUpdateVersion
        {
            get { return _windowsUpdateVersion; }

            set
            {
                if (!Equals(_windowsUpdateVersion, value))
                {
                    _windowsUpdateVersion = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WindowsUpdateVersion)));
                }
            }
        }

        private string _wuapiDllVersion;

        public string WuapiDllVersion
        {
            get { return _wuapiDllVersion; }

            set
            {
                if (!Equals(_wuapiDllVersion, value))
                {
                    _wuapiDllVersion = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WuapiDllVersion)));
                }
            }
        }

        private bool _isCheckUpdateEnabled = true;

        public bool IsCheckUpdateEnabled
        {
            get { return _isCheckUpdateEnabled; }

            set
            {
                if (!Equals(_isCheckUpdateEnabled, value))
                {
                    _isCheckUpdateEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCheckUpdateEnabled)));
                }
            }
        }

        private bool _isChecking;

        public bool IsChecking
        {
            get { return _isChecking; }

            set
            {
                if (!Equals(_isChecking, value))
                {
                    _isChecking = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsChecking)));
                }
            }
        }

        private bool _isAvailableHideEnabled;

        private bool IsAvailableHideEnabled
        {
            get { return _isAvailableHideEnabled; }

            set
            {
                if (!Equals(_isAvailableHideEnabled, value))
                {
                    _isAvailableHideEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAvailableHideEnabled)));
                }
            }
        }

        private bool _isAvailableInstallEnabled;

        private bool IsAvailableInstallEnabled
        {
            get { return _isAvailableInstallEnabled; }

            set
            {
                if (!Equals(_isAvailableInstallEnabled, value))
                {
                    _isAvailableInstallEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAvailableInstallEnabled)));
                }
            }
        }

        private bool _isAvailableCancelInstallEnabled;

        private bool IsAvailableCancelInstallEnabled
        {
            get { return _isAvailableCancelInstallEnabled; }

            set
            {
                if (!Equals(_isAvailableCancelInstallEnabled, value))
                {
                    _isAvailableCancelInstallEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAvailableCancelInstallEnabled)));
                }
            }
        }

        private bool _isInstalledUninstallEnabled;

        private bool IsInstalledUninstallEnabled
        {
            get { return _isInstalledUninstallEnabled; }

            set
            {
                if (!Equals(_isInstalledUninstallEnabled, value))
                {
                    _isInstalledUninstallEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsInstalledUninstallEnabled)));
                }
            }
        }

        private bool _isInstalledCancelInstallEnabled;

        private bool IsInstalledCancelInstallEnabled
        {
            get { return _isInstalledCancelInstallEnabled; }

            set
            {
                if (!Equals(_isInstalledCancelInstallEnabled, value))
                {
                    _isInstalledCancelInstallEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsInstalledCancelInstallEnabled)));
                }
            }
        }

        private bool _isHiddenShowEnabled;

        private bool IsHiddenShowEnabled
        {
            get { return _isHiddenShowEnabled; }

            set
            {
                if (!Equals(_isHiddenShowEnabled, value))
                {
                    _isHiddenShowEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsHiddenShowEnabled)));
                }
            }
        }

        private bool _isExcludeDrivers;

        public bool IsExcludeDrivers
        {
            get { return _isExcludeDrivers; }

            set
            {
                if (!Equals(_isExcludeDrivers, value))
                {
                    _isExcludeDrivers = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsExcludeDrivers)));
                }
            }
        }

        private bool _isNeedRebootPrompt;

        public bool IsNeedRebootPrompt
        {
            get { return _isNeedRebootPrompt; }

            set
            {
                if (!Equals(_isNeedRebootPrompt, value))
                {
                    _isNeedRebootPrompt = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsNeedRebootPrompt)));
                }
            }
        }

        private bool _isIncludePotentiallySupersededUpdate;

        public bool IsIncludePotentiallySupersededUpdate
        {
            get { return _isIncludePotentiallySupersededUpdate; }

            set
            {
                if (!Equals(_isIncludePotentiallySupersededUpdate, value))
                {
                    _isIncludePotentiallySupersededUpdate = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsIncludePotentiallySupersededUpdate)));
                }
            }
        }

        private KeyValuePair<string, string> _selectedUpdateSource;

        public KeyValuePair<string, string> SelectedUpdateSource
        {
            get { return _selectedUpdateSource; }

            set
            {
                if (!Equals(_selectedUpdateSource, value))
                {
                    _selectedUpdateSource = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedUpdateSource)));
                }
            }
        }

        private List<KeyValuePair<string, string>> UpdateSourceList { get; } =
        [
            new KeyValuePair<string,string>("Microsoft Update", ResourceService.UpdateManagerResource.GetString("MicrosoftUpdate")),
            new KeyValuePair<string,string>("DCat Flighting Prod", ResourceService.UpdateManagerResource.GetString("DCatFlightingProd")),
            new KeyValuePair<string,string>("Windows Store(DCat Prod)", ResourceService.UpdateManagerResource.GetString("WindowsStore")),
            new KeyValuePair<string,string>("Windows Update", ResourceService.UpdateManagerResource.GetString("WindowsUpdate")),
        ];

        private ObservableCollection<UpdateModel> AvailableUpdateCollection { get; } = [];

        private ObservableCollection<UpdateModel> InstalledUpdateCollection { get; } = [];

        private ObservableCollection<UpdateModel> HiddenUpdateCollection { get; } = [];

        private ObservableCollection<UpdateModel> UpdateHistoryCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public UpdateManagerPage()
        {
            InitializeComponent();
            SelectedItem = UpdateManagerNavigationView.MenuItems[0] as Microsoft.UI.Xaml.Controls.NavigationViewItem;
            SelectedUpdateSource = UpdateSourceList[0];
        }

        #region 第一部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 可用更新：取消更新
        /// </summary>
        private void OnAvailableCancelInstallExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string updateID && !string.IsNullOrEmpty(updateID))
            {
                foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
                {
                    if (availableUpdateItem.UpdateID.Equals(updateID, StringComparison.OrdinalIgnoreCase) && availableUpdateItem.IsUpdating)
                    {
                        availableUpdateItem.UpdateProgress = ResourceService.UpdateManagerResource.GetString("UpdateCanceling");
                        availableUpdateItem.IsUpdateCanceled = true;
                        break;
                    }
                }

                IsAvailableHideEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating && !item.UpdateInformation.IsHidden);
                IsAvailableInstallEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating);
                IsAvailableCancelInstallEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && item.IsUpdating && !item.IsUpdateCanceled);

                Task.Run(() =>
                {
                    try
                    {
                        if (downloadJobDict.TryGetValue(updateID, out IDownloadJob downloadJob))
                        {
                            downloadJob.RequestAbort();
                            downloadJobDict.Remove(updateID);
                        }

                        if (installationJobDict.TryGetValue(updateID, out IInstallationJob installationJob))
                        {
                            installationJob.RequestAbort();
                            installationJobDict.Remove(updateID);
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, "Cancel install update failed", e);
                    }
                });
            }
        }

        /// <summary>
        /// 可用更新：修改可用更新项选中状态
        /// </summary>
        private void OnAvailableCheckClickExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter as UpdateModel is UpdateModel updateItem)
            {
                foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
                {
                    if (availableUpdateItem.UpdateID.Equals(updateItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                    {
                        availableUpdateItem.IsSelected = !availableUpdateItem.IsSelected;
                        break;
                    }
                }

                IsAvailableHideEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating && !item.UpdateInformation.IsHidden);
                IsAvailableInstallEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating);
                IsAvailableCancelInstallEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && item.IsUpdating && !item.IsUpdateCanceled);
            }
        }

        /// <summary>
        /// 可用更新：隐藏
        /// </summary>
        private async void OnAvailableHideExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is UpdateModel updateItem)
            {
                foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
                {
                    if (availableUpdateItem.UpdateID.Equals(updateItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                    {
                        availableUpdateItem.IsSelected = false;
                        break;
                    }
                }

                bool hideResult = await Task.Run(() =>
                {
                    bool result = false;

                    // 隐藏更新（只有 Power Users 管理组的管理员和成员才能设置此属性的值）
                    if (RuntimeHelper.IsElevated && !updateItem.IsUpdating)
                    {
                        try
                        {
                            foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
                            {
                                if (availableUpdateItem.UpdateID.Equals(updateItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                                {
                                    availableUpdateItem.UpdateInformation.IsHidden = true;
                                    availableUpdateItem.UpdateInformation.Update.IsHidden = true;
                                    break;
                                }
                            }

                            result = true;
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, "Hide updates modify hidden property failed", e);
                        }
                    }

                    return result;
                });

                // 隐藏成功时更新界面显示内容
                if (hideResult)
                {
                    try
                    {
                        for (int index = 0; index < AvailableUpdateCollection.Count; index++)
                        {
                            if (AvailableUpdateCollection[index].UpdateID.Equals(updateItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                            {
                                AvailableUpdateCollection[index].UpdateProgress = string.Empty;
                                AvailableUpdateCollection.RemoveAt(index);
                                break;
                            }
                        }

                        HiddenUpdateCollection.Add(updateItem);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, "Hide updates update UI failed", e);
                    }

                    IsAvailableHideEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating && !item.UpdateInformation.IsHidden);
                    IsAvailableInstallEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating);
                    IsAvailableCancelInstallEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && item.IsUpdating && !item.IsUpdateCanceled);
                    IsHiddenShowEnabled = HiddenUpdateCollection.Any(item => item.IsSelected && item.UpdateInformation.IsHidden);
                }
            }
        }

        /// <summary>
        /// 可用更新：安装
        /// </summary>
        private async void OnAvailableInstallExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is UpdateModel updateItem)
            {
                bool isUpdating = false;

                foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
                {
                    if (availableUpdateItem.UpdateID.Equals(updateItem.UpdateID, StringComparison.OrdinalIgnoreCase) && !availableUpdateItem.IsUpdating)
                    {
                        isUpdating = true;
                        availableUpdateItem.IsUpdating = true;
                        availableUpdateItem.IsUpdateCanceled = false;
                        availableUpdateItem.UpdatePercentage = 0;
                        availableUpdateItem.IsUpdatePreparing = true;
                        availableUpdateItem.UpdateProgress = ResourceService.UpdateManagerResource.GetString("UpdatePrepareInstalling");
                        break;
                    }
                }

                if (isUpdating)
                {
                    try
                    {
                        IsCheckUpdateEnabled = false;
                        bool updateResult = false;
                        UpdateCollection updateCollection = new() { updateItem.UpdateInformation.Update };

                        // 先下载更新
                        IDownloadResult downloadResult = await Task.Run(() =>
                        {
                            try
                            {
                                AutoResetEvent downloadEvent = new(false);

                                IDownloadJob downloadJob = null;
                                UpdateDownloader updateDownloader = updateSession.CreateUpdateDownloader();
                                updateDownloader.Updates = updateCollection;

                                DownloadProgressChangedCallback downloadProgressChangedCallback = new();
                                DownloadCompletedCallback downloadCompletedCallback = new();
                                downloadProgressChangedCallback.DownloadProgressChanged += (sender, args) => OnDownloadProgressChanged(sender, args, updateItem);
                                downloadCompletedCallback.DownloadCompleted += (sender, args) => downloadEvent.Set();
                                downloadJob = updateDownloader.BeginDownload(downloadProgressChangedCallback, downloadCompletedCallback, null);

                                if (!downloadJobDict.ContainsKey(updateItem.UpdateID))
                                {
                                    downloadJobDict.Add(updateItem.UpdateID, downloadJob);
                                }

                                downloadEvent.WaitOne();
                                downloadEvent.Dispose();
                                return updateDownloader.EndDownload(downloadJob);
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(EventLevel.Error, "Download updates failed", e);
                                return null;
                            }
                        });

                        if (downloadResult is not null)
                        {
                            // 更新下载成功
                            if (downloadResult.ResultCode is OperationResultCode.orcSucceeded)
                            {
                                foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
                                {
                                    if (availableUpdateItem.UpdateID.Equals(updateItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                                    {
                                        availableUpdateItem.UpdatePercentage = 50;
                                        availableUpdateItem.UpdateProgress = ResourceService.UpdateManagerResource.GetString("DownloadUpdateCompleted");
                                        break;
                                    }
                                }
                                updateResult = true;
                            }
                            // 更新下载已取消
                            else if (downloadResult.ResultCode is OperationResultCode.orcAborted)
                            {
                                foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
                                {
                                    if (availableUpdateItem.UpdateID.Equals(updateItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                                    {
                                        availableUpdateItem.IsUpdateCanceled = false;
                                        availableUpdateItem.IsUpdating = false;
                                        availableUpdateItem.UpdateProgress = ResourceService.UpdateManagerResource.GetString("DownloadUpdateCanceled");
                                    }
                                }
                            }
                            // 更新下载失败
                            else
                            {
                                foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
                                {
                                    if (availableUpdateItem.UpdateID.Equals(updateItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                                    {
                                        availableUpdateItem.IsUpdateCanceled = false;
                                        availableUpdateItem.IsUpdating = false;
                                        Exception exception = Marshal.GetExceptionForHR(downloadResult.HResult);

                                        availableUpdateItem.UpdateProgress = exception is not null
                                            ? string.Format(ResourceService.UpdateManagerResource.GetString("DownloadUpdateFailedWithInformation"), exception.Message)
                                            : string.Format(ResourceService.UpdateManagerResource.GetString("DownloadUpdateFailedWithCode"), downloadResult.HResult);
                                        break;
                                    }
                                }
                            }
                        }
                        // 更新下载失败
                        else
                        {
                            foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
                            {
                                if (availableUpdateItem.UpdateID.Equals(updateItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                                {
                                    availableUpdateItem.IsUpdateCanceled = false;
                                    availableUpdateItem.IsUpdating = false;
                                    Exception exception = Marshal.GetExceptionForHR(downloadResult.HResult);

                                    availableUpdateItem.UpdateProgress = exception is not null
                                        ? string.Format(ResourceService.UpdateManagerResource.GetString("DownloadUpdateFailedWithInformation"), exception.Message)
                                        : string.Format(ResourceService.UpdateManagerResource.GetString("DownloadUpdateFailedWithCode"), downloadResult.HResult);
                                    break;
                                }
                            }
                        }

                        // 移除更新下载任务
                        downloadJobDict.Remove(updateItem.UpdateID);

                        // 保证更新下载成功后再进行安装更新
                        if (updateResult)
                        {
                            // 后面再安装更新
                            IInstallationResult installationResult = await Task.Run(() =>
                            {
                                try
                                {
                                    AutoResetEvent installEvent = new(false);

                                    IInstallationJob installationJob = null;
                                    UpdateInstaller updateInstaller = updateSession.CreateUpdateInstaller() as UpdateInstaller;
                                    updateInstaller.Updates = updateCollection;
                                    updateInstaller.ForceQuiet = true;
                                    InstallationCompletedCallback installationCompletedCallback = new();
                                    InstallationProgressChangedCallback installationProgressChangedCallback = new();
                                    installationCompletedCallback.InstallationCompleted += (sender, args) => installEvent.Set();
                                    installationProgressChangedCallback.InstallationProgressChanged += (sender, args) => OnInstallationProgressChanged(sender, args, updateItem);
                                    installationJob = updateInstaller.BeginInstall(installationProgressChangedCallback, installationCompletedCallback, null);

                                    if (!installationJobDict.ContainsKey(updateItem.UpdateID))
                                    {
                                        installationJobDict.Add(updateItem.UpdateID, installationJob);
                                    }

                                    installEvent.WaitOne();
                                    installEvent.Dispose();
                                    return updateInstaller.EndInstall(installationJob);
                                }
                                catch (Exception e)
                                {
                                    LogService.WriteLog(EventLevel.Error, "Install updates failed", e);
                                    return null;
                                }
                            });

                            if (installationResult is not null)
                            {
                                // 更新安装成功
                                if (installationResult.ResultCode is OperationResultCode.orcSucceeded)
                                {
                                    foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
                                    {
                                        if (availableUpdateItem.UpdateID.Equals(updateItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                                        {
                                            availableUpdateItem.UpdatePercentage = 100;
                                            availableUpdateItem.IsUpdateCanceled = true;
                                            availableUpdateItem.UpdateProgress = installationResult.RebootRequired
                                                ? ResourceService.UpdateManagerResource.GetString("InstallUpdateCompletedNeedReboot")
                                                : ResourceService.UpdateManagerResource.GetString("InstallUpdateCompleted");

                                            if (installationResult.RebootRequired && !IsNeedRebootPrompt)
                                            {
                                                IsNeedRebootPrompt = true;
                                            }
                                            break;
                                        }
                                    }
                                }
                                // 更新安装已取消
                                else if (installationResult.ResultCode is OperationResultCode.orcAborted)
                                {
                                    foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
                                    {
                                        if (availableUpdateItem.UpdateID.Equals(updateItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                                        {
                                            availableUpdateItem.IsUpdateCanceled = false;
                                            availableUpdateItem.IsUpdating = false;
                                            availableUpdateItem.UpdateProgress = ResourceService.UpdateManagerResource.GetString("InstallUpdateCanceled");
                                        }
                                    }
                                }
                                // 更新安装失败
                                else
                                {
                                    foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
                                    {
                                        if (availableUpdateItem.UpdateID.Equals(updateItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                                        {
                                            availableUpdateItem.IsUpdateCanceled = false;
                                            availableUpdateItem.IsUpdating = false;
                                            Exception exception = Marshal.GetExceptionForHR(installationResult.HResult);

                                            availableUpdateItem.UpdateProgress = exception is not null
                                                ? string.Format(ResourceService.UpdateManagerResource.GetString("InstallUpdateFailedWithInformation"), exception.Message)
                                                : string.Format(ResourceService.UpdateManagerResource.GetString("InstallUpdateFailedWithCode"), installationResult.HResult);
                                            break;
                                        }
                                    }
                                }
                            }
                            // 更新安装失败
                            else
                            {
                                foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
                                {
                                    if (availableUpdateItem.UpdateID.Equals(updateItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                                    {
                                        availableUpdateItem.IsUpdateCanceled = false;
                                        availableUpdateItem.IsUpdating = false;
                                        Exception exception = Marshal.GetExceptionForHR(installationResult.HResult);

                                        availableUpdateItem.UpdateProgress = exception is not null
                                            ? string.Format(ResourceService.UpdateManagerResource.GetString("InstallUpdateFailedWithInformation"), exception.Message)
                                            : string.Format(ResourceService.UpdateManagerResource.GetString("InstallUpdateFailedWithCode"), installationResult.HResult);
                                        break;
                                    }
                                }
                            }

                            // 移除更新安装任务
                            installationJobDict.Remove(updateItem.UpdateID);
                        }

                        // 当前更新的下载和安装所有步骤都已完成
                        IsAvailableHideEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating && !item.UpdateInformation.IsHidden);
                        IsAvailableInstallEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating);
                        IsAvailableCancelInstallEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && item.IsUpdating && !item.IsUpdateCanceled);

                        // 所有更新下载、安装和卸载完成，恢复检查更新功能
                        if (downloadJobDict.Count is 0 && installationJobDict.Count is 0 && uninstallationJobDict.Count is 0)
                        {
                            IsCheckUpdateEnabled = true;
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, "Download and install updates failed", e);
                    }
                }
            }
        }

        /// <summary>
        /// 更新历史记录，复制更新描述信息
        /// </summary>
        private async void OnCopyInformationExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is UpdateModel updateItem)
            {
                string copyString = await Task.Run(() =>
                {
                    StringBuilder copyInformationBuilder = new();
                    copyInformationBuilder.Append(ResourceService.UpdateManagerResource.GetString("UpdateName"));
                    copyInformationBuilder.AppendLine(string.IsNullOrEmpty(updateItem.UpdateInformation.Title) ? ResourceService.UpdateManagerResource.GetString("Unknown") : updateItem.UpdateInformation.Title);
                    copyInformationBuilder.Append(ResourceService.UpdateManagerResource.GetString("Description"));
                    copyInformationBuilder.AppendLine(string.IsNullOrEmpty(updateItem.UpdateInformation.Description) ? ResourceService.UpdateManagerResource.GetString("Unknown") : updateItem.UpdateInformation.Description);
                    copyInformationBuilder.Append(ResourceService.UpdateManagerResource.GetString("IsBeta"));
                    copyInformationBuilder.AppendLine(updateItem.UpdateInformation.IsBeta ? ResourceService.UpdateManagerResource.GetString("Yes") : ResourceService.UpdateManagerResource.GetString("No"));
                    copyInformationBuilder.Append(ResourceService.UpdateManagerResource.GetString("IsMandatory"));
                    copyInformationBuilder.AppendLine(updateItem.UpdateInformation.IsMandatory ? ResourceService.UpdateManagerResource.GetString("Yes") : ResourceService.UpdateManagerResource.GetString("No"));
                    copyInformationBuilder.Append(ResourceService.UpdateManagerResource.GetString("MaxDownloadSize"));
                    copyInformationBuilder.AppendLine(FileSizeHelper.ConvertFileSizeToString(Convert.ToDouble(updateItem.UpdateInformation.Update.MaxDownloadSize)));
                    copyInformationBuilder.Append(ResourceService.UpdateManagerResource.GetString("MinDownloadSize"));
                    copyInformationBuilder.AppendLine(FileSizeHelper.ConvertFileSizeToString(Convert.ToDouble(updateItem.UpdateInformation.Update.MinDownloadSize)));
                    copyInformationBuilder.Append(ResourceService.UpdateManagerResource.GetString("MsrcSeverity"));
                    copyInformationBuilder.AppendLine(string.IsNullOrEmpty(updateItem.UpdateInformation.MsrcSeverity) ? ResourceService.UpdateManagerResource.GetString("Unknown") : updateItem.UpdateInformation.MsrcSeverity);
                    copyInformationBuilder.Append(ResourceService.UpdateManagerResource.GetString("RecommendedCpuSpeed"));
                    copyInformationBuilder.AppendLine(updateItem.UpdateInformation.RecommendedCpuSpeed.Equals(0) ? ResourceService.UpdateManagerResource.GetString("Unknown") : string.Format("{0} MHz", updateItem.UpdateInformation.RecommendedCpuSpeed));
                    copyInformationBuilder.Append(ResourceService.UpdateManagerResource.GetString("RecommendedHardDiskSpace"));
                    copyInformationBuilder.AppendLine(updateItem.UpdateInformation.RecommendedHardDiskSpace.Equals(0) ? ResourceService.UpdateManagerResource.GetString("Unknown") : string.Format("{0} MB", updateItem.UpdateInformation.RecommendedHardDiskSpace));
                    copyInformationBuilder.Append(ResourceService.UpdateManagerResource.GetString("RecommendedMemory"));
                    copyInformationBuilder.AppendLine(updateItem.UpdateInformation.RecommendedMemory.Equals(0) ? ResourceService.UpdateManagerResource.GetString("Unknown") : string.Format("{0} MB", updateItem.UpdateInformation.RecommendedMemory));
                    copyInformationBuilder.Append(ResourceService.UpdateManagerResource.GetString("ReleaseNotes"));
                    copyInformationBuilder.AppendLine(string.IsNullOrEmpty(updateItem.UpdateInformation.ReleaseNotes) ? ResourceService.UpdateManagerResource.GetString("Unknown") : updateItem.UpdateInformation.ReleaseNotes);
                    copyInformationBuilder.Append(ResourceService.UpdateManagerResource.GetString("SupportedUrl"));
                    copyInformationBuilder.AppendLine(updateItem.UpdateInformation.SupportURL);

                    if (updateItem.UpdateInformation.UpdateType is UpdateType.utDriver)
                    {
                        copyInformationBuilder.Append(ResourceService.UpdateManagerResource.GetString("DeviceProblemNumber"));
                        copyInformationBuilder.AppendLine(Convert.ToString(updateItem.DriverInformation.DeviceProblemNumber));
                        copyInformationBuilder.Append(ResourceService.UpdateManagerResource.GetString("DriverClass"));
                        copyInformationBuilder.AppendLine(string.IsNullOrEmpty(updateItem.DriverInformation.DriverClass) ? ResourceService.UpdateManagerResource.GetString("Unknown") : updateItem.DriverInformation.DriverClass);
                        copyInformationBuilder.Append(ResourceService.UpdateManagerResource.GetString("DriverHardwareID"));
                        copyInformationBuilder.AppendLine(string.IsNullOrEmpty(updateItem.DriverInformation.DriverHardwareID) ? ResourceService.UpdateManagerResource.GetString("Unknown") : updateItem.DriverInformation.DriverHardwareID);
                        copyInformationBuilder.Append(ResourceService.UpdateManagerResource.GetString("DriverManufacturer"));
                        copyInformationBuilder.AppendLine(string.IsNullOrEmpty(updateItem.DriverInformation.DriverManufacturer) ? ResourceService.UpdateManagerResource.GetString("Unknown") : updateItem.DriverInformation.DriverManufacturer);
                        copyInformationBuilder.Append(ResourceService.UpdateManagerResource.GetString("DriverModel"));
                        copyInformationBuilder.AppendLine(string.IsNullOrEmpty(updateItem.DriverInformation.DriverModel) ? ResourceService.UpdateManagerResource.GetString("Unknown") : updateItem.DriverInformation.DriverModel);
                        copyInformationBuilder.Append(ResourceService.UpdateManagerResource.GetString("DriverProvider"));
                        copyInformationBuilder.AppendLine(string.IsNullOrEmpty(updateItem.DriverInformation.DriverProvider) ? ResourceService.UpdateManagerResource.GetString("Unknown") : updateItem.DriverInformation.DriverProvider);
                        copyInformationBuilder.Append(ResourceService.UpdateManagerResource.GetString("DriverVerDate"));
                        copyInformationBuilder.AppendLine(updateItem.DriverInformation.DriverVerDate.ToString("yyyy/MM/dd"));
                    }

                    return copyInformationBuilder.ToString();
                });

                bool copyResult = CopyPasteHelper.CopyToClipboard(copyString);
                await TeachingTipHelper.ShowAsync(new DataCopyTip(DataCopyKind.UpdateInformation, copyResult));
            }
        }

        /// <summary>
        /// 隐藏更新：修改隐藏更新项选中状态
        /// </summary>
        private void OnHiddenCheckBoxClickExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter as UpdateModel is UpdateModel updateItem)
            {
                foreach (UpdateModel hiddenUpdateItem in HiddenUpdateCollection)
                {
                    if (hiddenUpdateItem.UpdateID.Equals(updateItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                    {
                        hiddenUpdateItem.IsSelected = !hiddenUpdateItem.IsSelected;
                        break;
                    }
                }

                IsHiddenShowEnabled = HiddenUpdateCollection.Any(item => item.IsSelected && item.UpdateInformation.IsHidden);
            }
        }

        /// <summary>
        /// 隐藏更新：显示
        /// </summary>
        private async void OnHiddenShowExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is UpdateModel updateItem)
            {
                foreach (UpdateModel hiddenUpdateItem in HiddenUpdateCollection)
                {
                    if (hiddenUpdateItem.UpdateID.Equals(updateItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                    {
                        hiddenUpdateItem.IsSelected = false;
                        break;
                    }
                }

                bool showResult = await Task.Run(() =>
                {
                    bool result = false;

                    // 显示更新（只有 Power Users 管理组的管理员和成员才能设置此属性的值）
                    if (RuntimeHelper.IsElevated && !updateItem.IsUpdating)
                    {
                        try
                        {
                            foreach (UpdateModel hiddenUpdateItem in HiddenUpdateCollection)
                            {
                                if (hiddenUpdateItem.UpdateID.Equals(updateItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                                {
                                    hiddenUpdateItem.UpdateInformation.IsHidden = false;
                                    hiddenUpdateItem.UpdateInformation.Update.IsHidden = false;
                                    break;
                                }
                            }

                            result = true;
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, "Show updates modify hidden property failed", e);
                        }
                    }

                    return result;
                });

                // 显示成功时更新界面显示内容
                if (showResult)
                {
                    try
                    {
                        for (int index = 0; index < HiddenUpdateCollection.Count; index++)
                        {
                            if (HiddenUpdateCollection[index].UpdateID.Equals(updateItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                            {
                                HiddenUpdateCollection[index].UpdateProgress = ResourceService.UpdateManagerResource.GetString("UpdateNotInstalled");
                                HiddenUpdateCollection.RemoveAt(index);
                                break;
                            }
                        }

                        AvailableUpdateCollection.Add(updateItem);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, "Hide updates update UI failed", e);
                    }

                    IsAvailableHideEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating && !item.UpdateInformation.IsHidden);
                    IsAvailableInstallEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating);
                    IsAvailableCancelInstallEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && item.IsUpdating && !item.IsUpdateCanceled);
                    IsHiddenShowEnabled = HiddenUpdateCollection.Any(item => item.IsSelected && item.UpdateInformation.IsHidden);
                }
            }
        }

        /// <summary>
        /// 已安装更新：取消卸载更新
        /// </summary>
        private void OnInstalledCancelUninstallExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string updateID && !string.IsNullOrEmpty(updateID))
            {
                foreach (UpdateModel installedUpdateItem in InstalledUpdateCollection)
                {
                    if (installedUpdateItem.UpdateID.Equals(updateID, StringComparison.OrdinalIgnoreCase) && installedUpdateItem.IsUpdating)
                    {
                        installedUpdateItem.UpdateProgress = ResourceService.UpdateManagerResource.GetString("UpdateCanceling");
                        installedUpdateItem.IsUpdateCanceled = true;
                        break;
                    }
                }

                IsInstalledUninstallEnabled = InstalledUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating && item.UpdateInformation.IsUninstallable);
                IsInstalledCancelInstallEnabled = InstalledUpdateCollection.Any(item => item.IsSelected && item.IsUpdating && !item.IsUpdateCanceled && item.UpdateInformation.IsUninstallable);

                Task.Run(() =>
                {
                    try
                    {
                        if (uninstallationJobDict.TryGetValue(updateID, out IInstallationJob installationJob))
                        {
                            installationJob.RequestAbort();
                            uninstallationJobDict.Remove(updateID);
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, "Cancel uninstall update failed", e);
                    }
                });
            }
        }

        /// <summary>
        /// 已安装更新：修改已安装更新项选中状态
        /// </summary>
        private void OnInstalledCheckBoxClickExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter as UpdateModel is UpdateModel updateItem)
            {
                foreach (UpdateModel installedUpdateItem in InstalledUpdateCollection)
                {
                    if (installedUpdateItem.UpdateID.Equals(updateItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                    {
                        installedUpdateItem.IsSelected = !installedUpdateItem.IsSelected;
                        break;
                    }
                }

                IsInstalledUninstallEnabled = InstalledUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating && item.UpdateInformation.IsUninstallable);
                IsInstalledCancelInstallEnabled = InstalledUpdateCollection.Any(item => item.IsSelected && item.IsUpdating && !item.IsUpdateCanceled && item.UpdateInformation.IsUninstallable);
            }
        }

        /// <summary>
        /// 已安装更新：卸载更新
        /// </summary>
        private async void OnInstalledUninstallExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is UpdateModel updateItem)
            {
                bool isUpdating = false;

                foreach (UpdateModel installedUpdateItem in InstalledUpdateCollection)
                {
                    if (installedUpdateItem.UpdateID.Equals(updateItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                    {
                        isUpdating = true;
                        installedUpdateItem.IsUpdating = true;
                        installedUpdateItem.IsUpdateCanceled = false;
                        installedUpdateItem.UpdatePercentage = 0;
                        installedUpdateItem.IsUpdatePreparing = true;
                        installedUpdateItem.UpdateProgress = ResourceService.UpdateManagerResource.GetString("UpdatePrepareUninstalling");
                    }
                }

                if (isUpdating)
                {
                    try
                    {
                        IsCheckUpdateEnabled = false;
                        UpdateCollection updateCollection = new() { updateItem.UpdateInformation.Update };

                        // 卸载更新
                        IInstallationResult installationResult = await Task.Run(() =>
                        {
                            try
                            {
                                AutoResetEvent uninstallEvent = new(false);

                                IInstallationJob installationJob = null;
                                UpdateInstaller updateInstaller = updateSession.CreateUpdateInstaller() as UpdateInstaller;
                                updateInstaller.Updates = updateCollection;
                                updateInstaller.ForceQuiet = true;
                                InstallationCompletedCallback installationCompletedCallback = new();
                                InstallationProgressChangedCallback installationProgressChangedCallback = new();
                                installationCompletedCallback.InstallationCompleted += (sender, args) => uninstallEvent.Set();
                                installationProgressChangedCallback.InstallationProgressChanged += (sender, args) => OnUninstallationProgressChanged(sender, args, updateItem);
                                installationJob = updateInstaller.BeginUninstall(installationProgressChangedCallback, installationCompletedCallback, null);

                                if (!uninstallationJobDict.ContainsKey(updateItem.UpdateID))
                                {
                                    uninstallationJobDict.Add(updateItem.UpdateID, installationJob);
                                }

                                uninstallEvent.WaitOne();
                                uninstallEvent.Dispose();
                                return updateInstaller.EndUninstall(installationJob);
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(EventLevel.Error, "Uninstall updates failed", e);
                                return null;
                            }
                        });

                        if (installationResult is not null)
                        {
                            // 更新卸载成功
                            if (installationResult.ResultCode is OperationResultCode.orcSucceeded)
                            {
                                foreach (UpdateModel installedUpdateItem in InstalledUpdateCollection)
                                {
                                    if (installedUpdateItem.UpdateID.Equals(updateItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                                    {
                                        installedUpdateItem.UpdatePercentage = 100;
                                        installedUpdateItem.IsUpdateCanceled = true;
                                        installedUpdateItem.UpdateProgress = installationResult.RebootRequired
                                                   ? ResourceService.UpdateManagerResource.GetString("UninstallUpdateCompletedNeedReboot")
                                                   : ResourceService.UpdateManagerResource.GetString("UninstallUpdateCompleted");

                                        if (installationResult.RebootRequired && !IsNeedRebootPrompt)
                                        {
                                            IsNeedRebootPrompt = true;
                                        }
                                        break;
                                    }
                                }
                            }
                            // 更新卸载已取消
                            else if (installationResult.ResultCode is OperationResultCode.orcAborted)
                            {
                                foreach (UpdateModel installedUpdateItem in InstalledUpdateCollection)
                                {
                                    if (installedUpdateItem.UpdateID.Equals(updateItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                                    {
                                        installedUpdateItem.IsUpdateCanceled = false;
                                        installedUpdateItem.IsUpdating = false;
                                        installedUpdateItem.UpdateProgress = ResourceService.UpdateManagerResource.GetString("UninstallUpdateCanceled");
                                    }
                                }
                            }
                            // 更新卸载失败
                            else
                            {
                                foreach (UpdateModel installedUpdateItem in InstalledUpdateCollection)
                                {
                                    if (installedUpdateItem.UpdateID.Equals(updateItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                                    {
                                        installedUpdateItem.IsUpdateCanceled = false;
                                        installedUpdateItem.IsUpdating = false;
                                        Exception exception = Marshal.GetExceptionForHR(installationResult.HResult);

                                        installedUpdateItem.UpdateProgress = exception is not null
                                            ? string.Format(ResourceService.UpdateManagerResource.GetString("UninstallUpdateFailedWithInformation"), exception.Message)
                                            : string.Format(ResourceService.UpdateManagerResource.GetString("UninstallUpdateFailedWithCode"), installationResult.HResult);
                                        break;
                                    }
                                }
                            }
                        }
                        // 更新卸载失败
                        else
                        {
                            foreach (UpdateModel installedUpdateItem in InstalledUpdateCollection)
                            {
                                if (installedUpdateItem.UpdateID.Equals(updateItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                                {
                                    installedUpdateItem.IsUpdateCanceled = false;
                                    installedUpdateItem.IsUpdating = false;
                                    Exception exception = Marshal.GetExceptionForHR(installationResult.HResult);

                                    installedUpdateItem.UpdateProgress = exception is not null
                                        ? string.Format(ResourceService.UpdateManagerResource.GetString("UninstallUpdateFailedWithInformation"), exception.Message)
                                        : string.Format(ResourceService.UpdateManagerResource.GetString("UninstallUpdateFailedWithCode"), installationResult.HResult);
                                    break;
                                }
                            }
                        }

                        // 移除更新卸载任务
                        uninstallationJobDict.Remove(updateItem.UpdateID);

                        IsInstalledUninstallEnabled = InstalledUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating && item.UpdateInformation.IsUninstallable);
                        IsInstalledCancelInstallEnabled = InstalledUpdateCollection.Any(item => item.IsSelected && item.IsUpdating && !item.IsUpdateCanceled && item.UpdateInformation.IsUninstallable);

                        // 所有更新下载、安装和卸载完成，恢复检查更新功能
                        if (downloadJobDict.Count is 0 && installationJobDict.Count is 0 && uninstallationJobDict.Count is 0)
                        {
                            IsCheckUpdateEnabled = true;
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, "Uninstall updates failed", e);
                    }
                }
            }
        }

        /// <summary>
        /// 了解详细信息
        /// </summary>
        private void OnLearnMoreExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string supportUrl && !string.IsNullOrEmpty(supportUrl))
            {
                Task.Run(() =>
                {
                    try
                    {
                        Process.Start(supportUrl);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, "Open support url failed", e);
                    }
                });
            }
        }

        /// <summary>
        /// 更新历史记录：打开更新对应的受支持的链接
        /// </summary>
        private void OnOpenSupportUrlExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string supportUrl && !string.IsNullOrEmpty(supportUrl))
            {
                Task.Run(() =>
                {
                    try
                    {
                        Process.Start(supportUrl);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, "Open support url failed", e);
                    }
                });
            }
        }

        /// <summary>
        /// 已安装更新：使用命令卸载更新
        /// </summary>
        private void OnUninstallWithCmdExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string title && !string.IsNullOrEmpty(title))
            {
                Task.Run(() =>
                {
                    try
                    {
                        Match matchResult = updateNumberRegex.Match(title);

                        if (matchResult is not null && matchResult.Success && matchResult.Value.Length > 2)
                        {
                            string kbNumbder = matchResult.Value.Substring(2);
                            string executeFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "wusa.exe");
                            ProcessStartInfo processStartInfo = new()
                            {
                                FileName = "wusa.exe",
                                Arguments = string.Format("/uninstall /kb:{0}", kbNumbder),
                                UseShellExecute = true
                            };

                            Process.Start(processStartInfo);
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, "Uninstall with cmd failed", e);
                    }
                });
            }
        }

        #endregion 第一部分：XamlUICommand 命令调用时挂载的事件

        #region 第二部分：Windows 更新管理页面——挂载的事件

        /// <summary>
        /// 更新页面初始化触发的事件
        /// </summary>
        private async void OnLoaded(object sender, RoutedEventArgs args)
        {
            if (!isLoaded)
            {
                isLoaded = true;

                if (RuntimeHelper.IsElevated)
                {
                    IsExcludeDrivers = await Task.Run(() =>
                    {
                        try
                        {
                            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate", true);
                            int returnValue = Convert.ToInt32(registryKey.GetValue("ExcludeWUDriversInQualityUpdate"));
                            return Convert.ToBoolean(returnValue);
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Warning, "Get exclude driver options failed", e);
                            return false;
                        }
                    });
                }
                else
                {
                    return;
                }

                // 获取更新版本信息
                string windowsUpdateVersion = string.Empty;
                string wuapiDllVersion = string.Empty;

                await Task.Run(() =>
                {
                    try
                    {
                        updateSession.ClientApplicationID = "WindowsTools:" + Guid.NewGuid().ToString();
                        updateSearcher = updateSession.CreateUpdateSearcher() as UpdateSearcher;
                        updateSearcher.IgnoreDownloadPriority = true;
                        WindowsUpdateAgentInfo windowsUpdateAgentInfo = new();
                        object apiMajorVersion = windowsUpdateAgentInfo.GetInfo("ApiMajorVersion");
                        object apiMinorVersion = windowsUpdateAgentInfo.GetInfo("ApiMinorVersion");
                        object productVersionString = windowsUpdateAgentInfo.GetInfo("ProductVersionString");
                        windowsUpdateVersion = string.Format("{0}.{1}", apiMajorVersion, apiMinorVersion);
                        wuapiDllVersion = productVersionString.ToString();
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, "Get windows update information failed", e);
                    }
                });

                WindowsUpdateVersion = windowsUpdateVersion;
                WuapiDllVersion = wuapiDllVersion;

                // 获取历史更新记录
                List<UpdateModel> updateHistoryList = await Task.Run(GetUpdateHistoryList);

                UpdateHistoryCollection.Clear();
                foreach (UpdateModel updateHistoryItem in updateHistoryList)
                {
                    UpdateHistoryCollection.Add(updateHistoryItem);
                }
            }
        }

        /// <summary>
        /// 检查更新
        /// </summary>
        private async void OnCheckUpdateClicked(object sender, RoutedEventArgs args)
        {
            IsCheckUpdateEnabled = false;
            IsChecking = true;
            await CheckUpdate();
        }

        /// <summary>
        /// 打开 Windows 更新
        /// </summary>
        private void OnWindowsUpdateClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("ms-settings:windowsupdate");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Open windows update settings failed", e);
                }
            });
        }

        /// <summary>
        /// 打开预览体验计划设置
        /// </summary>
        private void OnWIPSettingsClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("ms-settings:windowsinsider");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Open windows insider settings failed", e);
                }
            });
        }

        /// <summary>
        /// 查看 Windows 更新的版本信息
        /// </summary>
        private void OnWindowsUpdateInformationClicked(object sender, RoutedEventArgs args)
        {
            FlyoutBase.ShowAttachedFlyout(OthersButton);
        }

        /// <summary>
        /// 当菜单中的项收到交互（如单击或点击）时发生
        /// </summary>
        private void OnItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            if (args.InvokedItemContainer is Microsoft.UI.Xaml.Controls.NavigationViewItem navigationViewItem && !navigationViewItem.Equals(SelectedItem))
            {
                SelectedItem = navigationViewItem;
            }
        }

        /// <summary>
        /// 可用更新：全选
        /// </summary>
        private void OnAvailableSelectAllClicked(object sender, RoutedEventArgs args)
        {
            foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
            {
                availableUpdateItem.IsSelected = true;
            }

            IsAvailableHideEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating && !item.UpdateInformation.IsHidden);
            IsAvailableInstallEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating);
            IsAvailableCancelInstallEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && item.IsUpdating && !item.IsUpdateCanceled);
        }

        /// <summary>
        /// 可用更新：全部不选
        /// </summary>
        private void OnAvailableSelectNoneClicked(object sender, RoutedEventArgs args)
        {
            foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
            {
                availableUpdateItem.IsSelected = false;
            }

            IsAvailableHideEnabled = false;
            IsAvailableInstallEnabled = false;
            IsAvailableCancelInstallEnabled = false;
        }

        /// <summary>
        /// 可用更新：隐藏
        /// </summary>
        private async void OnAvailableHideClicked(object sender, RoutedEventArgs args)
        {
            List<UpdateModel> hideList = AvailableUpdateCollection.Where(item => item.IsSelected).ToList();

            foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
            {
                availableUpdateItem.IsSelected = false;
            }

            bool hideResult = await Task.Run(() =>
            {
                bool result = false;
                if (RuntimeHelper.IsElevated)
                {
                    foreach (UpdateModel hideItem in hideList)
                    {
                        foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
                        {
                            if (availableUpdateItem.UpdateID.Equals(hideItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                            {
                                if (!availableUpdateItem.IsUpdating)
                                {
                                    try
                                    {
                                        availableUpdateItem.UpdateInformation.IsHidden = true;
                                        availableUpdateItem.UpdateInformation.Update.IsHidden = true;
                                    }
                                    catch (Exception e)
                                    {
                                        LogService.WriteLog(EventLevel.Error, "Hide updates modify hidden property failed", e);
                                        continue;
                                    }
                                }

                                break;
                            }
                        }
                    }

                    result = true;
                }

                return result;
            });

            if (hideResult)
            {
                foreach (UpdateModel hideItem in hideList)
                {
                    try
                    {
                        if (hideItem.UpdateInformation.Update.IsHidden)
                        {
                            for (int index = 0; index < AvailableUpdateCollection.Count; index++)
                            {
                                if (AvailableUpdateCollection[index].UpdateID.Equals(hideItem.UpdateID))
                                {
                                    AvailableUpdateCollection[index].UpdateProgress = string.Empty;
                                    AvailableUpdateCollection.RemoveAt(index);
                                    break;
                                }
                            }

                            HiddenUpdateCollection.Add(hideItem);
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, "Hide updates update UI failed", e);
                    }
                }

                IsAvailableHideEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating && !item.UpdateInformation.IsHidden);
                IsAvailableInstallEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating);
                IsAvailableCancelInstallEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && item.IsUpdating && !item.IsUpdateCanceled);
                IsHiddenShowEnabled = HiddenUpdateCollection.Any(item => item.IsSelected && item.UpdateInformation.IsHidden);
            }
        }

        /// <summary>
        /// 可用更新：安装选定的更新
        /// </summary>
        private void OnAvailableInstallClicked(object sender, RoutedEventArgs args)
        {
            List<UpdateModel> installList = AvailableUpdateCollection.Where(item => item.IsSelected).ToList();

            foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
            {
                availableUpdateItem.IsSelected = false;
            }

            IsCheckUpdateEnabled = false;
            foreach (UpdateModel installItem in installList)
            {
                if (!installItem.IsUpdating)
                {
                    // 更新更新项的状态
                    foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
                    {
                        if (availableUpdateItem.UpdateID.Equals(installItem.UpdateID, StringComparison.OrdinalIgnoreCase) && !availableUpdateItem.IsUpdating)
                        {
                            availableUpdateItem.IsUpdating = true;
                            availableUpdateItem.IsUpdateCanceled = false;
                            availableUpdateItem.UpdatePercentage = 0;
                            availableUpdateItem.IsUpdatePreparing = true;
                            availableUpdateItem.UpdateProgress = ResourceService.UpdateManagerResource.GetString("UpdatePrepareInstalling");
                            break;
                        }
                    }

                    Task.Run(() =>
                    {
                        AutoResetEvent updateProgressEvent = new(false);
                        bool updateResult = false;
                        UpdateCollection updateCollection = new() { installItem.UpdateInformation.Update };

                        // 先下载更新
                        IDownloadResult downloadResult = null;

                        try
                        {
                            AutoResetEvent downloadEvent = new(false);

                            IDownloadJob downloadJob = null;
                            UpdateDownloader updateDownloader = updateSession.CreateUpdateDownloader();
                            updateDownloader.Updates = updateCollection;

                            DownloadProgressChangedCallback downloadProgressChangedCallback = new();
                            DownloadCompletedCallback downloadCompletedCallback = new();
                            downloadProgressChangedCallback.DownloadProgressChanged += (sender, args) => OnDownloadProgressChanged(sender, args, installItem);
                            downloadCompletedCallback.DownloadCompleted += (sender, args) => downloadEvent.Set();
                            downloadJob = updateDownloader.BeginDownload(downloadProgressChangedCallback, downloadCompletedCallback, null);

                            if (!downloadJobDict.ContainsKey(installItem.UpdateID))
                            {
                                downloadJobDict.Add(installItem.UpdateID, downloadJob);
                            }

                            downloadEvent.WaitOne();
                            downloadEvent.Dispose();
                            downloadResult = updateDownloader.EndDownload(downloadJob);
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, "Download updates failed", e);
                        }

                        synchronizationContext.Post(_ =>
                        {
                            if (downloadResult is not null)
                            {
                                // 更新下载成功
                                if (downloadResult.ResultCode is OperationResultCode.orcSucceeded)
                                {
                                    foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
                                    {
                                        if (availableUpdateItem.UpdateID.Equals(installItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                                        {
                                            availableUpdateItem.UpdatePercentage = 50;
                                            availableUpdateItem.UpdateProgress = ResourceService.UpdateManagerResource.GetString("DownloadUpdateCompleted");
                                            break;
                                        }
                                    }
                                    updateResult = true;
                                }
                                // 更新下载已取消
                                else if (downloadResult.ResultCode is OperationResultCode.orcAborted)
                                {
                                    foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
                                    {
                                        if (availableUpdateItem.UpdateID.Equals(installItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                                        {
                                            availableUpdateItem.IsUpdateCanceled = false;
                                            availableUpdateItem.IsUpdating = false;
                                            availableUpdateItem.UpdateProgress = ResourceService.UpdateManagerResource.GetString("DownloadUpdateCanceled");
                                        }
                                    }
                                }
                                // 更新下载失败
                                else
                                {
                                    foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
                                    {
                                        if (availableUpdateItem.UpdateID.Equals(installItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                                        {
                                            availableUpdateItem.IsUpdateCanceled = false;
                                            availableUpdateItem.IsUpdating = false;
                                            Exception exception = Marshal.GetExceptionForHR(downloadResult.HResult);

                                            availableUpdateItem.UpdateProgress = exception is not null
                                                ? string.Format(ResourceService.UpdateManagerResource.GetString("DownloadUpdateFailedWithInformation"), exception.Message)
                                                : string.Format(ResourceService.UpdateManagerResource.GetString("DownloadUpdateFailedWithCode"), downloadResult.HResult);
                                            break;
                                        }
                                    }
                                }
                            }
                            // 更新下载失败
                            else
                            {
                                foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
                                {
                                    if (availableUpdateItem.UpdateID.Equals(installItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                                    {
                                        availableUpdateItem.IsUpdateCanceled = false;
                                        availableUpdateItem.IsUpdating = false;
                                        Exception exception = Marshal.GetExceptionForHR(downloadResult.HResult);

                                        availableUpdateItem.UpdateProgress = exception is not null
                                            ? string.Format(ResourceService.UpdateManagerResource.GetString("DownloadUpdateFailedWithInformation"), exception.Message)
                                            : string.Format(ResourceService.UpdateManagerResource.GetString("DownloadUpdateFailedWithCode"), downloadResult.HResult);
                                        break;
                                    }
                                }
                            }

                            updateProgressEvent.Set();
                        }, null);

                        updateProgressEvent.WaitOne();

                        // 移除更新下载任务
                        downloadJobDict.Remove(installItem.UpdateID);

                        // 保证更新下载成功后再进行安装更新
                        if (updateResult)
                        {
                            IInstallationResult installationResult = null;

                            try
                            {
                                AutoResetEvent installEvent = new(false);

                                IInstallationJob installationJob = null;
                                UpdateInstaller updateInstaller = updateSession.CreateUpdateInstaller() as UpdateInstaller;
                                updateInstaller.Updates = updateCollection;
                                updateInstaller.ForceQuiet = true;
                                InstallationCompletedCallback installationCompletedCallback = new();
                                InstallationProgressChangedCallback installationProgressChangedCallback = new();
                                installationCompletedCallback.InstallationCompleted += (sender, args) => installEvent.Set();
                                installationProgressChangedCallback.InstallationProgressChanged += (sender, args) => OnInstallationProgressChanged(sender, args, installItem);
                                installationJob = updateInstaller.BeginInstall(installationProgressChangedCallback, installationCompletedCallback, null);

                                if (!installationJobDict.ContainsKey(installItem.UpdateID))
                                {
                                    installationJobDict.Add(installItem.UpdateID, installationJob);
                                }

                                installEvent.WaitOne();
                                installEvent.Dispose();
                                installationResult = updateInstaller.EndInstall(installationJob);
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(EventLevel.Error, "Install updates failed", e);
                            }

                            synchronizationContext.Post(_ =>
                            {
                                if (installationResult is not null)
                                {
                                    // 更新安装成功
                                    if (installationResult.ResultCode is OperationResultCode.orcSucceeded)
                                    {
                                        foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
                                        {
                                            if (availableUpdateItem.UpdateID.Equals(installItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                                            {
                                                availableUpdateItem.UpdatePercentage = 100;
                                                availableUpdateItem.IsUpdateCanceled = true;
                                                availableUpdateItem.UpdateProgress = installationResult.RebootRequired
                                                    ? ResourceService.UpdateManagerResource.GetString("InstallUpdateCompletedNeedReboot")
                                                    : ResourceService.UpdateManagerResource.GetString("InstallUpdateCompleted");

                                                if (installationResult.RebootRequired && !IsNeedRebootPrompt)
                                                {
                                                    IsNeedRebootPrompt = true;
                                                }
                                                break;
                                            }
                                        }
                                    }
                                    // 更新安装已取消
                                    else if (installationResult.ResultCode is OperationResultCode.orcAborted)
                                    {
                                        foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
                                        {
                                            if (availableUpdateItem.UpdateID.Equals(installItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                                            {
                                                availableUpdateItem.IsUpdateCanceled = false;
                                                availableUpdateItem.IsUpdating = false;
                                                availableUpdateItem.UpdateProgress = ResourceService.UpdateManagerResource.GetString("InstallUpdateCanceled");
                                            }
                                        }
                                    }
                                    // 更新安装失败
                                    else
                                    {
                                        foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
                                        {
                                            if (availableUpdateItem.UpdateID.Equals(installItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                                            {
                                                availableUpdateItem.IsUpdateCanceled = false;
                                                availableUpdateItem.IsUpdating = false;
                                                Exception exception = Marshal.GetExceptionForHR(installationResult.HResult);

                                                availableUpdateItem.UpdateProgress = exception is not null
                                                    ? string.Format(ResourceService.UpdateManagerResource.GetString("InstallUpdateFailedWithInformation"), exception.Message)
                                                    : string.Format(ResourceService.UpdateManagerResource.GetString("InstallUpdateFailedWithCode"), installationResult.HResult);
                                                break;
                                            }
                                        }
                                    }
                                }
                                // 更新安装失败
                                else
                                {
                                    foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
                                    {
                                        if (availableUpdateItem.UpdateID.Equals(installItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                                        {
                                            availableUpdateItem.IsUpdateCanceled = false;
                                            availableUpdateItem.IsUpdating = false;
                                            Exception exception = Marshal.GetExceptionForHR(installationResult.HResult);

                                            availableUpdateItem.UpdateProgress = exception is not null
                                                ? string.Format(ResourceService.UpdateManagerResource.GetString("InstallUpdateFailedWithInformation"), exception.Message)
                                                : string.Format(ResourceService.UpdateManagerResource.GetString("InstallUpdateFailedWithCode"), installationResult.HResult);
                                            break;
                                        }
                                    }
                                }

                                updateProgressEvent.Set();
                            }, null);

                            updateProgressEvent.WaitOne();
                            updateProgressEvent.Dispose();

                            // 移除更新安装任务
                            installationJobDict.Remove(installItem.UpdateID);
                        }

                        // 当前更新的下载和安装所有步骤都已完成
                        synchronizationContext.Post(_ =>
                        {
                            IsAvailableHideEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating && !item.UpdateInformation.IsHidden);
                            IsAvailableInstallEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating);
                            IsAvailableCancelInstallEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && item.IsUpdating && !item.IsUpdateCanceled);

                            // 所有更新下载、安装和卸载完成，恢复检查更新功能
                            if (downloadJobDict.Count is 0 && installationJobDict.Count is 0 && uninstallationJobDict.Count is 0)
                            {
                                IsCheckUpdateEnabled = true;
                            }
                        }, null);
                    });
                }
            }

            IsAvailableHideEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating && !item.UpdateInformation.IsHidden);
            IsAvailableInstallEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating);
            IsAvailableCancelInstallEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && item.IsUpdating && !item.IsUpdateCanceled);
        }

        /// <summary>
        /// 可用更新：取消安装选定的更新
        /// </summary>
        private void OnAvailableCancelInstallClicked(object sender, RoutedEventArgs args)
        {
            List<UpdateModel> cancelList = AvailableUpdateCollection.Where(item => item.IsSelected).ToList();

            foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
            {
                availableUpdateItem.IsSelected = false;
            }

            foreach (UpdateModel cancelItem in cancelList)
            {
                foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
                {
                    if (availableUpdateItem.UpdateID.Equals(cancelItem.UpdateID, StringComparison.OrdinalIgnoreCase) && availableUpdateItem.IsUpdating)
                    {
                        availableUpdateItem.UpdateProgress = ResourceService.UpdateManagerResource.GetString("UpdateCanceling");
                        availableUpdateItem.IsUpdateCanceled = true;
                        break;
                    }
                }

                Task.Run(() =>
                {
                    try
                    {
                        if (downloadJobDict.TryGetValue(cancelItem.UpdateID, out IDownloadJob downloadJob))
                        {
                            downloadJob.RequestAbort();
                            downloadJobDict.Remove(cancelItem.UpdateID);
                        }

                        if (installationJobDict.TryGetValue(cancelItem.UpdateID, out IInstallationJob installationJob))
                        {
                            installationJob.RequestAbort();
                            installationJobDict.Remove(cancelItem.UpdateID);
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, "Cancel install update failed", e);
                    }
                });
            }

            IsAvailableHideEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating && !item.UpdateInformation.IsHidden);
            IsAvailableInstallEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating);
            IsAvailableCancelInstallEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && item.IsUpdating && !item.IsUpdateCanceled);
        }

        /// <summary>
        /// 已安装更新：全选
        /// </summary>
        private void OnInstalledSelectAllClicked(object sender, RoutedEventArgs args)
        {
            foreach (UpdateModel installedUpdateItem in InstalledUpdateCollection)
            {
                installedUpdateItem.IsSelected = true;
            }

            IsInstalledUninstallEnabled = InstalledUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating && item.UpdateInformation.IsUninstallable);
            IsInstalledCancelInstallEnabled = InstalledUpdateCollection.Any(item => item.IsSelected && item.IsUpdating && !item.IsUpdateCanceled && item.UpdateInformation.IsUninstallable);
        }

        /// <summary>
        /// 已安装更新：全部不选
        /// </summary>
        private void OnInstalledSelectNoneClicked(object sender, RoutedEventArgs args)
        {
            foreach (UpdateModel installedUpdateItem in InstalledUpdateCollection)
            {
                installedUpdateItem.IsSelected = false;
            }

            IsInstalledUninstallEnabled = false;
            IsInstalledCancelInstallEnabled = false;
        }

        /// <summary>
        /// 已安装更新：卸载
        /// </summary>
        private void OnInstalledUninstallClicked(object sender, RoutedEventArgs args)
        {
            List<UpdateModel> uninstallList = InstalledUpdateCollection.Where(item => item.IsSelected).ToList();

            foreach (UpdateModel installedUpdateItem in InstalledUpdateCollection)
            {
                installedUpdateItem.IsSelected = false;
            }

            IsCheckUpdateEnabled = false;
            foreach (UpdateModel uninstallItem in uninstallList)
            {
                if (!uninstallItem.IsUpdating)
                {
                    // 更新卸载项的状态
                    foreach (UpdateModel installedUpdateItem in InstalledUpdateCollection)
                    {
                        if (installedUpdateItem.UpdateID.Equals(uninstallItem.UpdateID, StringComparison.OrdinalIgnoreCase) && !installedUpdateItem.IsUpdating)
                        {
                            installedUpdateItem.IsUpdating = true;
                            installedUpdateItem.IsUpdateCanceled = false;
                            installedUpdateItem.UpdatePercentage = 0;
                            installedUpdateItem.IsUpdatePreparing = true;
                            installedUpdateItem.UpdateProgress = ResourceService.UpdateManagerResource.GetString("UpdatePrepareUninstalling");
                            break;
                        }
                    }

                    Task.Run(() =>
                    {
                        AutoResetEvent updateProgressEvent = new(false);
                        UpdateCollection updateCollection = new() { uninstallItem.UpdateInformation.Update };

                        // 卸载更新
                        IInstallationResult installationResult = null;

                        try
                        {
                            AutoResetEvent uninstallEvent = new(false);

                            IInstallationJob installationJob = null;
                            UpdateInstaller updateInstaller = updateSession.CreateUpdateInstaller() as UpdateInstaller;
                            updateInstaller.Updates = updateCollection;
                            updateInstaller.ForceQuiet = true;
                            InstallationCompletedCallback installationCompletedCallback = new();
                            InstallationProgressChangedCallback installationProgressChangedCallback = new();
                            installationCompletedCallback.InstallationCompleted += (sender, args) => uninstallEvent.Set();
                            installationProgressChangedCallback.InstallationProgressChanged += (sender, args) => OnUninstallationProgressChanged(sender, args, uninstallItem);
                            installationJob = updateInstaller.BeginUninstall(installationProgressChangedCallback, installationCompletedCallback, null);

                            if (!uninstallationJobDict.ContainsKey(uninstallItem.UpdateID))
                            {
                                uninstallationJobDict.Add(uninstallItem.UpdateID, installationJob);
                            }

                            uninstallEvent.WaitOne();
                            uninstallEvent.Dispose();
                            installationResult = updateInstaller.EndUninstall(installationJob);
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, "Uninstall updates failed", e);
                        }

                        synchronizationContext.Post(_ =>
                        {
                            if (installationResult is not null)
                            {
                                // 更新卸载成功
                                if (installationResult.ResultCode is OperationResultCode.orcSucceeded)
                                {
                                    foreach (UpdateModel installedUpdateItem in InstalledUpdateCollection)
                                    {
                                        if (installedUpdateItem.UpdateID.Equals(uninstallItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                                        {
                                            installedUpdateItem.UpdatePercentage = 100;
                                            installedUpdateItem.IsUpdateCanceled = true;
                                            installedUpdateItem.UpdateProgress = installationResult.RebootRequired
                                                  ? ResourceService.UpdateManagerResource.GetString("UninstallUpdateCompletedNeedReboot")
                                                  : ResourceService.UpdateManagerResource.GetString("UninstallUpdateCompleted");

                                            if (installationResult.RebootRequired && !IsNeedRebootPrompt)
                                            {
                                                IsNeedRebootPrompt = true;
                                            }
                                            break;
                                        }
                                    }
                                }
                                // 更新卸载已取消
                                else if (installationResult.ResultCode is OperationResultCode.orcAborted)
                                {
                                    foreach (UpdateModel installedUpdateItem in InstalledUpdateCollection)
                                    {
                                        if (installedUpdateItem.UpdateID.Equals(uninstallItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                                        {
                                            installedUpdateItem.IsUpdateCanceled = false;
                                            installedUpdateItem.IsUpdating = false;
                                            installedUpdateItem.UpdateProgress = ResourceService.UpdateManagerResource.GetString("UninstallUpdateCanceled");
                                        }
                                    }
                                }
                                // 更新安装失败
                                else
                                {
                                    foreach (UpdateModel installedUpdateItem in InstalledUpdateCollection)
                                    {
                                        if (installedUpdateItem.UpdateID.Equals(uninstallItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                                        {
                                            installedUpdateItem.IsUpdateCanceled = false;
                                            installedUpdateItem.IsUpdating = false;
                                            Exception exception = Marshal.GetExceptionForHR(installationResult.HResult);

                                            installedUpdateItem.UpdateProgress = exception is not null
                                                ? string.Format(ResourceService.UpdateManagerResource.GetString("UninstallUpdateFailedWithInformation"), exception.Message)
                                                : string.Format(ResourceService.UpdateManagerResource.GetString("UninstallUpdateFailedWithCode"), installationResult.HResult);
                                            break;
                                        }
                                    }
                                }
                            }
                            // 更新安装失败
                            else
                            {
                                foreach (UpdateModel installedUpdateItem in InstalledUpdateCollection)
                                {
                                    if (installedUpdateItem.UpdateID.Equals(uninstallItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                                    {
                                        installedUpdateItem.IsUpdateCanceled = false;
                                        installedUpdateItem.IsUpdating = false;
                                        Exception exception = Marshal.GetExceptionForHR(installationResult.HResult);

                                        installedUpdateItem.UpdateProgress = exception is not null
                                            ? string.Format(ResourceService.UpdateManagerResource.GetString("UninstallUpdateFailedWithInformation"), exception.Message)
                                            : string.Format(ResourceService.UpdateManagerResource.GetString("UninstallUpdateFailedWithCode"), installationResult.HResult);
                                        break;
                                    }
                                }
                            }

                            updateProgressEvent.Set();
                        }, null);

                        updateProgressEvent.WaitOne();
                        updateProgressEvent.Dispose();

                        // 移除更新卸载任务
                        uninstallationJobDict.Remove(uninstallItem.UpdateID);

                        // 当前更新的卸载所有步骤都已完成
                        synchronizationContext.Post(_ =>
                        {
                            IsInstalledUninstallEnabled = InstalledUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating && item.UpdateInformation.IsUninstallable);
                            IsInstalledCancelInstallEnabled = InstalledUpdateCollection.Any(item => item.IsSelected && item.IsUpdating && !item.IsUpdateCanceled && item.UpdateInformation.IsUninstallable);

                            // 所有更新下载、安装和卸载完成，恢复检查更新功能
                            if (downloadJobDict.Count is 0 && installationJobDict.Count is 0 && uninstallationJobDict.Count is 0)
                            {
                                IsCheckUpdateEnabled = true;
                            }
                        }, null);
                    });
                }
            }

            IsInstalledUninstallEnabled = InstalledUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating && item.UpdateInformation.IsUninstallable);
            IsInstalledCancelInstallEnabled = InstalledUpdateCollection.Any(item => item.IsSelected && item.IsUpdating && !item.IsUpdateCanceled && item.UpdateInformation.IsUninstallable);
        }

        /// <summary>
        /// 已安装更新：取消卸载更新
        /// </summary>
        private void OnInstalledCancelUninstallClicked(object sender, RoutedEventArgs args)
        {
            List<UpdateModel> cancelList = InstalledUpdateCollection.Where(item => item.IsSelected).ToList();

            foreach (UpdateModel installedUpdateItem in InstalledUpdateCollection)
            {
                installedUpdateItem.IsSelected = false;
            }

            foreach (UpdateModel cancelItem in cancelList)
            {
                foreach (UpdateModel installedUpdateItem in InstalledUpdateCollection)
                {
                    if (installedUpdateItem.UpdateID.Equals(cancelItem.UpdateID, StringComparison.OrdinalIgnoreCase) && installedUpdateItem.IsUpdating)
                    {
                        installedUpdateItem.UpdateProgress = ResourceService.UpdateManagerResource.GetString("UpdateCanceling");
                        installedUpdateItem.IsUpdateCanceled = true;
                        break;
                    }
                }

                Task.Run(() =>
                {
                    try
                    {
                        if (uninstallationJobDict.TryGetValue(cancelItem.UpdateID, out IInstallationJob installationJob))
                        {
                            installationJob.RequestAbort();
                            uninstallationJobDict.Remove(cancelItem.UpdateID);
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, "Cancel uninstall update failed", e);
                    }
                });
            }

            IsInstalledUninstallEnabled = InstalledUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating && item.UpdateInformation.IsUninstallable);
            IsInstalledCancelInstallEnabled = InstalledUpdateCollection.Any(item => item.IsSelected && item.IsUpdating && !item.IsUpdateCanceled && item.UpdateInformation.IsUninstallable);
        }

        /// <summary>
        /// 隐藏更新：全选
        /// </summary>
        private void OnHiddenSelectAllClicked(object sender, RoutedEventArgs args)
        {
            foreach (UpdateModel hiddenUpdateItem in HiddenUpdateCollection)
            {
                hiddenUpdateItem.IsSelected = true;
            }

            IsHiddenShowEnabled = HiddenUpdateCollection.Any(item => item.IsSelected && item.UpdateInformation.IsHidden);
        }

        /// <summary>
        /// 隐藏更新：全部不选
        /// </summary>
        private void OnHiddenSelectNoneClicked(object sender, RoutedEventArgs args)
        {
            foreach (UpdateModel hiddenUpdateItem in HiddenUpdateCollection)
            {
                hiddenUpdateItem.IsSelected = false;
            }

            IsHiddenShowEnabled = false;
        }

        /// <summary>
        /// 隐藏更新：显示
        /// </summary>
        private async void OnHiddenShowClicked(object sender, RoutedEventArgs args)
        {
            List<UpdateModel> showList = HiddenUpdateCollection.Where(item => item.IsSelected).ToList();

            foreach (UpdateModel hiddenUpdateItem in HiddenUpdateCollection)
            {
                hiddenUpdateItem.IsSelected = false;
            }

            bool showResult = await Task.Run(() =>
            {
                bool result = false;
                if (RuntimeHelper.IsElevated)
                {
                    foreach (UpdateModel showItem in showList)
                    {
                        foreach (UpdateModel hiddenUpdateItem in HiddenUpdateCollection)
                        {
                            if (hiddenUpdateItem.UpdateID.Equals(showItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                            {
                                if (!showItem.IsUpdating)
                                {
                                    try
                                    {
                                        hiddenUpdateItem.UpdateInformation.IsHidden = false;
                                        hiddenUpdateItem.UpdateInformation.Update.IsHidden = false;
                                    }
                                    catch (Exception e)
                                    {
                                        LogService.WriteLog(EventLevel.Error, "Show updates modify hidden property failed", e);
                                        continue;
                                    }
                                }

                                break;
                            }
                        }
                    }

                    result = true;
                }

                return result;
            });

            if (showResult)
            {
                foreach (UpdateModel showItem in showList)
                {
                    try
                    {
                        if (!showItem.UpdateInformation.Update.IsHidden)
                        {
                            for (int index = 0; index < HiddenUpdateCollection.Count; index++)
                            {
                                if (HiddenUpdateCollection[index].UpdateID.Equals(showItem.UpdateID))
                                {
                                    HiddenUpdateCollection[index].UpdateProgress = ResourceService.UpdateManagerResource.GetString("UpdateNotInstalled");
                                    HiddenUpdateCollection.RemoveAt(index);
                                    break;
                                }
                            }

                            AvailableUpdateCollection.Add(showItem);
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, "Show updates update UI failed", e);
                    }
                }

                IsAvailableHideEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating && !item.UpdateInformation.IsHidden);
                IsAvailableInstallEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating);
                IsAvailableCancelInstallEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && item.IsUpdating && !item.IsUpdateCanceled);
                IsHiddenShowEnabled = HiddenUpdateCollection.Any(item => item.IsSelected && item.UpdateInformation.IsHidden);
            }
        }

        /// <summary>
        /// 重启以完成后续更新的安装或卸载
        /// </summary>
        private void OnRebootClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                ShutdownHelper.Restart(ResourceService.UpdateManagerResource.GetString("RestartPC"), TimeSpan.FromSeconds(120));
            });
        }

        /// <summary>
        /// 点击重新更新提示栏关闭按钮关闭通知显示
        /// </summary>
        private void OnHideNeedRebootPromptClicked(object sender, RoutedEventArgs args)
        {
            IsNeedRebootPrompt = false;
        }

        /// <summary>
        /// 搜索结果中是否包含被取代的更新
        /// </summary>
        private void OnIncludePotentiallySupersededUpdateToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                IsIncludePotentiallySupersededUpdate = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 设置更新源
        /// </summary>
        private void OnUpdateSourceSelectClicked(object sender, RoutedEventArgs args)
        {
            if (sender is MenuFlyoutItem menuFlyoutItem && menuFlyoutItem.Tag is not null)
            {
                SelectedUpdateSource = UpdateSourceList[Convert.ToInt32(menuFlyoutItem.Tag)];
            }
        }

        /// <summary>
        /// Windows 更新不包括驱动程序
        /// </summary>
        private async void OnExcludeDriversToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                bool value = toggleSwitch.IsOn;
                IsExcludeDrivers = await Task.Run(() =>
                {
                    RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate", "ExcludeWUDriversInQualityUpdate", value);
                    return RegistryHelper.ReadRegistryKey<bool>(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate", "ExcludeWUDriversInQualityUpdate");
                });
            }
        }

        /// <summary>
        /// 清除历史更新记录
        /// </summary>
        private void OnCleanUpdateHistoryClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                ProcessStartInfo processStartInfo = new()
                {
                    FileName = "PowerShell.exe",
                    Arguments = string.Format(@"-windowstyle hidden -command ""Start-Process cmd -ArgumentList '/s,/c,net stop usosvc & net stop wuauserv & del %SystemRoot%\SoftwareDistribution\DataStore\Logs\edb.log & del /f /q %ALLUSERSPROFILE%\USOPrivate\UpdateStore\* & net start usosvc & net start wuauserv & UsoClient.exe RefreshSettings' -Verb runAs""")
                };
                Process.Start(processStartInfo);
            });
        }

        /// <summary>
        /// 更改设备的预览计划频道
        /// </summary>
        private void OnPreviewChannelSelectClicked(object sender, RoutedEventArgs args)
        {
            if (sender is MenuFlyoutItem menuFlyoutItem && menuFlyoutItem.Tag is string previewChannel && !string.IsNullOrEmpty(previewChannel))
            {
                if (previewChannel.Equals("ExitPreviewChannel", StringComparison.OrdinalIgnoreCase))
                {
                    ExitCustomPreviewChannel();
                }
                else
                {
                    EnterCustomPreviewChannel(previewChannel);
                }
            }
        }

        #endregion 第二部分：Windows 更新管理页面——挂载的事件

        #region 第三部分：自定义事件

        /// <summary>
        /// 下载更新中，进度变化时更新 UI 下载进度
        /// </summary>
        private void OnDownloadProgressChanged(object sender, EventArgs args, UpdateModel updateItem)
        {
            IDownloadProgress downloadProgress = (sender as DownloadProgressChangedCallback).CallbackArgs.Progress;
            double percentage = downloadProgress.CurrentUpdatePercentComplete;

            // 初始化当前更新的下载
            if (downloadProgress.CurrentUpdateDownloadPhase is DownloadPhase.dphInitializing)
            {
                synchronizationContext.Post(_ =>
                {
                    foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
                    {
                        if (availableUpdateItem.UpdateID.Equals(updateItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                        {
                            availableUpdateItem.IsUpdatePreparing = false;
                            availableUpdateItem.UpdatePercentage = percentage / 2;
                            availableUpdateItem.UpdateProgress = string.Format(ResourceService.UpdateManagerResource.GetString("DownloadUpdateProgressInitializing"), percentage);
                            break;
                        }
                    }
                }, null);
            }
            // 下载当前更新
            else if (downloadProgress.CurrentUpdateDownloadPhase is DownloadPhase.dphDownloading)
            {
                synchronizationContext.Post(_ =>
                {
                    foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
                    {
                        if (availableUpdateItem.UpdateID.Equals(updateItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                        {
                            availableUpdateItem.IsUpdatePreparing = false;
                            availableUpdateItem.UpdatePercentage = percentage / 2;
                            availableUpdateItem.UpdateProgress = string.Format(ResourceService.UpdateManagerResource.GetString("DownloadUpdateProgressDownloading"), percentage);
                            break;
                        }
                    }
                }, null);
            }
            else if (downloadProgress.CurrentUpdateDownloadPhase is DownloadPhase.dphVerifying)
            {
                synchronizationContext.Post(_ =>
                {
                    foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
                    {
                        if (availableUpdateItem.UpdateID.Equals(updateItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                        {
                            availableUpdateItem.IsUpdatePreparing = false;
                            availableUpdateItem.UpdatePercentage = percentage / 2;
                            availableUpdateItem.UpdateProgress = string.Format(ResourceService.UpdateManagerResource.GetString("DownloadUpdateProgressVerifying"), percentage);
                            break;
                        }
                    }
                }, null);
            }
        }

        /// <summary>
        /// 安装更新中，进度变化时更改 UI 卸载进度
        /// </summary>
        private void OnInstallationProgressChanged(object sender, EventArgs args, UpdateModel updateItem)
        {
            IInstallationProgress installationProgress = (sender as InstallationProgressChangedCallback).CallbackArgs.Progress;
            double percentage = installationProgress.CurrentUpdatePercentComplete;

            synchronizationContext.Post(_ =>
            {
                foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
                {
                    if (availableUpdateItem.UpdateID.Equals(updateItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                    {
                        availableUpdateItem.UpdatePercentage = 50 + percentage / 2;
                        availableUpdateItem.UpdateProgress = string.Format(ResourceService.UpdateManagerResource.GetString("InstallUpdateProgress"), percentage);
                        break;
                    }
                }
            }, null);
        }

        /// <summary>
        /// 卸载更新中，进度变化时更改 UI 卸载进度
        /// </summary>
        private void OnUninstallationProgressChanged(object sender, EventArgs args, UpdateModel updateItem)
        {
            IInstallationProgress installationProgress = (sender as InstallationProgressChangedCallback).CallbackArgs.Progress;
            double percentage = installationProgress.CurrentUpdatePercentComplete;

            synchronizationContext.Post(_ =>
            {
                foreach (UpdateModel installedUpdateItem in InstalledUpdateCollection)
                {
                    if (installedUpdateItem.UpdateID.Equals(updateItem.UpdateID))
                    {
                        installedUpdateItem.UpdatePercentage = percentage;
                        installedUpdateItem.UpdateProgress = string.Format(ResourceService.UpdateManagerResource.GetString("UninstallUpdateProgress"), percentage);
                        break;
                    }
                }
            }, null);
        }

        #endregion 第三部分：自定义事件

        /// <summary>
        /// 检查更新
        /// </summary>
        private async Task CheckUpdate()
        {
            List<UpdateModel> availableUpdateList = [];
            List<UpdateModel> installedUpdateList = [];
            List<UpdateModel> hiddenUpdateList = [];
            List<UpdateModel> updateHistoryList = [];

            bool searchResult = await Task.Run(() =>
            {
                bool result = false;

                // 设置搜索结果中是否接收已替代的更新
                updateSearcher.IncludePotentiallySupersededUpdates = IsIncludePotentiallySupersededUpdate;
                updateSearcher.ServerSelection = ServerSelection.ssDefault;
                updateSearcher.IgnoreDownloadPriority = true;

                // 设置更新源
                foreach (IUpdateService2 updateService in updateServiceManager.Services)
                {
                    if (updateService.Name.Equals(SelectedUpdateSource.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        updateSearcher.ServiceID = updateService.ServiceID;
                        break;
                    }
                }

                try
                {
                    AutoResetEvent searchEvent = new(false);
                    string query = "(IsInstalled = 0 and IsHidden = 0 and DeploymentAction=*) or (IsInstalled = 1 and IsHidden = 0 and DeploymentAction=*) or (IsHidden = 1 and DeploymentAction=*)";
                    SearchCompletedCallback searchCompletedCallback = new();
                    searchCompletedCallback.SearchCompleted += (sender, args) => searchEvent.Set();
                    ISearchJob searchJob = updateSearcher.BeginSearch(query, searchCompletedCallback, null);
                    searchEvent.WaitOne();
                    searchEvent.Dispose();
                    ISearchResult searchResult = updateSearcher.EndSearch(searchJob);

                    // 搜索更新内容
                    if (searchResult is not null)
                    {
                        // 读取已搜索到的更新
                        foreach (IUpdate5 update in searchResult.Updates)
                        {
                            UpdateInformation updateInformation = new()
                            {
                                Update = update,
                                Description = update.Description,
                                EulaText = update.EulaText,
                                IsBeta = update.IsBeta,
                                IsHidden = update.IsHidden,
                                IsInstalled = update.IsInstalled,
                                IsMandatory = update.IsMandatory,
                                IsUninstallable = update.IsUninstallable,
                                MaxDownloadSize = update.MaxDownloadSize,
                                MinDownloadSize = update.MinDownloadSize,
                                MsrcSeverity = update.MsrcSeverity,
                                RecommendedCpuSpeed = update.RecommendedCpuSpeed,
                                RecommendedHardDiskSpace = update.RecommendedHardDiskSpace,
                                RecommendedMemory = update.RecommendedMemory,
                                RebootRequired = update.RebootRequired,
                                ReleaseNotes = update.ReleaseNotes,
                                SupportURL = update.SupportUrl,
                                Title = update.Title,
                                UpdateType = update.Type,
                                UpdateID = string.IsNullOrEmpty(update.Identity.UpdateID) ? Guid.NewGuid().ToString() : update.Identity.UpdateID,
                            };

                            foreach (object item in update.CveIDs)
                            {
                                updateInformation.CveIDList.Add(item.ToString());
                            }

                            foreach (object item in update.MoreInfoUrls)
                            {
                                updateInformation.KBArticleIDList.Add(item.ToString());
                            }

                            foreach (object item in update.MoreInfoUrls)
                            {
                                updateInformation.MoreInfoList.Add(item.ToString());
                            }

                            foreach (object item in update.Languages)
                            {
                                updateInformation.SupportedLanguageList.Add(item.ToString());
                            }

                            UpdateModel updateItem = new()
                            {
                                UpdateInformation = updateInformation,
                                Description = string.IsNullOrEmpty(updateInformation.Description) ? ResourceService.UpdateManagerResource.GetString("Unknown") : updateInformation.Description,
                                EulaText = string.IsNullOrEmpty(updateInformation.EulaText) ? ResourceService.UpdateManagerResource.GetString("Unknown") : updateInformation.EulaText,
                                IsBeta = updateInformation.IsBeta ? ResourceService.UpdateManagerResource.GetString("Yes") : ResourceService.UpdateManagerResource.GetString("No"),
                                IsMandatory = updateInformation.IsMandatory ? ResourceService.UpdateManagerResource.GetString("Yes") : ResourceService.UpdateManagerResource.GetString("No"),
                                MaxDownloadSize = FileSizeHelper.ConvertFileSizeToString(Convert.ToDouble(updateInformation.Update.MaxDownloadSize)),
                                MinDownloadSize = FileSizeHelper.ConvertFileSizeToString(Convert.ToDouble(updateInformation.Update.MinDownloadSize)),
                                MsrcSeverity = string.IsNullOrEmpty(updateInformation.MsrcSeverity) ? ResourceService.UpdateManagerResource.GetString("Unknown") : updateInformation.MsrcSeverity,
                                RecommendedCpuSpeed = updateInformation.RecommendedCpuSpeed.Equals(0) ? ResourceService.UpdateManagerResource.GetString("Unknown") : string.Format("{0} MHz", updateInformation.RecommendedCpuSpeed),
                                RecommendedHardDiskSpace = updateInformation.RecommendedHardDiskSpace.Equals(0) ? ResourceService.UpdateManagerResource.GetString("Unknown") : string.Format("{0} MB", updateInformation.RecommendedHardDiskSpace),
                                RecommendedMemory = updateInformation.RecommendedMemory.Equals(0) ? ResourceService.UpdateManagerResource.GetString("Unknown") : string.Format("{0} MB", updateInformation.RecommendedMemory),
                                ReleaseNotes = string.IsNullOrEmpty(updateInformation.ReleaseNotes) ? ResourceService.UpdateManagerResource.GetString("Unknown") : updateInformation.ReleaseNotes,
                                SupportURL = updateInformation.SupportURL,
                                Title = string.IsNullOrEmpty(updateInformation.Title) ? ResourceService.UpdateManagerResource.GetString("Unknown") : updateInformation.Title,
                                UpdateID = updateInformation.UpdateID,
                                IsUpdating = false,
                                UpdateProgress = string.Empty,
                                IsSelected = false,
                                IsUpdateCanceled = false,
                                IsUpdatePreparing = false,
                                UpdatePercentage = 0,
                                DriverInformation = new DriverInformation(),
                                DeviceProblemNumber = string.Empty,
                                DriverClass = string.Empty,
                                DriverHardwareID = string.Empty,
                                DriverManufacturer = string.Empty,
                                DriverModel = string.Empty,
                                DriverProvider = string.Empty,
                                DriverVerDate = string.Empty,
                            };

                            if (updateInformation.UpdateType is UpdateType.utSoftware)
                            {
                                updateItem.UpdateType = ResourceService.UpdateManagerResource.GetString("UpdateTypeSoftware");
                            }
                            else if (updateInformation.UpdateType is UpdateType.utDriver)
                            {
                                updateItem.UpdateType = ResourceService.UpdateManagerResource.GetString("UpdateTypeDriver");
                                IWindowsDriverUpdate5 windowsDriverUpdate = update as IWindowsDriverUpdate5;

                                if (windowsDriverUpdate is not null)
                                {
                                    DriverInformation driverInformation = new()
                                    {
                                        DeviceProblemNumber = windowsDriverUpdate.DeviceProblemNumber,
                                        DriverClass = windowsDriverUpdate.DriverClass,
                                        DriverHardwareID = windowsDriverUpdate.DriverHardwareID,
                                        DriverManufacturer = windowsDriverUpdate.DriverManufacturer,
                                        DriverModel = windowsDriverUpdate.DriverModel,
                                        DriverProvider = windowsDriverUpdate.DriverProvider,
                                        DriverVerDate = windowsDriverUpdate.DriverVerDate,
                                        WindowsDriverUpdate = windowsDriverUpdate
                                    };

                                    updateItem.DriverInformation = driverInformation;
                                    updateItem.DeviceProblemNumber = Convert.ToString(driverInformation.DeviceProblemNumber);
                                    updateItem.DriverClass = string.IsNullOrEmpty(driverInformation.DriverClass) ? ResourceService.UpdateManagerResource.GetString("Unknown") : driverInformation.DriverClass;
                                    updateItem.DriverHardwareID = string.IsNullOrEmpty(driverInformation.DriverHardwareID) ? ResourceService.UpdateManagerResource.GetString("Unknown") : driverInformation.DriverHardwareID;
                                    updateItem.DriverManufacturer = string.IsNullOrEmpty(driverInformation.DriverManufacturer) ? ResourceService.UpdateManagerResource.GetString("Unknown") : driverInformation.DriverManufacturer;
                                    updateItem.DriverModel = string.IsNullOrEmpty(driverInformation.DriverModel) ? ResourceService.UpdateManagerResource.GetString("Unknown") : driverInformation.DriverModel;
                                    updateItem.DriverProvider = string.IsNullOrEmpty(driverInformation.DriverProvider) ? ResourceService.UpdateManagerResource.GetString("Unknown") : driverInformation.DriverProvider;
                                    updateItem.DriverVerDate = driverInformation.DriverVerDate.ToString("yyyy/MM/dd");
                                }
                            }
                            else
                            {
                                updateItem.UpdateType = ResourceService.UpdateManagerResource.GetString("Unknown");
                            }

                            updateItem.CveIDList.AddRange(updateInformation.CveIDList);
                            updateItem.KBArticleIDList.AddRange(updateInformation.KBArticleIDList);
                            updateItem.SupportedLanguageList.AddRange(updateInformation.SupportedLanguageList);
                            updateItem.MoreInfoList.AddRange(updateInformation.MoreInfoList);

                            // 隐藏的更新
                            if (updateInformation.Update.IsHidden)
                            {
                                updateItem.UpdateProgress = string.Empty;
                                hiddenUpdateList.Add(updateItem);
                            }
                            // 已安装的更新
                            else if (update.IsInstalled)
                            {
                                updateItem.UpdateProgress = ResourceService.UpdateManagerResource.GetString("UpdateInstalled");
                                installedUpdateList.Add(updateItem);
                            }
                            // 可用更新
                            else
                            {
                                updateItem.UpdateProgress = ResourceService.UpdateManagerResource.GetString("UpdateNotInstalled");
                                availableUpdateList.Add(updateItem);
                            }
                        }
                        result = true;
                    }

                    updateHistoryList.AddRange(GetUpdateHistoryList());
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Search update failed", e);
                }

                return result;
            });

            if (searchResult)
            {
                AvailableUpdateCollection.Clear();
                foreach (UpdateModel availableUpdateItem in availableUpdateList)
                {
                    AvailableUpdateCollection.Add(availableUpdateItem);
                }

                InstalledUpdateCollection.Clear();
                foreach (UpdateModel installedUpdateItem in installedUpdateList)
                {
                    InstalledUpdateCollection.Add(installedUpdateItem);
                }

                HiddenUpdateCollection.Clear();
                foreach (UpdateModel hiddenUpdateItem in hiddenUpdateList)
                {
                    HiddenUpdateCollection.Add(hiddenUpdateItem);
                }

                UpdateHistoryCollection.Clear();
                foreach (UpdateModel updateHistoryItem in updateHistoryList)
                {
                    UpdateHistoryCollection.Add(updateHistoryItem);
                }
            }

            IsCheckUpdateEnabled = true;
            IsChecking = false;
        }

        /// <summary>
        /// 获取历史更新记录
        /// </summary>
        private List<UpdateModel> GetUpdateHistoryList()
        {
            List<UpdateModel> updateHistoryList = [];

            int updateHistoryCount = updateSearcher.GetTotalHistoryCount();

            if (updateHistoryCount > 0)
            {
                foreach (IUpdateHistoryEntry2 updateHistoryEntry in updateSearcher.QueryHistory(0, updateHistoryCount))
                {
                    if (!string.IsNullOrEmpty(updateHistoryEntry.Title))
                    {
                        UpdateHistoryInformation updateHistoryInformation = new()
                        {
                            ClientApplicationID = updateHistoryEntry.ClientApplicationID,
                            Date = updateHistoryEntry.Date,
                            HResult = updateHistoryEntry.HResult,
                            OperationResultCode = updateHistoryEntry.ResultCode,
                            SupportUrl = updateHistoryEntry.SupportUrl,
                            Title = updateHistoryEntry.Title,
                            UpdateHistoryEntry = updateHistoryEntry,
                            UpdateID = !string.IsNullOrEmpty(updateHistoryEntry.UpdateIdentity.UpdateID) ? Guid.NewGuid().ToString() : updateHistoryEntry.UpdateIdentity.UpdateID
                        };

                        UpdateModel updateItem = new()
                        {
                            UpdateHistoryInformation = updateHistoryInformation,
                            Date = updateHistoryInformation.Date.ToString("yyyy/MM/dd"),
                            HistoryUpdateResult = GetUpdateResult(updateHistoryInformation.OperationResultCode, updateHistoryInformation.Date, updateHistoryInformation.HResult),
                            SupportURL = updateHistoryInformation.SupportUrl,
                            Title = updateHistoryInformation.Title,
                            UpdateID = updateHistoryInformation.UpdateID
                        };

                        updateHistoryList.Add(updateItem);
                    }
                }
            }

            return updateHistoryList;
        }

        /// <summary>
        /// 获取更新的安装状态
        /// </summary>
        private string GetUpdateResult(OperationResultCode operationResultCode, DateTime date, int hResult)
        {
            switch (operationResultCode)
            {
                case OperationResultCode.orcAborted:
                    {
                        return string.Format(ResourceService.UpdateManagerResource.GetString("UpdateAborted"), date.ToString("yyyy/MM/dd"), hResult);
                    }
                case OperationResultCode.orcSucceeded:
                    {
                        return string.Format(ResourceService.UpdateManagerResource.GetString("UpdateSucceed"), date.ToString("yyyy/MM/dd"));
                    }
                case OperationResultCode.orcSucceededWithErrors:
                    {
                        return string.Format(ResourceService.UpdateManagerResource.GetString("UpdateSucceedWithErrors"), date.ToString("yyyy/MM/dd"), hResult);
                    }
                case OperationResultCode.orcFailed:
                    {
                        return string.Format(ResourceService.UpdateManagerResource.GetString("UpdateFailed"), date.ToString("yyyy/MM/dd"), hResult);
                    }
                default:
                    {
                        return ResourceService.UpdateManagerResource.GetString("Unknown");
                    }
            }
        }

        /// <summary>
        /// 移除自定义预览体验计划设置
        /// </summary>
        private bool ExitCustomPreviewChannel()
        {
            Task.Run(() =>
            {
                try
                {
                    RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\Account", null);
                    RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\Applicability", null);
                    RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\Cache", null);
                    RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\ClientState", null);
                    RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\UI", null);
                    RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\Restricted", null);
                    RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\ToastNotification", null);
                    RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\SLS\Programs\WUMUDCat", null);
                    RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\SLS\Programs\RingExternal", null);
                    RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\SLS\Programs\RingPreview", null);
                    RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\SLS\Programs\RingInsiderSlow", null);
                    RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\SLS\Programs\RingInsiderFast", null);
                    RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\DataCollection", "AllowTelemetry");
                    RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, @"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\DataCollection", "AllowTelemetry");
                    RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, @"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate", "BranchReadinessLevel");
                    RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, @"HKEY_LOCAL_MACHINE\SYSTEM\Setup\WindowsUpdate", "AllowWindowsUpdate");
                    RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, @"HKEY_LOCAL_MACHINE\SYSTEM\Setup\MoSetup", "AllowUpgradesWithUnsupportedTPMOrCPU");
                    RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, @"HKEY_LOCAL_MACHINE\SYSTEM\Setup\LabConfig", "BypassRAMCheck");
                    RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, @"HKEY_LOCAL_MACHINE\SYSTEM\Setup\LabConfig", "BypassSecureBootCheck");
                    RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, @"HKEY_LOCAL_MACHINE\SYSTEM\Setup\LabConfig", "BypassStorageCheck");
                    RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, @"HKEY_LOCAL_MACHINE\SYSTEM\Setup\LabConfig", "BypassTPMCheck");
                    RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\PCHC", "UpgradeEligibility");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Remove custom preview channel settings failed", e);
                }
            });

            return default;
        }

        // TODO:未完成
        /// <summary>
        /// 添加自定义预览体验计划设置
        /// </summary>
        private bool EnterCustomPreviewChannel(string previewChannel)
        {
            Task.Run(() =>
            {
                try
                {
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Orchestrator", "EnableUUPScan", 1);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\SLS\Programs\RingExternal", "Enabled", 1);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\SLS\Programs\WUMUDCat", "WUMUDCATEnabled", 1);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\Applicability", "EnablePreviewBuilds", 2);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\Applicability", "IsBuildFlightingEnabled", 1);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\Applicability", "IsConfigSettingsFlightingEnabled", 1);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\Applicability", "TestFlags", 32);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\Applicability", "RingId", 11);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\Applicability", "Ring", "External");
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\Applicability", "ContentType", "Mainline");
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\Applicability", "BranchName", "ReleasePreview"); // 改成对应的通道
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\UI\Strings", "StickyXaml", "设置中呈现的 Xaml 内容"); // 改成对应的字符串
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\UI\Visibility", "UIHiddenElements", 65535);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\UI\Visibility", "UIDisabledElements", 65535);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\UI\Visibility", "UIServiceDrivenElementVisibility", 0);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\UI\Visibility", "UIErrorMessageVisibility", 192);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\DataCollection", "AllowTelemetry", 3);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate", "BranchReadinessLevel", 8); // 改成对应的通道
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\UI\Visibility", "UIHiddenElements_Rejuv", 65534);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\UI\Visibility", "UIHiddenElements_Rejuv", 65535);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\UI\Selection", "UIRing", 65535);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\UI\Selection", "UIContentType", 65535);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\Applicability", "UIBranch", "ReleasePreview");// 改成对应的通道
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\Applicability", "RingBackup", "External");
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\Applicability", "RingBackupV2", "External");
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\Applicability", "BranchBackup", "ReleasePreview"); // 改成对应的通道
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\Cache", "PropertyIgnoreList", "AccountsBlob;;CTACBlob;FlightIDBlob;ServiceDrivenActionResults");
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\Cache", "RequestedCTACAppIds", "WU;FSS");
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\Account", "SupportedTypes", 3);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\Account", "Status", 8);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\Applicability", "UseSettingsExperience", 0);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\ClientState", "AllowFSSCommunications", 1);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\ClientState", "UICapabilities", 1);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\ClientState", "IgnoreConsolidation", 0);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\ClientState", "MsaUserTicketHr", 0);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\ClientState", "MsaDeviceTicketHr", 0);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\ClientState", "ValidateOnlineHr", 0);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\ClientState", "LastHR", 0);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\ClientState", "ErrorState", 0);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\Applicability", "PilotInfoRing", 3);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\Applicability", "RegistryAllowlistVersion", 4);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\Applicability", "FileAllowlistVersion", 1);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\UI", "UIControllableState", 0);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\UI\Selection", "UIDialogConsent", 0);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\UI\Selection", "UIUsage", 26);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\UI\Selection", "OptOutState", 25);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\UI\Selection", "AdvancedToggleState", 24);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SYSTEM\Setup\WindowsUpdate", "AllowWindowsUpdate", 1);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SYSTEM\Setup\MoSetup", "AllowUpgradesWithUnsupportedTPMOrCPU", 1);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SYSTEM\Setup\LabConfig", "BypassRAMCheck", 1);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SYSTEM\Setup\LabConfig", "BypassSecureBootCheck", 1);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SYSTEM\Setup\LabConfig", "BypassStorageCheck", 1);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SYSTEM\Setup\LabConfig", "BypassTPMCheck", 1);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\PCHC", "UpgradeEligibility", 1);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\UI\Strings", "设置中呈现的 Xaml 内容", 1); // 改成对应的字符串
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Add custom preview channel settings failed", e);
                }
            });

            return default;
        }
    }
}
