namespace WindowsTools.WindowsAPI.PInvoke.Dismapi
{
    /// <summary>
    /// 指定驱动程序的签名状态。
    /// </summary>
    public enum DismDriverSignature
    {
        /// <summary>
        /// 驱动程序的签名状态未知。 DISM 仅检查启动关键驱动程序的有效签名。
        /// </summary>
        DismDriverSignatureUnknown = 0,

        /// <summary>
        /// 驱动程序未签名。
        /// </summary>
        DismDriverSignatureUnsigned = 1,

        /// <summary>
        /// 驱动程序已签名。
        /// </summary>
        DismDriverSignatureSigned = 2
    }
}
