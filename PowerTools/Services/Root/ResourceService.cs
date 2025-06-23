using System.Reflection;
using System.Resources;

namespace PowerTools.Services.Root
{
    /// <summary>
    /// 应用资源服务
    /// </summary>
    public static class ResourceService
    {
        private static Assembly CurrentAssembly { get; } = Assembly.GetExecutingAssembly();

        public static ResourceManager AllToolsResource { get; } = new("PowerTools.Strings.AllTools", CurrentAssembly);

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

        public static ResourceManager NotificationTipResource { get; } = new("PowerTools.Strings.NotificationTip", CurrentAssembly);

        public static ResourceManager PriExtractResource { get; } = new("PowerTools.Strings.PriExtract", CurrentAssembly);

        public static ResourceManager SettingsResource { get; } = new("PowerTools.Strings.Settings", CurrentAssembly);

        public static ResourceManager SettingsAboutResource { get; } = new("PowerTools.Strings.SettingsAbout", CurrentAssembly);

        public static ResourceManager SettingsAdvancedResource { get; } = new("PowerTools.Strings.SettingsAdvanced", CurrentAssembly);

        public static ResourceManager SettingsDownloadResource { get; } = new("PowerTools.Strings.SettingsDownload", CurrentAssembly);

        public static ResourceManager SettingsGeneralResource { get; } = new("PowerTools.Strings.SettingsGeneral", CurrentAssembly);

        public static ResourceManager ShellMenuResource { get; } = new("PowerTools.Strings.ShellMenu", CurrentAssembly);

        public static ResourceManager ShellMenuEditResource { get; } = new("PowerTools.Strings.ShellMenuEdit", CurrentAssembly);

        public static ResourceManager ShellMenuListResource { get; } = new("PowerTools.Strings.ShellMenuList", CurrentAssembly);

        public static ResourceManager SimulateUpdateResource { get; } = new("PowerTools.Strings.SimulateUpdate", CurrentAssembly);

        public static ResourceManager ThemeSwitchResource { get; } = new("PowerTools.Strings.ThemeSwitch", CurrentAssembly);

        public static ResourceManager UpdateManagerResource { get; } = new("PowerTools.Strings.UpdateManager", CurrentAssembly);

        public static ResourceManager UpperAndLowerCaseResource { get; } = new("PowerTools.Strings.UpperAndLowerCase", CurrentAssembly);

        public static ResourceManager WindowResource { get; } = new("PowerTools.Strings.Window", CurrentAssembly);

        public static ResourceManager WinSATResource { get; } = new("PowerTools.Strings.WinSAT", CurrentAssembly);
    }
}
