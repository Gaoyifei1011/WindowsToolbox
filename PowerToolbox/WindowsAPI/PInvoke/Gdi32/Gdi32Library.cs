using System;
using System.Runtime.InteropServices;

// 抑制 CA1401 警告
#pragma warning disable CA1401

namespace PowerToolbox.WindowsAPI.PInvoke.Gdi32
{
    public static class Gdi32Library
    {
        private const string Gdi32 = "gdi32.dll";

        /// <summary>
        /// CreateSolidBrush 函数创建具有指定纯色的逻辑画笔。
        /// </summary>
        /// <param name="crColor">画笔的颜色。 若要创建 COLORREF 颜色值，请使用 RGB 宏。</param>
        /// <returns>如果函数成功，则返回值将标识逻辑画笔。如果函数失败，则返回值为 NULL。</returns>
        [DllImport(Gdi32, CharSet = CharSet.Unicode, EntryPoint = "CreateSolidBrush", PreserveSig = true, SetLastError = false)]
        public static extern IntPtr CreateSolidBrush(int crColor);

        /// <summary>
        /// DeleteObject 函数删除逻辑笔、画笔、字体、位图、区域或调色板，从而释放与对象关联的所有系统资源。 删除对象后，指定的句柄不再有效。
        /// </summary>
        /// <param name="ho">逻辑笔、画笔、字体、位图、区域或调色板的句柄。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果指定的句柄无效或当前已选择到 DC 中，则返回值为零。</returns>
        [DllImport(Gdi32, CharSet = CharSet.Unicode, EntryPoint = "DeleteObject", PreserveSig = true, SetLastError = false)]
        public static extern int DeleteObject(IntPtr ho);

        /// <summary>
        /// GetStockObject 函数检索其中一支股票笔、画笔、字体或调色板的句柄。
        /// </summary>
        /// <param name="i">常用对象的类型。</param>
        /// <returns>如果函数成功，则返回值是请求的逻辑对象的句柄。如果函数失败，则返回值为 NULL。</returns>
        [DllImport(Gdi32, CharSet = CharSet.Unicode, EntryPoint = "GetStockObject", PreserveSig = true, SetLastError = false)]
        public static extern IntPtr GetStockObject(StockObject i);
    }
}
