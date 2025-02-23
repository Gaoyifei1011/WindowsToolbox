using System.Runtime.InteropServices;
using WindowsTools.WindowsAPI.PInvoke.Kernel32;

namespace WindowsTools.WindowsAPI.PInvoke.Dismapi
{
    /// <summary>
    /// 包含与 .inf 文件关联的驱动程序的基本信息。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 4)]
    public struct DismDriverPackage
    {
        /// <summary>
        /// 已发布的驱动程序的名称。
        /// </summary>
        public string PublishedName;

        /// <summary>
        /// 驱动程序的原始文件名。
        /// </summary>
        public string OriginalFileName;

        /// <summary>
        /// 如果驱动程序包含在 Windows 分发媒体中，并作为 Windows 的一部分自动安装，则为 TRUE，否则为 FALSE。
        /// </summary>
        public bool InBox;

        /// <summary>
        /// 驱动程序的目录文件。
        /// </summary>
        public string CatalogFile;

        /// <summary>
        /// 驱动程序的类名称。
        /// </summary>
        public string ClassName;

        /// <summary>
        /// 驱动程序的类 GUID。
        /// </summary>
        public string ClassGuid;

        /// <summary>
        /// 驱动程序的类说明。
        /// </summary>
        public string ClassDescription;

        /// <summary>
        /// 如果驱动程序是启动关键型，则为 TRUE，否则为 FALSE。
        /// </summary>
        public bool BootCritical;

        /// <summary>
        /// DismDriverSignature 枚举中指示驱动程序签名状态的值。
        /// </summary>
        public DismDriverSignature DriverSignature;

        /// <summary>
        /// 驱动程序的提供程序。
        /// </summary>
        public string ProviderName;

        /// <summary>
        /// 驱动程序的制造商生成日期。
        /// </summary>
        public SYSTEMTIME Date;

        /// <summary>
        /// 驱动程序的主要版本号。
        /// </summary>
        public uint MajorVersion;

        /// <summary>
        /// 驱动程序的次要版本号。
        /// </summary>
        public uint MinorVersion;

        /// <summary>
        /// 驱动程序的生成号。
        /// </summary>
        public uint Build;

        /// <summary>
        /// 驱动程序的修订号。
        /// </summary>
        public uint Revision;
    }
}
