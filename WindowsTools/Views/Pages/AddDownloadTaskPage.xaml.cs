using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WindowsTools.Services.Controls.Pages;
using WindowsTools.Services.Root;
using WindowsTools.Strings;
using WindowsTools.Views.Windows;
using WindowsTools.WindowsAPI.PInvoke.Shell32;

namespace WindowsTools.Views.Pages
{
    /// <summary>
    /// 添加下载任务页面
    /// </summary>
    public sealed partial class AddDownloadTaskPage : Page, INotifyPropertyChanged
    {
        private bool _isPrimaryButtonEnabled;

        public bool IsPrimaryButtonEnabled
        {
            get { return _isPrimaryButtonEnabled; }

            set
            {
                if (!Equals(_isPrimaryButtonEnabled, value))
                {
                    _isPrimaryButtonEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPrimaryButtonEnabled)));
                }
            }
        }

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

        public AddDownloadTaskPage()
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
            DownloadLinkText = (sender as global::Windows.UI.Xaml.Controls.TextBox).Text;

            if (!string.IsNullOrEmpty(DownloadLinkText))
            {
                Task.Run(() =>
                {
                    try
                    {
                        string filename = HttpUtility.UrlDecode(new Uri(DownloadLinkText).Segments.Last());

                        if (!string.IsNullOrEmpty(filename))
                        {
                            AddDownloadTaskWindow.Current?.BeginInvoke(() =>
                            {
                                DownloadFileNameText = filename;
                                IsPrimaryButtonEnabled = !string.IsNullOrEmpty(DownloadLinkText) && !string.IsNullOrEmpty(DownloadFolderText);
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(System.Diagnostics.Tracing.EventLevel.Warning, "Parse download link file name failed", e);
                    }
                });
            }
        }

        /// <summary>
        /// 获取输入的下载链接
        /// </summary>
        private void OnDownloadFileNameTextChanged(object sender, TextChangedEventArgs args)
        {
            DownloadFileNameText = (sender as global::Windows.UI.Xaml.Controls.TextBox).Text;

            IsPrimaryButtonEnabled = !string.IsNullOrEmpty(DownloadLinkText) && !string.IsNullOrEmpty(DownloadFileNameText) && !string.IsNullOrEmpty(DownloadFolderText);
        }

        /// <summary>
        /// 获取输入的下载目录
        /// </summary>
        private void OnDownloadFolderTextChanged(object sender, TextChangedEventArgs args)
        {
            DownloadFolderText = (sender as global::Windows.UI.Xaml.Controls.TextBox).Text;
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
        private void OnDownloadClicked(object sender, RoutedEventArgs args)
        {
            if (DownloadFileNameText.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || DownloadFolderText.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                AddDownloadTaskWindow.Current?.Close();
            }

            DeliveryOptimizationService.CreateDownload(DownloadLinkText, Path.Combine(DownloadFolderText, DownloadFileNameText));
            AddDownloadTaskWindow.Current?.Close();
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        private void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            AddDownloadTaskWindow.Current?.Close();
        }
    }
}
