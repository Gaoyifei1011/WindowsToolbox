using System;
using System.Diagnostics.Tracing;
using System.IO;
using WindowsTools.Helpers.Root;
using WindowsTools.Services.Root;
using WindowsTools.Strings;
using WindowsTools.WindowsAPI.PInvoke.Shell32;

namespace WindowsTools.Services.Shell
{
    /// <summary>
    /// 自定义扩展菜单服务
    /// </summary>
    public static class ShellMenuService
    {
        private static readonly string shellMenuKey = @"Software\WindowsTools\ShellMenu";
        private static Guid FOLDERID_LocalAppData = new("F1B32785-6FBA-4FCF-9D55-7B8E7F157091");
        public static DirectoryInfo ShellMenuConfigDirectory { get; private set; }

        public static string RootMenuIconPath { get; private set; }

        public static string RootMenuText { get; private set; }

        /// <summary>
        /// 初始化自定义扩展菜单配置
        /// </summary>
        public static void InitializeShellMenu()
        {
            InitializeConfigDirectory();
            InitializeRootMenu();
        }

        /// <summary>
        /// 初始化配置存储目录
        /// </summary>
        private static void InitializeConfigDirectory()
        {
            Shell32Library.SHGetKnownFolderPath(FOLDERID_LocalAppData, KNOWN_FOLDER_FLAG.KF_FLAG_FORCE_APP_DATA_REDIRECTION, IntPtr.Zero, out string localAppdataPath);

            if (!string.IsNullOrEmpty(localAppdataPath))
            {
                try
                {
                    if (Directory.Exists(localAppdataPath))
                    {
                        string shellMenuPath = Path.Combine(localAppdataPath, "ShellMenu");
                        ShellMenuConfigDirectory = Directory.CreateDirectory(shellMenuPath);
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Create Shell menu config folder failed", e);
                }
            }
        }

        /// <summary>
        /// 初始化根菜单内容
        /// </summary>
        private static void InitializeRootMenu()
        {
            bool? useDefaultIcon = RegistryHelper.ReadRegistryValue<bool?>(string.Format(@"{0}\{1}", shellMenuKey, "RootMenu"), "UseDefaultIcon");
            string iconPath = RegistryHelper.ReadRegistryValue<string>(string.Format(@"{0}\{1}", shellMenuKey, "RootMenu"), "IconPath");
            bool? useDefaultText = RegistryHelper.ReadRegistryValue<bool?>(string.Format(@"{0}\{1}", shellMenuKey, "RootMenu"), "UseDefaultText");
            string text = RegistryHelper.ReadRegistryValue<string>(string.Format(@"{0}\{1}", shellMenuKey, "RootMenu"), "Text");

            if (useDefaultIcon.HasValue && useDefaultIcon.Value is false)
            {
                RootMenuIconPath = iconPath;
            }
            else
            {
                SetRootMenuIcon(true, string.Empty);
                RootMenuIconPath = Path.Combine(AppContext.BaseDirectory, @"Assets\WindowsTools.ico");
            }

            if (useDefaultText.HasValue && useDefaultText is false)
            {
                RootMenuText = text;
            }
            else
            {
                SetRootMenuText(true, string.Empty);
                RootMenuText = ShellMenu.DefaultRootText;
            }
        }

        /// <summary>
        /// 设置根菜单图标
        /// </summary>
        public static void SetRootMenuIcon(bool useDefaultIcon, string iconPath = "")
        {
            RegistryHelper.SaveRegistryValue(string.Format(@"{0}\{1}", shellMenuKey, "RootMenu"), "UseDefaultIcon", useDefaultIcon);
            if (useDefaultIcon)
            {
                RootMenuIconPath = Path.Combine(AppContext.BaseDirectory, @"Assets\WindowsTools.ico");
                RegistryHelper.SaveRegistryValue(string.Format(@"{0}\{1}", shellMenuKey, "RootMenu"), "IconPath", string.Empty);
            }
            else
            {
                RootMenuIconPath = iconPath;
                RegistryHelper.SaveRegistryValue(string.Format(@"{0}\{1}", shellMenuKey, "RootMenu"), "IconPath", iconPath);
            }
        }

        /// <summary>
        /// 设置根菜单文本
        /// </summary>
        public static void SetRootMenuText(bool useDefaultText, string text = "")
        {
            RegistryHelper.SaveRegistryValue(string.Format(@"{0}\{1}", shellMenuKey, "RootMenu"), "UseDefaultText", useDefaultText);
            if (useDefaultText)
            {
                RootMenuText = ShellMenu.DefaultRootText;
                RegistryHelper.SaveRegistryValue(string.Format(@"{0}\{1}", shellMenuKey, "RootMenu"), "Text", string.Empty);
            }
            else
            {
                RootMenuText = text;
                RegistryHelper.SaveRegistryValue(string.Format(@"{0}\{1}", shellMenuKey, "RootMenu"), "Text", text);
            }
        }
    }
}
