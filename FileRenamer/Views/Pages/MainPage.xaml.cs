using FileRenamer.Extensions.DataType.Enums;
using FileRenamer.Helpers.Controls;
using FileRenamer.Helpers.Root;
using FileRenamer.Models;
using FileRenamer.Services.Window;
using FileRenamer.UI.Dialogs;
using FileRenamer.UI.Notifications;
using FileRenamer.WindowsAPI.PInvoke.User32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.UI.Shell;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace FileRenamer.Views.Pages
{
    /// <summary>
    /// 应用主窗口页面
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        private ElementTheme _windowTheme;

        public ElementTheme WindowTheme
        {
            get { return _windowTheme; }

            set
            {
                _windowTheme = value;
                OnPropertyChanged();
            }
        }

        private bool _isBackEnabled;

        public bool IsBackEnabled
        {
            get { return _isBackEnabled; }

            set
            {
                _isBackEnabled = value;
                OnPropertyChanged();
            }
        }

        private bool _isWindowMaximized;

        public bool IsWindowMaximized
        {
            get { return _isWindowMaximized; }

            set
            {
                _isWindowMaximized = value;
                OnPropertyChanged();
            }
        }

        private Microsoft.UI.Xaml.Controls.NavigationViewItem _selectedItem;

        public Microsoft.UI.Xaml.Controls.NavigationViewItem SelectedItem
        {
            get { return _selectedItem; }

            set
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }

        private List<KeyValuePair<string, Type>> PageList = new List<KeyValuePair<string, Type>>()
        {
            new KeyValuePair<string, Type>("FileName",typeof(FileNamePage)),
            new KeyValuePair<string, Type>("ExtensionName", typeof(ExtensionNamePage)),
            new KeyValuePair<string, Type>("UpperAndLowerCase", typeof(UpperAndLowerCasePage)),
            new KeyValuePair<string, Type>("FileProperties", typeof(FilePropertiesPage)),
            new KeyValuePair<string, Type>("About", typeof(AboutPage)),
            new KeyValuePair<string, Type>("Settings",typeof(SettingsPage) ),
        };

        public event PropertyChangedEventHandler PropertyChanged;

        public MainPage()
        {
            InitializeComponent();
            NavigationService.NavigationFrame = MainFrame;
        }

        /// <summary>
        /// 窗口关闭
        /// </summary>
        private void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            Program.ApplicationRoot.Dispose();
        }

        /// <summary>
        /// 导航完成后发生
        /// </summary>
        private void OnFrameNavigated(object sender, NavigationEventArgs args)
        {
            Type CurrentPageType = NavigationService.GetCurrentPageType();
            SelectedItem = NavigationService.NavigationItemList.Find(item => item.NavigationPage == CurrentPageType).NavigationItem;
            IsBackEnabled = NavigationService.CanGoBack();
        }

        /// <summary>
        /// 导航失败时发生
        /// </summary>
        private void OnFrameNavgationFailed(object sender, NavigationFailedEventArgs args)
        {
            throw new ApplicationException(string.Format(Strings.Window.NavigationFailed, args.SourcePageType.FullName));
        }

        /// <summary>
        /// 窗口最大化
        /// </summary>
        private void OnMaximizeClicked(object sender, RoutedEventArgs args)
        {
            Program.MainWindow.WindowState = FormWindowState.Maximized;
        }

        /// <summary>
        /// 窗口最小化
        /// </summary>
        private void OnMinimizeClicked(object sender, RoutedEventArgs args)
        {
            Program.MainWindow.WindowState = FormWindowState.Minimized;
        }

        /// <summary>
        /// 窗口移动
        /// </summary>
        private async void OnMoveClicked(object sender, RoutedEventArgs args)
        {
            MenuFlyoutItem menuItem = sender as MenuFlyoutItem;
            if (menuItem.Tag is not null)
            {
                ((MenuFlyout)menuItem.Tag).Hide();
                await Task.Delay(10);
                User32Library.SendMessage(Program.MainWindow.Handle, WindowMessage.WM_SYSCOMMAND, 0xF010, IntPtr.Zero);
            }
        }

        /// <summary>
        /// 窗口还原
        /// </summary>
        private void OnRestoreClicked(object sender, RoutedEventArgs args)
        {
            Program.MainWindow.WindowState = FormWindowState.Normal;
        }

        /// <summary>
        /// 窗口大小
        /// </summary>
        private void OnSizeClicked(object sender, RoutedEventArgs args)
        {
            MenuFlyoutItem menuItem = sender as MenuFlyoutItem;
            if (menuItem.Tag is not null)
            {
                ((MenuFlyout)menuItem.Tag).Hide();
                User32Library.SendMessage(Program.MainWindow.Handle, WindowMessage.WM_SYSCOMMAND, 0xF000, IntPtr.Zero);
            }
        }

        /// <summary>
        /// 当后退按钮收到交互（如单击或点击）时发生
        /// </summary>
        private void OnBackRequested(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs args)
        {
            NavigationService.NavigationFrom();
        }

        /// <summary>
        /// 当菜单中的项收到交互（如单击或点击）时发生
        /// </summary>
        private void OnItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            Microsoft.UI.Xaml.Controls.NavigationViewItemBase navigationViewItem = args.InvokedItemContainer;
            if (navigationViewItem.Tag is not null)
            {
                NavigationModel navigationItem = NavigationService.NavigationItemList.Find(item => item.NavigationTag == PageList[Convert.ToInt32(navigationViewItem.Tag)].Key);

                if (SelectedItem != navigationItem.NavigationItem)
                {
                    NavigationService.NavigateTo(navigationItem.NavigationPage);
                }
            }
        }

        /// <summary>
        /// 导航控件加载完成后初始化内容，初始化导航控件属性和应用的背景色
        /// </summary>
        private void OnNavigationViewLoaded(object sender, RoutedEventArgs args)
        {
            PropertyChanged += OnPropertyChanged;

            Microsoft.UI.Xaml.Controls.NavigationView navigationView = sender as Microsoft.UI.Xaml.Controls.NavigationView;
            if (navigationView is not null)
            {
                foreach (object item in navigationView.MenuItems)
                {
                    Microsoft.UI.Xaml.Controls.NavigationViewItem navigationViewItem = item as Microsoft.UI.Xaml.Controls.NavigationViewItem;
                    if (navigationViewItem is not null)
                    {
                        int TagIndex = Convert.ToInt32(navigationViewItem.Tag);

                        NavigationService.NavigationItemList.Add(new NavigationModel()
                        {
                            NavigationTag = PageList[TagIndex].Key,
                            NavigationItem = navigationViewItem,
                            NavigationPage = PageList[TagIndex].Value,
                        });
                    }
                }

                SelectedItem = NavigationService.NavigationItemList[0].NavigationItem;
                NavigationService.NavigateTo(typeof(FileNamePage));
                IsBackEnabled = NavigationService.CanGoBack();
            }

            string[] arguments = Environment.GetCommandLineArgs();

            if (arguments.Length is 2)
            {
                if (arguments[1] is "ExtensionName")
                {
                    NavigationService.NavigateTo(typeof(ExtensionNamePage));
                }
                else if (arguments[1] is "UpperAndLowerCase")
                {
                    NavigationService.NavigateTo(typeof(UpperAndLowerCasePage));
                }
                else if (arguments[1] is "FileProperties")
                {
                    NavigationService.NavigateTo(typeof(FilePropertiesPage));
                }
            }
            else if (arguments.Length is 3 && arguments[2] is "ShellContextMenu")
            {
                if (arguments[1] is "FileName")
                {
                    FileNamePage fileNamePage = MainFrame.Content as FileNamePage;

                    if (fileNamePage is not null)
                    {
                        Task.Run(() =>
                        {
                            try
                            {
                                if (File.Exists(Program.TempFilePath))
                                {
                                    string tempFileText = File.ReadAllText(Program.TempFilePath);
                                    File.Delete(Program.TempFilePath);

                                    string[] fileNamePathList = tempFileText.Split('\n').Where(item => !string.IsNullOrWhiteSpace(item)).ToArray();

                                    foreach (string fileNamePath in fileNamePathList)
                                    {
                                        string fileName = IOHelper.FilterInvalidPathChars(fileNamePath);

                                        Program.MainWindow.BeginInvoke(() =>
                                        {
                                            fileNamePage.FileNameDataList.Add(new OldAndNewNameModel()
                                            {
                                                OriginalFileName = Path.GetFileName(fileName),
                                                OriginalFilePath = fileName
                                            });
                                        });
                                    }
                                }
                            }
                            catch (Exception) { }
                        });
                    }
                }
                else if (arguments[1] is "ExtensionName")
                {
                    NavigationService.NavigateTo(typeof(ExtensionNamePage));

                    ExtensionNamePage extensionNamePage = MainFrame.Content as ExtensionNamePage;

                    if (extensionNamePage is not null)
                    {
                        Task.Run(() =>
                        {
                            try
                            {
                                if (File.Exists(Program.TempFilePath))
                                {
                                    string tempFileText = File.ReadAllText(Program.TempFilePath);
                                    File.Delete(Program.TempFilePath);

                                    string[] extensionNamePathList = tempFileText.Split('\n').Where(item => !string.IsNullOrEmpty(item)).ToArray();

                                    foreach (string extensionNamePath in extensionNamePathList)
                                    {
                                        string extensionName = IOHelper.FilterInvalidPathChars(extensionNamePath);

                                        if (!IOHelper.IsDir(extensionName))
                                        {
                                            Program.MainWindow.BeginInvoke(() =>
                                            {
                                                extensionNamePage.ExtensionNameDataList.Add(new OldAndNewNameModel()
                                                {
                                                    OriginalFileName = Path.GetFileName(extensionName),
                                                    OriginalFilePath = extensionName
                                                });
                                            });
                                        }
                                    }
                                }
                            }
                            catch (Exception) { }
                        });
                    }
                }
                else if (arguments[1] is "UpperAndLowerCase")
                {
                    NavigationService.NavigateTo(typeof(UpperAndLowerCasePage));

                    UpperAndLowerCasePage upperAndLowerCasePage = MainFrame.Content as UpperAndLowerCasePage;

                    if (upperAndLowerCasePage is not null)
                    {
                        Task.Run(() =>
                        {
                            try
                            {
                                if (File.Exists(Program.TempFilePath))
                                {
                                    string tempFileText = File.ReadAllText(Program.TempFilePath);
                                    File.Delete(Program.TempFilePath);

                                    string[] upperAndLowerCasePathList = tempFileText.Split('\n').Where(item => !string.IsNullOrEmpty(item)).ToArray();

                                    foreach (string upperAndLowerCasePath in upperAndLowerCasePathList)
                                    {
                                        string upperAndLowerCase = IOHelper.FilterInvalidPathChars(upperAndLowerCasePath);

                                        Program.MainWindow.BeginInvoke(() =>
                                        {
                                            upperAndLowerCasePage.UpperAndLowerCaseDataList.Add(new OldAndNewNameModel()
                                            {
                                                OriginalFileName = Path.GetFileName(upperAndLowerCase),
                                                OriginalFilePath = upperAndLowerCase
                                            });
                                        });
                                    }
                                }
                            }
                            catch (Exception) { }
                        });
                    }
                }
                else if (arguments[1] is "FileProperties")
                {
                    NavigationService.NavigateTo(typeof(FilePropertiesPage));

                    FilePropertiesPage filePropertiesPage = MainFrame.Content as FilePropertiesPage;

                    if (filePropertiesPage is not null)
                    {
                        Task.Run(() =>
                        {
                            try
                            {
                                if (File.Exists(Program.TempFilePath))
                                {
                                    string tempFileText = File.ReadAllText(Program.TempFilePath);
                                    File.Delete(Program.TempFilePath);

                                    string[] filePropertiesPathList = tempFileText.Split('\n').Where(item => !string.IsNullOrEmpty(item)).ToArray();

                                    foreach (string filePropertiesPath in filePropertiesPathList)
                                    {
                                        string fileProperties = IOHelper.FilterInvalidPathChars(filePropertiesPath);

                                        Program.MainWindow.BeginInvoke(() =>
                                        {
                                            filePropertiesPage.FilePropertiesDataList.Add(new OldAndNewPropertiesModel()
                                            {
                                                FileName = Path.GetFileName(fileProperties),
                                                FilePath = fileProperties
                                            });
                                        });
                                    }
                                }
                            }
                            catch (Exception) { }
                        });
                    }
                }
            }
        }

        /// <summary>
        /// 窗口被卸载时，注销所有事件
        /// </summary>
        private void OnNavigationViewUnLoaded(object sender, RoutedEventArgs args)
        {
            PropertyChanged -= OnPropertyChanged;
        }

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
            NavigationService.NavigateTo(typeof(AboutPage), AppNaviagtionArgs.SettingsHelp);
        }

        /// <summary>
        /// 创建应用的桌面快捷方式
        /// </summary>
        private void OnCreateDesktopShortcutClicked(object sender, RoutedEventArgs args)
        {
            bool IsCreatedSuccessfully = false;

            Task.Run(async () =>
            {
                try
                {
                    IWshRuntimeLibrary.IWshShell shell = new IWshRuntimeLibrary.WshShell();
                    IWshRuntimeLibrary.WshShortcut AppShortcut = (IWshRuntimeLibrary.WshShortcut)shell.CreateShortcut(string.Format(@"{0}\{1}.lnk", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), Strings.Resources.AppDisplayName));
                    if (RuntimeHelper.IsMSIX)
                    {
                        IReadOnlyList<AppListEntry> AppEntries = await Package.Current.GetAppListEntriesAsync();
                        AppListEntry DefaultEntry = AppEntries[0];
                        AppShortcut.TargetPath = string.Format(@"shell:AppsFolder\{0}", DefaultEntry.AppUserModelId);
                        AppShortcut.Save();
                    }
                    else
                    {
                        AppShortcut.TargetPath = Process.GetCurrentProcess().MainModule.FileName;
                        AppShortcut.Save();
                    }
                    IsCreatedSuccessfully = true;
                }
                catch (Exception) { }
                finally
                {
                    Program.MainWindow.BeginInvoke(() =>
                    {
                        new QuickOperationNotification(this, QuickOperationKind.DesktopShortcut, IsCreatedSuccessfully).Show();
                    });
                }
            });
        }

        /// <summary>
        /// 将应用固定到“开始”屏幕
        /// </summary>
        private void OnPinToStartScreenClicked(object sender, RoutedEventArgs args)
        {
            bool IsPinnedSuccessfully = false;

            Task.Run(async () =>
            {
                try
                {
                    IReadOnlyList<AppListEntry> AppEntries = await Package.Current.GetAppListEntriesAsync();

                    AppListEntry DefaultEntry = AppEntries[0];

                    if (DefaultEntry is not null)
                    {
                        StartScreenManager startScreenManager = StartScreenManager.GetDefault();

                        bool containsEntry = await startScreenManager.ContainsAppListEntryAsync(DefaultEntry);

                        if (!containsEntry)
                        {
                            await startScreenManager.RequestAddAppListEntryAsync(DefaultEntry);
                        }
                    }
                    IsPinnedSuccessfully = true;
                }
                catch (Exception) { }
                finally
                {
                    Program.MainWindow.BeginInvoke(() =>
                    {
                        new QuickOperationNotification(this, QuickOperationKind.StartScreen, IsPinnedSuccessfully).Show();
                    });
                }
            });
        }

        // 将应用固定到任务栏
        private async void OnPinToTaskbarClicked(object sender, RoutedEventArgs args)
        {
            bool IsPinnedSuccessfully = false;

            try
            {
                string featureId = "com.microsoft.windows.taskbar.pin";
                string token = FeatureAccessHelper.GenerateTokenFromFeatureId(featureId);
                string attestation = FeatureAccessHelper.GenerateAttestation(featureId);
                LimitedAccessFeatureRequestResult accessResult = LimitedAccessFeatures.TryUnlockFeature(featureId, token, attestation);

                if (accessResult.Status is LimitedAccessFeatureStatus.Available)
                {
                    IsPinnedSuccessfully = await TaskbarManager.GetDefault().RequestPinCurrentAppAsync();
                }
            }
            catch (Exception) { }
            finally
            {
                new QuickOperationNotification(this, QuickOperationKind.Taskbar, IsPinnedSuccessfully).Show();
            }
        }

        /// <summary>
        /// 查看许可证
        /// </summary>
        private async void OnShowLicenseClicked(object sender, RoutedEventArgs args)
        {
            await ContentDialogHelper.ShowAsync(new LicenseDialog(), this);
        }

        /// <summary>
        /// 查看更新日志
        /// </summary>
        private void OnShowReleaseNotesClicked(object sender, RoutedEventArgs args)
        {
            Process.Start("explorer.exe", "https://github.com/Gaoyifei1011/FileRenamer/releases");
        }

        /// <summary>
        /// 可通知的属性发生更改时的事件处理
        /// </summary>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(WindowTheme))
            {
                Program.MainWindow.SetAppTheme();
            }
        }

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 检查当前页面是否为目标页面
        /// </summary>
        public bool IsCurrentPage(object selectedItem, int pageIndex)
        {
            return selectedItem.ToString().Equals(pageIndex.ToString(), StringComparison.OrdinalIgnoreCase);
        }
    }
}
