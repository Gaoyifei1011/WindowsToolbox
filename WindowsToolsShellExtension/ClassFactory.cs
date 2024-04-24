using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using WindowsToolsShellExtension.WindowsAPI.ComTypes;

namespace WindowsToolsShellExtension
{
    /// <summary>
    /// 允许创建对象的类。
    /// </summary>
    [GeneratedComClass]
    public partial class ClassFactory : IClassFactory
    {
        private readonly Func<object> createFunc;

        public ClassFactory(Func<object> createFunc)
        {
            this.createFunc = createFunc;
        }

        public unsafe int CreateInstance([Optional] void* pUnkOuter, in Guid riid, void** ppvObject)
        {
            object obj = createFunc.Invoke();

            IntPtr result = Program.StrategyBasedComWrappers.GetOrCreateComInterfaceForObject(obj!, CreateComInterfaceFlags.None);

            IUnknown* punk = (IUnknown*)result;
            int hr = punk->QueryInterface(riid, out var pvObject);

            if (hr is 0)
            {
                *ppvObject = pvObject;
            }

            punk->Release();

            return hr;
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
