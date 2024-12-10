using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace WindowsTools.Models
{
    /// <summary>
    /// 右键菜单项数据模型
    /// </summary>
    public class ContextMenuModel : INotifyPropertyChanged
    {
        private Visibility _isVisible;

        public Visibility IsVisible
        {
            get { return _isVisible; }

            set
            {
                if (!Equals(_isVisible, value))
                {
                    _isVisible = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsVisible)));
                }
            }
        }

        public ImageSource PackageIcon { get; set; }

        public Uri PackageIconUri { get; set; }

        public string PackageDisplayName { get; set; }

        public string PackageFullName { get; set; }

        public string PackagePath { get; set; }

        public ObservableCollection<ContextMenuItemModel> ContextMenuItemCollection { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
