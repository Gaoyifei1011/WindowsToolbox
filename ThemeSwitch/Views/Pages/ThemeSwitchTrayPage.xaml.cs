using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ThemeSwitch.Helpers.Root;
using ThemeSwitch.Services.Controls.Settings;
using ThemeSwitch.Services.Root;
using ThemeSwitch.Views.Windows;
using ThemeSwitch.WindowsAPI.PInvoke.User32;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// 抑制 CA1806，CA1822，IDE0060 警告
#pragma warning disable CA1806,CA1822,IDE0060

namespace ThemeSwitch.Views.Pages
{
    /// <summary>
    /// 主题切换托盘页面
    /// </summary>
    public sealed partial class ThemeSwitchTrayPage : Page, INotifyPropertyChanged
    {
        private ElementTheme _windowTheme;

        public ElementTheme WindowTheme
        {
            get { return _windowTheme; }

            set
            {
                if (!Equals(_windowTheme, value))
                {
                    _windowTheme = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WindowTheme)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ThemeSwitchTrayPage()
        {
            InitializeComponent();
        }

        #region 第一部分：系统托盘页面——挂载的事件

        /// <summary>
        /// 系统托盘页面初始化触发的事件
        /// </summary>
        private async void OnLoaded(object sender, RoutedEventArgs args)
        {
            WindowTheme = await Task.Run(GetSystemTheme);
        }

        /// <summary>
        /// 打开主程序
        /// </summary>
        private void OnOpenMainProgramClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("powertoolbox:");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, nameof(ThemeSwitch), nameof(ThemeSwitchTrayPage), nameof(OnOpenMainProgramClicked), 1, e);
                }
            });
        }

        /// <summary>
        /// 切换系统主题
        /// </summary>
        private void OnSwitchSystemThemeClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                ElementTheme systemTheme = GetSystemTheme();
                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "SystemUsesLightTheme", systemTheme is ElementTheme.Light ? 0 : 1);
                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "ColorPrevalence", AutoThemeSwitchService.IsShowColorInDarkThemeValue && systemTheme is ElementTheme.Light ? 1 : 0);
                User32Library.SendMessageTimeout(new IntPtr(0xffff), WindowMessage.WM_SETTINGCHANGE, UIntPtr.Zero, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
            });
        }

        /// <summary>
        /// 切换应用主题
        /// </summary>
        private void OnSwitchAppThemeClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                ElementTheme appTheme = GetAppTheme();
                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", appTheme is ElementTheme.Light ? 0 : 1);
                User32Library.SendMessageTimeout(new IntPtr(0xffff), WindowMessage.WM_SETTINGCHANGE, UIntPtr.Zero, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
            });
        }

        /// <summary>
        /// 退出程序
        /// </summary>
        private void OnExitClicked(object sender, RoutedEventArgs args)
        {
            ThemeSwitchTrayWindow.Current?.Close();
        }

        #endregion 第一部分：系统托盘页面——挂载的事件

        /// <summary>
        /// 更新系统托盘主题
        /// </summary>
        public async Task UpdateSystemTrayThemeAsync()
        {
            WindowTheme = await Task.Run(GetSystemTheme);
        }

        /// <summary>
        /// 获取系统主题样式
        /// </summary>
        private ElementTheme GetSystemTheme()
        {
            return RegistryHelper.ReadRegistryKey<bool>(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "SystemUsesLightTheme") ? ElementTheme.Light : ElementTheme.Dark;
        }

        /// <summary>
        /// 获取应用主题样式
        /// </summary>
        private ElementTheme GetAppTheme()
        {
            return RegistryHelper.ReadRegistryKey<bool>(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme") ? ElementTheme.Light : ElementTheme.Dark;
        }
    }
}