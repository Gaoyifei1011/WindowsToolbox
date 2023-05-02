using FileRenamer.Contracts;
using FileRenamer.Extensions.Command;
using FileRenamer.Models.Controls.Settings.Appearance;
using FileRenamer.Services.Controls.Settings.Appearance;
using FileRenamer.ViewModels.Base;
using System;
using System.Collections.Generic;
using Windows.System;

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

        // 打开系统主题设置
        public IRelayCommand SettingsColorCommand => new RelayCommand(async () =>
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:colors"));
        });

        // 主题修改设置
        public IRelayCommand ThemeSelectCommand => new RelayCommand<string>(async (themeIndex) =>
        {
            Theme = ThemeList[Convert.ToInt32(themeIndex)];
            await ThemeService.SetThemeAsync(Theme);
            ThemeService.SetWindowTheme();
        });
    }
}
