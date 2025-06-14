﻿using Microsoft.UI.Xaml.Controls;
using PowerTools.Extensions.DataType.Enums;
using PowerTools.Models;
using PowerTools.Services.Root;
using PowerTools.Services.Settings;
using PowerTools.Views.Dialogs;
using PowerTools.Views.TeachingTips;
using PowerTools.Views.Windows;
using PowerTools.WindowsAPI.ComTypes;
using PowerTools.WindowsAPI.PInvoke.Rstrtmgr;
using PowerTools.WindowsAPI.PInvoke.Shell32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// 抑制 CA1806，IDE0060 警告
#pragma warning disable CA1806,IDE0060

namespace PowerTools.Views.Pages
{
    /// <summary>
    /// 设置页面
    /// </summary>
    public sealed partial class SettingsPage : Page, INotifyPropertyChanged
    {
        private KeyValuePair<string, string> _theme = default;

        public KeyValuePair<string, string> Theme
        {
            get { return _theme; }

            set
            {
                if (!Equals(_theme, value))
                {
                    _theme = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Theme)));
                }
            }
        }

        private KeyValuePair<string, string> _backdrop = default;

        public KeyValuePair<string, string> Backdrop
        {
            get { return _backdrop; }

            set
            {
                if (!Equals(_backdrop, value))
                {
                    _backdrop = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Backdrop)));
                }
            }
        }

        private bool _alwaysShowBackdropValue = AlwaysShowBackdropService.AlwaysShowBackdropValue;

        public bool AlwaysShowBackdropValue
        {
            get { return _alwaysShowBackdropValue; }

            set
            {
                if (!Equals(_alwaysShowBackdropValue, value))
                {
                    _alwaysShowBackdropValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AlwaysShowBackdropValue)));
                }
            }
        }

        private KeyValuePair<string, string> _appLanguage = LanguageService.AppLanguage;

        public KeyValuePair<string, string> AppLanguage
        {
            get { return _appLanguage; }

            set
            {
                if (!Equals(_appLanguage, value))
                {
                    _appLanguage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AppLanguage)));
                }
            }
        }

        private bool _topMostValue = TopMostService.TopMostValue;

        public bool TopMostValue
        {
            get { return _topMostValue; }

            set
            {
                if (!Equals(_topMostValue, value))
                {
                    _topMostValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TopMostValue)));
                }
            }
        }

        private string _downloadFolder = DownloadOptionsService.DownloadFolder;

        public string DownloadFolder
        {
            get { return _downloadFolder; }

            set
            {
                if (!Equals(_downloadFolder, value))
                {
                    _downloadFolder = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DownloadFolder)));
                }
            }
        }

        private KeyValuePair<string, string> _doEngineMode = default;

        public KeyValuePair<string, string> DoEngineMode
        {
            get { return _doEngineMode; }

            set
            {
                if (!Equals(_doEngineMode, value))
                {
                    _doEngineMode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DoEngineMode)));
                }
            }
        }

        private bool _isRestarting = false;

        public bool IsRestarting
        {
            get { return _isRestarting; }

            set
            {
                if (!Equals(_isRestarting, value))
                {
                    _isRestarting = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRestarting)));
                }
            }
        }

        private bool _fileShellMenuValue = FileShellMenuService.FileShellMenuValue;

        public bool FileShellMenuValue
        {
            get { return _fileShellMenuValue; }

            set
            {
                if (!Equals(_fileShellMenuValue, value))
                {
                    _fileShellMenuValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FileShellMenuValue)));
                }
            }
        }

        private List<KeyValuePair<string, string>> ThemeList { get; } = [];

        private List<KeyValuePair<string, string>> BackdropList { get; } = [];

        private List<KeyValuePair<string, string>> DoEngineModeList { get; } = [];

        private ObservableCollection<LanguageModel> LanguageCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsPage()
        {
            InitializeComponent();

            foreach (KeyValuePair<string, string> languageItem in LanguageService.LanguageList)
            {
                if (LanguageService.AppLanguage.Key.Equals(languageItem.Key))
                {
                    AppLanguage = languageItem;
                    LanguageCollection.Add(new LanguageModel()
                    {
                        LangaugeInfo = languageItem,
                        IsChecked = true
                    });
                }
                else
                {
                    LanguageCollection.Add(new LanguageModel()
                    {
                        LangaugeInfo = languageItem,
                        IsChecked = false
                    });
                }
            }
        }

        #region 第一部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 修改应用语言
        /// </summary>
        private async void OnLanguageExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (LanguageFlyout.IsOpen)
            {
                LanguageFlyout.Hide();
            }

            if (args.Parameter is LanguageModel languageItem)
            {
                foreach (LanguageModel item in LanguageCollection)
                {
                    item.IsChecked = false;
                    if (languageItem.LangaugeInfo.Key.Equals(item.LangaugeInfo.Key))
                    {
                        AppLanguage = item.LangaugeInfo;
                        item.IsChecked = true;
                    }
                }

                LanguageService.SetLanguage(AppLanguage);
                await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.LanguageChange));
            }
        }

        #endregion 第一部分：XamlUICommand 命令调用时挂载的事件

        #region 第二部分：设置页面——挂载的事件

        /// <summary>
        /// 打开重启应用确认的窗口对话框
        /// </summary>
        private async void OnRestartAppsClicked(object sender, RoutedEventArgs args)
        {
            await MainWindow.Current.ShowDialogAsync(new RestartAppsDialog());
        }

        /// <summary>
        /// 设置说明
        /// </summary>
        private void OnSettingsInstructionClicked(object sender, RoutedEventArgs args)
        {
            (MainWindow.Current.Content as MainPage).NavigateTo(typeof(AboutPage));
        }

        /// <summary>
        /// 背景色修改设置
        /// </summary>
        private void OnBackdropSelectClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is not null)
            {
                Backdrop = BackdropList[Convert.ToInt32(radioMenuFlyoutItem.Tag)];
                BackdropService.SetBackdrop(Backdrop.Key);
            }
        }

        /// <summary>
        /// 打开系统主题设置
        /// </summary>
        private void OnSystemThemeSettingsClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("ms-settings:colors");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Open system theme settings failed", e);
                }
            });
        }

        /// <summary>
        /// 打开系统主题色设置
        /// </summary>
        private void OnSystemBackdropSettingsClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("ms-settings:easeofaccess-visualeffects");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Open system backdrop settings failed", e);
                }
            });
        }

        /// <summary>
        /// 打开系统语言设置
        /// </summary>
        private void OnSystemLanguageSettingsClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("ms-settings:regionlanguage-languageoptions");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Open system language settings failed", e);
                }
            });
        }

        /// <summary>
        /// 主题修改设置
        /// </summary>
        private void OnThemeSelectClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is not null)
            {
                Theme = ThemeList[Convert.ToInt32(radioMenuFlyoutItem.Tag)];
                ThemeService.SetTheme(Theme.Key);
            }
        }

        /// <summary>
        /// 下载引擎说明
        /// </summary>
        private void OnLearnDoEngineClicked(object sender, RoutedEventArgs args)
        {
            (MainWindow.Current.Content as MainPage).NavigateTo(typeof(AboutPage));
        }

        /// <summary>
        /// 下载引擎方式设置
        /// </summary>

        private void OnDoEngineModeSelectClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is not null)
            {
                DoEngineMode = DoEngineModeList[Convert.ToInt32(radioMenuFlyoutItem.Tag)];
                DownloadOptionsService.SetDoEngineMode(DoEngineMode.Key);
            }
        }

        /// <summary>
        /// 重新启动资源管理器
        /// </summary>
        private async void OnRestartExplorerClicked(object sender, RoutedEventArgs args)
        {
            IsRestarting = true;

            await Task.Run(() =>
            {
                try
                {
                    int dwRmStatus = RstrtmgrLibrary.RmStartSession(out uint dwSessionHandle, 0, Guid.Empty.ToString());

                    if (dwRmStatus is 0)
                    {
                        Process[] processList = Process.GetProcessesByName("explorer");
                        RM_UNIQUE_PROCESS[] lpRmProcList = new RM_UNIQUE_PROCESS[processList.Length];

                        for (int index = 0; index < processList.Length; index++)
                        {
                            lpRmProcList[index].dwProcessId = processList[index].Id;
                            FILETIME fileTime = new();
                            long time = processList[index].StartTime.ToFileTime();
                            fileTime.dwLowDateTime = (int)(time & 0xFFFFFFFF);
                            fileTime.dwHighDateTime = (int)(time >> 32);
                            lpRmProcList[index].ProcessStartTime = fileTime;
                        }

                        dwRmStatus = RstrtmgrLibrary.RmRegisterResources(dwSessionHandle, 0, null, (uint)processList.Length, lpRmProcList, 0, null);

                        if (dwRmStatus is 0)
                        {
                            dwRmStatus = RstrtmgrLibrary.RmShutdown(dwSessionHandle, RM_SHUTDOWN_TYPE.RmForceShutdown, null);

                            if (dwRmStatus is 0)
                            {
                                dwRmStatus = RstrtmgrLibrary.RmRestart(dwSessionHandle, 0, null);

                                if (dwRmStatus is 0)
                                {
                                    dwRmStatus = RstrtmgrLibrary.RmEndSession(dwSessionHandle);
                                }
                                else
                                {
                                    LogService.WriteLog(EventLevel.Error, "Restart explorer restart failed", new Exception());
                                }
                            }
                            else
                            {
                                LogService.WriteLog(EventLevel.Error, "Restart explorer shutdown failed", new Exception());
                            }
                        }
                        else
                        {
                            LogService.WriteLog(EventLevel.Error, "Restart explorer register resources failed", new Exception());
                        }
                    }
                    else
                    {
                        LogService.WriteLog(EventLevel.Error, "Restart explorer start session failed", new Exception());
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Restart explorer failed", e);
                }
            });

            IsRestarting = false;
        }

        /// <summary>
        /// 是否开启始终显示背景色
        /// </summary>
        private void OnAlwaysShowBackdropToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                AlwaysShowBackdropService.SetAlwaysShowBackdropValue(toggleSwitch.IsOn);
                AlwaysShowBackdropValue = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 语言设置菜单打开时自动定位到选中项
        /// </summary>
        private void OnOpened(object sender, object args)
        {
            foreach (LanguageModel languageItem in LanguageCollection)
            {
                if (languageItem.IsChecked)
                {
                    LanguageListView.ScrollIntoView(languageItem);
                    break;
                }
            }
        }

        /// <summary>
        /// 打开文件存放目录
        /// </summary>
        private void OnOpenFolderClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start(DownloadFolder);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Open saved folder failed", e);
                }
            });
        }

        /// <summary>
        /// 修改下载目录
        /// </summary>
        private void OnChangeFolderClicked(object sender, RoutedEventArgs args)
        {
            if (sender is MenuFlyoutItem menuFlyoutItem && menuFlyoutItem.Tag is not null)
            {
                switch ((string)menuFlyoutItem.Tag)
                {
                    case "Download":
                        {
                            Shell32Library.SHGetKnownFolderPath(new("374DE290-123F-4565-9164-39C4925E467B"), KNOWN_FOLDER_FLAG.KF_FLAG_DEFAULT, IntPtr.Zero, out string downloadFolder);
                            DownloadFolder = downloadFolder;
                            DownloadOptionsService.SetFolder(DownloadFolder);
                            break;
                        }
                    case "Desktop":
                        {
                            DownloadFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                            DownloadOptionsService.SetFolder(DownloadFolder);
                            break;
                        }
                    case "Custom":
                        {
                            OpenFolderDialog dialog = new()
                            {
                                Description = ResourceService.SettingsResource.GetString("SelectFolder"),
                                RootFolder = Environment.SpecialFolder.Desktop
                            };
                            DialogResult result = dialog.ShowDialog();
                            if (result is DialogResult.OK || result is DialogResult.Yes)
                            {
                                DownloadFolder = dialog.SelectedPath;
                                DownloadOptionsService.SetFolder(DownloadFolder);
                            }
                            dialog.Dispose();
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 是否开启显示文件右键菜单
        /// </summary>
        private void OnFileShellMenuToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                FileShellMenuService.SetFileShellMenuValue(toggleSwitch.IsOn);
                FileShellMenuValue = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 是否开启应用窗口置顶
        /// </summary>
        private void OnTopMostToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                TopMostService.SetTopMostValue(toggleSwitch.IsOn);
                TopMostValue = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 打开日志文件夹
        /// </summary>
        private void OnOpenLogFolderClicked(object sender, RoutedEventArgs args)
        {
            LogService.OpenLogFolder();
        }

        /// <summary>
        /// 清除所有日志记录
        /// </summary>
        private async void OnClearClicked(object sender, RoutedEventArgs args)
        {
            bool result = await LogService.ClearLogAsync();
            await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.LogClean, result));
        }

        /// <summary>
        /// 当应用未启用背景色设置时，自动关闭始终显示背景色设置
        /// </summary>
        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is ToggleSwitch)
            {
                AlwaysShowBackdropService.SetAlwaysShowBackdropValue(false);
                AlwaysShowBackdropValue = false;
            }
        }

        #endregion 第二部分：设置页面——挂载的事件

        private string LocalizeDisplayNumber(KeyValuePair<string, string> selectedBackdrop)
        {
            int index = BackdropList.FindIndex(item => item.Key.Equals(selectedBackdrop.Key));

            if (index is 0)
            {
                return selectedBackdrop.Value;
            }
            else if (index is 1 || index is 2)
            {
                return string.Format("{0} {1}", ResourceService.SettingsResource.GetString("Mica"), selectedBackdrop.Value);
            }
            else if (index is 3 || index is 4 || index is 5)
            {
                return string.Format("{0} {1}", ResourceService.SettingsResource.GetString("DesktopAcrylic"), selectedBackdrop.Value);
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
