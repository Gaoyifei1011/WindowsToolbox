using System.Runtime.InteropServices;
using System;
using System.Text;

// 抑制 CA1401 警告
#pragma warning disable CA1401

namespace WindowsTools.WindowsAPI.PInvoke.Setupapi
{
    /// <summary>
    /// Setupapi.dll 函数库
    /// </summary>
    public static class SetupapiLibrary
    {
        private const string Setupapi = "setupapi.dll";

        /// <summary>
        /// SetupGetInfDriverStoreLocation 函数检索驱动程序存储中 INF 文件 的完全限定文件名（目录路径和文件名），该文件对应于系统 INF 文件目录中的指定 INF 文件或驱动程序存储中的指定 INF 文件。
        /// </summary>
        /// <param name="FileName">指向 NULL 终止的字符串的指针，该字符串包含系统 INF 文件目录中 INF 文件的名称和完整目录路径（ 可选）。 或者，此参数是指向以 NULL 结尾的字符串的指针，该字符串包含驱动程序存储中 INF 文件的完全限定文件名（目录路径和文件名）。</param>
        /// <param name="AlternatePlatformInfo">保留供系统使用。</param>
        /// <param name="LocaleName">保留供系统使用。</param>
        /// <param name="ReturnBuffer">指向缓冲区的指针，其中函数返回一个 NULL 终止的字符串，其中包含指定 INF 文件的完全限定文件名。 此参数可以设置为 NULL。 支持的最大路径大小为MAX_PATH。</param>
        /// <param name="ReturnBufferSize"></param>
        /// <param name="RequiredSize">指向接收 ReturnBuffer 缓冲区的大小（以字符为单位）的 DWORD 类型的变量的指针。</param>
        /// <returns>如果 SetupGetInfDriverStoreLocation 成功，则函数返回 true ;否则，该函数返回 FALSE。</returns>
        [DllImport(Setupapi, CharSet = CharSet.Unicode, EntryPoint = "SetupGetInfDriverStoreLocationW", PreserveSig = true, SetLastError = false)]
        public static extern bool SetupGetInfDriverStoreLocation([MarshalAs(UnmanagedType.LPWStr)] string FileName, IntPtr AlternatePlatformInfo, IntPtr LocaleName, StringBuilder ReturnBuffer, int ReturnBufferSize, out int RequiredSize);
    }
}
