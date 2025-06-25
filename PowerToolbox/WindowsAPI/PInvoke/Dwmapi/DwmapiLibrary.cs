using System;
using System.Drawing.Printing;
using System.Runtime.InteropServices;

// 抑制 CA1401 警告
#pragma warning disable CA1401

namespace PowerToolbox.WindowsAPI.PInvoke.Dwmapi
{
    /// <summary>
    /// Dwmapi.dll 函数库
    /// </summary>
    public static partial class DwmapiLibrary
    {
        private const string Dwmapi = "dwmapi.dll";

        /// <summary>
        /// 将窗口框架扩展到工作区。
        /// </summary>
        /// <param name="hwnd">框架将扩展到工作区的窗口的句柄。</param>
        /// <param name="pMarInset">指向 MARGINS 结构的指针，该结构描述在将帧扩展到工作区时要使用的边距。</param>
        /// <returns>如果此函数成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [DllImport(Dwmapi, CharSet = CharSet.Unicode, EntryPoint = "DwmExtendFrameIntoClientArea", PreserveSig = true, SetLastError = false)]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref Margins pMarInset);

        /// <summary>
        /// 检索应用于窗口的指定桌面窗口管理器（DWM）属性的当前值。 有关编程指南和代码示例，请参阅 控制非客户端区域呈现。
        /// </summary>
        /// <param name="hwnd">要从中检索属性值的窗口的句柄。</param>
        /// <param name="dwAttribute">描述要检索的值的标志，指定为 DWMWINDOWATTRIBUTE 枚举的值。 此参数指定要检索的属性，pvAttribute 参数指向在其中检索属性值的对象。</param>
        /// <param name="pvAttribute">指向一个值的指针，当此函数成功返回时，将接收该属性的当前值。 检索的值的类型取决于 dwAttribute 参数的值。 DWMWINDOWATTRIBUTE 枚举主题指示，在每个标志的行中，应向 pvAttribute 参数传递指向的值类型。</param>
        /// <param name="cbAttribute">通过 pvAttribute 参数接收的属性值的大小（以字节为单位）。 检索到的值的类型及其大小（以字节为单位）取决于 dwAttribute 参数的值。</param>
        /// <returns>如果函数成功，则返回 S_OK。 否则，它将返回 HRESULT错误代码。</returns>
        [DllImport(Dwmapi, CharSet = CharSet.Unicode, EntryPoint = "DwmGetWindowAttribute", PreserveSig = true, SetLastError = false)]
        public static extern int DwmGetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE dwAttribute, out IntPtr pvAttribute, int cbAttribute);
    }
}
