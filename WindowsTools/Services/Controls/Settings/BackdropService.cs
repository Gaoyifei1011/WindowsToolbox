using System;
using System.Collections;
using System.Collections.Generic;
using WindowsTools.Extensions.DataType.Constant;
using WindowsTools.Services.Root;
using WindowsTools.Views.Windows;

namespace WindowsTools.Services.Controls.Settings
{
    /// <summary>
    /// 应用背景色设置服务
    /// </summary>
    public static class BackdropService
    {
        private static string settingsKey = ConfigKey.BackdropKey;

        private static DictionaryEntry defaultAppBackdrop;

        public static DictionaryEntry AppBackdrop { get; private set; }

        public static List<DictionaryEntry> BackdropList { get; private set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的背景色值
        /// </summary>
        public static void InitializeBackdrop()
        {
            BackdropList = ResourceService.BackdropList;

            defaultAppBackdrop = BackdropList.Find(item => item.Value.ToString().Equals("Default", StringComparison.OrdinalIgnoreCase));

            AppBackdrop = GetBackdrop();
        }

        /// <summary>
        /// 获取设置存储的背景色值，如果设置没有存储，使用默认值
        /// </summary>
        private static DictionaryEntry GetBackdrop()
        {
            string backdrop = LocalSettingsService.ReadSetting<string>(settingsKey);

            if (string.IsNullOrEmpty(backdrop))
            {
                SetBackdrop(defaultAppBackdrop);
                return defaultAppBackdrop;
            }

            DictionaryEntry selectedBackdrop = BackdropList.Find(item => item.Value.Equals(backdrop));

            return selectedBackdrop.Key is null ? defaultAppBackdrop : selectedBackdrop;
        }

        /// <summary>
        /// 应用背景色发生修改时修改设置存储的背景色值
        /// </summary>
        public static void SetBackdrop(DictionaryEntry backdrop)
        {
            AppBackdrop = backdrop;

            LocalSettingsService.SaveSetting(settingsKey, backdrop.Value);
        }

        /// <summary>
        /// 设置应用显示的背景色
        /// </summary>
        public static void SetAppBackdrop()
        {
            MainWindow.Current.SetWindowBackdrop();
        }
    }
}
