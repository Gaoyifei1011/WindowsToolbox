using System;
using System.Runtime.InteropServices;

namespace FileRenamer.WindowsAPI.PInvoke.User32
{
    public static class User32Library
    {
        private const string User32 = "User32.dll";

        /// <summary>
        /// 返回指定窗口的每英寸点 (dpi) 值。
        /// </summary>
        /// <param name="hwnd">要获取相关信息的窗口。</param>
        /// <returns>窗口的 DPI，无效 的 <param name="hwnd"> 值将导致返回值 0。</returns>
        [DllImport(User32, CharSet = CharSet.Ansi, EntryPoint = "GetDpiForWindow", SetLastError = false)]
        public static extern int GetDpiForWindow(IntPtr hwnd);
    }
}
