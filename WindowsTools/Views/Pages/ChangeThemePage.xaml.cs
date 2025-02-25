using Microsoft.UI.Xaml.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using WindowsTools.Helpers.Root;
using WindowsTools.Services.Root;
using WindowsTools.WindowsAPI.PInvoke.User32;

// 抑制 CA1806，CA1822，IDE0060 警告
#pragma warning disable CA1806,CA1822,IDE0060

namespace WindowsTools.Views.Pages
{
    /// <summary>
    /// 修改主题页面
    /// </summary>
    public sealed partial class ChangeThemePage : Page, INotifyPropertyChanged
    {
        private KeyValuePair<ElementTheme, string> _selectedSystemThemeStyle;

        public KeyValuePair<ElementTheme, string> SelectedSystemThemeStyle
        {
            get { return _selectedSystemThemeStyle; }

            set
            {
                if (!Equals(_selectedSystemThemeStyle, value))
                {
                    _selectedSystemThemeStyle = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedSystemThemeStyle)));
                }
            }
        }

        private KeyValuePair<ElementTheme, string> _selectedAppThemeStyle;

        public KeyValuePair<ElementTheme, string> SelectedAppThemeStyle
        {
            get { return _selectedAppThemeStyle; }

            set
            {
                if (!Equals(_selectedAppThemeStyle, value))
                {
                    _selectedAppThemeStyle = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedAppThemeStyle)));
                }
            }
        }

        private bool _isShowThemeColorInStartAndTaskbar = false;

        public bool IsShowThemeColorInStartAndTaskbar
        {
            get { return _isShowThemeColorInStartAndTaskbar; }

            set
            {
                if (!Equals(_isShowThemeColorInStartAndTaskbar, value))
                {
                    _isShowThemeColorInStartAndTaskbar = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsShowThemeColorInStartAndTaskbar)));
                }
            }
        }

        private bool _isShowThemeColorInStartAndTaskbarEnabled = false;

        public bool IsShowThemeColorInStartAndTaskbarEnabled
        {
            get { return _isShowThemeColorInStartAndTaskbarEnabled; }

            set
            {
                if (!Equals(_isShowThemeColorInStartAndTaskbarEnabled, value))
                {
                    _isShowThemeColorInStartAndTaskbarEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsShowThemeColorInStartAndTaskbarEnabled)));
                }
            }
        }

        private bool _isAutoChangeThemeValue;

        public bool IsAutoChangeThemeValue
        {
            get { return _isAutoChangeThemeValue; }

            set
            {
                if (!Equals(_isAutoChangeThemeValue, value))
                {
                    _isAutoChangeThemeValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAutoChangeThemeValue)));
                }
            }
        }

        private bool _isAutoChangeSystemThemeValue;

        public bool IsAutoChangeSystemThemeValue
        {
            get { return _isAutoChangeSystemThemeValue; }

            set
            {
                if (!Equals(_isAutoChangeSystemThemeValue, value))
                {
                    _isAutoChangeSystemThemeValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAutoChangeSystemThemeValue)));
                }
            }
        }

        private bool _isAutoChangeAppThemeValue;

        public bool IsAutoChangeAppThemeValue
        {
            get { return _isAutoChangeAppThemeValue; }

            set
            {
                if (!Equals(_isAutoChangeAppThemeValue, value))
                {
                    _isAutoChangeAppThemeValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAutoChangeAppThemeValue)));
                }
            }
        }

        private bool _isShowColorInDarkTheme;

        public bool IsShowColorInDarkTheme
        {
            get { return _isShowColorInDarkTheme; }

            set
            {
                if (!Equals(_isShowColorInDarkTheme, value))
                {
                    _isShowColorInDarkTheme = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsShowColorInDarkTheme)));
                }
            }
        }

        private TimeSpan _systemThemeLightTime = DateTimeOffset.Now.TimeOfDay;

        public TimeSpan SystemThemeLightTime
        {
            get { return _systemThemeLightTime; }

            set
            {
                if (!Equals(_systemThemeLightTime, value))
                {
                    _systemThemeLightTime = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SystemThemeLightTime)));
                }
            }
        }

        private TimeSpan _systemThemeDarkTime = DateTimeOffset.Now.TimeOfDay;

        public TimeSpan SystemThemeDarkTime
        {
            get { return _systemThemeDarkTime; }

            set
            {
                if (!Equals(_systemThemeDarkTime, value))
                {
                    _systemThemeDarkTime = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SystemThemeDarkTime)));
                }
            }
        }

        private TimeSpan _appThemeLightTime = DateTimeOffset.Now.TimeOfDay;

        public TimeSpan AppThemeLightTime
        {
            get { return _appThemeLightTime; }

            set
            {
                if (!Equals(_appThemeLightTime, value))
                {
                    _appThemeLightTime = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AppThemeLightTime)));
                }
            }
        }

        private TimeSpan _appThemeDarkTime = DateTimeOffset.Now.TimeOfDay;

        public TimeSpan AppThemeDarkTime
        {
            get { return _appThemeDarkTime; }

            set
            {
                if (!Equals(_appThemeDarkTime, value))
                {
                    _appThemeDarkTime = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AppThemeDarkTime)));
                }
            }
        }

        private List<KeyValuePair<ElementTheme, string>> SystemThemeStyleList { get; } =
        [
            new KeyValuePair<ElementTheme,string>(ElementTheme.Light, ResourceService.ChangeThemeResource.GetString("Light")),
            new KeyValuePair<ElementTheme,string>(ElementTheme.Dark, ResourceService.ChangeThemeResource.GetString("Dark")),
        ];

        private List<KeyValuePair<ElementTheme, string>> AppThemeStyleList { get; } =
        [
            new KeyValuePair<ElementTheme,string>(ElementTheme.Light, ResourceService.ChangeThemeResource.GetString("Light")),
            new KeyValuePair<ElementTheme,string>(ElementTheme.Dark, ResourceService.ChangeThemeResource.GetString("Dark")),
        ];

        public event PropertyChangedEventHandler PropertyChanged;

        public ChangeThemePage()
        {
            InitializeComponent();
            SelectedSystemThemeStyle = SystemThemeStyleList[0];
            SelectedAppThemeStyle = AppThemeStyleList[0];
        }

        #region 第一部分：修改主题页面——挂载的事件

        private async void OnLoaded(object sender, RoutedEventArgs args)
        {
            ElementTheme systemTheme = await Task.Run(GetSystemTheme);
            SelectedSystemThemeStyle = SystemThemeStyleList.Find(item => item.Key.Equals(systemTheme));

            ElementTheme appTheme = await Task.Run(GetAppTheme);
            SelectedAppThemeStyle = AppThemeStyleList.Find(item => item.Key.Equals(appTheme));

            IsShowThemeColorInStartAndTaskbarEnabled = SelectedSystemThemeStyle.Equals(SystemThemeStyleList[1]);
            bool showThemeColorInStartAndTaskbar = await Task.Run(GetShowThemeColorInStartAndTaskbar);
            IsShowThemeColorInStartAndTaskbar = showThemeColorInStartAndTaskbar;
        }

        /// <summary>
        /// 打开系统个性化
        /// </summary>
        private void OnOpenPersonalizeClicked(object sender, RoutedEventArgs args)
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
        /// 刷新主题样式设置值
        /// </summary>
        private async void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            ElementTheme systemTheme = await Task.Run(GetSystemTheme);
            SelectedSystemThemeStyle = SystemThemeStyleList.Find(item => item.Key.Equals(systemTheme));

            ElementTheme appTheme = await Task.Run(GetAppTheme);
            SelectedAppThemeStyle = AppThemeStyleList.Find(item => item.Key.Equals(appTheme));

            IsShowThemeColorInStartAndTaskbarEnabled = SelectedSystemThemeStyle.Equals(SystemThemeStyleList[1]);
            bool showThemeColorInStartAndTaskbar = await Task.Run(GetShowThemeColorInStartAndTaskbar);
            IsShowThemeColorInStartAndTaskbar = showThemeColorInStartAndTaskbar;
        }

        /// <summary>
        /// 修改系统主题样式
        /// </summary>
        private void OnSystemThemeStyleClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is not null)
            {
                SelectedSystemThemeStyle = SystemThemeStyleList[Convert.ToInt32(radioMenuFlyoutItem.Tag)];
                int systemTheme = 0;

                if (SelectedSystemThemeStyle.Equals(SystemThemeStyleList[0]))
                {
                    systemTheme = 1;
                    IsShowThemeColorInStartAndTaskbarEnabled = false;
                    IsShowThemeColorInStartAndTaskbar = false;
                }
                else if (SelectedSystemThemeStyle.Equals(SystemThemeStyleList[1]))
                {
                    systemTheme = 0;
                    IsShowThemeColorInStartAndTaskbarEnabled = true;
                }

                Task.Run(() =>
                {
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "SystemUsesLightTheme", systemTheme);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "ColorPrevalence", IsShowThemeColorInStartAndTaskbar);
                    User32Library.SendMessageTimeout(new IntPtr(0xffff), WindowMessage.WM_SETTINGCHANGE, UIntPtr.Zero, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
                });
            }
        }

        /// <summary>
        /// 修改应用主题样式
        /// </summary>
        private void OnAppThemeStyleClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is not null)
            {
                SelectedAppThemeStyle = AppThemeStyleList[Convert.ToInt32(radioMenuFlyoutItem.Tag)];
                int appTheme = 0;

                if (SelectedAppThemeStyle.Equals(AppThemeStyleList[0]))
                {
                    appTheme = 1;
                }
                else if (SelectedSystemThemeStyle.Equals(SystemThemeStyleList[1]))
                {
                    appTheme = 0;
                }

                Task.Run(() =>
                {
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", appTheme);
                    User32Library.SendMessageTimeout(new IntPtr(0xffff), WindowMessage.WM_SETTINGCHANGE, UIntPtr.Zero, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
                });
            }
        }

        /// <summary>
        /// 在“开始菜单”和任务栏中显示主题色
        /// </summary>
        private void OnShowThemeColorInStartAndTaskbarToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                IsShowThemeColorInStartAndTaskbar = toggleSwitch.IsOn;

                Task.Run(() =>
                {
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "ColorPrevalence", IsShowThemeColorInStartAndTaskbar);
                    User32Library.SendMessageTimeout(new IntPtr(0xffff), WindowMessage.WM_SETTINGCHANGE, UIntPtr.Zero, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
                });
            }
        }

        /// <summary>
        /// 保存自动修改主题设置值
        /// </summary>
        private void OnSaveClicked(Microsoft.UI.Xaml.Controls.SplitButton sender, Microsoft.UI.Xaml.Controls.SplitButtonClickEventArgs args)
        {
        }

        /// <summary>
        /// 是否启用自动切换主题
        /// </summary>
        private void OnAutoChangeThemeToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                IsAutoChangeThemeValue = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 修改选定的自动修改系统主题设置选项
        /// </summary>
        private void OnAutoChangeSystemThemeToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                IsAutoChangeSystemThemeValue = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 显示设置时间控件
        /// </summary>
        private void OnShowSetTimeFlyoutClicked(object sender, RoutedEventArgs args)
        {
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        /// <summary>
        /// 修改选定的自动修改系统主题设置选项
        /// </summary>
        private void OnAutoChangeAppThemeToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                IsAutoChangeAppThemeValue = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 切换系统深色主题时显示主题色
        /// </summary>
        private void OnShowColorInDarkThemeToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                IsShowColorInDarkTheme = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 关闭浮出控件
        /// </summary>
        private void OnCloseFlyoutClicked(object sender, RoutedEventArgs args)
        {
            string tag = Convert.ToString((sender as Button).Tag);

            if (!string.IsNullOrEmpty(tag))
            {
                if (tag.Equals("SystemThemeSetTimeFlyout", StringComparison.OrdinalIgnoreCase) && SystemThemeSetTimeFlyout.IsOpen)
                {
                    SystemThemeSetTimeFlyout.Hide();
                }
                else if (tag.Equals("AppThemeSetTimeFlyout", StringComparison.OrdinalIgnoreCase) && AppThemeSetTimeFlyout.IsOpen)
                {
                    AppThemeSetTimeFlyout.Hide();
                }
            }
        }

        /// <summary>
        /// 修改选定的自动修改系统浅色主题时设定时间
        /// </summary>
        private void OnSystemThemeLightTimeChanged(object sender, TimePickerValueChangedEventArgs args)
        {
            if (sender is TimePicker)
            {
                SystemThemeLightTime = args.NewTime;
            }
        }

        /// <summary>
        /// 修改选定的自动修改系统浅色主题时设定时间
        /// </summary>
        private void OnSystemThemeDarkTimeChanged(object sender, TimePickerValueChangedEventArgs args)
        {
            if (sender is TimePicker)
            {
                SystemThemeDarkTime = args.NewTime;
            }
        }

        /// <summary>
        /// 修改选定的自动修改应用浅色主题时设定时间
        /// </summary>
        private void OnAppThemeLightTimeChanged(object sender, TimePickerValueChangedEventArgs args)
        {
            if (sender is TimePicker)
            {
                AppThemeLightTime = args.NewTime;
            }
        }

        /// <summary>
        /// 修改选定的自动修改应用浅色主题时设定时间
        /// </summary>
        private void OnAppThemeDarkTimeChanged(object sender, TimePickerValueChangedEventArgs args)
        {
            if (sender is TimePicker)
            {
                AppThemeDarkTime = args.NewTime;
            }
        }

        #endregion 第一部分：修改主题页面——挂载的事件

        /// <summary>
        /// 获取系统主题样式
        /// </summary>
        private ElementTheme GetSystemTheme()
        {
            return RegistryHelper.ReadRegistryKey<int>(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "SystemUsesLightTheme") is 1 ? ElementTheme.Light : ElementTheme.Dark;
        }

        /// <summary>
        /// 获取应用主题样式
        /// </summary>
        private ElementTheme GetAppTheme()
        {
            return RegistryHelper.ReadRegistryKey<int>(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme") is 1 ? ElementTheme.Light : ElementTheme.Dark;
        }

        /// <summary>
        /// 获取在“开始菜单”和任务栏中显示主题色设置值
        /// </summary>
        private bool GetShowThemeColorInStartAndTaskbar()
        {
            return RegistryHelper.ReadRegistryKey<bool>(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "ColorPrevalence");
        }
    }
}
