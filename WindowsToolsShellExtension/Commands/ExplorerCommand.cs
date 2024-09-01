using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text.RegularExpressions;
using WindowsToolsShellExtension.Extensions.ShellMenu;
using WindowsToolsShellExtension.Helpers.Root;
using WindowsToolsShellExtension.Services.Controls.Settings;
using WindowsToolsShellExtension.Services.Shell;
using WindowsToolsShellExtension.WindowsAPI.ComTypes;

namespace WindowsToolsShellExtension.Commands
{
    /// <summary>
    /// 菜单项
    /// </summary>
    [GeneratedComClass, Guid("5A730150-DE8D-0C88-FD1A-99B7E954BDDB")]
    public partial class ExplorerCommand(ShellMenuItem shellMenuItem) : IExplorerCommand, IObjectWithSite
    {
        private readonly Guid SID_SFolderView = new("CDE725B0-CCC9-4519-917E-325D72FAB4CE");
        private readonly Guid IID_IUnknown = new("00000000-0000-0000-C000-000000000046");
        private readonly StrategyBasedComWrappers strategyBasedComWrappers = new();
        private IntPtr site = IntPtr.Zero;

        [GeneratedRegex(@"{files:split'([\s\S]*)'")]
        private static partial Regex MultiFileRegex { get; }

        [GeneratedRegex(@"{folders:split'([\s\S]*)'")]
        private static partial Regex MultiFolderRegex { get; }

        /// <summary>
        /// 根菜单标题
        /// </summary>
        public int GetTitle(IShellItemArray psiItemArray, out string ppszName)
        {
            if (shellMenuItem is not null)
            {
                ppszName = shellMenuItem.MenuTitleText;
                return 0;
            }
            else
            {
                ppszName = string.Empty;
                return unchecked((int)0x80004001);
            }
        }

        /// <summary>
        /// 根菜单图标路径
        /// </summary>
        public int GetIcon(IShellItemArray psiItemArray, out string ppszIcon)
        {
            if (shellMenuItem is not null)
            {
                // 是否使用图标
                if (shellMenuItem.ShouldUseIcon)
                {
                    // 是否使用应用程序图标
                    if (shellMenuItem.ShouldUseProgramIcon)
                    {
                        if (File.Exists(shellMenuItem.MenuProgramPath))
                        {
                            ppszIcon = shellMenuItem.MenuProgramPath;
                            return 0;
                        }
                        else
                        {
                            ppszIcon = string.Empty;
                            return unchecked((int)0x80004001);
                        }
                    }
                    else
                    {
                        // 是否使用主题图标
                        if (shellMenuItem.ShouldUseThemeIcon)
                        {
                            // 浅色主题图标
                            if (ThemeHelper.AppsUseLightTheme)
                            {
                                if (File.Exists(shellMenuItem.LightThemeIconPath))
                                {
                                    ppszIcon = shellMenuItem.LightThemeIconPath;
                                    return 0;
                                }
                                else
                                {
                                    ppszIcon = string.Empty;
                                    return unchecked((int)0x80004001);
                                }
                            }
                            else
                            {
                                // 深色主题图标
                                if (File.Exists(shellMenuItem.DarkThemeIconPath))
                                {
                                    ppszIcon = shellMenuItem.DarkThemeIconPath;
                                    return 0;
                                }
                                else
                                {
                                    ppszIcon = string.Empty;
                                    return unchecked((int)0x80004001);
                                }
                            }
                        }
                        else
                        {
                            // 默认图标
                            if (File.Exists(shellMenuItem.DefaultIconPath))
                            {
                                ppszIcon = shellMenuItem.DefaultIconPath;
                                return 0;
                            }
                            else
                            {
                                ppszIcon = string.Empty;
                                return unchecked((int)0x80004001);
                            }
                        }
                    }
                }
                else
                {
                    ppszIcon = string.Empty;
                    return unchecked((int)0x80004001);
                }
            }
            else
            {
                ppszIcon = string.Empty;
                return unchecked((int)0x80004001);
            }
        }

        /// <summary>
        /// 根菜单工具提示
        /// </summary>
        public int GetToolTip(IShellItemArray psiItemArray, out string ppszInfotip)
        {
            ppszInfotip = string.Empty;
            return unchecked((int)0x80004001);
        }

        /// <summary>
        /// 根菜单命令 ID
        /// </summary>
        public int GetCanonicalName(out Guid pguidCommandName)
        {
            pguidCommandName = typeof(ExplorerCommand).GUID;
            return 0;
        }

        /// <summary>
        /// 根菜单命令状态（根据应用设置来决定是否显示菜单）
        /// </summary>
        public int GetState(IShellItemArray psiItemArray, bool fOkToBeSlow, out EXPCMDSTATE pCmdState)
        {
            if (FileShellMenuService.GetFileShellMenuValue() && shellMenuItem is not null)
            {
                // Directory\Background
                if (psiItemArray is null && site != IntPtr.Zero)
                {
                    string folderPath = string.Empty;

                    // 查询点击背景时对应的文件夹路径
                    Marshal.QueryInterface(site, typeof(WindowsAPI.ComTypes.IServiceProvider).GUID, out IntPtr serviceProviderPtr);
                    WindowsAPI.ComTypes.IServiceProvider serviceProvider = (WindowsAPI.ComTypes.IServiceProvider)strategyBasedComWrappers.GetOrCreateObjectForComInstance(serviceProviderPtr, CreateObjectFlags.None);

                    serviceProvider.QueryService(SID_SFolderView, typeof(IFolderView).GUID, out IntPtr folderViewPtr);
                    if (folderViewPtr != IntPtr.Zero)
                    {
                        IFolderView folderView = (IFolderView)strategyBasedComWrappers.GetOrCreateObjectForComInstance(folderViewPtr, CreateObjectFlags.None);

                        Guid iShellItemGuid = typeof(IShellItem).GUID;
                        folderView.GetFolder(ref iShellItemGuid, out IntPtr iShellItemPtr);

                        IShellItem shellItem = (IShellItem)strategyBasedComWrappers.GetOrCreateObjectForComInstance(iShellItemPtr, CreateObjectFlags.None);
                        shellItem.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out folderPath);
                    }

                    // 检查点击背景右键菜单是来自桌面的还是普通目录
                    if (!string.IsNullOrEmpty(folderPath))
                    {
                        string standardDesktopPath = Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.Desktop).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
                        string clickedPath = Path.GetFullPath(folderPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

                        pCmdState = standardDesktopPath.Equals(clickedPath, StringComparison.OrdinalIgnoreCase) ? shellMenuItem.FolderDesktop ? EXPCMDSTATE.ECS_ENABLED : EXPCMDSTATE.ECS_HIDDEN : shellMenuItem.FolderBackground ? EXPCMDSTATE.ECS_ENABLED : EXPCMDSTATE.ECS_HIDDEN;
                    }
                    else
                    {
                        pCmdState = EXPCMDSTATE.ECS_HIDDEN;
                    }
                }
                else
                {
                    if (psiItemArray.GetCount(out uint count) is 0 && count >= 1 && psiItemArray.GetItemAt(0, out IShellItem shellItem) is 0)
                    {
                        shellItem.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out string filePath);
                        FileInfo fileInfo = new(filePath);

                        // 选中的是目录
                        if ((fileInfo.Attributes & FileAttributes.Directory) is not 0)
                        {
                            string rootPath = Path.GetPathRoot(filePath);

                            // 相同，为驱动器目录
                            pCmdState = filePath.Equals(rootPath, StringComparison.OrdinalIgnoreCase) ? shellMenuItem.FolderDrive ? EXPCMDSTATE.ECS_ENABLED : EXPCMDSTATE.ECS_HIDDEN : shellMenuItem.FolderDirectory ? EXPCMDSTATE.ECS_ENABLED : EXPCMDSTATE.ECS_HIDDEN;
                        }
                        // 选中的第一个是文件
                        else
                        {
                            // 匹配文件格式
                            if (shellMenuItem.MenuFileMatchRule.Equals(ShellMenuService.FileMatchRuleList[0]))
                            {
                                pCmdState = EXPCMDSTATE.ECS_HIDDEN;
                            }
                            else if (shellMenuItem.MenuFileMatchRule.Equals(ShellMenuService.FileMatchRuleList[1]))
                            {
                                string[] fileMatchFormatArray = shellMenuItem.MenuFileMatchFormatText.Split('|');

                                bool isMatched = false;
                                foreach (string fileMatchFormat in fileMatchFormatArray)
                                {
                                    if (filePath.Equals(fileMatchFormat.Trim(), StringComparison.OrdinalIgnoreCase))
                                    {
                                        isMatched = true;
                                        break;
                                    }
                                }

                                pCmdState = isMatched ? EXPCMDSTATE.ECS_ENABLED : EXPCMDSTATE.ECS_HIDDEN;
                            }
                            else if (shellMenuItem.MenuFileMatchRule.Equals(ShellMenuService.FileMatchRuleList[2]))
                            {
                                string[] fileMatchFormatArray = shellMenuItem.MenuFileMatchFormatText.Split('|');
                                bool isMatched = false;

                                foreach (string fileMatchFormat in fileMatchFormatArray)
                                {
                                    Regex fileMatchRegex = new(fileMatchFormat.Trim());

                                    if (fileMatchRegex.IsMatch(filePath))
                                    {
                                        isMatched = true;
                                        break;
                                    }
                                }

                                pCmdState = isMatched ? EXPCMDSTATE.ECS_ENABLED : EXPCMDSTATE.ECS_HIDDEN;
                            }
                            else if (shellMenuItem.MenuFileMatchRule.Equals(ShellMenuService.FileMatchRuleList[3]))
                            {
                                string[] fileMatchFormatArray = shellMenuItem.MenuFileMatchFormatText.Split('|');
                                bool isMatched = false;
                                string extensionName = Path.GetExtension(filePath);

                                foreach (string fileMatchFormat in fileMatchFormatArray)
                                {
                                    if (extensionName.Equals(fileMatchFormat.Trim(), StringComparison.OrdinalIgnoreCase))
                                    {
                                        isMatched = true;
                                        break;
                                    }
                                }

                                pCmdState = isMatched ? EXPCMDSTATE.ECS_ENABLED : EXPCMDSTATE.ECS_HIDDEN;
                            }
                            else if (shellMenuItem.MenuFileMatchRule.Equals(ShellMenuService.FileMatchRuleList[4]))
                            {
                                pCmdState = EXPCMDSTATE.ECS_ENABLED;
                            }
                            else
                            {
                                pCmdState = EXPCMDSTATE.ECS_HIDDEN;
                            }
                        }
                    }
                    else
                    {
                        pCmdState = EXPCMDSTATE.ECS_HIDDEN;
                    }
                }
            }
            else
            {
                pCmdState = EXPCMDSTATE.ECS_HIDDEN;
            }

            return 0;
        }

        /// <summary>
        /// 根菜单命令响应处理
        /// </summary>
        public int Invoke(IShellItemArray psiItemArray, IntPtr pbc)
        {
            // 没有子菜单，该菜单可以直接调用命令
            if (shellMenuItem is not null && shellMenuItem.SubShellMenuItem.Count is 0)
            {
                // Directory\Background
                if (psiItemArray is null && site != IntPtr.Zero)
                {
                    string folderPath = string.Empty;

                    // 查询点击背景时对应的文件夹路径
                    Marshal.QueryInterface(site, typeof(WindowsAPI.ComTypes.IServiceProvider).GUID, out IntPtr serviceProviderPtr);
                    WindowsAPI.ComTypes.IServiceProvider serviceProvider = (WindowsAPI.ComTypes.IServiceProvider)strategyBasedComWrappers.GetOrCreateObjectForComInstance(serviceProviderPtr, CreateObjectFlags.None);

                    serviceProvider.QueryService(SID_SFolderView, typeof(IFolderView).GUID, out IntPtr folderViewPtr);
                    if (folderViewPtr != IntPtr.Zero)
                    {
                        IFolderView folderView = (IFolderView)strategyBasedComWrappers.GetOrCreateObjectForComInstance(folderViewPtr, CreateObjectFlags.None);

                        Guid iShellItemGuid = typeof(IShellItem).GUID;
                        folderView.GetFolder(ref iShellItemGuid, out IntPtr iShellItemPtr);

                        IShellItem shellItem = (IShellItem)strategyBasedComWrappers.GetOrCreateObjectForComInstance(iShellItemPtr, CreateObjectFlags.None);

                        shellItem?.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out folderPath);
                    }

                    // 读取参数
                    string parameter = string.IsNullOrEmpty(shellMenuItem.MenuParameter) ? string.Empty : shellMenuItem.MenuParameter;

                    if (!string.IsNullOrEmpty(folderPath))
                    {
                        parameter = parameter.Replace("{folder}", folderPath);
                        ProcessHelper.StartProcess(shellMenuItem.MenuProgramPath, parameter, out _);
                    }
                }
                else
                {
                    if (psiItemArray.GetCount(out uint count) is 0)
                    {
                        // 选中单个文件
                        if (count is 1)
                        {
                            if (psiItemArray.GetItemAt(0, out IShellItem shellItem) is 0)
                            {
                                shellItem.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out string filePath);
                                FileInfo fileInfo = new(filePath);

                                // 读取参数
                                string parameter = string.IsNullOrEmpty(shellMenuItem.MenuParameter) ? string.Empty : shellMenuItem.MenuParameter;

                                // 选中的是目录
                                parameter = (fileInfo.Attributes & FileAttributes.Directory) is not 0 ? parameter.Replace("{folder}", filePath) : parameter.Replace("{file}", filePath);

                                ProcessHelper.StartProcess(shellMenuItem.MenuProgramPath, parameter, out _);
                            }
                        }
                        else
                        {
                            List<string> fileList = [];
                            List<string> folderList = [];

                            for (uint index = 0; index < count; index++)
                            {
                                if (psiItemArray.GetItemAt(index, out IShellItem shellItem) is 0)
                                {
                                    shellItem.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out string filePath);
                                    FileInfo fileInfo = new(filePath);

                                    // 当前项是目录
                                    if ((fileInfo.Attributes & FileAttributes.Directory) is not 0)
                                    {
                                        folderList.Add(filePath);
                                    }
                                    // 当前项是文件
                                    else
                                    {
                                        fileList.Add(filePath);
                                    }
                                }
                            }

                            // 读取参数
                            string parameter = string.IsNullOrEmpty(shellMenuItem.MenuParameter) ? string.Empty : shellMenuItem.MenuParameter;

                            // 匹配多文件规则和匹配多目录规则
                            MatchCollection multiFileMatchCollection = MultiFileRegex.Matches(parameter);
                            MatchCollection multiFolderMatchCollection = MultiFolderRegex.Matches(parameter);

                            int fileMatchedCount = 0;
                            int folderMatchedCount = 0;
                            List<string> multiFileMatchedStringList = [];
                            List<string> multiFolderMatchedStringList = [];
                            List<string> replaceFileStringList = [];
                            List<string> replaceFolderStringList = [];

                            // 匹配文件
                            foreach (Match multiFileMatchItem in multiFileMatchCollection)
                            {
                                GroupCollection multiFileGroupCollection = multiFileMatchItem.Groups;

                                if (multiFileGroupCollection.Count is 3)
                                {
                                    multiFileMatchedStringList.Add(multiFileGroupCollection[1].Value);
                                    replaceFileStringList.Add(multiFileGroupCollection[2].Value);
                                    fileMatchedCount++;
                                }
                            }

                            // 匹配目录
                            foreach (Match multiFolderMatchItem in multiFolderMatchCollection)
                            {
                                GroupCollection multiFolderGroupCollection = multiFolderMatchItem.Groups;

                                if (multiFolderGroupCollection.Count is 3)
                                {
                                    multiFolderMatchedStringList.Add(multiFolderGroupCollection[1].Value);
                                    replaceFolderStringList.Add(multiFolderGroupCollection[2].Value);
                                    folderMatchedCount++;
                                }
                            }

                            // 替换匹配到的所有文件
                            for (int index = 0; index < fileMatchedCount; index++)
                            {
                                string fileString = string.Join(replaceFileStringList[index], fileList);
                                parameter = parameter.Replace(multiFileMatchedStringList[index], fileString);
                            }

                            // 替换匹配到的所有目录
                            for (int index = 0; index < folderMatchedCount; index++)
                            {
                                string folderString = string.Join(replaceFolderStringList[index], folderList);
                                parameter = parameter.Replace(multiFolderMatchedStringList[index], folderString);
                            }

                            ProcessHelper.StartProcess(shellMenuItem.MenuProgramPath, parameter, out _);
                        }
                    }
                }
            }
            return 0;
        }

        /// <summary>
        /// 根菜单命令关联标志
        /// </summary>
        public int GetFlags(out EXPCMDFLAGS pFlags)
        {
            if (shellMenuItem is not null)
            {
                pFlags = shellMenuItem.SubShellMenuItem.Count is 0 ? EXPCMDFLAGS.ECF_DEFAULT : EXPCMDFLAGS.ECF_HASSUBCOMMANDS;

                return 0;
            }
            else
            {
                pFlags = EXPCMDFLAGS.ECF_DEFAULT;
                return unchecked((int)0x80004001);
            }
        }

        /// <summary>
        /// 根菜单子命令遍历
        /// </summary>
        public int EnumSubCommands(out IEnumExplorerCommand ppEnum)
        {
            if (shellMenuItem is not null)
            {
                ExplorerCommand[] subExplorerCommandArray = new ExplorerCommand[shellMenuItem.SubShellMenuItem.Count];
                for (int index = 0; index < shellMenuItem.SubShellMenuItem.Count; index++)
                {
                    subExplorerCommandArray[index] = new ExplorerCommand(shellMenuItem.SubShellMenuItem[index]);
                }

                ppEnum = new EnumExplorerCommand(subExplorerCommandArray);
                return 0;
            }
            else
            {
                ppEnum = new EnumExplorerCommand([]);
                return unchecked((int)0x80004001);
            }
        }

        /// <summary>
        /// 允许容器向对象传递指向其站点接口的指针。
        /// </summary>
        public int SetSite(IntPtr pUnkSite)
        {
            if (pUnkSite == IntPtr.Zero)
            {
                return unchecked((int)0x80004002);
            }
            else
            {
                return Marshal.QueryInterface(pUnkSite, IID_IUnknown, out site);
            }
        }

        /// <summary>
        /// 检索使用 SetSite 传递的最新站点。
        /// </summary>
        public int GetSite(ref Guid riid, out IntPtr ppvSite)
        {
            if (site != IntPtr.Zero)
            {
                return Marshal.QueryInterface(site, IID_IUnknown, out ppvSite);
            }
            else
            {
                ppvSite = IntPtr.Zero;
                return unchecked((int)0x80004005);
            }
        }
    }
}
