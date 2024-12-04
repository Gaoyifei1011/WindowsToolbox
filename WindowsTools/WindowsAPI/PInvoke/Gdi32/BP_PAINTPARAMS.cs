using System;
using WindowsTools.WindowsAPI.PInvoke.Uxtheme;

namespace WindowsTools.WindowsAPI.PInvoke.Gdi32
{
    /// <summary>
    /// 定义 BeginBufferedPaint 的绘制操作参数。
    /// </summary>
    public unsafe struct BP_PAINTPARAMS
    {
        /// <summary>
        /// 此结构的大小（以字节为单位）。
        /// </summary>
        public uint cbSize;

        /// <summary>
        /// 以下一个或多个值。
        /// </summary>
        public BPPF dwFlags;

        /// <summary>
        /// 指向排除 RECT 结构的指针。 此矩形从剪裁区域中排除。 对于没有排除矩形，可以为 NULL 。
        /// </summary>
        public IntPtr prcExclude;

        /// <summary>
        /// 指向 BLENDFUNCTION 结构的指针，该结构通过指定源位图和目标位图的混合函数来控制混合。 如果 为 NULL，则源缓冲区将复制到目标，不进行混合。
        /// </summary>
        public IntPtr pBlendFunction;
    }
}
