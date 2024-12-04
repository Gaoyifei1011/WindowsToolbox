using System;
using System.Runtime.InteropServices;
using WindowsTools.WindowsAPI.PInvoke.Gdi32;
using WindowsTools.WindowsAPI.PInvoke.User32;

// 抑制 CA1401 警告
#pragma warning disable CA1401

namespace WindowsTools.WindowsAPI.PInvoke.Uxtheme
{
    /// <summary>
    /// Uxtheme.dll 函数库
    /// </summary>
    public static class UxthemeLibrary
    {
        private const string Uxtheme = "uxtheme.dll";

        /// <summary>
        /// 开始缓冲绘制操作。
        /// </summary>
        /// <param name="hdcTarget">将绘制缓冲区的目标 DC 的句柄。</param>
        /// <param name="prcTarget">指向 RECT 结构的指针，该结构指定要在其中绘制的目标 DC 的区域。</param>
        /// <param name="dwFormat">指定缓冲区格式的 BP_BUFFERFORMAT 枚举的成员。</param>
        /// <param name="pPaintParams">指向定义绘制操作参数 的BP_PAINTPARAMS 结构的指针。 此值可以为 NULL。</param>
        /// <param name="phdc">当此函数返回时， 指向新设备上下文的句柄。</param>
        /// <returns>
        /// 缓冲的绘制上下文的句柄。 如果此函数失败，则返回值为 NULL， phdc 为 NULL。
        /// 调用 EndBufferedPaint 时释放返回的句柄。
        /// 在调用 BeginBufferedPaint 之前，应用程序应在调用线程上调用 BufferedPaintInit，并在线程终止之前调用 BufferedPaintUnInit。 调用 BufferedPaintInit 失败可能会导致性能下降，因为每个缓冲的绘制操作的内部数据都会被初始化和销毁。
        /// </returns>
        [DllImport(Uxtheme, CharSet = CharSet.Unicode, EntryPoint = "BeginBufferedPaint", PreserveSig = true, SetLastError = false)]
        public static extern unsafe IntPtr BeginBufferedPaint(IntPtr hdcTarget, ref RECT prcTarget, BP_BUFFERFORMAT dwFormat, ref BP_PAINTPARAMS pPaintParams, out IntPtr phdc);

        /// <summary>
        /// 初始化当前线程的缓冲绘制。
        /// </summary>
        /// <returns>如果此函数成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [DllImport(Uxtheme, CharSet = CharSet.Unicode, EntryPoint = "BufferedPaintInit", PreserveSig = true, SetLastError = false)]
        public static extern IntPtr BufferedPaintInit();

        /// <summary>
        /// 将 alpha 设置为给定矩形中的指定值。 alpha 控制与缓冲区混合到目标设备上下文 (DC) 时应用的透明度。
        /// </summary>
        /// <param name="hBufferedPaint">缓冲的绘制上下文的句柄，通过 BeginBufferedPaint 获取。</param>
        /// <param name="prc">指向 RECT 结构的指针，该结构指定要在其中设置 alpha 的矩形。 将此参数设置为 NULL 可指定整个缓冲区。</param>
        /// <param name="alpha">要设置的 alpha 值。 alpha 值的范围可以是零 (完全透明) 到 255 (完全不透明) 。</param>
        /// <returns>如果此函数成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [DllImport(Uxtheme, CharSet = CharSet.Unicode, EntryPoint = "BufferedPaintInit", PreserveSig = true, SetLastError = false)]
        public static extern int BufferedPaintSetAlpha(IntPtr hBufferedPaint, ref RECT prc, byte alpha);

        /// <summary>
        /// 完成缓冲绘制操作并释放关联的缓冲绘制句柄。
        /// </summary>
        /// <param name="hBufferedPaint">缓冲的绘制上下文的句柄，通过 BeginBufferedPaint 获取。</param>
        /// <param name="fUpdateTarget">若要将缓冲区复制到目标 DC，则为 TRUE。</param>
        /// <returns>如果此函数成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [DllImport(Uxtheme, CharSet = CharSet.Unicode, EntryPoint = "EndBufferedPaint", PreserveSig = true, SetLastError = false)]
        public static extern int EndBufferedPaint(IntPtr hBufferedPaint, bool fUpdateTarget);

        /// <summary>
        /// 设置 win32 右键菜单的样式
        /// </summary>
        /// <param name="preferredAppMode">菜单样式</param>
        [DllImport(Uxtheme, CharSet = CharSet.Unicode, EntryPoint = "#135", PreserveSig = true, SetLastError = false)]
        public static extern int SetPreferredAppMode(PreferredAppMode preferredAppMode);

        /// <summary>
        /// 设置属性以控制如何将视觉样式应用于指定窗口。
        /// </summary>
        /// <param name="hWnd">要向其应用更改的窗口句柄。</param>
        /// <param name="eAttribute">指定要设置的属性的类型。 此参数的值确定应在 pvAttribute 参数中传递的数据类型。 </param>
        /// <param name="pvAttribute">一个指针，指定要设置的属性。 类型由 eAttribute 值的值确定。</param>
        /// <param name="cbAttribute">指定 pvAttribute指向的数据的大小（以字节为单位）。</param>
        /// <returns>如果此函数成功，则返回 S_OK。 否则，它将返回 HRESULT 错误代码。</returns>
        [DllImport(Uxtheme, CharSet = CharSet.Unicode, EntryPoint = "SetWindowThemeAttribute", PreserveSig = true, SetLastError = false)]
        public static extern int SetWindowThemeAttribute(IntPtr hWnd, WINDOWTHEMEATTRIBUTETYPE eAttribute, ref WTA_OPTIONS pvAttribute, uint cbAttribute);

        /// <summary>
        /// 刷新右键菜单样式
        /// </summary>
        [DllImport(Uxtheme, CharSet = CharSet.Unicode, EntryPoint = "#136", PreserveSig = true, SetLastError = false)]
        public static extern int FlushMenuThemes();
    }
}
