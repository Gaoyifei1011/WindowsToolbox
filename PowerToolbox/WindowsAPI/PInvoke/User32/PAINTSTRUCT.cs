using System;
using System.Runtime.InteropServices;

namespace PowerToolbox.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// PAINTSTRUCT 结构包含应用程序的信息。 此信息可用于绘制该应用程序拥有的窗口的工作区。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct PAINTSTRUCT
    {
        /// <summary>
        /// 要用于绘制的显示 DC 的句柄。
        /// </summary>
        public IntPtr hdc;

        /// <summary>
        /// 指示是否必须擦除背景。 如果应用程序应擦除背景，则此值为非零值。 如果创建窗口类时没有背景画笔，则应用程序负责擦除背景。 有关详细信息，请参阅 WNDCLASS 结构的 hbrBackground 成员的说明。
        /// </summary>
        public bool fErase;

        /// <summary>
        /// RECT 结构，指定请求绘制的矩形的左上角和右下角，以相对于工作区左上角的设备单位表示。
        /// </summary>
        public RECT rcPaint;

        /// <summary>
        /// 保留;系统在内部使用。
        /// </summary>
        public bool fRestore;

        /// <summary>
        /// 保留;系统在内部使用。
        /// </summary>
        public bool fIncUpdate;

        /// <summary>
        /// 保留;系统在内部使用。
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] rgbReserved;
    }
}
