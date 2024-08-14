using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.UI.Xaml.Media.Imaging;
using WindowsTools.Extensions.DataType.Enums;

namespace WindowsTools.Models
{
    /// <summary>
    /// 扩展菜单项数据模型
    /// </summary>
    public sealed class ShellMenuItemModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 菜单标题
        /// </summary>
        private string _menuTitleText;

        public string MenuTitleText
        {
            get { return _menuTitleText; }

            set
            {
                if (!Equals(_menuTitleText, value))
                {
                    _menuTitleText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MenuTitleText)));
                }
            }
        }

        /// <summary>
        /// 是否使用应用程序图标
        /// </summary>
        private bool _shouldUseProgramIcon;

        public bool ShouldUseProgramIcon
        {
            get { return _shouldUseProgramIcon; }

            set
            {
                if (!Equals(_shouldUseProgramIcon, value))
                {
                    _shouldUseProgramIcon = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShouldUseProgramIcon)));
                }
            }
        }

        private bool _shouldEnableThemeIcon;

        public bool ShouldEnableThemeIcon
        {
            get { return _shouldEnableThemeIcon; }

            set
            {
                if (!Equals(_shouldEnableThemeIcon, value))
                {
                    _shouldEnableThemeIcon = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShouldEnableThemeIcon)));
                }
            }
        }

        /// <summary>
        /// 菜单项图标
        /// </summary>
        private BitmapSource _menuIcon;

        public BitmapSource MenuIcon
        {
            get { return _menuIcon; }

            set
            {
                if (!Equals(_menuIcon, value))
                {
                    _menuIcon = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MenuIcon)));
                }
            }
        }

        /// <summary>
        /// 是否选中当前菜单项
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
        /// 菜单键值
        /// </summary>
        public string MenuKey { get; set; }

        /// <summary>
        /// 菜单项 GUID 值
        /// </summary>
        public Guid MenuGuid { get; set; }

        /// <summary>
        /// 菜单类型
        /// </summary>
        public MenuType MenuType { get; set; }

        /// <summary>
        /// 默认的菜单项图标
        /// </summary>
        public string DefaultIconPath { get; set; }

        /// <summary>
        /// 浅色主题下的菜单项图标
        /// </summary>
        public string LightThemeIconPath { get; set; }

        /// <summary>
        /// 深色主题下的菜单项图标
        /// </summary>
        public string DarkThemeIconPath { get; set; }

        /// <summary>
        /// 菜单程序路径
        /// </summary>
        public string MenuProgramPathText { get; set; }

        /// <summary>
        /// 菜单参数
        /// </summary>
        public string MenuParameter { get; set; }

        /// <summary>
        /// 是否启用文件夹背景菜单项
        /// </summary>
        public bool FolderBackground { get; set; }

        /// <summary>
        /// 是否启用文件夹桌面菜单项
        /// </summary>
        public bool FolderDesktop { get; set; }

        /// <summary>
        /// 是否启用文件夹目录菜单项
        /// </summary>
        public bool FolderDirectory { get; set; }

        /// <summary>
        /// 是否启用文件夹驱动器菜单项
        /// </summary>
        public bool FolderDrive { get; set; }

        /// <summary>
        /// 菜单项文件匹配规则
        /// </summary>
        public string MenuFileMatchRule { get; set; }

        /// <summary>
        /// 菜单项文件匹配格式
        /// </summary>
        public string MenuFileMatchFormatText { get; set; }

        /// <summary>
        /// 子菜单
        /// </summary>
        public ObservableCollection<ShellMenuItemModel> SubMenuItemCollection { get; set; } = [];

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
