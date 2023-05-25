using FileRenamer.Models.Controls.Settings.Appearance;
using FileRenamer.Services.Controls.Settings.Appearance;
using FileRenamer.ViewModels.Base;
using FileRenamer.Views.CustomControls.DialogsAndFlyouts;
using System;
using System.Collections.Generic;
using Windows.System;
using Windows.UI.Xaml;

namespace FileRenamer.ViewModels.Controls.Settings.Appearance
{
    public sealed class ThemeViewModel : ViewModelBase
    {
        public List<ThemeModel> ThemeList { get; } = ThemeService.ThemeList;

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

        /// <summary>
        /// 打开系统主题设置
        /// </summary>
        public async void OnSettingsColorClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:colors"));
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
