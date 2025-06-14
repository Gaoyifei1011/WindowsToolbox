using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Media;

namespace PowerTools.Models
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
