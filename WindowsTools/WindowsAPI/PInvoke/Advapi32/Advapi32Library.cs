using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace WindowsTools.WindowsAPI.PInvoke.Advapi32
{
    public static class Advapi32Library
    {
        public const string Advapi32 = "advapi32.dll";

        /// <summary>
        /// 将字符串格式的安全标识符 (SID) 转换为有效的功能 SID。 可以使用此函数检索 ConvertSidToStringSid 函数转换为字符串格式的 SID。
        /// </summary>
        /// <param name="StringSid">
        /// 指向以 null 结尾的字符串的指针，该字符串包含要转换的字符串格式 SID。
        /// SID 字符串可以使用标准 S-R-I-S-S...SID 字符串的格式，或 SID 字符串常量格式，例如内置管理员的“BA”。 有关 SID 字符串表示法的详细信息，请参阅 SID 组件。
        /// </param>
        /// <param name="Sid">指向变量的指针，该变量接收指向转换后的 SID 的指针。 若要释放返回的缓冲区，请调用 LocalFree 函数。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。 </returns>
        [DllImport(Advapi32, CharSet = CharSet.Unicode, EntryPoint = "ConvertStringSidToSidW", SetLastError = true)]
        internal static extern bool ConvertStringSidToSid(string StringSid, out IntPtr Sid);
    }
}
