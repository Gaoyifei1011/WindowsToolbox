using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using WindowsTools.Strings;
using WindowsTools.UI.Backdrop;

namespace WindowsTools.Services.Root
{
    /// <summary>
    /// 应用资源服务
    /// </summary>
    public static class ResourceService
    {
        public static List<KeyValuePair<string, string>> ThemeList { get; } = [];

        public static List<KeyValuePair<string, string>> BackdropList { get; } = [];

        public static List<KeyValuePair<string, string>> DoEngineModeList { get; } = [];

        /// <summary>
        /// 初始化应用本地化信息
        /// </summary>
        public static void LocalizeReosurce()
        {
            InitializeBackdropList();
            InitializeThemeList();
            InitializeDoEngineModeList();
        }

        /// <summary>
        /// 初始化应用主题信息列表
        /// </summary>
        private static void InitializeThemeList()
        {
            ThemeList.Add(new KeyValuePair<string, string>(Convert.ToString(ElementTheme.Default), Settings.ThemeDefault));
            ThemeList.Add(new KeyValuePair<string, string>(Convert.ToString(ElementTheme.Light), Settings.ThemeLight));
            ThemeList.Add(new KeyValuePair<string, string>(Convert.ToString(ElementTheme.Dark), Settings.ThemeDark));
        }

        /// <summary>
        /// 初始化应用背景色信息列表
        /// </summary>
        private static void InitializeBackdropList()
        {
            BackdropList.Add(new KeyValuePair<string, string>(nameof(ElementTheme.Default), Settings.BackdropDefault));
            BackdropList.Add(new KeyValuePair<string, string>(nameof(MicaKind) + nameof(MicaKind.Base), Settings.BackdropMica));
            BackdropList.Add(new KeyValuePair<string, string>(nameof(MicaKind) + nameof(MicaKind.BaseAlt), Settings.BackdropMicaAlt));
            BackdropList.Add(new KeyValuePair<string, string>(nameof(DesktopAcrylicKind) + nameof(DesktopAcrylicKind.Default), Settings.BackdropAcrylic));
            BackdropList.Add(new KeyValuePair<string, string>(nameof(DesktopAcrylicKind) + nameof(DesktopAcrylicKind.Base), Settings.BackdropAcrylicBase));
            BackdropList.Add(new KeyValuePair<string, string>(nameof(DesktopAcrylicKind) + nameof(DesktopAcrylicKind.Thin), Settings.BackdropAcrylicThin));
        }

        /// <summary>
        /// 初始化下载引擎方式信息列表
        /// </summary>
        private static void InitializeDoEngineModeList()
        {
            DoEngineModeList.Add(new KeyValuePair<string, string>("DeliveryOptimization", Settings.DoEngineDo));
            DoEngineModeList.Add(new KeyValuePair<string, string>("BITS", Settings.DoEngineBits));
        }
    }
}
