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
        public static List<DictionaryEntry> ThemeList { get; } = new List<DictionaryEntry>();

        public static List<DictionaryEntry> BackdropList { get; } = new List<DictionaryEntry>();

        /// <summary>
        /// 初始化应用本地化信息
        /// </summary>
        public static void LocalizeReosurce()
        {
            InitializeBackdropList();
            InitializeThemeList();
        }

        /// <summary>
        /// 初始化应用主题信息列表
        /// </summary>
        private static void InitializeThemeList()
        {
            ThemeList.Add(new DictionaryEntry
            {
                Key = Settings.ThemeDefault,
                Value = Convert.ToString(ElementTheme.Default)
            });
            ThemeList.Add(new DictionaryEntry
            {
                Key = Settings.ThemeLight,
                Value = Convert.ToString(ElementTheme.Light)
            });
            ThemeList.Add(new DictionaryEntry
            {
                Key = Settings.ThemeDark,
                Value = Convert.ToString(ElementTheme.Dark)
            });
        }

        /// <summary>
        /// 初始化应用背景色信息列表
        /// </summary>
        private static void InitializeBackdropList()
        {
            BackdropList.Add(new DictionaryEntry
            {
                Key = Settings.BackdropDefault,
                Value = nameof(SystemBackdropTheme.Default)
            });

            BackdropList.Add(new DictionaryEntry
            {
                Key = Settings.BackdropMica,
                Value = nameof(MicaKind) + nameof(MicaKind.Base)
            });

            BackdropList.Add(new DictionaryEntry
            {
                Key = Settings.BackdropMicaAlt,
                Value = nameof(MicaKind) + nameof(MicaKind.BaseAlt)
            });

            BackdropList.Add(new DictionaryEntry
            {
                Key = Settings.BackdropAcrylic,
                Value = nameof(DesktopAcrylicKind) + nameof(DesktopAcrylicKind.Default)
            });

            BackdropList.Add(new DictionaryEntry
            {
                Key = Settings.BackdropAcrylicBase,
                Value = nameof(DesktopAcrylicKind) + nameof(DesktopAcrylicKind.Base)
            });

            BackdropList.Add(new DictionaryEntry
            {
                Key = Settings.BackdropAcrylicThin,
                Value = nameof(DesktopAcrylicKind) + nameof(DesktopAcrylicKind.Thin)
            });
        }
    }
}
