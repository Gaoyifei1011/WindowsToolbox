using PowerTools.WindowsAPI.PInvoke.User32;
using System;
using System.Runtime.InteropServices;

namespace PowerTools.WindowsAPI.PInvoke.Shell32
{
    /// <summary>
    /// 包含有关系统应用栏消息的信息。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct APPBARDATA
    {
        /// <summary>
        /// 结构大小（以字节为单位）。
        /// </summary>
        public int cbSize;

        /// <summary>
        /// 应用栏窗口的句柄。 并非所有邮件都使用此成员。 请参阅单个消息页，了解是否需要提供 hWind 值。
        /// </summary>
        public IntPtr hWnd;

        /// <summary>
        /// 应用程序定义的消息标识符。 应用程序对发送到 由 hWnd 成员标识的应用栏的通知消息使用指定的标识符。 发送 ABM_NEW 消息时使用此成员。
        /// </summary>
        public uint uCallbackMessage;

        /// <summary>
        /// 一个值，该值指定屏幕的边缘。
        /// </summary>
        public ABE uEdge;

        /// <summary>
        /// 其用法因消息而异的 RECT 结构：
        /// ABM_GETTASKBARPOS、ABM_QUERYPOS、ABM_SETPOS：应用栏或 Windows 任务栏的屏幕坐标边框。
        /// ABM_GETAUTOHIDEBAREX，ABM_SETAUTOHIDEBAREX：正在其上执行操作的监视器。 可以通过 GetMonitorInfo 函数检索此信息。
        /// </summary>
        public RECT rc;

        /// <summary>
        /// 依赖于消息的值。
        /// </summary>
        public int lParam;
    }
}
