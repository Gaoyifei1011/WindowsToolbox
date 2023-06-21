using System.Runtime.InteropServices;
using System.Text;

namespace FileRenamer.WindowsAPI.PInvoke.Kernel32
{
    /// <summary>
    /// Kernel32.dll 函数库
    /// </summary>
    public static class Kernel32Library
    {
        private const string Kernel32 = "kernel32.dll";

        public const int APPMODEL_ERROR_NO_PACKAGE = 15700;

        /// <summary>
        /// 获取调用进程的包全名。
        /// </summary>
        /// <param name="packageFullNameLength">
        /// 输入时， packageFullName 缓冲区的大小（以字符为单位）。 在输出中，返回的包全名的大小（以字符为单位，包括 null 终止符）。
        /// </param>
        /// <param name="packageFullName">包全名。</param>
        /// <returns>
        /// 如果函数成功，它将返回 ERROR_SUCCESS。 否则，该函数将返回错误代码。 可能的错误代码包括以下内容。
        /// APPMODEL_ERROR_NO_PACKAGE：此过程没有包标识。
        /// ERROR_INSUFFICIENT_BUFFER：缓冲区不够大，无法保存数据。 所需大小由 packageFullNameLength 指定。
        /// </returns>
        [DllImport(Kernel32, CharSet = CharSet.Unicode, EntryPoint = "GetCurrentPackageFullName", SetLastError = true)]
        public static extern int GetCurrentPackageFullName(ref int packageFullNameLength, StringBuilder packageFullName);
    }
}
