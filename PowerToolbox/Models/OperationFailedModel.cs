using System;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 操作失败信息数据模型
    /// </summary>
    public sealed class OperationFailedModel
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        public Exception Exception { get; set; }
    }
}
