using FileRenamer.Extensions.DataType.Constant;
using FileRenamer.Models.Controls.Settings.Appearance;
using FileRenamer.Services.Root;
using FileRenamer.Views.Pages;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace FileRenamer.Services.Controls.Settings.Appearance
{
    /// <summary>
    /// 应用主题设置服务
    /// </summary>
    public static class ThemeService
    {
        private static string ThemeSettingsKey { get; } = ConfigKey.ThemeKey;

        private static ThemeModel DefaultAppTheme { get; set; }

        public static ThemeModel AppTheme { get; set; }

        public static List<ThemeModel> ThemeList { get; set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的主题值
        /// </summary>
        public static async Task InitializeAsync()
        {
            ThemeList = ResourceService.ThemeList;

            DefaultAppTheme = ThemeList.Find(item => item.InternalName == Convert.ToString(ElementTheme.Default));

            (bool, ThemeModel) ThemeResult = await GetThemeAsync();

            AppTheme = ThemeResult.Item2;

            if (ThemeResult.Item1)
            {
                await SetThemeAsync(AppTheme, false);
            }
        }

        /// <summary>
        /// 获取设置存储的主题值，如果设置没有存储，使用默认值
        /// </summary>
        private static async Task<(bool, ThemeModel)> GetThemeAsync()
        {
            string theme = await ConfigService.ReadSettingAsync<string>(ThemeSettingsKey);

            if (string.IsNullOrEmpty(theme))
            {
                return (true, ThemeList.Find(item => item.InternalName.Equals(DefaultAppTheme.InternalName, StringComparison.OrdinalIgnoreCase)));
            }

            return (false, ThemeList.Find(item => item.InternalName.Equals(theme, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// 应用主题发生修改时修改设置存储的主题值
        /// </summary>
        public static async Task SetThemeAsync(ThemeModel theme, [Optional, DefaultParameterValue(true)] bool isNotFirstSet)
        {
            if (isNotFirstSet)
            {
                AppTheme = theme;
            }

            await ConfigService.SaveSettingAsync(ThemeSettingsKey, theme.InternalName);
        }

        /// <summary>
        /// 设置应用显示的主题
        /// </summary>
        public static void SetWindowTheme()
        {
            Program.MainWindow.MainPage.ViewModel.WindowTheme = (ElementTheme)Enum.Parse(typeof(ElementTheme), AppTheme.InternalName);
        }
    }
}
