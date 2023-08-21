using FileRenamer.Extensions.DataType.Enums;
using FileRenamer.Helpers.Controls;
using FileRenamer.Helpers.Root;
using FileRenamer.Models;
using FileRenamer.Services.Root;
using FileRenamer.Services.Window;
using FileRenamer.UI.Dialogs;
using FileRenamer.UI.Notifications;
using FileRenamer.WindowsAPI.PInvoke.User32;
using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

        private NavigationViewItem _selectedItem;

        public NavigationViewItem SelectedItem
        {
            get { return _selectedItem; }

            set
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }

        private Dictionary<string, Type> PageDict { get; } = new Dictionary<string, Type>()
        {
            {"FileName",typeof(FileNamePage) },
            {"ExtensionName",typeof(ExtensionNamePage) },
            {"UpperAndLowerCase",typeof(UpperAndLowerCasePage) },
            {"FileProperties",typeof(FilePropertiesPage) },
            {"About",typeof(AboutPage) },
            {"Settings",typeof(SettingsPage) }
        };

        public List<string> TagList { get; } = new List<string>()
        {
            "FileName",
            "ExtensionName",
            "UpperAndLowerCase",
            "FileProperties",
            "About",
            "Settings"
        };

        public event PropertyChangedEventHandler PropertyChanged;

        public MainPage()
        {
            InitializeComponent();
            NavigationService.NavigationFrame = MainFrame;
        }

        /// <summary>
        /// 检查当前页面是否为目标页面
        /// </summary>
        public bool IsCurrentPage(object selectedItem, string pageName)
        {
            return selectedItem.ToString() == pageName;
        }

        /// <summary>
        /// 窗口关闭
        /// </summary>
        public void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            Program.ApplicationRoot.Dispose();
        }

        /// <summary>
        /// 导航完成后发生
        /// </summary>
        public void OnFrameNavigated(object sender, NavigationEventArgs args)
        {
            Type CurrentPageType = NavigationService.GetCurrentPageType();
            SelectedItem = NavigationService.NavigationItemList.Find(item => item.NavigationPage == CurrentPageType).NavigationItem;
            IsBackEnabled = NavigationService.CanGoBack();
        }

        /// <summary>
        /// 导航失败时发生
        /// </summary>
        public void OnFrameNavgationFailed(object sender, NavigationFailedEventArgs args)
        {
            throw new ApplicationException(string.Format(ResourceService.GetLocalized("Window/NavigationFailed"), args.SourcePageType.FullName));
        }

        /// <summary>
        /// 窗口最大化
        /// </summary>
        public void OnMaximizeClicked(object sender, RoutedEventArgs args)
        {
            Program.MainWindow.WindowState = FormWindowState.Maximized;
        }

        /// <summary>
        /// 窗口最小化
        /// </summary>
        public void OnMinimizeClicked(object sender, RoutedEventArgs args)
        {
            Program.MainWindow.WindowState = FormWindowState.Minimized;
        }

        /// <summary>
        /// 窗口移动
        /// </summary>
        public async void OnMoveClicked(object sender, RoutedEventArgs args)
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
        public void OnRestoreClicked(object sender, RoutedEventArgs args)
        {
            Program.MainWindow.WindowState = FormWindowState.Normal;
        }

        /// <summary>
        /// 窗口大小
        /// </summary>
        public void OnSizeClicked(object sender, RoutedEventArgs args)
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
        public void OnBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            NavigationService.NavigationFrom();
        }

        /// <summary>
        /// 当菜单中的项收到交互（如单击或点击）时发生
        /// </summary>
        public void OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            NavigationViewItemBase navigationViewItem = args.InvokedItemContainer;
            if (navigationViewItem.Tag is not null)
            {
                NavigationModel navigationItem = NavigationService.NavigationItemList.Find(item => item.NavigationTag == Convert.ToString(navigationViewItem.Tag));
                if (SelectedItem != navigationItem.NavigationItem)
                {
                    NavigationService.NavigateTo(navigationItem.NavigationPage);
                }
            }
        }

        /// <summary>
        /// 导航控件加载完成后初始化内容，初始化导航控件属性和应用的背景色
        /// </summary>
        public void OnNavigationViewLoaded(object sender, RoutedEventArgs args)
        {
            PropertyChanged += OnPropertyChanged;

            NavigationView navigationView = sender as NavigationView;
            if (navigationView is not null)
            {
                foreach (object item in navigationView.MenuItems)
                {
                    NavigationViewItem navigationViewItem = item as NavigationViewItem;
                    if (navigationViewItem is not null)
                    {
                        string Tag = Convert.ToString(navigationViewItem.Tag);

                        NavigationService.NavigationItemList.Add(new NavigationModel()
                        {
                            NavigationTag = Tag,
                            NavigationItem = navigationViewItem,
                            NavigationPage = PageDict[Tag],
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
        }

        /// <summary>
        /// 窗口被卸载时，注销所有事件
        /// </summary>
        public void OnNavigationViewUnLoaded(object sender, RoutedEventArgs args)
        {
            PropertyChanged -= OnPropertyChanged;
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
        /// 打开重启应用确认的窗口对话框
        /// </summary>
        public async void OnRestartAppsClicked(object sender, RoutedEventArgs args)
        {
            await ContentDialogHelper.ShowAsync(new RestartAppsDialog(), this);
        }

        /// <summary>
        /// 设置说明
        /// </summary>
        public void OnSettingsInstructionClicked(object sender, RoutedEventArgs args)
        {
            NavigationService.NavigateTo(typeof(AboutPage), AppNaviagtionArgs.SettingsHelp);
        }

        /// <summary>
        /// 创建应用的桌面快捷方式
        /// </summary>
        public void OnCreateDesktopShortcutClicked(object sender, RoutedEventArgs args)
        {
            bool IsCreatedSuccessfully = false;

            Task.Run(async () =>
            {
                try
                {
                    IWshShell shell = new WshShell();
                    WshShortcut AppShortcut = (WshShortcut)shell.CreateShortcut(string.Format(@"{0}\{1}.lnk", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), ResourceService.GetLocalized("Resources/AppDisplayName")));
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
                        new QuickOperationNotification(this, QuickOperationType.DesktopShortcut, IsCreatedSuccessfully).Show();
                    });
                }
            });
        }

        /// <summary>
        /// 将应用固定到“开始”屏幕
        /// </summary>
        public void OnPinToStartScreenClicked(object sender, RoutedEventArgs args)
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
                        new QuickOperationNotification(this, QuickOperationType.StartScreen, IsPinnedSuccessfully).Show();
                    });
                }
            });
        }

        // 将应用固定到任务栏
        public async void OnPinToTaskbarClicked(object sender, RoutedEventArgs args)
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
                new QuickOperationNotification(this, QuickOperationType.Taskbar, IsPinnedSuccessfully).Show();
            }
        }

        /// <summary>
        /// 查看许可证
        /// </summary>
        public async void OnShowLicenseClicked(object sender, RoutedEventArgs args)
        {
            await ContentDialogHelper.ShowAsync(new LicenseDialog(), this);
        }

        /// <summary>
        /// 查看更新日志
        /// </summary>
        public void OnShowReleaseNotesClicked(object sender, RoutedEventArgs args)
        {
            Process.Start("explorer.exe", "https://github.com/Gaoyifei1011/FileRenamer/releases");
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
