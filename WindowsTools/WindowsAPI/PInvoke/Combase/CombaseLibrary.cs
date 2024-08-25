using System;
using System.Runtime.InteropServices;

namespace WindowsTools.WindowsAPI.PInvoke.Combase
{
    public static class CombaseLibrary
    {
        private const string Combase = "combase.dll";

        /// <summary>
        /// 获取指定运行时类的激活工厂。
        /// </summary>
        /// <param name="activatableClassId">可激活类的 ID。</param>
        /// <param name="iid">接口的引用 ID。</param>
        /// <param name="factory">激活工厂。</param>
        /// <returns>如果此函数成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [DllImport(Combase, CharSet = CharSet.Unicode, ExactSpelling = true, EntryPoint = "RoGetActivationFactory", SetLastError = false), PreserveSig]
        public static extern int RoGetActivationFactory([MarshalAs(UnmanagedType.HString)] string activatableClassId, Guid iid, out IntPtr factory);
    }
}
