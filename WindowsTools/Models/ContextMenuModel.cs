using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.UI.Xaml.Media;

namespace WindowsTools.Models
{
    /// <summary>
    /// 右键菜单项数据模型
    /// </summary>
    public class ContextMenuModel
    {
        public ImageSource PackageIcon { get; set; }

        public Uri PackageIconUri { get; set; }

        public string PackageDisplayName { get; set; }

        public string PackageFullName { get; set; }

        public string PackagePath { get; set; }

        public ObservableCollection<ContextMenuItemModel> ContextMenuItemCollection { get; set; }
    }
}
