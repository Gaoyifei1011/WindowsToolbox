﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WindowsTools.Extensions.DataType.Enums;
using WindowsTools.Helpers.Controls;
using WindowsTools.Helpers.Controls.Extensions;
using WindowsTools.Services.Controls.Settings;
using WindowsTools.Services.Root;
using WindowsTools.Strings;
using WindowsTools.UI.Dialogs;
using WindowsTools.UI.TeachingTips;
using WindowsTools.Views.Windows;
using WindowsTools.WindowsAPI.PInvoke.Shell32;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace WindowsTools.Views.Pages
{
    /// <summary>
    /// 设置页面
    /// </summary>
    public sealed partial class SettingsPage : Page, INotifyPropertyChanged
    {
        private DictionaryEntry _theme = ThemeService.AppTheme;

        public DictionaryEntry Theme
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

        private DictionaryEntry _backdrop = BackdropService.AppBackdrop;

        public DictionaryEntry Backdrop
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

        private DictionaryEntry _appLanguage = LanguageService.AppLanguage;

        public DictionaryEntry AppLanguage
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

        private DictionaryEntry _doEngineMode = DownloadOptionsService.DoEngineMode;

        public DictionaryEntry DoEngineMode
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

        private DictionaryEntry _exitMode = ExitModeService.ExitMode;

        public DictionaryEntry ExitMode
        {
            get { return _exitMode; }

            set
            {
                if (!Equals(_exitMode, value))
                {
                    _exitMode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ExitMode)));
                }
            }
        }

        private List<DictionaryEntry> ThemeList { get; } = ThemeService.ThemeList;

        private List<DictionaryEntry> BackdropList { get; } = BackdropService.BackdropList;

        private List<DictionaryEntry> LanguageList { get; } = LanguageService.LanguageList;

        private List<DictionaryEntry> DoEngineModeList { get; } = DownloadOptionsService.DoEngineModeList;

        private List<DictionaryEntry> ExitModeList { get; } = ExitModeService.ExitModeList;

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsPage()
        {
            InitializeComponent();

            for (int index = 0; index < LanguageList.Count; index++)
            {
                DictionaryEntry languageItem = LanguageList[index];
                ToggleMenuFlyoutItem toggleMenuFlyoutItem = new()
                {
                    Text = languageItem.Key.ToString(),
                    Height = 32,
                    Padding = new Thickness(11, 0, 11, 0),
                    Tag = index
                };

                if (AppLanguage.Value.Equals(LanguageList[index].Value))
                {
                    toggleMenuFlyoutItem.IsChecked = true;
                }

                toggleMenuFlyoutItem.Click += (sender, args) =>
                {
                    foreach (MenuFlyoutItemBase menuFlyoutItemBase in LanguageFlyout.Items)
                    {
                        ToggleMenuFlyoutItem toggleMenuFlyoutItem = menuFlyoutItemBase as ToggleMenuFlyoutItem;
                        if (toggleMenuFlyoutItem is not null && toggleMenuFlyoutItem.IsChecked)
                        {
                            toggleMenuFlyoutItem.IsChecked = false;
                        }
                    }

                    int selectedIndex = Convert.ToInt32((sender as ToggleMenuFlyoutItem).Tag);
                    (LanguageFlyout.Items[selectedIndex] as ToggleMenuFlyoutItem).IsChecked = true;

                    if (AppLanguage.Value.ToString() != LanguageList[selectedIndex].Value.ToString())
                    {
                        AppLanguage = LanguageList[selectedIndex];
                        LanguageService.SetLanguage(AppLanguage);
                        TeachingTipHelper.Show(new OperationResultTip(OperationKind.LanguageChange));
                    }
                };
                LanguageFlyout.Items.Add(toggleMenuFlyoutItem);
            }
        }

        #region 第一部分：设置页面——挂载的事件

        /// <summary>
        /// 打开重启应用确认的窗口对话框
        /// </summary>
        private async void OnRestartAppsClicked(object sender, RoutedEventArgs args)
        {
            await ContentDialogHelper.ShowAsync(new RestartAppsDialog(), this);
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
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            if (item.Tag is not null)
            {
                Backdrop = BackdropList[Convert.ToInt32(item.Tag)];
                BackdropService.SetBackdrop(Backdrop);
            }
        }

        /// <summary>
        /// 打开系统主题设置
        /// </summary>
        private void OnSystemThemeSettingsClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                Process.Start("ms-settings:colors");
            });
        }

        /// <summary>
        /// 打开系统主题色设置
        /// </summary>
        private void OnSystemBackdropSettingsClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                Process.Start("ms-settings:easeofaccess-visualeffects");
            });
        }

        /// <summary>
        /// 打开系统语言设置
        /// </summary>
        private void OnSystemLanguageSettingsClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                Process.Start("ms-settings:regionlanguage-languageoptions");
            });
        }

        /// <summary>
        /// 主题修改设置
        /// </summary>
        private void OnThemeSelectClicked(object sender, RoutedEventArgs args)
        {
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            if (item.Tag is not null)
            {
                Theme = ThemeList[Convert.ToInt32(item.Tag)];
                ThemeService.SetTheme(Theme);
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
            ToggleMenuFlyoutItem toggleMenuFlyoutItem = sender as ToggleMenuFlyoutItem;
            if (toggleMenuFlyoutItem.Tag is not null)
            {
                DoEngineMode = DoEngineModeList[Convert.ToInt32(toggleMenuFlyoutItem.Tag)];
                DownloadOptionsService.SetDoEngineMode(DoEngineMode);
            }
        }

        /// <summary>
        /// 重新启动资源管理器
        /// </summary>
        private void OnRestartExplorerClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                ProcessStartInfo restartInfo = new()
                {
                    FileName = "cmd.exe",
                    Arguments = "/C taskkill /f /im explorer.exe & start \"\" explorer.exe",
                    WindowStyle = ProcessWindowStyle.Hidden,
                };
                Process.Start(restartInfo);
            });
        }

        /// <summary>
        /// 是否开启始终显示背景色
        /// </summary>
        private void OnAlwaysShowBackdropToggled(object sender, RoutedEventArgs args)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch is not null)
            {
                AlwaysShowBackdropService.SetAlwaysShowBackdrop(toggleSwitch.IsOn);
                AlwaysShowBackdropValue = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 打开文件存放目录
        /// </summary>
        private void OnOpenFolderClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                Process.Start(DownloadFolder);
            });
        }

        /// <summary>
        /// 修改下载目录
        /// </summary>
        private void OnChangeFolderClicked(object sender, RoutedEventArgs args)
        {
            MenuFlyoutItem menuFlyoutItem = sender as MenuFlyoutItem;
            if (menuFlyoutItem.Tag is not null)
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
                            FolderBrowserDialog dialog = new()
                            {
                                Description = Settings.SelectFolder,
                                ShowNewFolderButton = true,
                                RootFolder = Environment.SpecialFolder.Desktop
                            };
                            DialogResult result = dialog.ShowDialog();
                            if (result is DialogResult.OK || result is DialogResult.Yes)
                            {
                                DownloadFolder = dialog.SelectedPath;
                                DownloadOptionsService.SetFolder(DownloadFolder);
                            }
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
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch is not null)
            {
                FileShellMenuService.SetFileShellMenu(toggleSwitch.IsOn);
                FileShellMenuValue = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 应用程序退出方式设置
        /// </summary>
        private void OnExitModeSelectClicked(object sender, RoutedEventArgs args)
        {
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            if (item.Tag is not null)
            {
                ExitMode = ExitModeList[Convert.ToInt32(item.Tag)];
                ExitModeService.SetExitMode(ExitMode);
            }
        }

        /// <summary>
        /// 是否开启应用窗口置顶
        /// </summary>
        private void OnTopMostToggled(object sender, RoutedEventArgs args)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch is not null)
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
        private void OnClearClicked(object sender, RoutedEventArgs args)
        {
            bool result = LogService.ClearLog();
            TeachingTipHelper.Show(new OperationResultTip(OperationKind.LogClean, result));
        }

        /// <summary>
        /// 当应用未启用背景色设置时，自动关闭始终显示背景色设置
        /// </summary>
        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch is not null)
            {
                AlwaysShowBackdropService.SetAlwaysShowBackdrop(false);
                AlwaysShowBackdropValue = false;
            }
        }

        #endregion 第一部分：设置页面——挂载的事件

        private string LocalizeDisplayNumber(DictionaryEntry selectedBackdrop)
        {
            int index = BackdropList.FindIndex(item => item.Value.Equals(selectedBackdrop.Value));

            if (index is 0)
            {
                return selectedBackdrop.Key.ToString();
            }
            else if (index is 1 || index is 2)
            {
                return Settings.Mica + " " + selectedBackdrop.Key.ToString();
            }
            else if (index is 3 || index is 4 || index is 5)
            {
                return Settings.DesktopAcrylic + " " + selectedBackdrop.Key.ToString();
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
