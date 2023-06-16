using FileRenamer.Extensions.DataType.Enums;
using FileRenamer.Models;
using FileRenamer.Services.Controls.Settings.Appearance;
using FileRenamer.Services.Root;
using FileRenamer.Services.Window;
using FileRenamer.UI.Dialogs;
using FileRenamer.UI.Notifications;
using FileRenamer.ViewModels.Base;
using FileRenamer.Views.Pages;
using FileRenamer.WindowsAPI.PInvoke.DwmApi;
using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace FileRenamer.ViewModels.Pages
{
    /// <summary>
    /// 应用主窗口页面视图模型
    /// </summary>
    public sealed class MainViewModel : ViewModelBase
    {
        private bool _isAboutPage;

        public bool IsAboutPage
        {
            get { return _isAboutPage; }

            set
            {
                _isAboutPage = value;
                OnPropertyChanged();
            }
        }

        private bool _isSettingsPage;

        public bool IsSettingsPage
        {
            get { return _isSettingsPage; }

            set
            {
                _isSettingsPage = value;
                OnPropertyChanged();
            }
        }

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

        /// <summary>
        /// 导航完成后发生
        /// </summary>
        public void OnFrameNavigated(object sender, NavigationEventArgs args)
        {
            Type CurrentPageType = NavigationService.GetCurrentPageType();
            SelectedItem = NavigationService.NavigationItemList.Find(item => item.NavigationPage == CurrentPageType).NavigationItem;
            IsBackEnabled = NavigationService.CanGoBack();
            IsAboutPage = Convert.ToString(SelectedItem.Tag) == TagList[4];
            IsSettingsPage = Convert.ToString(SelectedItem.Tag) == TagList[5];
        }

        /// <summary>
        /// 导航失败时发生
        /// </summary>
        public void OnFrameNavgationFailed(object sender, NavigationFailedEventArgs args)
        {
            throw new ApplicationException(string.Format(ResourceService.GetLocalized("Window/NavigationFailed"), args.SourcePageType.FullName));
        }

        /// <summary>
        /// 当后退按钮收到交互（如单击或点击）时发生
        /// </summary>
        public void OnNavigationViewBackRequested(object sender, NavigationViewBackRequestedEventArgs args)
        {
            NavigationService.NavigationFrom();
        }

        /// <summary>
        /// 导航控件加载完成后初始化内容，初始化导航视图控件属性和应用的背景色
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
        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(WindowTheme))
            {
                SetAppTheme();
            }
        }

        /// <summary>
        /// 打开重启应用确认的窗口对话框
        /// </summary>
        public async void OnRestartAppsClicked(object sender, RoutedEventArgs args)
        {
            await new RestartAppsDialog().ShowAsync();
        }

        /// <summary>
        /// 当菜单中的项收到交互（如单击或点击）时发生
        /// </summary>
        public void OnTapped(object sender, TappedRoutedEventArgs args)
        {
            NavigationViewItem navigationViewItem = sender as NavigationViewItem;
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
        /// 创建应用的桌面快捷方式
        /// </summary>
        public async void OnCreateDesktopShortcutClicked(object sender, RoutedEventArgs args)
        {
            bool IsCreatedSuccessfully = false;
            try
            {
                IWshShell shell = new WshShell();
                WshShortcut AppShortcut = (WshShortcut)shell.CreateShortcut(string.Format(@"{0}\{1}.lnk", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), ResourceService.GetLocalized("Resources/AppDisplayName")));
                IReadOnlyList<AppListEntry> AppEntries = await Package.Current.GetAppListEntriesAsync();
                AppListEntry DefaultEntry = AppEntries[0];
                AppShortcut.TargetPath = string.Format(@"shell:AppsFolder\{0}", DefaultEntry.AppUserModelId);
                AppShortcut.Save();
                IsCreatedSuccessfully = true;
            }
            catch (Exception) { }
            finally
            {
                new QuickOperationNotification(QuickOperationType.DesktopShortcut, IsCreatedSuccessfully).Show();
            }
        }

        /// <summary>
        /// 将应用固定到“开始”屏幕
        /// </summary>
        public async void OnPinToStartScreenClicked(object sender, RoutedEventArgs args)
        {
            bool IsPinnedSuccessfully = false;

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
                new QuickOperationNotification(QuickOperationType.StartScreen, IsPinnedSuccessfully).Show();
            }
        }

        /// <summary>
        /// 查看许可证
        /// </summary>
        public async void OnShowLicenseClicked(object sender, RoutedEventArgs args)
        {
            await new LicenseDialog().ShowAsync();
        }

        /// <summary>
        /// 查看更新日志
        /// </summary>
        public void OnShowReleaseNotesClicked(object sender, RoutedEventArgs args)
        {
            Process.Start("explorer.exe", "https://github.com/Gaoyifei1011/FileRenamer/releases");
        }

        /// <summary>
        /// 设置应用的主题色
        /// </summary>
        public void SetAppTheme()
        {
            if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[0].InternalName)
            {
                if (Application.Current.RequestedTheme is ApplicationTheme.Light)
                {
                    int useLightMode = 0;
                    DwmApiLibrary.DwmSetWindowAttribute(Program.MainWindow.Handle, DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, ref useLightMode, Marshal.SizeOf(typeof(int)));
                }
                else
                {
                    int useDarkMode = 1;
                    DwmApiLibrary.DwmSetWindowAttribute(Program.MainWindow.Handle, DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, ref useDarkMode, Marshal.SizeOf(typeof(int)));
                }
            }
            if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[1].InternalName)
            {
                int useLightMode = 0;
                DwmApiLibrary.DwmSetWindowAttribute(Program.MainWindow.Handle, DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, ref useLightMode, Marshal.SizeOf(typeof(int)));
            }
            else if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[2].InternalName)
            {
                int useDarkMode = 1;
                DwmApiLibrary.DwmSetWindowAttribute(Program.MainWindow.Handle, DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, ref useDarkMode, Marshal.SizeOf(typeof(int)));
            }
        }
    }
}
