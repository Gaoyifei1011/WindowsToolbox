﻿using System.ComponentModel;

namespace WindowsTools.Models
{
    /// <summary>
    /// 包文件索引嵌入的数据内容数据模型
    /// </summary>
    public class EmbeddedDataModel : INotifyPropertyChanged
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
        /// 嵌入的数据对应的键
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 嵌入的数据对应的索引
        /// </summary>
        public uint EmbeddedDataIndex { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
