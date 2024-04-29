using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WindowsTools.Services.Controls.Pages;
using WindowsTools.Strings;
using WindowsTools.WindowsAPI.PInvoke.Shell32;

namespace WindowsTools.UI.Dialogs
{
    /// <summary>
    /// 下载添加任务对话框
    /// </summary>
    public sealed partial class DownloadDialog : ContentDialog, INotifyPropertyChanged
    {
        private string _downloadLinkText;

        public string DownloadLinkText
        {
            get { return _downloadLinkText; }

            set
            {
                if (!Equals(_downloadLinkText, value))
                {
                    _downloadLinkText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DownloadLinkText)));
                }
            }
        }

        private string _downloadFileNameText;

        public string DownloadFileNameText
        {
            get { return _downloadFileNameText; }

            set
            {
                if (!Equals(_downloadFileNameText, value))
                {
                    _downloadFileNameText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DownloadFileNameText)));
                }
            }
        }

        private string _downloadFolderText;

        public string DownloadFolderText
        {
            get { return _downloadFolderText; }

            set
            {
                if (!Equals(_downloadFolderText, value))
                {
                    _downloadFolderText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DownloadFolderText)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public DownloadDialog()
        {
            InitializeComponent();
            Shell32Library.SHGetKnownFolderPath(new Guid("374DE290-123F-4565-9164-39C4925E467B"), KNOWN_FOLDER_FLAG.KF_FLAG_DEFAULT, IntPtr.Zero, out string downloadFolder);
            DownloadFolderText = downloadFolder;
            IsPrimaryButtonEnabled = !string.IsNullOrEmpty(DownloadLinkText) && !string.IsNullOrEmpty(DownloadFolderText);
        }

        /// <summary>
        /// 获取输入的下载链接
        /// </summary>
        private void OnDownloadLinkTextChanged(object sender, TextChangedEventArgs args)
        {
            DownloadLinkText = (sender as Windows.UI.Xaml.Controls.TextBox).Text;

            IsPrimaryButtonEnabled = !string.IsNullOrEmpty(DownloadLinkText) && !string.IsNullOrEmpty(DownloadFolderText);
        }

        /// <summary>
        /// 获取输入的下载链接
        /// </summary>
        private void OnDownloadFileNameTextChanged(object sender, TextChangedEventArgs args)
        {
            DownloadFileNameText = (sender as Windows.UI.Xaml.Controls.TextBox).Text;

            IsPrimaryButtonEnabled = !string.IsNullOrEmpty(DownloadLinkText) && !string.IsNullOrEmpty(DownloadFileNameText) && !string.IsNullOrEmpty(DownloadFolderText);
        }

        /// <summary>
        /// 获取输入的下载目录
        /// </summary>
        private void OnDownloadFolderTextChanged(object sender, TextChangedEventArgs args)
        {
            DownloadFolderText = (sender as Windows.UI.Xaml.Controls.TextBox).Text;
        }

        /// <summary>
        /// 选择文件夹
        /// </summary>
        private void OnSelectFolderClicked(object sender, RoutedEventArgs args)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = FileProperties.SelectFolder;
            dialog.ShowNewFolderButton = true;
            dialog.RootFolder = Environment.SpecialFolder.Desktop;
            DialogResult result = dialog.ShowDialog();
            if (result is DialogResult.OK || result is DialogResult.Yes)
            {
                DownloadFolderText = dialog.SelectedPath;
            }
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        private void OnDownloadClicked(object sender, ContentDialogButtonClickEventArgs args)
        {
            if (string.IsNullOrEmpty(DownloadFolderText) || string.IsNullOrEmpty(DownloadFileNameText))
            {
                return;
            }

            if (DownloadFileNameText.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || DownloadFolderText.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                return;
            }

            DeliveryOptimizationService.CreateDownload(DownloadLinkText, Path.Combine(DownloadFolderText, DownloadFileNameText));
        }
    }
}
