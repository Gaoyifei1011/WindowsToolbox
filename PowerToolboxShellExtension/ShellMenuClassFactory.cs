using PowerToolboxShellExtension.Commands;
using PowerToolboxShellExtension.Services.Shell;
using PowerToolboxShellExtension.WindowsAPI.ComTypes;
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace PowerToolboxShellExtension
{
    /// <summary>
    /// 允许创建对象的类。
    /// </summary>
    [GeneratedComClass]
    public partial class ShellMenuClassFactory : IClassFactory
    {
        private readonly IExplorerCommand explorerCommand = new ExplorerCommand(ShellMenuService.RootShellMenuItem);

        public unsafe int CreateInstance(IntPtr pUnkOuter, in Guid riid, out IntPtr ppvObject)
        {
            if (!pUnkOuter.Equals(IntPtr.Zero))
            {
                ppvObject = IntPtr.Zero;
                return unchecked((int)0x80040110);
            }

            ppvObject = Program.StrategyBasedComWrappers.GetOrCreateComInterfaceForObject(explorerCommand, CreateComInterfaceFlags.None);
            return 0;
        }

        public int LockServer(bool fLock)
        {
            return 0;
        }
    }
}
