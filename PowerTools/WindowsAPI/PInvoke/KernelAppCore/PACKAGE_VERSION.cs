using System.Runtime.InteropServices;

namespace PowerTools.WindowsAPI.PInvoke.KernelAppCore
{
    /// <summary>
    /// 表示包版本信息。
    /// </summary>
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
    public struct PACKAGE_VERSION
    {
        /// <summary>
        /// 以单个整型值表示的包的完整版本号。
        /// </summary>
        [FieldOffset(0)]
        public ulong Version;

        [FieldOffset(0)]
        public DUMMYSTRUCTNAME Parts;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct DUMMYSTRUCTNAME
        {
            /// <summary>
            /// 包的修订版本号。
            /// </summary>
            public ushort Revision;

            /// <summary>
            /// 包的内部版本号。
            /// </summary>
            public ushort Build;

            /// <summary>
            /// 包的次要版本号。
            /// </summary>
            public ushort Minor;

            /// <summary>
            /// 包的主版本号。
            /// </summary>
            public ushort Major;
        }
    }
}
