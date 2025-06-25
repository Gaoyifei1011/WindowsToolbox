using System;

namespace PowerToolbox.WindowsAPI.PInvoke.Uxtheme
{
    /// <summary>
    /// 指定修改窗口视觉样式属性的标志。 使用以下值之一或按位组合。
    /// </summary>
    [Flags]
    public enum WTNCA
    {
        /// <summary>
        /// 防止绘制窗口描述文字。
        /// </summary>
        WTNCA_NODRAWCAPTION = 0x00000001,

        /// <summary>
        /// 阻止绘制系统图标。
        /// </summary>
        WTNCA_NODRAWICON = 0x00000002,

        /// <summary>
        /// 防止显示系统图标菜单。
        /// </summary>
        WTNCA_NOSYSMENU = 0x00000004,

        /// <summary>
        /// 即使在从右到左 (RTL) 布局中，也会阻止问号的镜像。
        /// </summary>
        WTNCA_NOMIRRORHELP = 0x00000008,

        /// <summary>
        /// 包含所有有效位的掩码。
        /// </summary>
        WTNCA_VALIDBITS = 0x0000000F
    }
}
