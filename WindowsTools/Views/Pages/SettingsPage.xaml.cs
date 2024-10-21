using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using WindowsTools.Extensions.DataType.Enums;
using WindowsTools.Helpers.Controls;
using WindowsTools.Models;
using WindowsTools.Services.Controls.Settings;
using WindowsTools.Services.Root;
using WindowsTools.Strings;
using WindowsTools.UI.Dialogs;
using WindowsTools.UI.TeachingTips;
using WindowsTools.Views.Windows;
using WindowsTools.WindowsAPI.ComTypes;
using WindowsTools.WindowsAPI.PInvoke.Shell32;

// 抑制 CA1806，IDE0060 警告
#pragma warning disable CA1806,IDE0060

namespace WindowsTools.Views.Pages
{
    /// <summary>
    /// 设置页面
    /// </summary>
    public sealed partial class SettingsPage : Page, INotifyPropertyChanged
    {
        private KeyValuePair<string, string> _theme = ThemeService.AppTheme;

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

        private KeyValuePair<string, string> _backdrop = BackdropService.AppBackdrop;

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

        private KeyValuePair<string, string> _doEngineMode = DownloadOptionsService.DoEngineMode;

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

        private List<KeyValuePair<string, string>> ThemeList { get; } = ThemeService.ThemeList;

        private List<KeyValuePair<string, string>> BackdropList { get; } = BackdropService.BackdropList;

        private List<KeyValuePair<string, string>> DoEngineModeList { get; } = DownloadOptionsService.DoEngineModeList;

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
                await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.LanguageChange));
            }
        }

        #endregion 第一部分：XamlUICommand 命令调用时挂载的事件

        #region 第二部分：设置页面——挂载的事件

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
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is not null)
            {
                Backdrop = BackdropList[Convert.ToInt32(radioMenuFlyoutItem.Tag)];
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
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is not null)
            {
                Theme = ThemeList[Convert.ToInt32(radioMenuFlyoutItem.Tag)];
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
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is not null)
            {
                DoEngineMode = DoEngineModeList[Convert.ToInt32(radioMenuFlyoutItem.Tag)];
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
            if (sender is ToggleSwitch toggleSwitch)
            {
                AlwaysShowBackdropService.SetAlwaysShowBackdrop(toggleSwitch.IsOn);
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
                Process.Start(DownloadFolder);
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
                                Description = Settings.SelectFolder,
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
                FileShellMenuService.SetFileShellMenu(toggleSwitch.IsOn);
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
            bool result = LogService.ClearLog();
            await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.LogClean, result));
        }

        /// <summary>
        /// 当应用未启用背景色设置时，自动关闭始终显示背景色设置
        /// </summary>
        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is ToggleSwitch)
            {
                AlwaysShowBackdropService.SetAlwaysShowBackdrop(false);
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
                return string.Format("{0} {1}", Settings.Mica, selectedBackdrop.Value);
            }
            else if (index is 3 || index is 4 || index is 5)
            {
                return string.Format("{0} {1}", Settings.DesktopAcrylic, selectedBackdrop.Value);
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
