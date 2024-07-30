using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using WindowsToolsShellExtension.Commands;
using WindowsToolsShellExtension.WindowsAPI.ComTypes;

namespace WindowsToolsShellExtension
{
    /// <summary>
    /// 允许创建对象的类。
    /// </summary>
    [GeneratedComClass]
    public partial class ClassFactory : IClassFactory
    {
        private readonly Func<object> rootExplorerCommandFunc = new(() => { return new RootExplorerCommand(); });

        public int CreateInstance(IntPtr pUnkOuter, in Guid riid, out object ppvObject)
        {
            if (pUnkOuter != IntPtr.Zero)
            {
                ppvObject = null;
                const int CLASS_E_NOAGGREGATION = unchecked((int)0x80040110);
                throw new COMException("Class does not support aggregation", CLASS_E_NOAGGREGATION);
            }

            object obj = rootExplorerCommandFunc.Invoke();
            ppvObject = obj;
            return 0;
        }

        public int LockServer([MarshalAs(UnmanagedType.Bool)] bool fLock)
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
