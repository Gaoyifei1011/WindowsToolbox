namespace WindowsTools.WindowsAPI.PInvoke.Dismapi
{
    /// <summary>
    /// 指定在日志文件中报告的信息的类型。
    /// </summary>
    public enum DismLogLevel
    {
        /// <summary>
        /// 日志文件仅包含错误。
        /// </summary>
        DismLogErrors = 0,

        /// <summary>
        /// 日志文件包含错误和警告。
        /// </summary>
        DismLogErrorsWarnings = 1,

        /// <summary>
        /// 日志文件包含错误、警告和其他信息。
        /// </summary>
        DismLogErrorsWarningsInfo = 2
    }
}
