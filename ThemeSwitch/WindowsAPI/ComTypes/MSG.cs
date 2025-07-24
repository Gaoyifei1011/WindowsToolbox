using System;
using System.Runtime.InteropServices;

namespace ThemeSwitch.WindowsAPI.ComTypes
{
    /// <summary>
    /// 包含来自线程的消息队列的消息信息。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct MSG
    {
        /// <summary>
        /// 其窗口过程接收消息的窗口的句柄。 当消息是线程消息时，此成员为 NULL 。
        /// </summary>
        public nint hwnd;

        /// <summary>
        /// 消息的标识符。 应用程序只能使用低字;高字由系统保留。
        /// </summary>
        public uint message;

        /// <summary>
        /// 关于消息的附加信息。 确切含义取决于 消息 成员的值。
        /// </summary>
        public nint wParam;

        /// <summary>
        /// 关于消息的附加信息。 确切含义取决于 消息 成员的值。
        /// </summary>
        public nint lParam;

        /// <summary>
        /// 消息的发布时间。
        /// </summary>
        public uint time;

        /// <summary>
        /// 发布消息时的光标位置（以屏幕坐标表示）。
        /// </summary>
        public int pt_x;

        /// <summary>
        /// 发布消息时的光标位置（以屏幕坐标表示）。
        /// </summary>
        public int pt_y;
    }
}
