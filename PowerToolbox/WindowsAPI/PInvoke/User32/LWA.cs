namespace PowerToolbox.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 设置分层窗口的不透明度和透明度颜色键时对应的操作
    /// </summary>
    public enum LWA
    {
        /// <summary>
        /// 使用 crKey 作为透明度颜色。
        /// </summary>
        LWA_COLORKEY = 0x1,

        /// <summary>
        /// 使用 bAlpha 确定分层窗口的不透明度。
        /// </summary>
        LWA_ALPHA = 0x2
    }
}
