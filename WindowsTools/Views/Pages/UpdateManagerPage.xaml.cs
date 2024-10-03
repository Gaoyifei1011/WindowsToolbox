using Microsoft.Win32;
using System;
using System.Collections;
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
using Windows.UI.Xaml.Input;
using WindowsTools.Extensions.DataType.Enums;
using WindowsTools.Helpers.Controls;
using WindowsTools.Helpers.Root;
using WindowsTools.Models;
using WindowsTools.Services.Root;
using WindowsTools.Strings;
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

        private bool _isAUExpanderExpanded;

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

        private bool _isIUExpanderExpanded;

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

        private bool _isHUExpanderExpanded;

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

        private bool _isUHExpanderExpanded;

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

        private DictionaryEntry _selectedUpdateSource;

        public DictionaryEntry SelectedUpdateSource
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

        private DictionaryEntry _selectedPreviewChannel;

        public DictionaryEntry SelectedPreviewChannel
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

        private List<DictionaryEntry> UpdateSourceList { get; } =
        [
            new DictionaryEntry(UpdateManager.MicrosoftUpdate, "Microsoft Update"),
            new DictionaryEntry(UpdateManager.DCatFlightingProd, "DCat Flighting Prod"),
            new DictionaryEntry(UpdateManager.WindowsStore, "Windows Store(DCat Prod)"),
            new DictionaryEntry(UpdateManager.WindowsUpdate, "Windows Update"),
        ];

        private List<DictionaryEntry> PreviewChannelList { get; } =
        [
            new DictionaryEntry(UpdateManager.DonotEnter, "DoNotEnter"),
            new DictionaryEntry(UpdateManager.ReleasePreview, "ReleasePreview"),
            new DictionaryEntry(UpdateManager.Beta, "Beta"),
            new DictionaryEntry(UpdateManager.Dev, "Dev"),
            new DictionaryEntry(UpdateManager.Canary, "Canary"),
        ];

        private ObservableCollection<UpdateModel> AvailableUpdateCollection { get; } = [];

        private ObservableCollection<UpdateModel> InstalledUpdateCollection { get; } = [];

        private ObservableCollection<UpdateModel> HiddenUpdateCollection { get; } = [];

        private ObservableCollection<UpdateModel> UpdateHistoryCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public UpdateManagerPage()
        {
            InitializeComponent();
            SelectedPreviewChannel = PreviewChannelList[0];
            SelectedUpdateSource = UpdateSourceList[0];
            updateSession.ClientApplicationID = "Update Manager for Windows";
            updateSearcher = updateSession.CreateUpdateSearcher();
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
        /// 可用更新：隐藏
        /// </summary>
        private void OnAvailableHideExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is UpdateModel updateItem)
            {
                Task.Run(() =>
                {
                    // 隐藏更新（只有 Power Users 管理组的管理员和成员才能设置此属性的值）
                    if (RuntimeHelper.IsElevated)
                    {
                        try
                        {
                            updateItem.Update.IsHidden = true;

                            synchronizationContext.Post(_ =>
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

                                    HiddenUpdateCollection.Add(updateItem);

                                    IsAUExpanderExpanded = true;
                                    IsHUExpanderExpanded = true;
                                }
                                catch (Exception e)
                                {
                                    LogService.WriteLog(EventLevel.Error, "Hide updates update UI failed", e);
                                }
                            }, null);
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, "Hide updates modify hidden property failed", e);
                        }
                    }
                });
            }
        }

        /// <summary>
        /// 可用更新：安装
        /// </summary>
        private void OnAvailableInstallExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
        }

        /// <summary>
        /// 已安装更新：卸载
        /// </summary>
        private void OnInstalledUnInstallExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is UpdateModel updateItem)
            {
                IUpdateInstaller updateInstaller = updateSession.CreateUpdateInstaller();
                updateInstaller.Updates.Add(updateItem.Update);
                InstallationCompletedCallback installationCompletedCallback = new();
                InstallationProgressChangedCallback installationProgressChangedCallback = new();

                // 安装完成，更新信息并重新检查更新
                installationCompletedCallback.InstallationCompleted += (sender, args) =>
                {
                    CheckUpdate();
                };

                // 更新进行中，进度变化时更改 UI 变化信息
                installationProgressChangedCallback.InstallationProgressChanged += (sender, args) =>
                {
                    double percentage = installationProgressChangedCallback.CallbackArgs.Progress.CurrentUpdatePercentComplete / 100.0;
                    synchronizationContext.Post(_ =>
                    {
                        foreach (UpdateModel installedItem in InstalledUpdateCollection)
                        {
                            if (installedItem.UpdateID.Equals(updateItem.UpdateID))
                            {
                                installedItem.InstallationProgress = percentage;
                            }
                        }
                    }, null);
                };
                updateInstaller.BeginUninstall(installationProgressChangedCallback, installationCompletedCallback, null);
            }
        }

        /// <summary>
        /// 隐藏更新：显示
        /// </summary>
        private void OnHiddenShowExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is UpdateModel updateItem)
            {
                Task.Run(() =>
                {
                    // 隐藏更新（只有 Power Users 管理组的管理员和成员才能设置此属性的值）
                    if (RuntimeHelper.IsElevated)
                    {
                        try
                        {
                            updateItem.Update.IsHidden = false;

                            synchronizationContext.Post(_ =>
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

                                    AvailableUpdateCollection.Add(updateItem);

                                    IsAUExpanderExpanded = true;
                                    IsHUExpanderExpanded = true;
                                }
                                catch (Exception e)
                                {
                                    LogService.WriteLog(EventLevel.Error, "Show updates update UI failed", e);
                                }
                            }, null);
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, "Show updates modify hidden property failed", e);
                        }
                    }
                });
            }
        }

        /// <summary>
        /// 更新历史记录，复制更新描述信息
        /// </summary>
        private void OnCopyInformationExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is UpdateModel updateItem)
            {
                Task.Run(() =>
                {
                    StringBuilder copyInformationBuilder = new();
                    copyInformationBuilder.AppendLine(UpdateManager.Title);
                    copyInformationBuilder.AppendLine(updateItem.UpdateName);
                    copyInformationBuilder.AppendLine(UpdateManager.Description);
                    copyInformationBuilder.AppendLine(updateItem.Description);

                    synchronizationContext.Post(async (_) =>
                    {
                        bool copyResult = CopyPasteHelper.CopyToClipboard(copyInformationBuilder.ToString());
                        await TeachingTipHelper.ShowAsync(new DataCopyTip(DataCopyKind.UpdateInformation, copyResult));
                    }, null);
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
                    Process.Start(supportUrl);
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
                Process.Start("ms-settings:windowsupdate");
            });
        }

        /// <summary>
        /// 打开预览体验计划设置
        /// </summary>
        private void OnWIPSettingsClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                Process.Start("ms-settings:windowsinsider");
            });
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
        }

        /// <summary>
        /// 可用更新：隐藏
        /// </summary>
        private void OnAvailableHideClicked(object sender, RoutedEventArgs args)
        {
            List<UpdateModel> hideList = AvailableUpdateCollection.Where(item => item.IsSelected is true).ToList();
            Task.Run(() =>
            {
                if (RuntimeHelper.IsElevated)
                {
                    foreach (UpdateModel hideItem in hideList)
                    {
                        try
                        {
                            hideItem.Update.IsHidden = true;
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, "Hide updates modify hidden property failed", e);
                            continue;
                        }
                    }

                    synchronizationContext.Post(_ =>
                    {
                        foreach (UpdateModel hideItem in hideList)
                        {
                            try
                            {
                                if (hideItem.Update.IsHidden)
                                {
                                    for (int index = 0; index < AvailableUpdateCollection.Count; index++)
                                    {
                                        if (AvailableUpdateCollection[index].UpdateID.Equals(hideItem.UpdateID))
                                        {
                                            AvailableUpdateCollection.RemoveAt(index);
                                            break;
                                        }
                                    }

                                    HiddenUpdateCollection.Add(hideItem);

                                    IsAUExpanderExpanded = true;
                                    IsHUExpanderExpanded = true;
                                }
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(EventLevel.Error, "Hide updates update UI failed", e);
                            }
                        }
                    }, null);
                }
            });
        }

        /// <summary>
        /// 可用更新：安装
        /// </summary>
        private void OnAvailableInstallClicked(object sender, RoutedEventArgs args)
        {
            List<UpdateModel> installList = AvailableUpdateCollection.Where(item => item.IsSelected is true).ToList();
            foreach (UpdateModel hideItem in installList)
            {
            }
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
        }

        /// <summary>
        /// 已安装更新：卸载
        /// </summary>
        private void OnInstalledUnInstallClicked(object sender, RoutedEventArgs args)
        {
            List<UpdateModel> unInstallList = InstalledUpdateCollection.Where(item => item.IsSelected is true).ToList();
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
        }

        /// <summary>
        /// 隐藏更新：显示
        /// </summary>
        private void OnHiddenShowClicked(object sender, RoutedEventArgs args)
        {
            List<UpdateModel> showList = HiddenUpdateCollection.Where(item => item.IsSelected is true).ToList();
            Task.Run(() =>
            {
                if (RuntimeHelper.IsElevated)
                {
                    foreach (UpdateModel showItem in showList)
                    {
                        try
                        {
                            showItem.Update.IsHidden = false;
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, "Show updates modify hidden property failed", e);
                            continue;
                        }
                    }

                    synchronizationContext.Post(_ =>
                    {
                        foreach (UpdateModel showItem in showList)
                        {
                            try
                            {
                                if (showItem.Update.IsHidden is false)
                                {
                                    for (int index = 0; index < HiddenUpdateCollection.Count; index++)
                                    {
                                        if (HiddenUpdateCollection[index].UpdateID.Equals(showItem.UpdateID))
                                        {
                                            HiddenUpdateCollection.RemoveAt(index);
                                            break;
                                        }
                                    }

                                    AvailableUpdateCollection.Add(showItem);

                                    IsAUExpanderExpanded = true;
                                    IsHUExpanderExpanded = true;
                                }
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(EventLevel.Error, "Show updates update UI failed", e);
                            }
                        }
                    }, null);
                }
            });
        }

        /// <summary>
        /// 检查更新
        /// </summary>
        private void OnCheckUpdateClicked(object sender, RoutedEventArgs args)
        {
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
                        synchronizationContext.Post(_ =>
                        {
                            IsExcludeDrivers = !IsExcludeDrivers;
                        }, null);
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
                foreach (IUpdate update in searchResult.Updates)
                {
                    // 隐藏的更新
                    if (update.IsHidden)
                    {
                        hiddenUpdateList.Add(new UpdateModel()
                        {
                            UpdateName = update.Title,
                            Update = update,
                            Description = update.Description,
                            Date = update.LastDeploymentChangeTime,
                            UpdateID = update.Identity.UpdateID,
                            Size = FileSizeHelper.ConvertFileSizeToString(Convert.ToDouble(update.MaxDownloadSize)),
                            SupportURL = update.SupportUrl
                        });
                    }
                    // 已安装的更新
                    else if (update.IsInstalled)
                    {
                        installedUpdateList.Add(new UpdateModel()
                        {
                            UpdateName = update.Title,
                            Update = update,
                            Description = update.Description,
                            Date = update.LastDeploymentChangeTime,
                            UpdateID = update.Identity.UpdateID,
                            Size = FileSizeHelper.ConvertFileSizeToString(Convert.ToDouble(update.MaxDownloadSize)),
                            SupportURL = update.SupportUrl
                        });
                    }
                    // 可用更新
                    else
                    {
                        availableUpdateList.Add(new UpdateModel()
                        {
                            UpdateName = update.Title,
                            Update = update,
                            Description = update.Description,
                            Date = update.LastDeploymentChangeTime,
                            UpdateID = update.Identity.UpdateID,
                            Size = FileSizeHelper.ConvertFileSizeToString(Convert.ToDouble(update.MaxDownloadSize)),
                            SupportURL = update.SupportUrl
                        });
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

                    IsChecking = false;
                    IsAUExpanderExpanded = true;
                    IsIUExpanderExpanded = true;
                    IsHUExpanderExpanded = true;
                }, null);

                GetUpdateHistory();
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
                    if (updateService.Name.Equals(SelectedUpdateSource.Value))
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
                                UpdateHistoryCollection.Add(new UpdateModel()
                                {
                                    UpdateName = updateHistoryEntry.Title,
                                    ApplicationID = updateHistoryEntry.ClientApplicationID,
                                    Date = updateHistoryEntry.Date,
                                    UpdateID = updateHistoryEntry.UpdateIdentity.UpdateID,
                                    Description = updateHistoryEntry.Description,
                                    SupportURL = updateHistoryEntry.SupportUrl,
                                    Status = status
                                });
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
                        return string.Format("{0} 0x{1:X8}", UpdateManager.NotStarted, hResult);
                    }
                case OperationResultCode.orcInProgress:
                    {
                        return string.Format("{0} 0x{1:X8}", UpdateManager.InProgress, hResult);
                    }
                case OperationResultCode.orcSucceeded:
                    {
                        return string.Format("{0} 0x{1:X8}", UpdateManager.Succeeded, hResult);
                    }
                case OperationResultCode.orcSucceededWithErrors:
                    {
                        return string.Format("{0} 0x{1:X8}", UpdateManager.SucceedWithErrors, hResult);
                    }
                case OperationResultCode.orcFailed:
                    {
                        return string.Format("{0} 0x{1:X8}", UpdateManager.Failed, hResult);
                    }
                case OperationResultCode.orcAborted:
                    {
                        return string.Format("{0} 0x{1:X8}", UpdateManager.Aborted, hResult);
                    }
                default:
                    {
                        return string.Empty;
                    }
            }
        }
    }
}
