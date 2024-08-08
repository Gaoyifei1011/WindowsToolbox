using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.UI.Xaml.Media;
using WindowsTools.Extensions.DataType.Enums;

namespace WindowsTools.Models
{
    /// <summary>
    /// 扩展菜单项数据模型
    /// </summary>
    public sealed class ShellMenuItemModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 菜单 Guid 号
        /// </summary>
        public Guid MenuGuid { get; set; }

        /// <summary>
        /// 菜单类型
        /// </summary>
        public MenuType MenuType { get; set; }

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
        /// 菜单项索引
        /// </summary>
        private int _menuIndex;

        public int MenuIndex
        {
            get { return _menuIndex; }

            set
            {
                if (!Equals(_menuIndex, value))
                {
                    _menuIndex = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MenuIndex)));
                }
            }
        }

        /// <summary>
        /// 菜单名称
        /// </summary>
        private string _title;

        public string Title
        {
            get { return _title; }

            set
            {
                if (!Equals(_title, value))
                {
                    _title = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Title)));
                }
            }
        }

        /// <summary>
        /// 菜单图标
        /// </summary>
        private ImageSource _icon;

        public ImageSource Icon
        {
            get { return _icon; }

            set
            {
                if (!Equals(_icon, value))
                {
                    _icon = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Icon)));
                }
            }
        }

        /// <summary>
        /// 图标路径
        /// </summary>
        private string _iconPath;

        public string IconPath
        {
            get { return _iconPath; }

            set
            {
                if (!Equals(_iconPath, value))
                {
                    _iconPath = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IconPath)));
                }
            }
        }

        /// <summary>
        /// 应用程序路径
        /// </summary>
        private string _programPath;

        public string ProgramPath
        {
            get { return _programPath; }

            set
            {
                if (!Equals(_programPath, value))
                {
                    _programPath = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProgramPath)));
                }
            }
        }

        /// <summary>
        /// 子菜单
        /// </summary>
        public ObservableCollection<ShellMenuItemModel> SubMenuItemCollection { get; set; } = [];

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
