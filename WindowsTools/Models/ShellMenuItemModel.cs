using System.ComponentModel;
using Windows.UI.Xaml.Media;

namespace WindowsTools.Models
{
    /// <summary>
    /// 扩展菜单项数据模型
    /// </summary>
    public class ShellMenuItemModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 菜单项索引
        /// </summary>
        private int _menuIndex;

        public int MenuIndex
        {
            get { return _menuIndex; }

            set
            {
                if (!Equals(value, _menuIndex))
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

        private string _param;

        public string Param
        {
            get { return _param; }

            set
            {
                if (!Equals(_param, value))
                {
                    _param = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Param)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
