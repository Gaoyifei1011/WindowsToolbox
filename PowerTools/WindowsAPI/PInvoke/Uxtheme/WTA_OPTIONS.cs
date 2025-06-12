using System.Runtime.InteropServices;

namespace PowerTools.WindowsAPI.PInvoke.Uxtheme
{
    /// <summary>
    /// 定义用于设置窗口视觉样式属性的选项。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct WTA_OPTIONS
    {
        /// <summary>
        /// 修改窗口视觉样式属性的标志的组合。 可以是 WTNCA 常量的组合。
        /// </summary>
        public WTNCA dwFlags;

        /// <summary>
        /// 一个位掩码，描述应如何应用 dwFlags 中指定的值。 如果 与 dwFlags 中的值对应的位为 0，则将删除该标志。 如果位为 1，则将添加 标志。
        /// </summary>
        public uint dwMask;
    }
}
