using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WindowsTools.Models;
using WindowsTools.Views.Windows;
using WindowsTools.WindowsAPI.PInvoke.Shell32;

namespace WindowsTools.Views.Pages
{
    /// <summary>
    /// 下载管理页面
    /// </summary>
    public sealed partial class DownloadManagerPage : Page, INotifyPropertyChanged
    {
        private string _searchDownload;

        public string SearchDownload
        {
            get { return _searchDownload; }

            set
            {
                if (!Equals(_searchDownload, value))
                {
                    _searchDownload = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchDownload)));
                }
            }
        }

        private bool _isSelectMode;

        public bool IsSelectMode
        {
            get { return _isSelectMode; }

            set
            {
                if (!Equals(_isSelectMode, value))
                {
                    _isSelectMode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelectMode)));
                }
            }
        }

        private ObservableCollection<DownloadModel> DownloadCollection { get; } = new ObservableCollection<DownloadModel>();

        public event PropertyChangedEventHandler PropertyChanged;

        public DownloadManagerPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 搜索文本框内容发生变化的事件
        /// </summary>
        private void OnSearchDownloadTextChanged(object sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            SearchDownload = (sender as TextBox).Text;
        }

        /// <summary>
        /// 查询搜索内容
        /// </summary>
        private void OnSearchDownloadQuerySubmitted(object sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
        }

        /// <summary>
        /// 添加任务
        /// </summary>
        private void OnAddTaskClicked(object sender, RoutedEventArgs args)
        {
        }

        /// <summary>
        /// 全部下载
        /// </summary>
        private void OnStartDownloadClicked(object sender, RoutedEventArgs args)
        {
        }

        /// <summary>
        /// 暂停下载
        /// </summary>
        private void OnPauseDownloadClicked(object sender, RoutedEventArgs args)
        {
        }

        /// <summary>
        /// 删除下载
        /// </summary>
        private void OnDeleteDownloadClicked(object sender, RoutedEventArgs args)
        {
        }

        /// <summary>
        /// 打开默认下载目录
        /// </summary>
        private void OnOpenDefaultDownloadFolderClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                Shell32Library.SHGetKnownFolderPath(new Guid("374DE290-123F-4565-9164-39C4925E467B"), KNOWN_FOLDER_FLAG.KF_FLAG_DEFAULT, IntPtr.Zero, out string downloadFolder);
                Process.Start(downloadFolder);
            });
        }

        /// <summary>
        /// 了解传递优化
        /// </summary>
        private void OnLearnDeliveryOptimizationClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                Process.Start("https://learn.microsoft.com/windows/deployment/do/waas-delivery-optimization");
            });
        }

        /// <summary>
        /// 打开传递优化设置
        /// </summary>
        private void OnOpenDeliveryOptimizationSettingsClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                Process.Start("ms-settings:delivery-optimization");
            });
        }

        /// <summary>
        /// 打开下载设置
        /// </summary>
        private void OnDownloadSettingsClicked(object sender, RoutedEventArgs args)
        {
            (MainWindow.Current.Content as MainPage).NavigateTo(typeof(SettingsPage));
        }

        private void OnItemClicked(object sender, ItemClickEventArgs args)
        {
        }
    }
}
