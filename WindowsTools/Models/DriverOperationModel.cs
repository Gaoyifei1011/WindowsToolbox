using System;
using System.ComponentModel;

namespace WindowsTools.Models
{
    /// <summary>
    /// 驱动操作数据模型
    /// </summary>
    public class DriverOperationModel : INotifyPropertyChanged
    {
        private bool _isOperating;

        public bool IsOperating
        {
            get { return _isOperating; }

            set
            {
                if (!Equals(_isOperating, value))
                {
                    _isOperating = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsOperating)));
                }
            }
        }

        /// <summary>
        /// 驱动 INF 名称
        /// </summary>
        public string DriverInfName { get; set; }

        /// <summary>
        /// 驱动操作信息
        /// </summary>
        public string DriverOperation { get; set; }

        /// <summary>
        /// 驱动操作唯一 ID
        /// </summary>
        public Guid DriverOperationGuid { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
