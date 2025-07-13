using System.ComponentModel;

namespace PowerToolbox.Models
{
    /// <summary>
    /// Hosts 文件数据模型
    /// </summary>
    public class HostsModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 主机项是否被选择
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
        /// 地址
        /// </summary>
        private string _address;

        public string Address
        {
            get { return _address; }

            set
            {
                if (!string.Equals(_address, value))
                {
                    _address = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Address)));
                }
            }
        }

        /// <summary>
        /// 主机信息
        /// </summary>
        private string _hosts;

        public string Hosts
        {
            get { return _hosts; }

            set
            {
                if (!string.Equals(_hosts, value))
                {
                    _hosts = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Hosts)));
                }
            }
        }

        /// <summary>
        /// 注释
        /// </summary>
        private string _annotation;

        public string Annotation
        {
            get { return _annotation; }

            set
            {
                if (!string.Equals(_annotation, value))
                {
                    _annotation = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Annotation)));
                }
            }
        }

        /// <summary>
        /// 是否可用
        /// </summary>
        private bool _isAvailable;

        public bool IsAvailable
        {
            get { return _isAvailable; }

            set
            {
                if (!Equals(_isAvailable, value))
                {
                    _isAvailable = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAvailable)));
                }
            }
        }

        /// <summary>
        /// 是否可用
        /// </summary>
        private string _isAvailableString;

        public string IsAvailableString
        {
            get { return _isAvailableString; }

            set
            {
                if (!Equals(_isAvailableString, value))
                {
                    _isAvailableString = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAvailableString)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
