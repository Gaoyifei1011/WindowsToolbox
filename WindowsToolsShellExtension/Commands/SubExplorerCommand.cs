using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using WindowsToolsShellExtension.WindowsAPI.ComTypes;

namespace WindowsToolsShellExtension.Commands
{
    [GeneratedComClass]
    public partial class SubExplorerCommand : IExplorerCommand
    {
        public int EnumSubCommands(out IEnumExplorerCommand ppEnum)
        {
            ppEnum = null;
            return 1;
        }

        public int GetCanonicalName(out Guid pguidCommandName)
        {
            pguidCommandName = typeof(SubExplorerCommand).GUID;
            return 0;
        }

        public int GetFlags(out EXPCMDFLAGS pFlags)
        {
            pFlags = EXPCMDFLAGS.ECF_DEFAULT;
            return 0;
        }

        public int GetIcon(IShellItemArray psiItemArray, [MarshalAs(UnmanagedType.LPWStr)] out string ppszIcon)
        {
            ppszIcon = string.Empty;
            return 0;
        }

        public int GetState(IShellItemArray psiItemArray, [MarshalAs(UnmanagedType.Bool)] bool fOkToBeSlow, out EXPCMDSTATE pCmdState)
        {
            pCmdState = EXPCMDSTATE.ECS_ENABLED;
            return 0;
        }

        public int GetTitle(IShellItemArray psiItemArray, [MarshalAs(UnmanagedType.LPWStr)] out string ppszName)
        {
            ppszName = "hello world";
            return 0;
        }

        public int GetToolTip(IShellItemArray psiItemArray, [MarshalAs(UnmanagedType.LPWStr)] out string ppszInfotip)
        {
            ppszInfotip = string.Empty;
            return 0;
        }

        public int Invoke(IShellItemArray psiItemArray, [MarshalAs(UnmanagedType.Interface)] object pbc)
        {
            return 0;
        }
    }
}
