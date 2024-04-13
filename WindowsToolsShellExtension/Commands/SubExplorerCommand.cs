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
            throw new NotImplementedException();
        }

        public int GetCanonicalName(out Guid pguidCommandName)
        {
            throw new NotImplementedException();
        }

        public int GetFlags(out EXPCMDFLAGS pFlags)
        {
            throw new NotImplementedException();
        }

        public int GetIcon(IShellItemArray psiItemArray, [MarshalAs(UnmanagedType.LPWStr)] out string ppszIcon)
        {
            throw new NotImplementedException();
        }

        public int GetState(IShellItemArray psiItemArray, [MarshalAs(UnmanagedType.Bool)] bool fOkToBeSlow, out EXPCMDSTATE pCmdState)
        {
            throw new NotImplementedException();
        }

        public int GetTitle(IShellItemArray psiItemArray, [MarshalAs(UnmanagedType.LPWStr)] out string ppszName)
        {
            throw new NotImplementedException();
        }

        public int GetToolTip(IShellItemArray psiItemArray, [MarshalAs(UnmanagedType.LPWStr)] out string ppszInfotip)
        {
            throw new NotImplementedException();
        }

        public int Invoke(IShellItemArray psiItemArray, [MarshalAs(UnmanagedType.Interface)] object pbc)
        {
            throw new NotImplementedException();
        }
    }
}
