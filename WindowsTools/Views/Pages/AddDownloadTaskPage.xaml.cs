using System;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using WindowsTools.Helpers.Controls;
using WindowsTools.Services.Controls.Download;
using WindowsTools.Services.Controls.Settings;
using WindowsTools.Services.Root;
using WindowsTools.Strings;
using WindowsTools.UI.Dialogs;
using WindowsTools.Views.Windows;

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

        private SolidColorBrush _dialogBackground;

        public SolidColorBrush DialogBackground
        {
            get { return _dialogBackground; }

            set
            {
                if (!Equals(_dialogBackground, value))
                {
                    _dialogBackground = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DialogBackground)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public AddDownloadTaskPage()
        {
            InitializeComponent();
            DownloadFolderText = DownloadOptionsService.DownloadFolder;
            RequestedTheme = (ElementTheme)Enum.Parse(typeof(ElementTheme), ThemeService.AppTheme.Value.ToString());
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
                        bool createSucceeded = Uri.TryCreate(DownloadLinkText, UriKind.Absolute, out Uri uri);
                        if (createSucceeded && uri.Segments.Length >= 1)
                        {
                            string fileName = uri.Segments[uri.Segments.Length - 1];
                            if (fileName is not "/")
                            {
                                AddDownloadTaskWindow.Current?.BeginInvoke(() =>
                                {
                                    DownloadFileNameText = fileName;
                                    IsPrimaryButtonEnabled = !string.IsNullOrEmpty(DownloadLinkText) && !string.IsNullOrEmpty(DownloadFolderText);
                                });
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Warning, "Parse download link file name failed", e);
                    }
                });
            }
            else
            {
                DownloadFileNameText = string.Empty;
                IsPrimaryButtonEnabled = !string.IsNullOrEmpty(DownloadLinkText) && !string.IsNullOrEmpty(DownloadFolderText);
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
            FolderBrowserDialog dialog = new()
            {
                Description = FileProperties.SelectFolder,
                ShowNewFolderButton = true,
                RootFolder = Environment.SpecialFolder.Desktop
            };
            DialogResult result = dialog.ShowDialog();
            if (result is DialogResult.OK || result is DialogResult.Yes)
            {
                DownloadFolderText = dialog.SelectedPath;
            }
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        private async void OnDownloadClicked(object sender, RoutedEventArgs args)
        {
            // 检查文件路径
            if (DownloadFileNameText.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || DownloadFolderText.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                FileNameInvalidTeachingTip.IsOpen = true;
                await Task.Delay(2000);
                FileNameInvalidTeachingTip.IsOpen = false;
                return;
            }

            string filePath = Path.Combine(DownloadFolderText, DownloadFileNameText);

            // 检查本地文件是否存在
            if (File.Exists(filePath))
            {
                MainWindow.Current.BeginInvoke(async () =>
                {
                    await ContentDialogHelper.ShowAsync(new FileCheckDialog(DownloadLinkText, filePath), MainWindow.Current.Content as FrameworkElement);
                });
                AddDownloadTaskWindow.Current?.Close();
            }
            else
            {
                DownloadSchedulerService.CreateDownload(DownloadLinkText, Path.Combine(DownloadFolderText, DownloadFileNameText));
                AddDownloadTaskWindow.Current?.Close();
            }
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
