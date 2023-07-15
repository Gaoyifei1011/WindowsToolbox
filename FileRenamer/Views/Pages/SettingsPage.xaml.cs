using FileRenamer.Extensions.DataType.Enums;
using FileRenamer.Helpers.Root;
using FileRenamer.Models;
using FileRenamer.Services.Controls.Settings.Appearance;
using FileRenamer.Services.Controls.Settings.Common;
using FileRenamer.Services.Window;
using FileRenamer.UI.Notifications;
using FileRenamer.Views.CustomControls.DialogsAndFlyouts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace FileRenamer.Views.Pages
{
    /// <summary>
    /// 设置页面
    /// </summary>
    public sealed partial class SettingsPage : Page, INotifyPropertyChanged
    {
        public bool CanUseBackdrop { get; set; }

        private ThemeModel _theme = ThemeService.AppTheme;

        public ThemeModel Theme
        {
            get { return _theme; }

            set
            {
                _theme = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Theme)));
            }
        }

        private BackdropModel _backdrop = BackdropService.AppBackdrop;

        public BackdropModel Backdrop
        {
            get { return _backdrop; }

            set
            {
                _backdrop = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Backdrop)));
            }
        }

        private LanguageModel _appLanguage = LanguageService.AppLanguage;

        public LanguageModel AppLanguage
        {
            get { return _appLanguage; }

            set
            {
                _appLanguage = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AppLanguage)));
            }
        }

        private bool _notification = NotificationService.AppNotification;

        public bool Notification
        {
            get { return _notification; }

            set
            {
                _notification = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Notification)));
            }
        }

        public List<ThemeModel> ThemeList { get; } = ThemeService.ThemeList;

        public List<BackdropModel> BackdropList { get; } = BackdropService.BackdropList;

        public List<LanguageModel> LanguageList { get; } = LanguageService.LanguageList;

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsPage()
        {
            InitializeComponent();
            int BuildNumber = InfoHelper.SystemVersion.Build;
            CanUseBackdrop = BuildNumber >= 22621;
        }

        public bool IsItemChecked(string selectedInternalName, string internalName)
        {
            return selectedInternalName == internalName;
        }

        /// <summary>
        /// 背景色修改设置
        /// </summary>
        public async void OnBackdropSelectClicked(object sender, RoutedEventArgs args)
        {
            RadioMenuFlyoutItem item = sender as RadioMenuFlyoutItem;
            if (item.Tag is not null)
            {
                Backdrop = BackdropList[Convert.ToInt32(item.Tag)];
                await BackdropService.SetBackdropAsync(Backdrop);
                BackdropService.SetAppBackdrop(Program.MainWindow.Handle);
            }
        }

        /// <summary>
        /// 组合框打开时设置对应的主题色
        /// </summary>
        public void OnDropDownOpened(object sender, object args)
        {
            IReadOnlyList<Popup> PopupRoot = VisualTreeHelper.GetOpenPopupsForXamlRoot(Program.MainWindow.MainPage.XamlRoot);
            foreach (Popup popup in PopupRoot)
            {
                if (popup.Child as Canvas is not null)
                {
                    (popup.Child as Canvas).RequestedTheme = Program.MainWindow.MainPage.WindowTheme;
                }
            }
        }

        /// <summary>
        /// 语言设置说明
        /// </summary>
        public void OnLanguageTipClicked(object sender, RoutedEventArgs args)
        {
            NavigationService.NavigateTo(typeof(AboutPage), AppNaviagtionArgs.SettingsHelp);
        }

        /// <summary>
        /// 设置是否开启应用通知
        /// </summary>
        public async void OnNotificationToggled(object sender, RoutedEventArgs args)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch is not null)
            {
                await NotificationService.SetNotificationAsync(toggleSwitch.IsOn);
                Notification = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 应用默认语言修改
        /// </summary>
        public async void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (args.RemovedItems.Count > 0)
            {
                AppLanguage = args.AddedItems[0] as LanguageModel;
                await LanguageService.SetLanguageAsync(AppLanguage);
                new LanguageChangeNotification().Show();
            }
        }

        /// <summary>
        /// 打开系统主题设置
        /// </summary>
        public void OnSettingsColorClicked(object sender, RoutedEventArgs args)
        {
            Process.Start("explorer.exe", "ms-settings:colors");
        }

        /// <summary>
        /// 打开系统通知设置
        /// </summary>
        public void OnSettingsNotificationClicked(object sender, RoutedEventArgs args)
        {
            Process.Start("explorer.exe", "ms-settings:notifications");
        }

        /// <summary>
        /// 主题修改设置
        /// </summary>
        public async void OnThemeSelectClicked(object sender, RoutedEventArgs args)
        {
            RadioMenuFlyoutItem item = sender as RadioMenuFlyoutItem;
            if (item.Tag is not null)
            {
                Theme = ThemeList[Convert.ToInt32(item.Tag)];
                await ThemeService.SetThemeAsync(Theme);
                ThemeService.SetWindowTheme();
            }
        }
    }
}
