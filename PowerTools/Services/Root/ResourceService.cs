using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;
using Windows.UI.Xaml;
using PowerTools.UI.Backdrop;

namespace PowerTools.Services.Root
{
    /// <summary>
    /// 应用资源服务
    /// </summary>
    public static class ResourceService
    {
        private static Assembly CurrentAssembly { get; } = Assembly.GetExecutingAssembly();

        public static List<KeyValuePair<string, string>> ThemeList { get; } = [];

        public static List<KeyValuePair<string, string>> BackdropList { get; } = [];

        public static List<KeyValuePair<string, string>> DoEngineModeList { get; } = [];

        public static ResourceManager AboutResource { get; } = new("PowerTools.Strings.About", CurrentAssembly);

        public static ResourceManager AllToolsResource { get; } = new("PowerTools.Strings.AllTools", CurrentAssembly);

        public static ResourceManager SwitchThemeResource { get; } = new("PowerTools.Strings.SwitchTheme", CurrentAssembly);

        public static ResourceManager ColorPickerResource { get; } = new("PowerTools.Strings.ColorPicker", CurrentAssembly);

        public static ResourceManager ContextMenuManagerResource { get; } = new("PowerTools.Strings.ContextMenuManager", CurrentAssembly);

        public static ResourceManager DialogResource { get; } = new("PowerTools.Strings.Dialog", CurrentAssembly);

        public static ResourceManager DownloadManagerResource { get; } = new("PowerTools.Strings.DownloadManager", CurrentAssembly);

        public static ResourceManager DriverManagerResource { get; } = new("PowerTools.Strings.DriverManager", CurrentAssembly);

        public static ResourceManager ExtensionNameResource { get; } = new("PowerTools.Strings.ExtensionName", CurrentAssembly);

        public static ResourceManager FileCertificateResource { get; } = new("PowerTools.Strings.FileCertificate", CurrentAssembly);

        public static ResourceManager FileManagerResource { get; } = new("PowerTools.Strings.FileManager", CurrentAssembly);

        public static ResourceManager FileNameResource { get; } = new("PowerTools.Strings.FileName", CurrentAssembly);

        public static ResourceManager FilePropertiesResource { get; } = new("PowerTools.Strings.FileProperties", CurrentAssembly);

        public static ResourceManager FileUnlockResource { get; } = new("PowerTools.Strings.FileUnlock", CurrentAssembly);

        public static ResourceManager IconExtractResource { get; } = new("PowerTools.Strings.IconExtract", CurrentAssembly);

        public static ResourceManager LoafResource { get; } = new("PowerTools.Strings.Loaf", CurrentAssembly);

        public static ResourceManager LoopbackManagerResource { get; } = new("PowerTools.Strings.LoopbackManager", CurrentAssembly);

        public static ResourceManager NotificationResource { get; } = new("PowerTools.Strings.Notification", CurrentAssembly);

        public static ResourceManager PriExtractResource { get; } = new("PowerTools.Strings.PriExtract", CurrentAssembly);

        public static ResourceManager SettingsResource { get; } = new("PowerTools.Strings.Settings", CurrentAssembly);

        public static ResourceManager ShellMenuResource { get; } = new("PowerTools.Strings.ShellMenu", CurrentAssembly);

        public static ResourceManager SimulateUpdateResource { get; } = new("PowerTools.Strings.SimulateUpdate", CurrentAssembly);

        public static ResourceManager SystemInfoResource { get; } = new("PowerTools.Strings.SystemInfo", CurrentAssembly);

        public static ResourceManager UpdateManagerResource { get; } = new("PowerTools.Strings.UpdateManager", CurrentAssembly);

        public static ResourceManager UpperAndLowerCaseResource { get; } = new("PowerTools.Strings.UpperAndLowerCase", CurrentAssembly);

        public static ResourceManager WindowResource { get; } = new("PowerTools.Strings.Window", CurrentAssembly);

        public static ResourceManager WinSATResource { get; } = new("PowerTools.Strings.WinSAT", CurrentAssembly);

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
            ThemeList.Add(new KeyValuePair<string, string>(nameof(ElementTheme.Default), SettingsResource.GetString("ThemeDefault")));
            ThemeList.Add(new KeyValuePair<string, string>(nameof(ElementTheme.Light), SettingsResource.GetString("ThemeLight")));
            ThemeList.Add(new KeyValuePair<string, string>(nameof(ElementTheme.Dark), SettingsResource.GetString("ThemeDark")));
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
