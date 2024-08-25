using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using WindowsToolsShellExtension.Commands;
using WindowsToolsShellExtension.Services.Shell;
using WindowsToolsShellExtension.WindowsAPI.ComTypes;

namespace WindowsToolsShellExtension
{
    /// <summary>
    /// 允许创建对象的类。
    /// </summary>
    [GeneratedComClass]
    public partial class ClassFactory : IClassFactory
    {
        private readonly StrategyBasedComWrappers strategyBasedComWrappers = new StrategyBasedComWrappers();
        private readonly Func<object> rootExplorerCommandFunc = new(() => { return new ExplorerCommand(ShellMenuService.RootShellMenuItem); });

        public int CreateInstance(IntPtr pUnkOuter, in Guid riid, out IntPtr ppvObject)
        {
            if (pUnkOuter != IntPtr.Zero)
            {
                ppvObject = IntPtr.Zero;
                return unchecked((int)0x80040110); // CLASS_E_NOAGGREGATION
            }

            object obj = rootExplorerCommandFunc.Invoke();
            ppvObject = strategyBasedComWrappers.GetOrCreateComInterfaceForObject(obj, CreateComInterfaceFlags.None);
            return 0;
        }

        public int LockServer(bool fLock)
        {
            if (fLock)
            {
                Program.DllAddRef();
            }
            else
            {
                Program.DllRelease();
            }

            return 0;
        }
    }
}
