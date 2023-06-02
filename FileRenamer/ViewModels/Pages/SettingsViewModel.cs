using FileRenamer.Extensions.DataType.Enums;
using FileRenamer.Models.Controls.Settings.Appearance;
using FileRenamer.Services.Controls.Settings.Appearance;
using FileRenamer.Services.Controls.Settings.Common;
using FileRenamer.Services.Window;
using FileRenamer.ViewModels.Base;
using FileRenamer.Views.CustomControls.DialogsAndFlyouts;
using FileRenamer.Views.Pages;
using System;
using System.Collections.Generic;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FileRenamer.ViewModels.Pages
{
    /// <summary>
    /// 设置页面数据模型
    /// </summary>
    public sealed class SettingsViewModel : ViewModelBase
    {
        private ThemeModel _theme = ThemeService.AppTheme;

        public ThemeModel Theme
        {
            get { return _theme; }

            set
            {
                _theme = value;
                OnPropertyChanged();
            }
        }

        private LanguageModel _language = LanguageService.AppLanguage;

        public LanguageModel Language
        {
            get { return _language; }

            set
            {
                _language = value;
                OnPropertyChanged();
            }
        }

        private bool _topMostValue = TopMostService.TopMostValue;

        public bool TopMostValue
        {
            get { return _topMostValue; }

            set
            {
                _topMostValue = value;
                OnPropertyChanged();
            }
        }

        private bool _notification = NotificationService.AppNotification;

        public bool Notification
        {
            get { return _notification; }

            set
            {
                _notification = value;
                OnPropertyChanged();
            }
        }

        public List<ThemeModel> ThemeList { get; } = ThemeService.ThemeList;

        public List<LanguageModel> LanguageList { get; } = LanguageService.LanguageList;

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
                Language = args.AddedItems[0] as LanguageModel;
                await LanguageService.SetLanguageAsync(Language);
                LanguageService.SetAppLanguage(Language);
                //new LanguageChangeNotification(true).Show();
            }
        }

        /// <summary>
        /// 打开系统主题设置
        /// </summary>
        public async void OnSettingsColorClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:colors"));
        }

        /// <summary>
        /// 打开系统通知设置
        /// </summary>
        public async void OnSettingsNotificationClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:notifications"));
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

        /// <summary>
        /// 是否开启应用窗口置顶
        /// </summary>
        public async void OnTopMostToggled(object sender, RoutedEventArgs args)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch is not null)
            {
                await TopMostService.SetTopMostValueAsync(toggleSwitch.IsOn);
                TopMostService.SetAppTopMost();
                TopMostValue = toggleSwitch.IsOn;
            }
        }
    }
}
