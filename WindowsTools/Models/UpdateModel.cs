using System.Collections.Generic;
using System.ComponentModel;
using WindowsTools.Extensions.DataType.Class;

namespace WindowsTools.Models
{
    /// <summary>
    /// Windows 更新数据模型
    /// </summary>
    public sealed class UpdateModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 更新信息
        /// </summary>
        public UpdateInformation UpdateInformation { get; set; }

        /// <summary>
        /// 更新描述内容
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 与更新关联的 Microsoft 软件许可条款的完整本地化文本
        /// </summary>
        public string EulaText { get; set; }

        /// <summary>
        /// 是否为测试版本的更新
        /// </summary>
        public string IsBeta { get; set; }

        /// <summary>
        /// 更新是否必须安装
        /// </summary>
        public string IsMandatory { get; set; }

        /// <summary>
        /// 更新最大下载大小
        /// </summary>
        public string MaxDownloadSize { get; set; }

        /// <summary>
        /// 更新最小下载大小
        /// </summary>
        public string MinDownloadSize { get; set; }

        /// <summary>
        /// 更新的严重等级
        /// </summary>
        public string MsrcSeverity { get; set; }

        /// <summary>
        /// 更新建议安装的 CPU 速度
        /// </summary>
        public string RecommendedCpuSpeed { get; set; }

        /// <summary>
        /// 更新建议安装的可用空间大小
        /// </summary>
        public string RecommendedHardDiskSpace { get; set; }

        /// <summary>
        /// 更新建议安装的物理内存大小
        /// </summary>
        public string RecommendedMemory { get; set; }

        /// <summary>
        /// 更新是否需要重启系统来完成安装或卸载更新
        /// </summary>
        public string RebootRequired { get; set; }

        /// <summary>
        /// 更新的本地化发行说明
        /// </summary>
        public string ReleaseNotes { get; set; }

        /// <summary>
        /// 更新支持的链接
        /// </summary>
        public string SupportURL { get; set; }

        /// <summary>
        /// 更新标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 更新类型
        /// </summary>
        public string UpdateType { get; set; }

        /// <summary>
        /// 更新的标识符
        /// </summary>
        public string UpdateID { get; set; }

        /// <summary>
        /// 与更新关联的 CVE ID 集合
        /// </summary>
        public List<string> CveIDList { get; } = [];

        /// <summary>
        /// 与更新关联的 Microsoft 知识库文章 ID 的集合。
        /// </summary>
        public List<string> KBArticleIDList { get; } = [];

        /// <summary>
        /// 更新支持的语言列表
        /// </summary>
        public List<string> LanguageList { get; } = [];

        /// <summary>
        /// 有关更新的详细信息的超链接
        /// </summary>
        public List<string> MoreInfoList { get; } = [];

        /// <summary>
        /// 当前更新是否选择
        /// </summary>
        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }

            set
            {
                if (!Equals(_isSelected, value))
                {
                    _isSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
                }
            }
        }

        /// <summary>
        /// 当前更新是否正在更新中
        /// </summary>
        private bool _isUpdating;

        public bool IsUpdating
        {
            get { return _isUpdating; }

            set
            {
                if (!Equals(_isUpdating, value))
                {
                    _isUpdating = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsUpdating)));
                }
            }
        }

        /// <summary>
        /// 更新进度
        /// </summary>
        private string _updateProgress;

        public string UpdateProgress
        {
            get { return _updateProgress; }

            set
            {
                if (!Equals(_updateProgress, value))
                {
                    _updateProgress = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UpdateProgress)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
