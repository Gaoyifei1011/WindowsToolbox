using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
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
        private readonly IUpdateSearcher updateSearcher;

        private readonly bool isInitialized;

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

        private bool _isAUExpanderExpanded = true;

        public bool IsAUExpanderExpanded
        {
            get { return _isAUExpanderExpanded; }

            set
            {
                if (!Equals(_isAUExpanderExpanded, value))
                {
                    _isAUExpanderExpanded = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAUExpanderExpanded)));
                }
            }
        }

        private bool _isIUExpanderExpanded = true;

        public bool IsIUExpanderExpanded
        {
            get { return _isIUExpanderExpanded; }

            set
            {
                if (!Equals(_isIUExpanderExpanded, value))
                {
                    _isIUExpanderExpanded = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsIUExpanderExpanded)));
                }
            }
        }

        private bool _isHUExpanderExpanded = true;

        public bool IsHUExpanderExpanded
        {
            get { return _isHUExpanderExpanded; }

            set
            {
                if (!Equals(_isHUExpanderExpanded, value))
                {
                    _isHUExpanderExpanded = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsHUExpanderExpanded)));
                }
            }
        }

        private bool _isUHExpanderExpanded = true;

        public bool IsUHExpanderExpanded
        {
            get { return _isUHExpanderExpanded; }

            set
            {
                if (!Equals(_isUHExpanderExpanded, value))
                {
                    _isUHExpanderExpanded = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsUHExpanderExpanded)));
                }
            }
        }

        private bool _isORExpanderExpanded;

        public bool IsORExpanderExpanded
        {
            get { return _isORExpanderExpanded; }

            set
            {
                if (!Equals(_isORExpanderExpanded, value))
                {
                    _isORExpanderExpanded = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsORExpanderExpanded)));
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

        private bool _isInstalledUnInstallEnabled;

        private bool IsInstalledUnInstallEnabled
        {
            get { return _isInstalledUnInstallEnabled; }

            set
            {
                if (!Equals(_isInstalledUnInstallEnabled, value))
                {
                    _isInstalledUnInstallEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsInstalledUnInstallEnabled)));
                }
            }
        }

        private bool _isHiddenEnabled;

        private bool IsHiddenEnabled
        {
            get { return _isHiddenEnabled; }

            set
            {
                if (!Equals(_isHiddenEnabled, value))
                {
                    _isHiddenEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsHiddenEnabled)));
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

        private KeyValuePair<string, string> _selectedPreviewChannel;

        public KeyValuePair<string, string> SelectedPreviewChannel
        {
            get { return _selectedPreviewChannel; }

            set
            {
                if (!Equals(_selectedPreviewChannel, value))
                {
                    _selectedPreviewChannel = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedPreviewChannel)));
                }
            }
        }

        private List<KeyValuePair<string, Type>> PageList { get; } =
        [
            new KeyValuePair<string, Type>("FileName",typeof(FileNamePage)),
            new KeyValuePair<string, Type>("ExtensionName", typeof(ExtensionNamePage)),
            new KeyValuePair<string, Type>("UpperAndLowerCase", typeof(UpperAndLowerCasePage)),
            new KeyValuePair<string, Type>("FileProperties", typeof(FilePropertiesPage)),
        ];

        private List<KeyValuePair<string, string>> UpdateSourceList { get; } =
        [
            new KeyValuePair<string,string>("Microsoft Update", ResourceService.UpdateManagerResource.GetString("MicrosoftUpdate")),
            new KeyValuePair<string,string>("DCat Flighting Prod", ResourceService.UpdateManagerResource.GetString("DCatFlightingProd")),
            new KeyValuePair<string,string>("Windows Store(DCat Prod)", ResourceService.UpdateManagerResource.GetString("WindowsStore")),
            new KeyValuePair<string,string>("Windows Update", ResourceService.UpdateManagerResource.GetString("WindowsUpdate")),
        ];

        private List<KeyValuePair<string, string>> PreviewChannelList { get; } =
        [
            new KeyValuePair<string,string>(ResourceService.UpdateManagerResource.GetString("DonotEnter"), "DoNotEnter"),
            new KeyValuePair<string,string>(ResourceService.UpdateManagerResource.GetString("ReleasePreview"), "ReleasePreview"),
            new KeyValuePair<string,string>(ResourceService.UpdateManagerResource.GetString("Beta"), "Beta"),
            new KeyValuePair<string,string>(ResourceService.UpdateManagerResource.GetString("Dev"), "Dev"),
            new KeyValuePair<string,string>(ResourceService.UpdateManagerResource.GetString("Canary"), "Canary"),
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
            SelectedPreviewChannel = PreviewChannelList[0];
            SelectedUpdateSource = UpdateSourceList[0];

            try
            {
                updateSession.ClientApplicationID = "WindowsTools:" + Guid.NewGuid().ToString();
                updateSearcher = updateSession.CreateUpdateSearcher();
                WindowsUpdateAgentInfo windowsUpdateAgentInfo = new();
                dynamic apiMajorVersion = windowsUpdateAgentInfo.GetInfo("ApiMajorVersion");
                dynamic apiMinorVersion = windowsUpdateAgentInfo.GetInfo("ApiMinorVersion");
                dynamic productVersionString = windowsUpdateAgentInfo.GetInfo("ProductVersionString");
                WindowsUpdateVersion = string.Format("{0}.{1}", apiMajorVersion, apiMinorVersion);
                WuapiDllVersion = productVersionString;
            }
            catch (Exception)
            {
                return;
            }

            GetUpdateHistory();

            if (RuntimeHelper.IsElevated)
            {
                try
                {
                    RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate", true);
                    int returnValue = Convert.ToInt32(registryKey.GetValue("ExcludeWUDriversInQualityUpdate"));
                    IsExcludeDrivers = Convert.ToBoolean(returnValue);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Warning, "Get exclude driver options failed", e);
                }
            }
        }

        #region 第一部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 可用更新：取消更新
        /// </summary>
        private void OnAvailableCancelInstallExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            // TODO ：未完成
        }

        /// <summary>
        /// 可用更新：隐藏
        /// </summary>
        private async void OnAvailableHideExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is UpdateModel updateItem)
            {
                bool hideResult = await Task.Run(() =>
                {
                    bool result = false;

                    // 隐藏更新（只有 Power Users 管理组的管理员和成员才能设置此属性的值）
                    if (RuntimeHelper.IsElevated)
                    {
                        try
                        {
                            updateItem.UpdateInformation.Update.IsHidden = true;
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
                            if (AvailableUpdateCollection[index].UpdateID.Equals(updateItem.UpdateID))
                            {
                                AvailableUpdateCollection.RemoveAt(index);
                                break;
                            }
                        }

                        updateItem.IsHidden = ResourceService.UpdateManagerResource.GetString("Yes");
                        HiddenUpdateCollection.Add(updateItem);

                        IsAUExpanderExpanded = true;
                        IsHUExpanderExpanded = true;
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, "Hide updates update UI failed", e);
                    }
                }
            }
        }

        /// <summary>
        /// 可用更新：安装
        /// </summary>
        private void OnAvailableInstallExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is UpdateModel updateItem)
            {
                foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
                {
                    if (availableUpdateItem.UpdateID.Equals(updateItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                    {
                        availableUpdateItem.IsUpdating = true;
                    }
                }

                Task.Run(() =>
                {
                    UpdateCollection updateCollection = new() { updateItem.UpdateInformation.Update };

                    try
                    {
                        AutoResetEvent downloadEvent = new(false);

                        // 先下载更新
                        IDownloadJob downloadJob = null;
                        UpdateDownloader updateDownloader = updateSession.CreateUpdateDownloader();
                        updateDownloader.Updates = updateCollection;

                        DownloadProgressChangedCallback downloadProgressChangedCallback = new();
                        DownloadCompletedCallback downloadCompletedCallback = new();
                        downloadProgressChangedCallback.DownloadProgressChanged += (sender, args) => OnDownloadProgressChanged(sender, args, updateItem);
                        downloadCompletedCallback.DownloadCompleted += (sender, args) => downloadEvent.Set();
                        downloadJob = updateDownloader.BeginDownload(downloadProgressChangedCallback, downloadCompletedCallback, null);
                        downloadEvent.WaitOne();
                        downloadEvent.Dispose();
                        IDownloadResult downloadResult = updateDownloader.EndDownload(downloadJob);

                        synchronizationContext.Post(_ =>
                        {
                            if (downloadResult is not null && downloadResult.ResultCode is OperationResultCode.orcSucceeded)
                            {
                                //OperationTextBlock.Text = "更新已下载完成";
                            }
                            else
                            {
                                //OperationTextBlock.Text = "更新下载失败，错误代码：" + downloadResult.HResult.ToString() + " " + Marshal.GetExceptionForHR(downloadResult.HResult).Message;
                                return;
                            }
                        }, null);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, "Download updates failed", e);
                    }

                    // 后面再安装更新
                    AutoResetEvent installEvent = new(false);

                    IInstallationJob installationJob = null;
                    IUpdateInstaller updateInstaller = updateSession.CreateUpdateInstaller();
                    updateInstaller.Updates = updateCollection;
                    InstallationCompletedCallback installationCompletedCallback = new();
                    InstallationProgressChangedCallback installationProgressChangedCallback = new();
                    installationCompletedCallback.InstallationCompleted += (sender, args) => installEvent.Set();
                    installationProgressChangedCallback.InstallationProgressChanged += (sender, args) => OnInstallationProgressChanged(sender, args, updateItem);
                    installationJob = updateInstaller.BeginInstall(installationProgressChangedCallback, installationCompletedCallback, null);
                    installEvent.WaitOne();
                    installEvent.Dispose();
                    IInstallationResult installationResult = updateInstaller.EndInstall(installationJob);

                    synchronizationContext.Post(_ =>
                    {
                        if (installationResult.ResultCode is OperationResultCode.orcSucceeded)
                        {
                            //OperationTextBlock.Text = "更新已安装完成";
                        }
                        else
                        {
                            //OperationTextBlock.Text = "更新安装失败，错误代码：" + installationResult.HResult.ToString() + " " + Marshal.GetExceptionForHR(installationResult.HResult).Message;
                        }

                        IsChecking = true;
                        CheckUpdate();
                    }, null);
                });
            }
        }

        /// <summary>
        /// 可用更新：修改可用更新项选中状态
        /// </summary>
        private void OnAvailableCheckClickExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            UpdateModel availableUpdateItem = args.Parameter as UpdateModel;

            if (availableUpdateItem is not null)
            {
                foreach (UpdateModel updateItem in AvailableUpdateCollection)
                {
                    if (updateItem.UpdateID.Equals(availableUpdateItem.UpdateID))
                    {
                        updateItem.IsSelected = !updateItem.IsSelected;
                        break;
                    }
                }

                IsAvailableInstallEnabled = AvailableUpdateCollection.Any(item => item.IsSelected);
            }
        }

        /// <summary>
        /// 已安装更新：卸载
        /// </summary>
        private void OnInstalledUnInstallExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is UpdateModel updateItem)
            {
                Task.Run(() =>
                {
                    IUpdateInstaller updateInstaller = updateSession.CreateUpdateInstaller();
                    updateInstaller.Updates = new UpdateCollection { updateItem.UpdateInformation.Update };
                    InstallationCompletedCallback unInstallationCompletedCallback = new();
                    InstallationProgressChangedCallback unInstallationProgressChangedCallback = new();
                    unInstallationCompletedCallback.InstallationCompleted += (sender, args) =>
                    {
                        synchronizationContext.Post(_ =>
                        {
                            IsChecking = true;
                        }, null);
                        CheckUpdate();
                    };

                    unInstallationProgressChangedCallback.InstallationProgressChanged += (sender, args) => OnUnInstallationProgressChanged(sender, args, updateItem);
                    updateInstaller.BeginUninstall(unInstallationProgressChangedCallback, unInstallationCompletedCallback, null);
                });
            }
        }

        /// <summary>
        /// 已安装更新：修改已安装更新项选中状态
        /// </summary>
        private void OnInstalledCheckBoxClickExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            UpdateModel installedUpdateItem = args.Parameter as UpdateModel;

            if (installedUpdateItem is not null)
            {
                foreach (UpdateModel updateItem in InstalledUpdateCollection)
                {
                    if (updateItem.UpdateID.Equals(installedUpdateItem.UpdateID))
                    {
                        updateItem.IsSelected = !updateItem.IsSelected;
                        break;
                    }
                }

                IsInstalledUnInstallEnabled = InstalledUpdateCollection.Any(item => item.IsSelected);
            }
        }

        /// <summary>
        /// 隐藏更新：显示
        /// </summary>
        private async void OnHiddenShowExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is UpdateModel updateItem)
            {
                bool hideResult = await Task.Run(() =>
                {
                    bool result = false;

                    // 隐藏更新（只有 Power Users 管理组的管理员和成员才能设置此属性的值）
                    if (RuntimeHelper.IsElevated)
                    {
                        try
                        {
                            updateItem.UpdateInformation.Update.IsHidden = false;
                            result = true;
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, "Show updates modify hidden property failed", e);
                        }
                    }

                    return result;
                });

                if (hideResult)
                {
                    try
                    {
                        for (int index = 0; index < HiddenUpdateCollection.Count; index++)
                        {
                            if (HiddenUpdateCollection[index].UpdateID.Equals(updateItem.UpdateID))
                            {
                                HiddenUpdateCollection.RemoveAt(index);
                                break;
                            }
                        }

                        updateItem.IsHidden = ResourceService.UpdateManagerResource.GetString("No");
                        AvailableUpdateCollection.Add(updateItem);

                        IsAUExpanderExpanded = true;
                        IsHUExpanderExpanded = true;
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, "Show updates update UI failed", e);
                    }
                }
            }
        }

        /// <summary>
        /// 隐藏更新：修改隐藏更新项选中状态
        /// </summary>
        private void OnHiddenCheckBoxClickExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            UpdateModel hiddenUpdateItem = args.Parameter as UpdateModel;

            if (hiddenUpdateItem is not null)
            {
                foreach (UpdateModel updateItem in HiddenUpdateCollection)
                {
                    if (updateItem.UpdateID.Equals(hiddenUpdateItem.UpdateID))
                    {
                        updateItem.IsSelected = !updateItem.IsSelected;
                        break;
                    }
                }

                IsHiddenEnabled = HiddenUpdateCollection.Any(item => item.IsSelected);
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
                    copyInformationBuilder.AppendLine(ResourceService.UpdateManagerResource.GetString("Title"));
                    copyInformationBuilder.AppendLine(updateItem.Title);
                    copyInformationBuilder.AppendLine(ResourceService.UpdateManagerResource.GetString("Description"));
                    copyInformationBuilder.AppendLine(updateItem.Description);
                    return copyInformationBuilder.ToString();
                });

                bool copyResult = CopyPasteHelper.CopyToClipboard(copyString);
                await TeachingTipHelper.ShowAsync(new DataCopyTip(DataCopyKind.UpdateInformation, copyResult));
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

        #endregion 第一部分：XamlUICommand 命令调用时挂载的事件

        #region 第二部分：Windows 更新管理页面——挂载的事件

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
            foreach (UpdateModel updateItem in AvailableUpdateCollection)
            {
                updateItem.IsSelected = true;
            }

            IsAvailableInstallEnabled = true;
        }

        /// <summary>
        /// 可用更新：全部不选
        /// </summary>
        private void OnAvailableSelectNoneClicked(object sender, RoutedEventArgs args)
        {
            foreach (UpdateModel updateItem in AvailableUpdateCollection)
            {
                updateItem.IsSelected = false;
            }

            IsAvailableInstallEnabled = false;
        }

        /// <summary>
        /// 可用更新：隐藏
        /// </summary>
        private async void OnAvailableHideClicked(object sender, RoutedEventArgs args)
        {
            List<UpdateModel> hideList = AvailableUpdateCollection.Where(item => item.IsSelected is true).ToList();
            bool hideResult = await Task.Run(() =>
            {
                bool result = false;
                if (RuntimeHelper.IsElevated)
                {
                    foreach (UpdateModel updateItem in hideList)
                    {
                        try
                        {
                            updateItem.UpdateInformation.Update.IsHidden = true;
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, "Hide updates modify hidden property failed", e);
                            continue;
                        }
                    }

                    result = true;
                }

                return result;
            });

            if (hideResult)
            {
                foreach (UpdateModel updateItem in hideList)
                {
                    try
                    {
                        if (updateItem.UpdateInformation.Update.IsHidden)
                        {
                            for (int index = 0; index < AvailableUpdateCollection.Count; index++)
                            {
                                if (AvailableUpdateCollection[index].UpdateID.Equals(updateItem.UpdateID))
                                {
                                    AvailableUpdateCollection.RemoveAt(index);
                                    break;
                                }
                            }

                            updateItem.IsHidden = ResourceService.UpdateManagerResource.GetString("Yes");
                            HiddenUpdateCollection.Add(updateItem);

                            IsAUExpanderExpanded = true;
                            IsHUExpanderExpanded = true;
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, "Hide updates update UI failed", e);
                    }
                }
            }
        }

        /// <summary>
        /// 可用更新：安装
        /// </summary>
        private void OnAvailableInstallClicked(object sender, RoutedEventArgs args)
        {
            List<UpdateModel> installList = AvailableUpdateCollection.Where(item => item.IsSelected is true).ToList();
            List<Task> taskList = [];

            Task.Run(() =>
            {
                foreach (UpdateModel updateItem in installList)
                {
                    taskList.Add(Task.Run(() =>
                    {
                        UpdateCollection updateCollection = new() { updateItem.UpdateInformation.Update };

                        try
                        {
                            AutoResetEvent downloadEvent = new(false);

                            // 先下载更新
                            IDownloadJob downloadJob = null;
                            UpdateDownloader updateDownloader = updateSession.CreateUpdateDownloader();
                            updateDownloader.Updates = updateCollection;

                            DownloadProgressChangedCallback downloadProgressChangedCallback = new();
                            DownloadCompletedCallback downloadCompletedCallback = new();
                            downloadProgressChangedCallback.DownloadProgressChanged += (sender, args) => OnDownloadProgressChanged(sender, args, updateItem);
                            downloadCompletedCallback.DownloadCompleted += (sender, args) => downloadEvent.Set();
                            downloadJob = updateDownloader.BeginDownload(downloadProgressChangedCallback, downloadCompletedCallback, null);
                            downloadEvent.WaitOne();
                            downloadEvent.Dispose();
                            IDownloadResult downloadResult = updateDownloader.EndDownload(downloadJob);

                            synchronizationContext.Post(_ =>
                            {
                                // TODO: 未完成
                                if (downloadResult is not null && downloadResult.ResultCode is OperationResultCode.orcSucceeded)
                                {
                                    //OperationTextBlock.Text = "更新已下载完成";
                                }
                                else
                                {
                                    //OperationTextBlock.Text = "更新下载失败，错误代码：" + downloadResult.HResult.ToString() + " " + Marshal.GetExceptionForHR(downloadResult.HResult).Message;
                                    return;
                                }
                            }, null);
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, "Download updates failed", e);
                        }

                        // 后面再安装更新
                        AutoResetEvent installEvent = new(false);

                        IInstallationJob installationJob = null;
                        IUpdateInstaller updateInstaller = updateSession.CreateUpdateInstaller();
                        updateInstaller.Updates = updateCollection;
                        InstallationCompletedCallback installationCompletedCallback = new();
                        InstallationProgressChangedCallback installationProgressChangedCallback = new();
                        installationCompletedCallback.InstallationCompleted += (sender, args) => installEvent.Set();
                        installationProgressChangedCallback.InstallationProgressChanged += (sender, args) => OnInstallationProgressChanged(sender, args, updateItem);
                        installationJob = updateInstaller.BeginInstall(installationProgressChangedCallback, installationCompletedCallback, null);
                        installEvent.WaitOne();
                        installEvent.Dispose();
                        IInstallationResult installationResult = updateInstaller.EndInstall(installationJob);

                        synchronizationContext.Post(_ =>
                        {
                            if (installationResult.ResultCode is OperationResultCode.orcSucceeded)
                            {
                                //OperationTextBlock.Text = "更新已安装完成";
                            }
                            else
                            {
                                //OperationTextBlock.Text = "更新安装失败，错误代码：" + installationResult.HResult.ToString() + " " + Marshal.GetExceptionForHR(installationResult.HResult).Message;
                            }

                            IsChecking = true;
                            CheckUpdate();
                        }, null);
                    }));

                    Task.WaitAll([.. taskList]);

                    // TODO : 添加更新完成提示
                }
            });
        }

        /// <summary>
        /// 已安装更新：全选
        /// </summary>
        private void OnInstalledSelectAllClicked(object sender, RoutedEventArgs args)
        {
            foreach (UpdateModel updateItem in InstalledUpdateCollection)
            {
                updateItem.IsSelected = true;
            }

            IsInstalledUnInstallEnabled = InstalledUpdateCollection.Any(item => item.UpdateInformation.Update.IsUninstallable && item.IsSelected);
        }

        /// <summary>
        /// 已安装更新：全部不选
        /// </summary>
        private void OnInstalledSelectNoneClicked(object sender, RoutedEventArgs args)
        {
            foreach (UpdateModel updateItem in InstalledUpdateCollection)
            {
                updateItem.IsSelected = false;
            }

            IsInstalledUnInstallEnabled = false;
        }

        /// <summary>
        /// 已安装更新：卸载
        /// </summary>
        private void OnInstalledUnInstallClicked(object sender, RoutedEventArgs args)
        {
            List<UpdateModel> unInstallList = InstalledUpdateCollection.Where(item => item.IsSelected is true).ToList();
            List<Task> taskList = [];

            Task.Run(() =>
            {
                foreach (UpdateModel updateItem in unInstallList)
                {
                    taskList.Add(Task.Run(() =>
                    {
                        AutoResetEvent autoResetEvent = new(false);

                        try
                        {
                            IUpdateInstaller updateInstaller = updateSession.CreateUpdateInstaller();
                            updateInstaller.Updates = new UpdateCollection { updateItem.UpdateInformation.Update };
                            InstallationCompletedCallback unInstallationCompletedCallback = new();
                            InstallationProgressChangedCallback unInstallationProgressChangedCallback = new();
                            unInstallationCompletedCallback.InstallationCompleted += (sender, args) => autoResetEvent.Set();
                            unInstallationProgressChangedCallback.InstallationProgressChanged += (sender, args) => OnUnInstallationProgressChanged(sender, args, updateItem);
                            updateInstaller.BeginUninstall(unInstallationProgressChangedCallback, unInstallationCompletedCallback, null);
                            autoResetEvent.WaitOne();
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, "Install update failed", e);
                            autoResetEvent.Set();
                        }
                        finally
                        {
                            autoResetEvent.Dispose();
                        }
                    }));
                }
            });

            Task.WaitAll([.. taskList]);

            // TODO : 添加更新未完成提示
        }

        /// <summary>
        /// 隐藏更新：全选
        /// </summary>
        private void OnHiddenSelectAllClicked(object sender, RoutedEventArgs args)
        {
            foreach (UpdateModel updateItem in HiddenUpdateCollection)
            {
                updateItem.IsSelected = true;
            }

            IsHiddenEnabled = true;
        }

        /// <summary>
        /// 隐藏更新：全部不选
        /// </summary>
        private void OnHiddenSelectNoneClicked(object sender, RoutedEventArgs args)
        {
            foreach (UpdateModel updateItem in HiddenUpdateCollection)
            {
                updateItem.IsSelected = false;
            }

            IsHiddenEnabled = false;
        }

        /// <summary>
        /// 隐藏更新：显示
        /// </summary>
        private async void OnHiddenShowClicked(object sender, RoutedEventArgs args)
        {
            List<UpdateModel> showList = HiddenUpdateCollection.Where(item => item.IsSelected is true).ToList();
            bool hiddenResult = await Task.Run(() =>
            {
                bool result = false;

                if (RuntimeHelper.IsElevated)
                {
                    foreach (UpdateModel updateItem in showList)
                    {
                        try
                        {
                            updateItem.UpdateInformation.Update.IsHidden = false;
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, "Show updates modify hidden property failed", e);
                            continue;
                        }
                    }

                    result = true;
                }

                return result;
            });

            if (hiddenResult)
            {
                foreach (UpdateModel updateItem in showList)
                {
                    try
                    {
                        if (updateItem.UpdateInformation.Update.IsHidden is false)
                        {
                            for (int index = 0; index < HiddenUpdateCollection.Count; index++)
                            {
                                if (HiddenUpdateCollection[index].UpdateID.Equals(updateItem.UpdateID))
                                {
                                    HiddenUpdateCollection.RemoveAt(index);
                                    break;
                                }
                            }

                            updateItem.IsHidden = ResourceService.UpdateManagerResource.GetString("No");
                            AvailableUpdateCollection.Add(updateItem);

                            IsAUExpanderExpanded = true;
                            IsHUExpanderExpanded = true;
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, "Show updates update UI failed", e);
                    }
                }
            }
        }

        /// <summary>
        /// 检查更新
        /// </summary>
        private void OnCheckUpdateClicked(object sender, RoutedEventArgs args)
        {
            IsCheckUpdateEnabled = false;
            IsChecking = true;
            CheckUpdate();
        }

        /// <summary>
        /// Windows 更新不包括驱动程序
        /// </summary>
        private void OnExcludeDriversToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                bool value = toggleSwitch.IsOn;
                Task.Run(() =>
                {
                    try
                    {
                        RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate", true);
                        registryKey?.SetValue("ExcludeWUDriversInQualityUpdate", Convert.ToInt32(Convert.ToBoolean(value)), RegistryValueKind.DWord);
                        int returnValue = Convert.ToInt32(registryKey.GetValue("ExcludeWUDriversInQualityUpdate"));
                        IsExcludeDrivers = Convert.ToBoolean(returnValue);
                        registryKey.Close();
                        registryKey.Dispose();
                    }
                    catch (Exception e)
                    {
                        IsExcludeDrivers = !IsExcludeDrivers;
                        LogService.WriteLog(EventLevel.Warning, "Set exclude driver options failed", e);
                    }
                });
            }
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
        /// 更改设备的预览计划频道
        /// </summary>
        private void OnPreviewChannelSelectClicked(object sender, RoutedEventArgs args)
        {
            if (sender is MenuFlyoutItem menuFlyoutItem && menuFlyoutItem.Tag is not null)
            {
                SelectedPreviewChannel = PreviewChannelList[Convert.ToInt32(menuFlyoutItem.Tag)];
            }
        }

        #endregion 第二部分：Windows 更新管理页面——挂载的事件

        #region 第三部分：自定义事件

        /// <summary>
        /// 下载更新中，进度变化时更新 UI 下载进度
        /// </summary>
        private void OnDownloadProgressChanged(object sender, EventArgs args, UpdateModel updateItem)
        {
            double percentage = (sender as DownloadProgressChangedCallback).CallbackArgs.Progress.CurrentUpdatePercentComplete;
            synchronizationContext.Post(_ =>
            {
                //OperationTextBlock.Text = string.Format("已下载 {0} % ", percentage);
                //foreach (UpdateModel updateItem in InstalledUpdateCollection)
                //{
                //    if (updateItem.UpdateID.Equals(updateItem.UpdateID))
                //    {
                //        // TODO : 未完成
                //        OperationTextBlock.Text = (string.Format("已下载 {0} %", percentage));
                //    }
                //}
            }, null);
        }

        /// <summary>
        /// 卸载更新中，进度变化时更改 UI 卸载进度
        /// </summary>
        private void OnUnInstallationProgressChanged(object sender, EventArgs args, UpdateModel updateItem)
        {
            double percentage = (sender as InstallationProgressChangedCallback).CallbackArgs.Progress.CurrentUpdatePercentComplete / 100.0;
            synchronizationContext.Post(_ =>
            {
                foreach (UpdateModel updateItem in InstalledUpdateCollection)
                {
                    if (updateItem.UpdateID.Equals(updateItem.UpdateID))
                    {
                        // TODO : 未完成
                        updateItem.UpdateProgress = string.Format(ResourceService.UpdateManagerResource.GetString("UnInstallProgress"), percentage);
                    }
                }
            }, null);
        }

        /// <summary>
        /// 安装更新中，进度变化时更改 UI 卸载进度
        /// </summary>
        private void OnInstallationProgressChanged(object sender, EventArgs args, UpdateModel updateItem)
        {
            double percentage = (sender as InstallationProgressChangedCallback).CallbackArgs.Progress.CurrentUpdatePercentComplete;
            synchronizationContext.Post(_ =>
            {
                foreach (UpdateModel updateItem in AvailableUpdateCollection)
                {
                    if (updateItem.UpdateID.Equals(updateItem.UpdateID))
                    {
                        // TODO : 未完成
                        //OperationTextBlock.Text = string.Format("已安装 {0} %", percentage);
                    }
                }
            }, null);
        }

        /// <summary>
        /// 更新搜索完成时触发的事件
        /// </summary>
        private void OnSearchCompleted(object sender, EventArgs args)
        {
            if (sender is SearchCompletedCallback searchCompletedCallback && searchCompletedCallback.SearchJob is not null)
            {
                ISearchResult searchResult = null;
                try
                {
                    searchResult = updateSearcher.EndSearch(searchCompletedCallback.SearchJob);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Warning, "Get update search result failed", e);
                    try
                    {
                        searchCompletedCallback.SearchCompleted -= OnSearchCompleted;
                    }
                    catch (Exception ex)
                    {
                        LogService.WriteLog(EventLevel.Error, "Unregister SearchCompletedCallback SearchCompleted event failed", ex);
                    }

                    synchronizationContext.Post(_ =>
                    {
                        IsChecking = false;
                        IsAUExpanderExpanded = true;
                        IsIUExpanderExpanded = true;
                        IsHUExpanderExpanded = true;
                    }, null);
                    return;
                }

                List<UpdateModel> availableUpdateList = [];
                List<UpdateModel> installedUpdateList = [];
                List<UpdateModel> hiddenUpdateList = [];

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
                        UpdateID = update.Identity.UpdateID,
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
                        updateInformation.LanguageList.Add(item.ToString());
                    }

                    UpdateModel updateItem = new()
                    {
                        UpdateInformation = updateInformation,
                        Description = updateInformation.Description,
                        EulaText = updateInformation.EulaText,
                        IsBeta = updateInformation.IsBeta ? ResourceService.UpdateManagerResource.GetString("Yes") : ResourceService.UpdateManagerResource.GetString("No"),
                        IsHidden = updateInformation.IsHidden ? ResourceService.UpdateManagerResource.GetString("Yes") : ResourceService.UpdateManagerResource.GetString("No"),
                        IsInstalled = updateInformation.IsInstalled ? ResourceService.UpdateManagerResource.GetString("Yes") : ResourceService.UpdateManagerResource.GetString("No"),
                        IsMandatory = updateInformation.IsMandatory ? ResourceService.UpdateManagerResource.GetString("Yes") : ResourceService.UpdateManagerResource.GetString("No"),
                        IsUninstallable = updateInformation.IsUninstallable ? ResourceService.UpdateManagerResource.GetString("Yes") : ResourceService.UpdateManagerResource.GetString("No"),
                        MaxDownloadSize = FileSizeHelper.ConvertFileSizeToString(Convert.ToDouble(updateInformation.Update.MaxDownloadSize)),
                        MinDownloadSize = FileSizeHelper.ConvertFileSizeToString(Convert.ToDouble(updateInformation.Update.MinDownloadSize)),
                        MsrcSeverity = updateInformation.MsrcSeverity,
                        RecommendedCpuSpeed = updateInformation.RecommendedCpuSpeed.Equals(0) ? ResourceService.UpdateManagerResource.GetString("Unknown") : string.Format("{0} MHz", updateInformation.RecommendedCpuSpeed),
                        RecommendedHardDiskSpace = updateInformation.RecommendedHardDiskSpace.Equals(0) ? ResourceService.UpdateManagerResource.GetString("Unknown") : string.Format("{0} MB", updateInformation.RecommendedHardDiskSpace),
                        RecommendedMemory = updateInformation.RecommendedHardDiskSpace.Equals(0) ? ResourceService.UpdateManagerResource.GetString("Unknown") : string.Format("{0} MB", updateInformation.RecommendedMemory),
                        RebootRequired = updateInformation.IsUninstallable ? ResourceService.UpdateManagerResource.GetString("Yes") : ResourceService.UpdateManagerResource.GetString("No"),
                        ReleaseNotes = updateInformation.ReleaseNotes,
                        SupportURL = updateInformation.SupportURL,
                        Title = updateInformation.Title,
                        UpdateID = updateInformation.UpdateID,
                        IsUpdating = false,
                        UpdateProgress = string.Empty,
                    };

                    if (updateInformation.UpdateType is UpdateType.utSoftware)
                    {
                        updateItem.UpdateType = ResourceService.UpdateManagerResource.GetString("UpdateTypeSoftware");
                    }
                    else if (updateInformation.UpdateType is UpdateType.utDriver)
                    {
                        updateItem.UpdateType = ResourceService.UpdateManagerResource.GetString("UpdateTypeDriver");
                    }
                    else
                    {
                        updateItem.UpdateType = ResourceService.UpdateManagerResource.GetString("Unknown");
                    }

                    updateItem.CveIDList.AddRange(updateInformation.CveIDList);
                    updateItem.KBArticleIDList.AddRange(updateInformation.KBArticleIDList);
                    updateItem.LanguageList.AddRange(updateInformation.LanguageList);
                    updateItem.MoreInfoList.AddRange(updateInformation.MoreInfoList);

                    // 隐藏的更新
                    if (updateInformation.Update.IsHidden)
                    {
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

                synchronizationContext.Post(_ =>
                {
                    AvailableUpdateCollection.Clear();
                    foreach (UpdateModel updateItem in availableUpdateList)
                    {
                        AvailableUpdateCollection.Add(updateItem);
                    }

                    InstalledUpdateCollection.Clear();
                    foreach (UpdateModel updateItem in installedUpdateList)
                    {
                        InstalledUpdateCollection.Add(updateItem);
                    }

                    HiddenUpdateCollection.Clear();
                    foreach (UpdateModel updateItem in hiddenUpdateList)
                    {
                        HiddenUpdateCollection.Add(updateItem);
                    }

                    IsCheckUpdateEnabled = true;
                    IsChecking = false;
                    IsAUExpanderExpanded = true;
                    IsIUExpanderExpanded = true;
                    IsHUExpanderExpanded = true;

                    GetUpdateHistory();
                }, null);
            }
        }

        #endregion 第三部分：自定义事件

        /// <summary>
        /// 检查更新
        /// </summary>
        private void CheckUpdate()
        {
            Task.Run(() =>
            {
                // 设置搜索结果中是否接收已替代的更新
                updateSearcher.IncludePotentiallySupersededUpdates = IsIncludePotentiallySupersededUpdate;
                updateSearcher.ServerSelection = ServerSelection.ssDefault;

                // 设置更新源
                foreach (IUpdateService updateService in updateServiceManager.Services)
                {
                    if (updateService.Name.Equals(SelectedUpdateSource.Key))
                    {
                        updateSearcher.ServiceID = updateService.ServiceID;
                    }
                }

                try
                {
                    string query = "(IsInstalled = 0 and IsHidden = 0 and DeploymentAction=*) or (IsInstalled = 1 and IsHidden = 0 and DeploymentAction=*) or (IsHidden = 1 and DeploymentAction=*)";
                    SearchCompletedCallback searchCompletedCallback = new();
                    searchCompletedCallback.SearchCompleted += OnSearchCompleted;
                    updateSearcher.BeginSearch(query, searchCompletedCallback, null);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Search update failed", e);
                    synchronizationContext.Post(_ =>
                    {
                        IsChecking = false;
                        IsAUExpanderExpanded = true;
                        IsIUExpanderExpanded = true;
                        IsHUExpanderExpanded = true;
                        IsCheckUpdateEnabled = true;
                    }, null);
                }
            });
        }

        /// <summary>
        /// 获取更新历史记录
        /// </summary>
        private void GetUpdateHistory()
        {
            UpdateHistoryCollection.Clear();

            Task.Run(() =>
            {
                int updateHistoryCount = updateSearcher.GetTotalHistoryCount();

                if (updateHistoryCount > 0)
                {
                    foreach (IUpdateHistoryEntry2 updateHistoryEntry in updateSearcher.QueryHistory(0, updateHistoryCount))
                    {
                        if (!string.IsNullOrEmpty(updateHistoryEntry.Title))
                        {
                            string status = GetStatus(updateHistoryEntry.ResultCode, updateHistoryEntry.HResult);

                            synchronizationContext.Post(_ =>
                            {
                                // TODO : 未完成
                                UpdateModel updateItem = new()
                                {
                                    Title = updateHistoryEntry.Title,
                                    Description = updateHistoryEntry.Description,
                                    UpdateID = updateHistoryEntry.UpdateIdentity.UpdateID,
                                    SupportURL = updateHistoryEntry.SupportUrl,
                                };

                                UpdateHistoryCollection.Add(updateItem);
                            }, null);
                        }
                    }
                }

                synchronizationContext.Post(_ =>
                {
                    IsUHExpanderExpanded = true;
                }, null);
            });
        }

        /// <summary>
        /// 获取更新的安装状态
        /// </summary>
        private string GetStatus(OperationResultCode operationResultCode, int hResult)
        {
            switch (operationResultCode)
            {
                case OperationResultCode.orcNotStarted:
                    {
                        return string.Format("{0} 0x{1:X8}", ResourceService.UpdateManagerResource.GetString("NotStarted"), hResult);
                    }
                case OperationResultCode.orcInProgress:
                    {
                        return string.Format("{0} 0x{1:X8}", ResourceService.UpdateManagerResource.GetString("InProgress"), hResult);
                    }
                case OperationResultCode.orcSucceeded:
                    {
                        return string.Format("{0} 0x{1:X8}", ResourceService.UpdateManagerResource.GetString("Succeeded"), hResult);
                    }
                case OperationResultCode.orcSucceededWithErrors:
                    {
                        return string.Format("{0} 0x{1:X8}", ResourceService.UpdateManagerResource.GetString("SucceedWithErrors"), hResult);
                    }
                case OperationResultCode.orcFailed:
                    {
                        return string.Format("{0} 0x{1:X8}", ResourceService.UpdateManagerResource.GetString("Failed"), hResult);
                    }
                case OperationResultCode.orcAborted:
                    {
                        return string.Format("{0} 0x{1:X8}", ResourceService.UpdateManagerResource.GetString("Aborted"), hResult);
                    }
                default:
                    {
                        return string.Empty;
                    }
            }
        }
    }
}
