using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WindowsTools.Models
{
    /// <summary>
    /// 包文件索引字符串数据模型
    /// </summary>
    public class StringModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 是否已选择
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
        /// 字符串对应的键
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 字符串对应的索引
        /// </summary>
        public uint StringIndex { get; set; }

        /// <summary>
        /// 字符串对应的内容
        /// </summary>
        public string Content { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
