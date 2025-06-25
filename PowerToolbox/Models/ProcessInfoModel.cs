using Windows.UI.Xaml.Media;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 进程信息数据模型
    /// </summary>
    public sealed class ProcessInfoModel
    {
        /// <summary>
        /// 进程名称
        /// </summary>
        public string ProcessName { get; set; }

        /// <summary>
        /// 进程 ID
        /// </summary>
        public int ProcessId { get; set; }

        /// <summary>
        /// 进程路径
        /// </summary>
        public string ProcessPath { get; set; }

        /// <summary>
        /// 进程所属的用户
        /// </summary>
        public string ProcessUser { get; set; }

        /// <summary>
        /// 进程图标
        /// </summary>
        public ImageSource ProcessIcon { get; set; }
    }
}
