﻿using System;
using WindowsTools.Extensions.DataType.Constant;
using WindowsTools.Services.Root;
using WindowsTools.Views.Windows;

namespace WindowsTools.Services.Controls.Settings
{
    /// <summary>
    /// 应用窗口置顶设置服务
    /// </summary>
    public static class TopMostService
    {
        private static string settingsKey = ConfigKey.TopMostKey;

        private static bool defaultTopMostValue = false;

        public static bool TopMostValue { get; private set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的窗口置顶值
        /// </summary>
        public static void InitializeTopMostValue()
        {
            TopMostValue = GetTopMostValue();
        }

        /// <summary>
        /// 获取设置存储的窗口置顶值，如果设置没有存储，使用默认值
        /// </summary>
        private static bool GetTopMostValue()
        {
            bool? topMostValue = LocalSettingsService.ReadSetting<bool?>(settingsKey);

            if (!topMostValue.HasValue)
            {
                SetTopMostValue(defaultTopMostValue);
                return defaultTopMostValue;
            }

            return Convert.ToBoolean(topMostValue);
        }

        /// <summary>
        /// 窗口置顶值发生修改时修改设置存储的窗口置顶值
        /// </summary>
        public static void SetTopMostValue(bool topMostValue)
        {
            TopMostValue = topMostValue;

            LocalSettingsService.SaveSetting(settingsKey, topMostValue);
        }

        /// <summary>
        /// 设置应用的窗口置顶状态
        /// </summary>
        public static void SetAppTopMost()
        {
            MainWindow.Current.TopMost = TopMostValue;
        }
    }
}
