using FileReanmer.WindowsAPI.PInvoke.User32;
using System;
using System.Runtime.InteropServices;

namespace FileRenamer.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// user32.dll 函数库
    /// </summary>
    public static class User32Library
    {
        private const string User32 = "user32.dll";

        /// <summary>
        /// 修改指定窗口的用户界面特权隔离 (UIPI) 消息筛选器。
        /// </summary>
        /// <param name="hWnd">要修改其 UIPI 消息筛选器的窗口的句柄。</param>
        /// <param name="message">消息筛选器允许通过或阻止的消息。</param>
        /// <param name="action">要执行的操作，可以执行以下值</param>
        /// <param name="pChangeFilterStruct">指向 CHANGEFILTERSTRUCT 结构的可选指针。</param>
        /// <returns>如果函数成功，则返回 TRUE;否则，它将返回 FALSE。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "ChangeWindowMessageFilterEx", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ChangeWindowMessageFilterEx(IntPtr hWnd, WindowMessage message, ChangeFilterAction action, in CHANGEFILTERSTRUCT pChangeFilterStruct);

        /// <summary>
        /// 将指定的消息发送到窗口或窗口。 <see cref="SendMessage"> 函数调用指定窗口的窗口过程，在窗口过程处理消息之前不会返回。
        /// </summary>
        /// <param name="hWnd">
        /// 窗口过程的句柄将接收消息。 如果此参数 HWND_BROADCAST ( (HWND) 0xffff) ，则会将消息发送到系统中的所有顶级窗口，
        /// 包括已禁用或不可见的未所有者窗口、重叠窗口和弹出窗口;但消息不会发送到子窗口。消息发送受 UIPI 的约束。
        /// 进程的线程只能将消息发送到较低或等于完整性级别的线程的消息队列。
        /// </param>
        /// <param name="wMsg">要发送的消息。</param>
        /// <param name="wParam">其他的消息特定信息。</param>
        /// <param name="lParam">其他的消息特定信息。</param>
        /// <returns>返回值指定消息处理的结果;这取决于发送的消息。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "SendMessageW", SetLastError = false)]
        public static extern IntPtr SendMessage(IntPtr hWnd, WindowMessage wMsg, int wParam, IntPtr lParam);

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
