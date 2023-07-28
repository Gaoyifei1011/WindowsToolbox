using FileRenamer.Extensions.DataType.Enums;
using FileRenamer.Helpers.Root;
using FileRenamer.Models;
using FileRenamer.Services.Controls.Settings;
using FileRenamer.Services.Window;
using FileRenamer.UI.Notifications;
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

        private BackdropModel _backdrop = BackdropService.AppBackdrop;

        public BackdropModel Backdrop
        {
            get { return _backdrop; }

            set
            {
                _backdrop = value;
                OnPropertyChanged();
            }
        }

        private LanguageModel _appLanguage = LanguageService.AppLanguage;

        public LanguageModel AppLanguage
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

        public List<ThemeModel> ThemeList { get; } = ThemeService.ThemeList;

        public List<BackdropModel> BackdropList { get; } = BackdropService.BackdropList;

        public List<LanguageModel> LanguageList { get; } = LanguageService.LanguageList;

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsPage()
        {
            InitializeComponent();
            int BuildNumber = InfoHelper.SystemVersion.Build;
            CanUseBackdrop = BuildNumber >= 22621;

            for (int index = 0; index < LanguageList.Count; index++)
            {
                LanguageModel languageItem = LanguageList[index];
                ToggleMenuFlyoutItem toggleMenuFlyoutItem = new ToggleMenuFlyoutItem()
                {
                    Text = languageItem.DisplayName,
                    Style = ResourceDictionaryHelper.MenuFlyoutResourceDict["ToggleMenuFlyoutItemStyle"] as Style,
                    Tag = index
                };
                if (AppLanguage.InternalName == LanguageList[index].InternalName)
                {
                    toggleMenuFlyoutItem.IsChecked = true;
                }

                toggleMenuFlyoutItem.Click += async (sender, args) =>
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

                    if (AppLanguage.InternalName != LanguageList[selectedIndex].InternalName)
                    {
                        AppLanguage = LanguageList[selectedIndex];
                        await LanguageService.SetLanguageAsync(AppLanguage);
                        new LanguageChangeNotification(this).Show();
                    }
                };
                LanguageFlyout.Items.Add(toggleMenuFlyoutItem);
            }
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
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            if (item.Tag is not null)
            {
                Backdrop = BackdropList[Convert.ToInt32(item.Tag)];
                await BackdropService.SetBackdropAsync(Backdrop);
                BackdropService.SetAppBackdrop(Program.MainWindow.Handle);
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
        /// 打开系统主题设置
        /// </summary>
        public void OnSettingsColorClicked(object sender, RoutedEventArgs args)
        {
            Process.Start("explorer.exe", "ms-settings:colors");
        }

        /// <summary>
        /// 打开系统主题色设置
        /// </summary>
        public void OnSettingsBackdropClicked(object sender, RoutedEventArgs args)
        {
            Process.Start("explorer.exe", "ms-settings:easeofaccess-visualeffects");
        }

        /// <summary>
        /// 主题修改设置
        /// </summary>
        public async void OnThemeSelectClicked(object sender, RoutedEventArgs args)
        {
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
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

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
