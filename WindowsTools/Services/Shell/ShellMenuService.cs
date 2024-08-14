using System;
using System.Diagnostics.Tracing;
using System.IO;
using WindowsTools.Extensions.Registry;
using WindowsTools.Extensions.ShellMenu;
using WindowsTools.Helpers.Root;
using WindowsTools.Services.Root;
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

        /// <summary>
        /// 初始化自定义扩展菜单配置
        /// </summary>
        public static void InitializeShellMenu()
        {
            InitializeConfigDirectory();
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
        /// 获取菜单项
        /// </summary>
        public static ShellMenuItem GetShellMenuItem()
        {
            // 获取 ShellMenu 项下的所有子项（包括递归后的项）
            RegistryEnumKeyItem shellMenuRegistryKeyItem = RegistryHelper.EnumSubKey(shellMenuKey);

            return shellMenuRegistryKeyItem.SubRegistryKeyList.Count is 1
                ? EnumShellMenuItem(shellMenuRegistryKeyItem.SubRegistryKeyList[0])
                : null;
        }

        /// <summary>
        /// 保存菜单项设置
        /// </summary>
        public static void SaveShellMenuItem(string menuKey, ShellMenuItem shellMenuItem)
        {
            menuKey = Path.Combine(shellMenuKey, menuKey);

            RegistryHelper.SaveRegistryValue(menuKey, "MenuGuid", shellMenuItem.MenuGuid.ToString());
            RegistryHelper.SaveRegistryValue(menuKey, "MenuTitleText", shellMenuItem.MenuTitleText);
            RegistryHelper.SaveRegistryValue(menuKey, "ShouldUseProgramIcon", shellMenuItem.ShouldUseProgramIcon);
            RegistryHelper.SaveRegistryValue(menuKey, "ShouldEnableThemeIcon", shellMenuItem.ShouldEnableThemeIcon);
            RegistryHelper.SaveRegistryValue(menuKey, "DefaultIconPath", shellMenuItem.DefaultIconPath);
            RegistryHelper.SaveRegistryValue(menuKey, "LightThemeIconPath", shellMenuItem.LightThemeIconPath);
            RegistryHelper.SaveRegistryValue(menuKey, "DarkThemeIconPath", shellMenuItem.DarkThemeIconPath);
            RegistryHelper.SaveRegistryValue(menuKey, "MenuProgramPathText", shellMenuItem.MenuProgramPathText);
            RegistryHelper.SaveRegistryValue(menuKey, "MenuParameter", shellMenuItem.MenuParameter);
            RegistryHelper.SaveRegistryValue(menuKey, "FolderBackground", shellMenuItem.FolderBackground);
            RegistryHelper.SaveRegistryValue(menuKey, "FolderDesktop", shellMenuItem.FolderDesktop);
            RegistryHelper.SaveRegistryValue(menuKey, "FolderDirectory", shellMenuItem.FolderDirectory);
            RegistryHelper.SaveRegistryValue(menuKey, "FolderDrive", shellMenuItem.FolderDrive);
            RegistryHelper.SaveRegistryValue(menuKey, "MenuFileMatchRule", shellMenuItem.MenuFileMatchRule);
            RegistryHelper.SaveRegistryValue(menuKey, "MenuFileMatchFormatText", shellMenuItem.MenuFileMatchFormatText);
            RegistryHelper.SaveRegistryValue(menuKey, "MenuIndex", shellMenuItem.MenuIndex);
        }

        /// <summary>
        /// 枚举并递归所有子项
        /// </summary>
        private static ShellMenuItem EnumShellMenuItem(RegistryEnumKeyItem registryEnumKeyItem)
        {
            ShellMenuItem currentMenuItem = GetShellItemInfo(registryEnumKeyItem.RootKey);
            ShellMenuItem shellMenuItem = new()
            {
                MenuKey = currentMenuItem.MenuKey,
                MenuGuid = currentMenuItem.MenuGuid,
                MenuTitleText = currentMenuItem.MenuTitleText,
                ShouldUseProgramIcon = currentMenuItem.ShouldUseProgramIcon,
                ShouldEnableThemeIcon = currentMenuItem.ShouldEnableThemeIcon,
                DefaultIconPath = currentMenuItem.DefaultIconPath,
                LightThemeIconPath = currentMenuItem.LightThemeIconPath,
                DarkThemeIconPath = currentMenuItem.DarkThemeIconPath,
                MenuProgramPathText = currentMenuItem.MenuProgramPathText,
                MenuParameter = currentMenuItem.MenuParameter,
                FolderBackground = currentMenuItem.FolderBackground,
                FolderDesktop = currentMenuItem.FolderDesktop,
                FolderDirectory = currentMenuItem.FolderDirectory,
                FolderDrive = currentMenuItem.FolderDrive,
                MenuFileMatchRule = currentMenuItem.MenuFileMatchFormatText,
                MenuFileMatchFormatText = currentMenuItem.MenuFileMatchFormatText
            };

            // 获取子菜单项
            if (registryEnumKeyItem.SubRegistryKeyList.Count > 0)
            {
                foreach (RegistryEnumKeyItem subRegistryKeyItem in registryEnumKeyItem.SubRegistryKeyList)
                {
                    shellMenuItem.SubShellMenuItem.Add(EnumShellMenuItem(subRegistryKeyItem));
                }
            }

            return shellMenuItem;
        }

        /// <summary>
        /// 获取菜单项信息
        /// </summary>
        private static ShellMenuItem GetShellItemInfo(string menuKey)
        {
            ShellMenuItem shellMenuItem = new();

            string menuTitleText = RegistryHelper.ReadRegistryValue<string>(menuKey, "MenuTitleText");
            string menuGuid = RegistryHelper.ReadRegistryValue<string>(menuKey, "MenuGuid");
            bool? shouldUseProgramIcon = RegistryHelper.ReadRegistryValue<bool?>(menuKey, "ShouldUseProgramIcon");
            bool? shouldEnableThemeIcon = RegistryHelper.ReadRegistryValue<bool?>(menuKey, "ShouldEnableThemeIcon");
            string defaultIconPath = RegistryHelper.ReadRegistryValue<string>(menuKey, "DefaultIconPath");
            string lightThemeIconPath = RegistryHelper.ReadRegistryValue<string>(menuKey, "LightThemeIconPath");
            string darkThemeIconPath = RegistryHelper.ReadRegistryValue<string>(menuKey, "DarkThemeIconPath");
            string menuProgramPathText = RegistryHelper.ReadRegistryValue<string>(menuKey, "MenuProgramPathText");
            string menuParameter = RegistryHelper.ReadRegistryValue<string>(menuKey, "MenuParameter");
            bool? folderBackground = RegistryHelper.ReadRegistryValue<bool?>(menuKey, "FolderBackground");
            bool? folderDesktop = RegistryHelper.ReadRegistryValue<bool?>(menuKey, "FolderDesktop");
            bool? folderDirectory = RegistryHelper.ReadRegistryValue<bool?>(menuKey, "FolderDirectory");
            bool? folderDrive = RegistryHelper.ReadRegistryValue<bool?>(menuKey, "FolderDrive");
            string menuFileMatchRule = RegistryHelper.ReadRegistryValue<string>(menuKey, "MenuFileMatchRule");
            string menuFileMatchFormatText = RegistryHelper.ReadRegistryValue<string>(menuKey, "MenuFileMatchFormatText");
            int? menuIndex = RegistryHelper.ReadRegistryValue<int?>(menuKey, "MenuIndex");

            shellMenuItem.MenuKey = menuKey;
            shellMenuItem.MenuGuid = string.IsNullOrEmpty(menuGuid) ? Guid.Empty : new Guid(menuGuid);
            shellMenuItem.MenuTitleText = menuTitleText;
            shellMenuItem.ShouldUseProgramIcon = shouldUseProgramIcon.HasValue && shouldUseProgramIcon.Value;
            shellMenuItem.ShouldEnableThemeIcon = shouldEnableThemeIcon.HasValue && shouldEnableThemeIcon.Value;
            shellMenuItem.DefaultIconPath = defaultIconPath;
            shellMenuItem.LightThemeIconPath = lightThemeIconPath;
            shellMenuItem.DarkThemeIconPath = darkThemeIconPath;
            shellMenuItem.MenuProgramPathText = menuProgramPathText;
            shellMenuItem.DarkThemeIconPath = darkThemeIconPath;
            shellMenuItem.MenuProgramPathText = menuProgramPathText;
            shellMenuItem.MenuParameter = menuParameter;
            shellMenuItem.FolderBackground = folderBackground.HasValue && folderBackground.Value;
            shellMenuItem.FolderDesktop = folderDesktop.HasValue && folderDesktop.Value;
            shellMenuItem.FolderDirectory = folderDirectory.HasValue && folderDirectory.Value;
            shellMenuItem.FolderDrive = folderDrive.HasValue && folderDrive.Value;
            shellMenuItem.MenuFileMatchRule = menuFileMatchRule;
            shellMenuItem.MenuFileMatchFormatText = menuFileMatchFormatText;
            shellMenuItem.MenuIndex = menuIndex ?? 0;

            return shellMenuItem;
        }
    }
}
