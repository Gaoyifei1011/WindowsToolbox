using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using WindowsTools.Models;

namespace WindowsTools.Views.Pages
{
    /// <summary>
    /// 提取图标页面
    /// </summary>
    public sealed partial class ExtractIconPage : Page, INotifyPropertyChanged
    {
        private string _getResults;

        public string GetResults
        {
            get { return _getResults; }

            set
            {
                _getResults = value;
                OnPropertyChanged();
            }
        }

        private string _noResources;

        public string NoResources
        {
            get { return _noResources; }

            set
            {
                _noResources = value;
                OnPropertyChanged();
            }
        }

        private DictionaryEntry _selectedIconFormat;

        public DictionaryEntry SelectedIconFormat
        {
            get { return _selectedIconFormat; }

            set
            {
                _selectedIconFormat = value;
                OnPropertyChanged();
            }
        }

        private DictionaryEntry _selectedIconSize;

        public DictionaryEntry SelectedIconSize
        {
            get { return _selectedIconSize; }

            set
            {
                _selectedIconSize = value;
                OnPropertyChanged();
            }
        }

        private ImageSource _imageSource;

        public ImageSource ImageSource
        {
            get { return _imageSource; }

            set
            {
                _imageSource = value;
                OnPropertyChanged();
            }
        }

        private List<DictionaryEntry> IconFormatList { get; } = new List<DictionaryEntry>()
        {
            new DictionaryEntry() { Key = ".ico", Value = ".ico" },
            new DictionaryEntry() { Key = ".png", Value = ".png" }
        };

        private List<DictionaryEntry> IconSizeList { get; set; } = new List<DictionaryEntry>()
        {
            new DictionaryEntry() { Key = "16 * 16", Value = 16 },
            new DictionaryEntry() { Key = "24 * 24", Value = 24 },
            new DictionaryEntry() { Key = "32 * 32", Value = 32 },
            new DictionaryEntry() { Key = "48 * 48", Value = 48 },
            new DictionaryEntry() { Key = "64 * 64", Value = 64 },
            new DictionaryEntry() { Key = "96 * 96", Value = 96 },
            new DictionaryEntry() { Key = "128 * 128", Value = 128 },
            new DictionaryEntry() { Key = "256 * 256", Value = 256 }
        };

        private ObservableCollection<IconModel> IconCollection = new ObservableCollection<IconModel>();

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ExtractIconPage()
        {
            InitializeComponent();
            SelectedIconFormat = IconFormatList[0];
            SelectedIconSize = IconSizeList[0];

            GetResults = Strings.ExtractIcon.GetResults;
            NoResources = Strings.ExtractIcon.NoResources;
        }

        #region 第二部分：提取图标页面——挂载的事件

        /// <summary>
        /// 选择图标格式
        /// </summary>
        private void OnIconFormatClicked(object sender, RoutedEventArgs args)
        {
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            if (item.Tag is not null)
            {
                SelectedIconFormat = IconFormatList[Convert.ToInt32(item.Tag)];
            }
        }

        /// <summary>
        /// 选择图标尺寸
        /// </summary>
        private void OnIconSizeClicked(object sender, RoutedEventArgs args)
        {
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            if (item.Tag is not null)
            {
                SelectedIconSize = IconSizeList[Convert.ToInt32(item.Tag)];
            }
        }

        /// <summary>
        /// 选择文件
        /// </summary>
        private void OnSelectFileClicked(object sender, RoutedEventArgs args)
        {
        }

        /// <summary>
        /// 导出选中的图标
        /// </summary>
        private void OnExportSelectedIconsClicked(object sender, RoutedEventArgs args)
        {
        }

        /// <summary>
        /// 导出所有图标
        /// </summary>
        private void OnExportAllIconsClicked(object sender, RoutedEventArgs args)
        {
        }

        #endregion 第二部分：提取图标页面——挂载的事件
    }
}
