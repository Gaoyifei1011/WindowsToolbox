using System.Runtime.InteropServices;

namespace PowerToolbox.WindowsAPI.PInvoke.KernelAppCore
{
    /// <summary>
    /// 表示包标识信息，例如名称、版本和发布者。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct PACKAGE_ID
    {
        /// <summary>
        /// 保留值；请勿使用。
        /// </summary>
        public uint reserved;

        /// <summary>
        /// 包的处理器体系结构。
        /// </summary>
        public PROCESSOR_ARCHITECTURE processorArchitecture;

        /// <summary>
        /// 包的版本。
        /// </summary>
        public PACKAGE_VERSION version;

        /// <summary>
        /// 包的名称。
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string name;

        /// <summary>
        /// 包的发布者。 如果包没有发布者，则此成员为 NULL。
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string publisher;

        /// <summary>
        /// 资源标识符 (包的 ID) 。 如果包没有资源 ID，则此成员为 NULL。
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string resourceId;

        /// <summary>
        /// 发布者标识符 (包的 ID) 。 如果包没有发布者 ID，则此成员为 NULL。
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string publisherId;
    }
}
