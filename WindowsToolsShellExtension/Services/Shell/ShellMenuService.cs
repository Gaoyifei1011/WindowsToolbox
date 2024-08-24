using System;
using System.IO;
using WindowsToolsShellExtension.Helpers.Root;
using WindowsToolsShellExtension.Strings;
using WindowsToolsShellExtension.WindowsAPI.PInvoke.Kernel32;
using WindowsToolsShellExtension.WindowsAPI.PInvoke.Shell32;

namespace WindowsToolsShellExtension.Services.Shell
{
    /// <summary>
    /// 自定义扩展菜单服务
    /// </summary>
    public static class ShellMenuService
    {
        private static readonly string shellMenuKey = @"Software\WindowsTools\ShellMenu";
        private static readonly Guid FOLDERID_LocalAppData = new("F1B32785-6FBA-4FCF-9D55-7B8E7F157091");
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
                catch (Exception)
                {
                    return;
                }
            }
        }

        /// <summary>
        /// 初始化根菜单内容
        /// </summary>
        private static unsafe void InitializeRootMenu()
        {
            string rootMenuKey = string.Format(@"{0}\{1}", shellMenuKey, "RootMenu");

            bool? useDefaultIcon = RegistryHelper.ReadRegistryKey<bool?>(rootMenuKey, "UseDefaultIcon");
            string iconPath = RegistryHelper.ReadRegistryKey<string>(rootMenuKey, "IconPath");
            bool? useDefaultText = RegistryHelper.ReadRegistryKey<bool?>(rootMenuKey, "UseDefaultText");
            string text = RegistryHelper.ReadRegistryKey<string>(rootMenuKey, "Text");

            if (useDefaultIcon.HasValue && useDefaultIcon.Value is false && File.Exists(iconPath))
            {
                RootMenuIconPath = iconPath;
            }
            else
            {
                char* pathbuffer = stackalloc char[256];
                int length = 256;
                Kernel32Library.GetCurrentPackagePath(ref length, pathbuffer);
                RootMenuIconPath = Path.Combine(new string(pathbuffer), @"Assets\WindowsTools.ico");
            }

            RootMenuText = useDefaultText.HasValue && useDefaultText is false && !string.IsNullOrEmpty(text) ? text : ShellMenu.RootItemTitle;
        }
    }
}
