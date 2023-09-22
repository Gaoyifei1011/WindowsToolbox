using FileRenamer.Extensions.DataType.Constant;
using FileRenamer.Models;
using FileRenamer.Services.Root;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;

namespace FileRenamer.Services.Controls.Settings
{
    /// <summary>
    /// 应用主题设置服务
    /// </summary>
    public static class ThemeService
    {
        private static string ThemeSettingsKey { get; } = ConfigKey.ThemeKey;

        private static GroupOptionsModel DefaultAppTheme { get; set; }

        public static GroupOptionsModel AppTheme { get; set; }

        public static List<GroupOptionsModel> ThemeList { get; set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的主题值
        /// </summary>
        public static void InitializeTheme()
        {
            ThemeList = ResourceService.ThemeList;

            DefaultAppTheme = ThemeList.Find(item => item.SelectedValue == Convert.ToString(ElementTheme.Default));

            AppTheme = GetTheme();
        }

        /// <summary>
        /// 获取设置存储的主题值，如果设置没有存储，使用默认值
        /// </summary>
        private static GroupOptionsModel GetTheme()
        {
            string theme = ConfigService.ReadSetting<string>(ThemeSettingsKey);

            if (string.IsNullOrEmpty(theme))
            {
                SetTheme(DefaultAppTheme);
                return ThemeList.Find(item => item.SelectedValue.Equals(DefaultAppTheme.SelectedValue, StringComparison.OrdinalIgnoreCase));
            }

            return ThemeList.Find(item => item.SelectedValue.Equals(theme, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 应用主题发生修改时修改设置存储的主题值
        /// </summary>
        public static void SetTheme(GroupOptionsModel theme)
        {
            AppTheme = theme;

            ConfigService.SaveSetting(ThemeSettingsKey, theme.SelectedValue);
        }

        /// <summary>
        /// 设置应用显示的主题
        /// </summary>
        public static void SetWindowTheme()
        {
            Program.MainWindow.MainPage.WindowTheme = (ElementTheme)Enum.Parse(typeof(ElementTheme), AppTheme.SelectedValue);
        }
    }
}
