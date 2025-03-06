using System;
using System.ComponentModel;
using Windows.UI.Xaml;

namespace WindowsTools.Models
{
    /// <summary>
    /// 网络回环数据模型
    /// </summary>
    public sealed class LoopbackModel : INotifyPropertyChanged
    {
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

        public bool IsOldChecked { get; set; }

        /// <summary>
        /// 应用程序运行的二进制路径
        /// </summary>
        public string[] AppBinariesPath { get; set; }

        /// <summary>
        /// 应用容器的全局唯一名称
        /// </summary>
        public string AppContainerName { get; set; }

        /// <summary>
        /// 应用容器的友好名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 应用容器其用途的说明、使用该容器的应用程序的目标等
        /// </summary>
        public string Description { get; set; }

        public string PackageFullName { get; set; }

        /// <summary>
        /// 应用容器的工作目录
        /// </summary>
        public string WorkingDirectory { get; set; }

        /// <summary>
        /// 应用容器所属用户的名称
        /// </summary>
        public string AppContainerUserName { get; set; }

        /// <summary>
        /// 应用容器的包标识符
        /// </summary>
        public IntPtr AppContainerSID { get; set; }

        /// <summary>
        /// 应用容器的包标识符名称
        /// </summary>
        public string AppContainerSIDName { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
