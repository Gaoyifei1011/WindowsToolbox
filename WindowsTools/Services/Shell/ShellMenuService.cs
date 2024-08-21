using System;
using System.Diagnostics.Tracing;
using System.IO;
using System.Threading.Tasks;
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
        // Stable Software\WindowsTools\Personalize\ShellMenu
        private static readonly string shellMenuKey = @"Software\WindowsTools\ShellMenuTest";

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
            // 获取根菜单项下的所有子项（包括递归后的项）
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
            RegistryHelper.SaveRegistryKey(menuKey, "MenuGuid", shellMenuItem.MenuGuid.ToString());
            RegistryHelper.SaveRegistryKey(menuKey, "MenuTitleText", shellMenuItem.MenuTitleText);
            RegistryHelper.SaveRegistryKey(menuKey, "ShouldUseIcon", shellMenuItem.ShouldUseIcon);
            RegistryHelper.SaveRegistryKey(menuKey, "ShouldUseProgramIcon", shellMenuItem.ShouldUseProgramIcon);
            RegistryHelper.SaveRegistryKey(menuKey, "ShouldUseThemeIcon", shellMenuItem.ShouldUseThemeIcon);
            RegistryHelper.SaveRegistryKey(menuKey, "DefaultIconPath", shellMenuItem.DefaultIconPath);
            RegistryHelper.SaveRegistryKey(menuKey, "LightThemeIconPath", shellMenuItem.LightThemeIconPath);
            RegistryHelper.SaveRegistryKey(menuKey, "DarkThemeIconPath", shellMenuItem.DarkThemeIconPath);
            RegistryHelper.SaveRegistryKey(menuKey, "MenuProgramPathText", shellMenuItem.MenuProgramPathText);
            RegistryHelper.SaveRegistryKey(menuKey, "MenuParameter", shellMenuItem.MenuParameter);
            RegistryHelper.SaveRegistryKey(menuKey, "FolderBackground", shellMenuItem.FolderBackground);
            RegistryHelper.SaveRegistryKey(menuKey, "FolderDesktop", shellMenuItem.FolderDesktop);
            RegistryHelper.SaveRegistryKey(menuKey, "FolderDirectory", shellMenuItem.FolderDirectory);
            RegistryHelper.SaveRegistryKey(menuKey, "FolderDrive", shellMenuItem.FolderDrive);
            RegistryHelper.SaveRegistryKey(menuKey, "MenuFileMatchRule", shellMenuItem.MenuFileMatchRule);
            RegistryHelper.SaveRegistryKey(menuKey, "MenuFileMatchFormatText", shellMenuItem.MenuFileMatchFormatText);
            RegistryHelper.SaveRegistryKey(menuKey, "MenuIndex", shellMenuItem.MenuIndex);
        }

        /// <summary>
        /// 删除菜单项
        /// </summary>
        public static void RemoveShellMenuItem(string menuKey)
        {
            Task.Run(() =>
            {
                // 获取当前菜单项下的所有子项（包括递归后的项）
                RegistryEnumKeyItem menuKeyItem = RegistryHelper.EnumSubKey(menuKey);

                EnumRemoveMenuItem(menuKeyItem);
                RegistryHelper.RemoveRegistryKey(menuKey);
            });
        }

        /// <summary>
        /// 枚举并递归所有子项
        /// </summary>
        private static ShellMenuItem EnumShellMenuItem(RegistryEnumKeyItem registryEnumKeyItem)
        {
            // 获取该项菜单内容
            ShellMenuItem currentMenuItem = GetShellItemInfo(registryEnumKeyItem.RootKey);
            ShellMenuItem shellMenuItem = new()
            {
                MenuKey = currentMenuItem.MenuKey,
                MenuGuid = currentMenuItem.MenuGuid,
                MenuTitleText = currentMenuItem.MenuTitleText,
                ShouldUseIcon = currentMenuItem.ShouldUseIcon,
                ShouldUseProgramIcon = currentMenuItem.ShouldUseProgramIcon,
                ShouldUseThemeIcon = currentMenuItem.ShouldUseThemeIcon,
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
                MenuFileMatchFormatText = currentMenuItem.MenuFileMatchFormatText,
                MenuIndex = currentMenuItem.MenuIndex
            };

            // 获取子菜单项
            if (registryEnumKeyItem.SubRegistryKeyList.Count > 0)
            {
                foreach (RegistryEnumKeyItem subRegistryKeyItem in registryEnumKeyItem.SubRegistryKeyList)
                {
                    shellMenuItem.SubShellMenuItem.Add(EnumShellMenuItem(subRegistryKeyItem));
                }

                // 递归获取完子列表后，根据列表的索引进行排序
                shellMenuItem.SubShellMenuItem.Sort((item1, item2) => item1.MenuIndex.CompareTo(item2.MenuIndex));
            }

            return shellMenuItem;
        }

        /// <summary>
        /// 枚举并递归删除所有该项及子项的资源文件
        /// </summary>
        private static void EnumRemoveMenuItem(RegistryEnumKeyItem registryEnumKeyItem)
        {
            // 获取该项菜单内容
            ShellMenuItem currentMenuItem = GetShellItemInfo(registryEnumKeyItem.RootKey);

            // 删除该项菜单的所有图标文件
            if (File.Exists(currentMenuItem.DefaultIconPath))
            {
                try
                {
                    File.Delete(currentMenuItem.DefaultIconPath);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, string.Format("Delete default icon {0} failed", currentMenuItem.DefaultIconPath), e);
                }
            }

            if (File.Exists(currentMenuItem.LightThemeIconPath))
            {
                try
                {
                    File.Delete(currentMenuItem.LightThemeIconPath);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, string.Format("Delete light theme icon {0} failed", currentMenuItem.LightThemeIconPath), e);
                }
            }

            if (File.Exists(currentMenuItem.DarkThemeIconPath))
            {
                try
                {
                    File.Delete(currentMenuItem.DarkThemeIconPath);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, string.Format("Delete dark icon {0} failed", currentMenuItem.DarkThemeIconPath), e);
                }
            }

            // 查找图标对应的文件夹路径，并删除文件夹
            try
            {
                if (!string.IsNullOrEmpty(currentMenuItem.DefaultIconPath))
                {
                    string defaultIconDirectoryPath = Path.GetDirectoryName(currentMenuItem.DefaultIconPath);

                    if (Directory.Exists(defaultIconDirectoryPath))
                    {
                        Directory.Delete(defaultIconDirectoryPath);
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, "Delete default icon path failed", e);
            }

            try
            {
                if (!string.IsNullOrEmpty(currentMenuItem.LightThemeIconPath))
                {
                    string lightThemeIconDirectoryPath = Path.GetDirectoryName(currentMenuItem.LightThemeIconPath);

                    if (Directory.Exists(lightThemeIconDirectoryPath))
                    {
                        Directory.Delete(lightThemeIconDirectoryPath);
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, "Delete light icon path failed", e);
            }

            try
            {
                if (!string.IsNullOrEmpty(currentMenuItem.DarkThemeIconPath))
                {
                    string darkThemeIconDirectoryPath = Path.GetDirectoryName(currentMenuItem.DarkThemeIconPath);

                    if (Directory.Exists(darkThemeIconDirectoryPath))
                    {
                        Directory.Delete(darkThemeIconDirectoryPath);
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, "Delete icon path failed", e);
            }

            // 获取子菜单项
            if (registryEnumKeyItem.SubRegistryKeyList.Count > 0)
            {
                foreach (RegistryEnumKeyItem subRegistryKeyItem in registryEnumKeyItem.SubRegistryKeyList)
                {
                    EnumRemoveMenuItem(subRegistryKeyItem);
                }
            }
        }

        /// <summary>
        /// 获取菜单项信息
        /// </summary>
        private static ShellMenuItem GetShellItemInfo(string menuKey)
        {
            ShellMenuItem shellMenuItem = new();

            string menuTitleText = RegistryHelper.ReadRegistryKey<string>(menuKey, "MenuTitleText");
            string menuGuid = RegistryHelper.ReadRegistryKey<string>(menuKey, "MenuGuid");
            bool? shouldUseIcon = RegistryHelper.ReadRegistryKey<bool?>(menuKey, "ShouldUseIcon");
            bool? shouldUseProgramIcon = RegistryHelper.ReadRegistryKey<bool?>(menuKey, "ShouldUseProgramIcon");
            bool? ShouldUseThemeIcon = RegistryHelper.ReadRegistryKey<bool?>(menuKey, "ShouldUseThemeIcon");
            string defaultIconPath = RegistryHelper.ReadRegistryKey<string>(menuKey, "DefaultIconPath");
            string lightThemeIconPath = RegistryHelper.ReadRegistryKey<string>(menuKey, "LightThemeIconPath");
            string darkThemeIconPath = RegistryHelper.ReadRegistryKey<string>(menuKey, "DarkThemeIconPath");
            string menuProgramPathText = RegistryHelper.ReadRegistryKey<string>(menuKey, "MenuProgramPathText");
            string menuParameter = RegistryHelper.ReadRegistryKey<string>(menuKey, "MenuParameter");
            bool? folderBackground = RegistryHelper.ReadRegistryKey<bool?>(menuKey, "FolderBackground");
            bool? folderDesktop = RegistryHelper.ReadRegistryKey<bool?>(menuKey, "FolderDesktop");
            bool? folderDirectory = RegistryHelper.ReadRegistryKey<bool?>(menuKey, "FolderDirectory");
            bool? folderDrive = RegistryHelper.ReadRegistryKey<bool?>(menuKey, "FolderDrive");
            string menuFileMatchRule = RegistryHelper.ReadRegistryKey<string>(menuKey, "MenuFileMatchRule");
            string menuFileMatchFormatText = RegistryHelper.ReadRegistryKey<string>(menuKey, "MenuFileMatchFormatText");
            int? menuIndex = RegistryHelper.ReadRegistryKey<int?>(menuKey, "MenuIndex");

            shellMenuItem.MenuKey = menuKey;
            shellMenuItem.MenuGuid = string.IsNullOrEmpty(menuGuid) ? Guid.Empty : new Guid(menuGuid);
            shellMenuItem.MenuTitleText = menuTitleText;
            shellMenuItem.ShouldUseIcon = shouldUseIcon.HasValue && shouldUseIcon.Value;
            shellMenuItem.ShouldUseProgramIcon = shouldUseProgramIcon.HasValue && shouldUseProgramIcon.Value;
            shellMenuItem.ShouldUseThemeIcon = ShouldUseThemeIcon.HasValue && ShouldUseThemeIcon.Value;
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
