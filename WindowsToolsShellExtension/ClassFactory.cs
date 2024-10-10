using System;
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
        private readonly IExplorerCommand rootExplorerCommand = new ExplorerCommand(ShellMenuService.RootShellMenuItem);

        public unsafe int CreateInstance(IntPtr pUnkOuter, in Guid riid, out IntPtr ppvObject)
        {
            if (pUnkOuter != IntPtr.Zero)
            {
                ppvObject = IntPtr.Zero;
                return unchecked((int)0x80040110);
            }

            ppvObject = (IntPtr)ComInterfaceMarshaller<IExplorerCommand>.ConvertToUnmanaged(rootExplorerCommand);
            return 0;
        }

        public int LockServer(bool fLock)
        {
            return 0;
        }
    }
}
