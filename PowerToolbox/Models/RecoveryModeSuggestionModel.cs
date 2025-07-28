namespace PowerToolbox.Models
{
    /// <summary>
    /// 恢复模式建议数据模型
    /// </summary>
    public class RecoveryModeSuggestionModel
    {
        /// <summary>
        /// 文件系统
        /// </summary>
        public string FileSystem { get; set; }

        /// <summary>
        /// 使用状况
        /// </summary>
        public string Circumstances { get; set; }

        /// <summary>
        /// 建议模式
        /// </summary>
        public string RecommendedMode { get; set; }
    }
}
