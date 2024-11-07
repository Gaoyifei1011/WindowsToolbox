using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;
using Windows.UI.Xaml;
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

        public static ResourceManager AboutResource { get; } = new("WindowsTools.Strings.About", Assembly.GetExecutingAssembly());

        public static ResourceManager AllToolsResource { get; } = new("WindowsTools.Strings.AllTools", Assembly.GetExecutingAssembly());

        public static ResourceManager ColorPickerResource { get; } = new("WindowsTools.Strings.ColorPicker", Assembly.GetExecutingAssembly());

        public static ResourceManager DialogResource { get; } = new("WindowsTools.Strings.Dialog", Assembly.GetExecutingAssembly());

        public static ResourceManager DownloadManagerResource { get; } = new("WindowsTools.Strings.DownloadManager", Assembly.GetExecutingAssembly());

        public static ResourceManager DriverManagerResource { get; } = new("WindowsTools.Strings.DriverManager", Assembly.GetExecutingAssembly());

        public static ResourceManager ExtensionNameResource { get; } = new("WindowsTools.Strings.ExtensionName", Assembly.GetExecutingAssembly());

        public static ResourceManager FileCertificateResource { get; } = new("WindowsTools.Strings.FileCertificate", Assembly.GetExecutingAssembly());

        public static ResourceManager FileNameResource { get; } = new("WindowsTools.Strings.FileName", Assembly.GetExecutingAssembly());

        public static ResourceManager FilePropertiesResource { get; } = new("WindowsTools.Strings.FileProperties", Assembly.GetExecutingAssembly());

        public static ResourceManager FileUnlockResource { get; } = new("WindowsTools.Strings.FileUnlock", Assembly.GetExecutingAssembly());

        public static ResourceManager IconExtractResource { get; } = new("WindowsTools.Strings.IconExtract", Assembly.GetExecutingAssembly());

        public static ResourceManager LoafResource { get; } = new("WindowsTools.Strings.Loaf", Assembly.GetExecutingAssembly());

        public static ResourceManager LoopbackManagerResource { get; } = new("WindowsTools.Strings.LoopbackManager", Assembly.GetExecutingAssembly());

        public static ResourceManager NotificationResource { get; } = new("WindowsTools.Strings.Notification", Assembly.GetExecutingAssembly());

        public static ResourceManager PriExtractResource { get; } = new("WindowsTools.Strings.PriExtract", Assembly.GetExecutingAssembly());

        public static ResourceManager SettingsResource { get; } = new("WindowsTools.Strings.Settings", Assembly.GetExecutingAssembly());

        public static ResourceManager ShellMenuResource { get; } = new("WindowsTools.Strings.ShellMenu", Assembly.GetExecutingAssembly());

        public static ResourceManager SimulateUpdateResource { get; } = new("WindowsTools.Strings.SimulateUpdate", Assembly.GetExecutingAssembly());

        public static ResourceManager SystemInfoResource { get; } = new("WindowsTools.Strings.SystemInfo", Assembly.GetExecutingAssembly());

        public static ResourceManager UpdateManagerResource { get; } = new("WindowsTools.Strings.UpdateManager", Assembly.GetExecutingAssembly());

        public static ResourceManager UpperAndLowerCaseResource { get; } = new("WindowsTools.Strings.UpperAndLowerCase", Assembly.GetExecutingAssembly());

        public static ResourceManager WindowResource { get; } = new("WindowsTools.Strings.Window", Assembly.GetExecutingAssembly());

        public static ResourceManager WinSATResource { get; } = new("WindowsTools.Strings.WinSAT", Assembly.GetExecutingAssembly());

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
            ThemeList.Add(new KeyValuePair<string, string>(Convert.ToString(ElementTheme.Default), SettingsResource.GetString("ThemeDefault")));
            ThemeList.Add(new KeyValuePair<string, string>(Convert.ToString(ElementTheme.Light), SettingsResource.GetString("ThemeLight")));
            ThemeList.Add(new KeyValuePair<string, string>(Convert.ToString(ElementTheme.Dark), SettingsResource.GetString("ThemeDark")));
        }

        /// <summary>
        /// 初始化应用背景色信息列表
        /// </summary>
        private static void InitializeBackdropList()
        {
            BackdropList.Add(new KeyValuePair<string, string>(nameof(ElementTheme.Default), SettingsResource.GetString("BackdropDefault")));
            BackdropList.Add(new KeyValuePair<string, string>(nameof(MicaKind) + nameof(MicaKind.Base), SettingsResource.GetString("BackdropMica")));
            BackdropList.Add(new KeyValuePair<string, string>(nameof(MicaKind) + nameof(MicaKind.BaseAlt), SettingsResource.GetString("BackdropMicaAlt")));
            BackdropList.Add(new KeyValuePair<string, string>(nameof(DesktopAcrylicKind) + nameof(DesktopAcrylicKind.Default), SettingsResource.GetString("BackdropAcrylic")));
            BackdropList.Add(new KeyValuePair<string, string>(nameof(DesktopAcrylicKind) + nameof(DesktopAcrylicKind.Base), SettingsResource.GetString("BackdropAcrylicBase")));
            BackdropList.Add(new KeyValuePair<string, string>(nameof(DesktopAcrylicKind) + nameof(DesktopAcrylicKind.Thin), SettingsResource.GetString("BackdropAcrylicThin")));
        }

        /// <summary>
        /// 初始化下载引擎方式信息列表
        /// </summary>
        private static void InitializeDoEngineModeList()
        {
            DoEngineModeList.Add(new KeyValuePair<string, string>("DeliveryOptimization", SettingsResource.GetString("DoEngineDo")));
            DoEngineModeList.Add(new KeyValuePair<string, string>("BITS", SettingsResource.GetString("DoEngineBits")));
        }
    }
}
