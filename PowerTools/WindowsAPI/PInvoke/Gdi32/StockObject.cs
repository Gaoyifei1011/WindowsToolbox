namespace PowerTools.WindowsAPI.PInvoke.Gdi32
{
    /// <summary>
    /// 常用对象的类型。
    /// </summary>
    public enum StockObject : int
    {
        /// <summary>
        /// 白色画笔。
        /// </summary>
        WHITE_BRUSH = 0,

        /// <summary>
        /// 浅灰色画笔。
        /// </summary>
        LTGRAY_BRUSH = 1,

        /// <summary>
        /// 灰色画笔。
        /// </summary>
        GRAY_BRUSH = 2,

        /// <summary>
        /// 深灰色画笔。
        /// </summary>
        DKGRAY_BRUSH = 3,

        /// <summary>
        /// 黑色画笔。
        /// </summary>
        BLACK_BRUSH = 4,

        /// <summary>
        /// null 画笔 (等效于 HOLLOW_BRUSH) 。
        /// </summary>
        NULL_BRUSH = 5,

        /// <summary>
        /// 空心画笔 (等效于NULL_BRUSH) 。
        /// </summary>
        HOLLOW_BRUSH = NULL_BRUSH,

        /// <summary>
        /// 白色触笔。
        /// </summary>
        WHITE_PEN = 6,

        /// <summary>
        /// 黑色触笔。
        /// </summary>
        BLACK_PEN = 7,

        /// <summary>
        /// null 触笔。 null 触控笔不绘制任何内容。
        /// </summary>
        NULL_PEN = 8,

        /// <summary>
        /// 原始设备制造商 (OEM) 依赖固定间距 (正方形) 字体。
        /// </summary>
        OEM_FIXED_FONT = 10,

        /// <summary>
        /// Windows 固定间距 (正) 系统字体。
        /// </summary>
        ANSI_FIXED_FONT = 11,

        /// <summary>
        /// Windows 可变间距 (比例空间) 系统字体。
        /// </summary>
        ANSI_VAR_FONT = 12,

        /// <summary>
        /// 系统字体。 默认情况下，系统使用系统字体来绘制菜单、对话框控件和文本。 不建议使用DEFAULT_GUI_FONT或SYSTEM_FONT来获取对话框和窗口使用的字体;有关详细信息，请参阅备注部分。
        /// 默认系统字体为 Tahoma。
        /// </summary>
        SYSTEM_FONT = 13,

        /// <summary>
        /// 设备依赖字体。
        /// </summary>
        DEVICE_DEFAULT_FONT = 14,

        /// <summary>
        /// 默认调色板。 此调色板由系统调色板中的静态颜色组成。
        /// </summary>
        DEFAULT_PALETTE = 15,

        /// <summary>
        /// 固定间距 (单调) 系统字体。 此库存对象仅用于与 3.0 之前的 16 位 Windows 版本兼容。
        /// </summary>
        SYSTEM_FIXED_FONT = 16,

        /// <summary>
        /// 用户界面对象（如菜单和对话框）的默认字体。 不建议使用DEFAULT_GUI_FONT或SYSTEM_FONT来获取对话框和窗口使用的字体;有关详细信息，请参阅备注部分。
        /// 默认字体为 Tahoma。
        /// </summary>
        DEFAULT_GUI_FONT = 17,

        /// <summary>
        /// 纯色画笔。 默认颜色为白色。
        /// </summary>
        DC_BRUSH = 18,

        /// <summary>
        /// 纯色笔颜色。 默认颜色为黑色。
        /// </summary>
        DC_PEN = 19,
    }
}
