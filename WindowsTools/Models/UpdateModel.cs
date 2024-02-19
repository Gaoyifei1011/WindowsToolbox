using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WUApiLib;

namespace WindowsTools.Models
{
    /// <summary>
    /// Windows 更新数据模型
    /// </summary>
    public class UpdateModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 更新名称
        /// </summary>
        public string UpdateName { get; set; }

        /// <summary>
        /// 更新的应用 ID
        /// </summary>
        public string ApplicationID { get; set; }

        /// <summary>
        /// 更新描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 更新支持链接
        /// </summary>
        public string SupportURL { get; set; }

        /// <summary>
        /// 更新的文件大小
        /// </summary>
        public string Size { get; set; }

        /// <summary>
        /// 更新安装状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 更新 ID
        /// </summary>
        public string UpdateID { get; set; }

        /// <summary>
        /// 更新安装日期
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 当前更新实例
        /// </summary>
        public IUpdate Update { get; set; }

        /// <summary>
        /// 当前更新是否选择
        /// </summary>
        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }

            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 安装或卸载进度
        /// </summary>
        private double _installationProgress;

        public double InstallationProgress
        {
            get { return _installationProgress; }

            set
            {
                _installationProgress = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
