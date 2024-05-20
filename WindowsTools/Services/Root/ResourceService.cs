using Microsoft.UI.Composition.SystemBackdrops;
using System;
using System.Collections;
using System.Collections.Generic;
using Windows.UI.Xaml;
using WindowsTools.Strings;

namespace WindowsTools.Services.Root
{
    /// <summary>
    /// 应用资源服务
    /// </summary>
    public static class ResourceService
    {
        public static List<DictionaryEntry> ThemeList { get; } = [];

        public static List<DictionaryEntry> BackdropList { get; } = [];

        public static List<DictionaryEntry> DoEngineModeList { get; } = [];

        public static List<DictionaryEntry> ExitModeList { get; } = [];

        /// <summary>
        /// 初始化应用本地化信息
        /// </summary>
        public static void LocalizeReosurce()
        {
            InitializeBackdropList();
            InitializeThemeList();
            InitializeDoEngineModeList();
            InitializeExitModeList();
        }

        /// <summary>
        /// 初始化应用主题信息列表
        /// </summary>
        private static void InitializeThemeList()
        {
            ThemeList.Add(new DictionaryEntry(Settings.ThemeDefault, Convert.ToString(ElementTheme.Default)));
            ThemeList.Add(new DictionaryEntry(Settings.ThemeLight, Convert.ToString(ElementTheme.Light)));
            ThemeList.Add(new DictionaryEntry(Settings.ThemeDark, Convert.ToString(ElementTheme.Dark)));
        }

        /// <summary>
        /// 初始化应用背景色信息列表
        /// </summary>
        private static void InitializeBackdropList()
        {
            BackdropList.Add(new DictionaryEntry(Settings.BackdropDefault, nameof(SystemBackdropTheme.Default)));
            BackdropList.Add(new DictionaryEntry(Settings.BackdropMica, nameof(MicaKind) + nameof(MicaKind.Base)));
            BackdropList.Add(new DictionaryEntry(Settings.BackdropMicaAlt, nameof(MicaKind) + nameof(MicaKind.BaseAlt)));
            BackdropList.Add(new DictionaryEntry(Settings.BackdropAcrylic, nameof(DesktopAcrylicKind) + nameof(DesktopAcrylicKind.Default)));
            BackdropList.Add(new DictionaryEntry(Settings.BackdropAcrylicBase, nameof(DesktopAcrylicKind) + nameof(DesktopAcrylicKind.Base)));
            BackdropList.Add(new DictionaryEntry(Settings.BackdropAcrylicThin, nameof(DesktopAcrylicKind) + nameof(DesktopAcrylicKind.Thin)));
        }

        /// <summary>
        /// 初始化下载引擎方式信息列表
        /// </summary>
        private static void InitializeDoEngineModeList()
        {
            DoEngineModeList.Add(new DictionaryEntry(Settings.DoEngineDo, "DeliveryOptimization"));
            DoEngineModeList.Add(new DictionaryEntry(Settings.DoEngineBits, "BITS"));
        }

        /// <summary>
        /// 初始化应用退出方式信息列表
        /// </summary>
        private static void InitializeExitModeList()
        {
            ExitModeList.Add(new DictionaryEntry(Settings.ExitApp, "ExitApp"));
            ExitModeList.Add(new DictionaryEntry(Settings.CloseWindow, "CloseWindow"));
        }
    }
}
