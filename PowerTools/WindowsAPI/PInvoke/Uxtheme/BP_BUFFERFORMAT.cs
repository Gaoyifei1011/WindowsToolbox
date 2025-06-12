namespace PowerTools.WindowsAPI.PInvoke.Uxtheme
{
    /// <summary>
    /// 指定缓冲区的格式。 由 BeginBufferedAnimation 和 BeginBufferedPaint 使用。
    /// </summary>
    public enum BP_BUFFERFORMAT
    {
        /// <summary>
        /// 兼容的位图。 每像素的位数基于与使用 BeginBufferedPaint 或 BeginBufferedAnimation 指定的 HDC 关联的设备的颜色格式，通常，这是显示设备。
        /// </summary>
        BPBF_COMPATIBLEBITMAP,

        /// <summary>
        /// 自下而上与设备无关的位图。 位图的原点为左下角。 使用每像素 32 位。
        /// </summary>
        BPBF_DIB,

        /// <summary>
        /// 自上而下与设备无关的位图。 位图的原点是左上角。 使用每像素 32 位。
        /// </summary>
        BPBF_TOPDOWNDIB,

        /// <summary>
        /// 自上而下、单色、与设备无关的位图。 使用每像素 1 位。
        /// </summary>
        BPBF_TOPDOWNMONODIB
    }
}
