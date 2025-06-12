using System;
using System.ComponentModel;

namespace PowerTools.Models
{
    /// <summary>
    /// 驱动数据模型
    /// </summary>
    public class DriverModel : INotifyPropertyChanged
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

        private bool _isProcessing;

        public bool IsProcessing
        {
            get { return _isProcessing; }

            set
            {
                if (!Equals(_isProcessing, value))
                {
                    _isProcessing = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsProcessing)));
                }
            }
        }

        /// <summary>
        /// 驱动名称
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// 驱动 INF 名称
        /// </summary>
        public string DriverInfName { get; set; }

        /// <summary>
        /// 驱动 OEM INF 名称
        /// </summary>
        public string DriverOEMInfName { get; set; }

        /// <summary>
        /// 驱动类别
        /// </summary>
        public string DriverType { get; set; }

        /// <summary>
        /// 驱动制造商
        /// </summary>
        public string DriverManufacturer { get; set; }

        /// <summary>
        /// 驱动版本
        /// </summary>
        public Version DriverVersion { get; set; }

        /// <summary>
        /// 驱动日期
        /// </summary>
        public DateTime DriverDate { get; set; }

        /// <summary>
        /// 驱动大小
        /// </summary>
        public string DriverSize { get; set; }

        /// <summary>
        /// 驱动路径
        /// </summary>
        public string DriverLocation { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        public string SignatureName { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
