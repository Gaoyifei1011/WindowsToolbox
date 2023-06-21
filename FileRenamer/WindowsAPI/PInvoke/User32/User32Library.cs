using System;
using System.Runtime.InteropServices;

namespace FileRenamer.WindowsAPI.PInvoke.User32
{
    public static class User32Library
    {
        private const string User32 = "user32.dll";

        /// <summary>
        /// 将创建指定窗口的线程引入前台并激活窗口。 键盘输入将定向到窗口，并为用户更改各种视觉提示。 系统为创建前台窗口的线程分配的优先级略高于其他线程的优先级。
        /// </summary>
        /// <param name="hWnd">应激活并带到前台的窗口的句柄。</param>
        /// <returns>如果将窗口带到前台，则返回值为非零值。如果未将窗口带到前台，则返回值为零。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "SetForegroundWindow", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}
