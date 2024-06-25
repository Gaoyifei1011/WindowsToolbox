using System;
using System.Runtime.InteropServices;
using System.Text;

namespace WindowsTools.WindowsAPI.PInvoke.Shlwapi
{
    public static class ShlwapiLibrary
    {
        public const string Shlwapi = "shlwapi.dll";

        /// <summary>
        /// 以间接字符串的形式给定该资源时，提取指定的文本资源 (以“@”符号) 开头的字符串。
        /// </summary>
        /// <param name="pszSource">指向缓冲区的指针，该缓冲区包含将从中检索资源的间接字符串。 此字符串应以“@”符号开头，并使用“备注”部分中讨论的窗体之一。 此函数将成功接受一个不以“@”符号开头的字符串，但该字符串将直接传递给 pszOutBuf。</param>
        /// <param name="pszOutBuf">指向缓冲区的指针，当此函数成功返回时，该缓冲区接收文本资源。 pszOutBuf 和 pszSource 可以指向同一个缓冲区，在这种情况下，将覆盖原始字符串。</param>
        /// <param name="cchOutBuf">pszOutBuf 指向的缓冲区的大小（以字符为单位）。</param>
        /// <param name="ppvReserved">未使用;设置为 NULL。</param>
        /// <returns>如果此函数成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [DllImport(Shlwapi, CharSet = CharSet.Unicode, EntryPoint = "SHLoadIndirectString", ExactSpelling = true)]
        internal static extern int SHLoadIndirectString(string pszSource, StringBuilder pszOutBuf, int cchOutBuf, IntPtr ppvReserved);
    }
}
