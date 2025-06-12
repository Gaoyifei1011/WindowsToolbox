using System.Runtime.InteropServices;

namespace PowerTools.WindowsAPI.PInvoke.KernelAppCore
{
    /// <summary>
    /// 表示包标识信息，其中包括包标识符、全名和安装位置。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct PACKAGE_INFO
    {
        /// <summary>
        /// 保留值；请勿使用。
        /// </summary>
        public uint reserved;

        /// <summary>
        /// 包的属性。
        /// </summary>
        public uint flags;

        /// <summary>
        /// 包的位置。
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string path;

        /// <summary>
        /// 包全名。
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string packageFullName;

        /// <summary>
        /// 包系列名称。
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)] public string packageFamilyName;

        /// <summary>
        /// 包标识符 (ID) 。
        /// </summary>
        public PACKAGE_ID packageId;
    }
}
