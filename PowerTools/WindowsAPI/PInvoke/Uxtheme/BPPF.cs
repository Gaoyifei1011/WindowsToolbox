using System;

namespace PowerTools.WindowsAPI.PInvoke.Uxtheme
{
    [Flags]
    public enum BPPF : uint
    {
        /// <summary>
        /// 在 BeginBufferedPaint 期间，将缓冲区初始化为 ARGB = {0， 0， 0， 0}。 这会擦除缓冲区的先前内容。
        /// </summary>
        BPPF_ERASE = 0x0001,

        /// <summary>
        /// 不要将目标 DC 的剪辑区域应用于双缓冲区。 如果未设置此标志，并且目标 DC 是窗口 DC，则由于重叠窗口而进行剪裁将应用于双缓冲区。
        /// </summary>
        BPPF_NOCLIP = 0x0002,

        /// <summary>
        /// 正在使用非客户端 DC。
        /// </summary>
        BPPF_NONCLIENT = 0x0004
    }
}
