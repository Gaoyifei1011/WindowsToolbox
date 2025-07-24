using System;
using System.Runtime.InteropServices;

namespace PowerToolbox.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 由 TrackMouseEvent 函数用来跟踪在指定的时间范围内，鼠标指针何时离开窗口或鼠标悬停在窗口上。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct TRACKMOUSEEVENT
    {
        /// <summary>
        /// TRACKMOUSEEVENT 结构的大小（以字节为单位）。
        /// </summary>
        public int cbSize;

        /// <summary>
        /// 请求的服务。
        /// </summary>
        public TRACKMOUSEEVENT_FLAGS dwFlags;

        /// <summary>
        /// 要跟踪的窗口的句柄。
        /// </summary>
        public nint hwndTrack;

        /// <summary>
        /// 如果在 dwFlags（以毫秒为单位）指定了TME_HOVER，则悬停超时。 可以 HOVER_DEFAULT，这意味着使用系统默认悬停超时。
        /// </summary>
        public uint dwHoverTime;
    }
}
