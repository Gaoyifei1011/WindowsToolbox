using FileRenamer.Helpers.Root;
using FileRenamer.Models;
using FileRenamer.Services.Controls.Settings;
using FileRenamer.UI.Notifications;
using GetStoreApp.Services.Controls.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FileRenamer.Views.Pages
{
    /// <summary>
    /// 设置页面
    /// </summary>
    public sealed partial class SettingsPage : Page, INotifyPropertyChanged
    {
        public bool CanUseBackdrop { get; set; }

        private GroupOptionsModel _theme = ThemeService.AppTheme;

        public GroupOptionsModel Theme
        {
            get { return _theme; }

            set
            {
                _theme = value;
                OnPropertyChanged();
            }
        }

        private GroupOptionsModel _backdrop = BackdropService.AppBackdrop;

        public GroupOptionsModel Backdrop
        {
            get { return _backdrop; }

            set
            {
                _backdrop = value;
                OnPropertyChanged();
            }
        }

        private bool _alwaysShowBackdropValue = AlwaysShowBackdropService.AlwaysShowBackdropValue;

        public bool AlwaysShowBackdropValue
        {
            get { return _alwaysShowBackdropValue; }

            set
            {
                _alwaysShowBackdropValue = value;
                OnPropertyChanged();
            }
        }

        private GroupOptionsModel _appLanguage = LanguageService.AppLanguage;

        public GroupOptionsModel AppLanguage
        {
            get { return _appLanguage; }

            set
            {
                _appLanguage = value;
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

        public List<GroupOptionsModel> ThemeList { get; } = ThemeService.ThemeList;

        public List<GroupOptionsModel> BackdropList { get; } = BackdropService.BackdropList;

        public List<GroupOptionsModel> LanguageList { get; } = LanguageService.LanguageList;

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsPage()
        {
            InitializeComponent();
            int BuildNumber = InfoHelper.SystemVersion.Build;
            CanUseBackdrop = BuildNumber >= 22621;

            for (int index = 0; index < LanguageList.Count; index++)
            {
                GroupOptionsModel languageItem = LanguageList[index];
                ToggleMenuFlyoutItem toggleMenuFlyoutItem = new ToggleMenuFlyoutItem()
                {
                    Text = languageItem.DisplayMember,
                    Style = ResourceDictionaryHelper.MenuFlyoutResourceDict["ToggleMenuFlyoutItemStyle"] as Style,
                    Tag = index
                };

                if (AppLanguage.SelectedValue == LanguageList[index].SelectedValue)
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

                    if (AppLanguage.SelectedValue != LanguageList[selectedIndex].SelectedValue)
                    {
                        AppLanguage = LanguageList[selectedIndex];
                        LanguageService.SetLanguage(AppLanguage);
                        new LanguageChangeNotification(this).Show();
                    }
                };
                LanguageFlyout.Items.Add(toggleMenuFlyoutItem);
            }
        }

        /// <summary>
        /// 背景色修改设置
        /// </summary>
        public void OnBackdropSelectClicked(object sender, RoutedEventArgs args)
        {
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            if (item.Tag is not null)
            {
                Backdrop = BackdropList[Convert.ToInt32(item.Tag)];
                BackdropService.SetBackdrop(Backdrop);
                BackdropService.SetAppBackdrop();
            }
        }

        /// <summary>
        /// 打开系统主题设置
        /// </summary>
        public void OnSystemThemeSettingsClicked(object sender, RoutedEventArgs args)
        {
            Process.Start("explorer.exe", "ms-settings:colors");
        }

        /// <summary>
        /// 打开系统主题色设置
        /// </summary>
        public void OnSystemBackdropSettingsClicked(object sender, RoutedEventArgs args)
        {
            Process.Start("explorer.exe", "ms-settings:easeofaccess-visualeffects");
        }

        public void OnSystemLanguageSettingsClicked(object sender, RoutedEventArgs args)
        {
            Process.Start("explorer.exe", "ms-settings:regionlanguage-languageoptions");
        }

        /// <summary>
        /// 主题修改设置
        /// </summary>
        public void OnThemeSelectClicked(object sender, RoutedEventArgs args)
        {
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            if (item.Tag is not null)
            {
                Theme = ThemeList[Convert.ToInt32(item.Tag)];
                ThemeService.SetTheme(Theme);
                ThemeService.SetWindowTheme();
            }
        }

        /// <summary>
        /// 开关按钮切换时修改相应设置
        /// </summary>
        public void OnAlwaysShowBackdropToggled(object sender, RoutedEventArgs args)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch is not null)
            {
                AlwaysShowBackdropService.SetAlwaysShowBackdrop(toggleSwitch.IsOn);
                AlwaysShowBackdropValue = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 是否开启应用窗口置顶
        /// </summary>
        public void OnTopMostToggled(object sender, RoutedEventArgs args)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch is not null)
            {
                TopMostService.SetTopMostValue(toggleSwitch.IsOn);
                TopMostService.SetAppTopMost();
                TopMostValue = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 当应用未启用背景色设置时，自动关闭始终显示背景色设置
        /// </summary>
        public void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch is not null)
            {
                AlwaysShowBackdropService.SetAlwaysShowBackdrop(false);
                AlwaysShowBackdropValue = false;
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
