using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using WindowsToolsShellExtension.Helpers.Root;
using WindowsToolsShellExtension.Services.Controls.Settings;
using WindowsToolsShellExtension.Strings;
using WindowsToolsShellExtension.WindowsAPI.ComTypes;
using WindowsToolsShellExtension.WindowsAPI.PInvoke.Kernel32;

namespace WindowsToolsShellExtension.Commands
{
    /// <summary>
    /// 根菜单项目
    /// </summary>
    [GeneratedComClass, Guid("5A730150-DE8D-0C88-FD1A-99B7E954BDDB")]
    public partial class RootExplorerCommand : IExplorerCommand
    {
        /// <summary>
        /// 根菜单标题
        /// </summary>
        public int GetTitle(IShellItemArray psiItemArray, [MarshalAs(UnmanagedType.LPWStr)] out string ppszName)
        {
            ppszName = ShellMenu.RootItemTitle;
            return 0;
        }

        /// <summary>
        /// 根菜单图标路径
        /// </summary>
        public unsafe int GetIcon(IShellItemArray psiItemArray, [MarshalAs(UnmanagedType.LPWStr)] out string ppszIcon)
        {
            char* pathbuffer = stackalloc char[256];
            int length = 256;
            Kernel32Library.GetCurrentPackagePath(ref length, pathbuffer);
            ppszIcon = Path.Combine(new string(pathbuffer), @"Assets\WindowsTools.ico");
            return 0;
        }

        /// <summary>
        /// 根菜单工具提示
        /// </summary>
        public int GetToolTip(IShellItemArray psiItemArray, [MarshalAs(UnmanagedType.LPWStr)] out string ppszInfotip)
        {
            ppszInfotip = string.Empty;
            return unchecked((int)0x80004001);
        }

        /// <summary>
        /// 根菜单命令 ID
        /// </summary>
        public int GetCanonicalName(out Guid pguidCommandName)
        {
            pguidCommandName = typeof(RootExplorerCommand).GUID;
            return 0;
        }

        /// <summary>
        /// 根菜单命令状态（根据应用设置来决定是否显示菜单）
        /// </summary>
        public int GetState(IShellItemArray psiItemArray, [MarshalAs(UnmanagedType.Bool)] bool fOkToBeSlow, out EXPCMDSTATE pCmdState)
        {
            pCmdState = FileShellMenuService.GetFileShellMenuValue() ? EXPCMDSTATE.ECS_ENABLED : EXPCMDSTATE.ECS_HIDDEN;
            return 0;
        }

        /// <summary>
        /// 根菜单命令响应处理
        /// </summary>
        public unsafe int Invoke(IShellItemArray psiItemArray, [MarshalAs(UnmanagedType.Interface)] object pbc)
        {
            char* pathbuffer = stackalloc char[256];
            int length = 256;
            Kernel32Library.GetCurrentPackagePath(ref length, pathbuffer);
            string exePath = Path.Combine(new string(pathbuffer), "WindowsTools.exe");

            ProcessHelper.StartProcess(exePath, string.Empty, out int processid);
            return 0;
        }

        /// <summary>
        /// 根菜单命令关联标志
        /// </summary>
        public int GetFlags(out EXPCMDFLAGS pFlags)
        {
            pFlags = EXPCMDFLAGS.ECF_DEFAULT;
            return 0;
        }

        /// <summary>
        /// 根菜单子命令遍历
        /// </summary>
        public int EnumSubCommands(out IEnumExplorerCommand ppEnum)
        {
            SubExplorerCommand[] subExplorerCommands = new SubExplorerCommand[1];
            subExplorerCommands[0] = new SubExplorerCommand();

            if (subExplorerCommands.Length > 0)
            {
                EnumExplorerCommand enumExplorerCommand = new EnumExplorerCommand(subExplorerCommands);
                ppEnum = enumExplorerCommand;

                return 0;
            }
            else
            {
                ppEnum = null;
                return 1;
            }
        }
    }
}
