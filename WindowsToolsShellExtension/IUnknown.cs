using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace WindowsToolsShellExtension
{
    [Guid("00000000-0000-0000-C000-000000000046")]
    public unsafe struct IUnknown
    {
        private void** lpVtbl;

        public unsafe int QueryInterface(in Guid riid, out void* ppvObject)
        {
            fixed (void** ppvObjectLocal = &ppvObject)
            {
                return QueryInterface(riid, ppvObjectLocal);
            }
        }

        public unsafe int QueryInterface(in Guid riid, void** ppvObject)
        {
            return ((delegate* unmanaged[Stdcall]<IUnknown*, in Guid, void**, int>)lpVtbl[0])((IUnknown*)Unsafe.AsPointer(ref this), in riid, ppvObject);
        }

        public uint AddRef()
        {
            return ((delegate* unmanaged[Stdcall]<IUnknown*, uint>)lpVtbl[1])((IUnknown*)Unsafe.AsPointer(ref this));
        }

        public uint Release()
        {
            return ((delegate* unmanaged[Stdcall]<IUnknown*, uint>)lpVtbl[2])((IUnknown*)Unsafe.AsPointer(ref this));
        }
    }
}
