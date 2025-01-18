using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using WindowsTools.Extensions.DataType.Enums;
using WindowsTools.Helpers.Controls;
using WindowsTools.Models;
using WindowsTools.Services.Controls.Download;
using WindowsTools.Services.Controls.Settings;
using WindowsTools.Services.Root;
using WindowsTools.UI.Dialogs;
using WindowsTools.Views.Windows;
using WindowsTools.WindowsAPI.ComTypes;
using WindowsTools.WindowsAPI.PInvoke.Shell32;

// 抑制 CA1806，CA1822，IDE0060 警告
#pragma warning disable CA1806,CA1822,IDE0060

namespace WindowsTools.Views.Pages
{
    /// <summary>
    /// 下载管理页面
    /// </summary>
    public sealed partial class DownloadManagerPage : Page, INotifyPropertyChanged
    {
        private readonly SynchronizationContext synchronizationContext = SynchronizationContext.Current;
        private bool isAllowClosed = false;

        private string _searchDownloadText;

        public string SearchDownloadText
        {
            get { return _searchDownloadText; }

            set
            {
                if (!Equals(_searchDownloadText, value))
                {
                    _searchDownloadText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchDownloadText)));
                }
            }
        }

        private bool _isSearchEmpty = false;

        public bool IsSearchEmpty
        {
            get { return _isSearchEmpty; }

            set
            {
                if (!Equals(_isSearchEmpty, value))
                {
                    _isSearchEmpty = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSearchEmpty)));
                }
            }
        }

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

        private ObservableCollection<DownloadModel> DownloadCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public DownloadManagerPage()
        {
            InitializeComponent();
            DownloadFolderText = DownloadOptionsService.DownloadFolder;
            IsPrimaryButtonEnabled = !string.IsNullOrEmpty(DownloadLinkText) && !string.IsNullOrEmpty(DownloadFolderText);
            DownloadSchedulerService.DownloadCreated += OnDownloadCreated;
            DownloadSchedulerService.DownloadContinued += OnDownloadContinued;
            DownloadSchedulerService.DownloadPaused += OnDownloadPaused;
            DownloadSchedulerService.DownloadDeleted += OnDownloadDeleted;
            DownloadSchedulerService.DownloadProgressing += OnDownloadProgressing;
            DownloadSchedulerService.DownloadCompleted += OnDownloadCompleted;
        }

        #region 第一部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 继续下载当前任务
        /// </summary>
        private void OnContinueExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            Guid downloadID = (Guid)args.Parameter;

            if (downloadID != Guid.Empty)
            {
                foreach (DownloadModel downloadItem in DownloadCollection)
                {
                    if (downloadItem.DownloadID.Equals(downloadID))
                    {
                        downloadItem.IsNotOperated = false;
                        break;
                    }
                }

                DownloadSchedulerService.ContinueDownload(downloadID);
            }
        }

        /// <summary>
        /// 暂停下载当前任务
        /// </summary>
        private void OnPauseExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            Guid downloadID = (Guid)args.Parameter;

            if (downloadID != Guid.Empty)
            {
                foreach (DownloadModel downloadItem in DownloadCollection)
                {
                    if (downloadItem.DownloadID.Equals(downloadID))
                    {
                        downloadItem.IsNotOperated = false;
                        break;
                    }
                }

                DownloadSchedulerService.PauseDownload(downloadID);
            }
        }

        /// <summary>
        /// 打开下载文件所属目录
        /// </summary>
        private void OnOpenFolderExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string filePath)
            {
                Task.Run(() =>
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(filePath))
                        {
                            if (File.Exists(filePath))
                            {
                                IntPtr pidlList = Shell32Library.ILCreateFromPath(filePath);
                                if (pidlList != IntPtr.Zero)
                                {
                                    Shell32Library.SHOpenFolderAndSelectItems(pidlList, 0, IntPtr.Zero, 0);
                                    Shell32Library.ILFree(pidlList);
                                }
                            }
                            else
                            {
                                string directoryPath = Path.GetDirectoryName(filePath);

                                if (Directory.Exists(directoryPath))
                                {
                                    Process.Start(directoryPath);
                                }
                                else
                                {
                                    Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, "Open download file path failed", e);
                    }
                });
            }
        }

        /// <summary>
        /// 删除当前任务
        /// </summary>
        private void OnDeleteExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is DownloadModel downloadItem && downloadItem.DownloadID != Guid.Empty)
            {
                foreach (DownloadModel item in DownloadCollection)
                {
                    if (downloadItem.DownloadID.Equals(item.DownloadID))
                    {
                        downloadItem.IsNotOperated = false;
                        break;
                    }
                }

                if (downloadItem.DownloadStatus is DownloadStatus.Downloading || downloadItem.DownloadStatus is DownloadStatus.Pause)
                {
                    DownloadSchedulerService.DeleteDownload(downloadItem.DownloadID);
                }
                else
                {
                    foreach (DownloadModel item in DownloadCollection)
                    {
                        if (downloadItem.DownloadID.Equals(item.DownloadID))
                        {
                            DownloadCollection.Remove(item);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 删除下载（包括文件）
        /// </summary>
        private async void OnDeleteWithFileExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is DownloadModel downloadItem)
            {
                bool result = await Task.Run(() =>
                {
                    if (!string.IsNullOrEmpty(downloadItem.FilePath) && File.Exists(downloadItem.FilePath))
                    {
                        try
                        {
                            File.Delete(downloadItem.FilePath);
                            return true;
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Warning, string.Format("Delete file {0} failed", downloadItem.FilePath), e);
                            return false;
                        }
                    }

                    return false;
                });

                if (result)
                {
                    foreach (DownloadModel item in DownloadCollection)
                    {
                        if (downloadItem.DownloadID.Equals(item.DownloadID))
                        {
                            DownloadCollection.Remove(item);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 文件共享
        /// </summary>
        private void OnShareFileExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string filePath && !string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                try
                {
                    IDataTransferManagerInterop dataTransferManagerInterop = (IDataTransferManagerInterop)WindowsRuntimeMarshal.GetActivationFactory(typeof(DataTransferManager));

                    dataTransferManagerInterop.GetForWindow(MainWindow.Current.Handle, new("A5CAEE9B-8708-49D1-8D36-67D25A8DA00C"), out DataTransferManager dataTransferManager);
                    dataTransferManager.DataRequested += async (sender, args) =>
                    {
                        DataRequestDeferral deferral = args.Request.GetDeferral();

                        args.Request.Data.Properties.Title = string.Format(ResourceService.DownloadManagerResource.GetString("ShareFileTitle"));
                        args.Request.Data.SetStorageItems(new List<StorageFile>() { await StorageFile.GetFileFromPathAsync(filePath) });
                        deferral.Complete();
                    };

                    dataTransferManagerInterop.ShowShareUIForWindow(MainWindow.Current.Handle);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Warning, "Share file failed.", e);
                }
            }
        }

        /// <summary>
        /// 查看文件信息
        /// </summary>
        private void OnFileInformationExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string filePath && !string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                SHELLEXECUTEINFO info = new()
                {
                    cbSize = Marshal.SizeOf<SHELLEXECUTEINFO>(),
                    lpVerb = "properties",
                    lpFile = filePath,
                    nShow = 5,
                    fMask = ShellExecuteMaskFlags.SEE_MASK_INVOKEIDLIST,
                    hwnd = IntPtr.Zero
                };
                Shell32Library.ShellExecuteEx(ref info);
            }
        }

        #endregion 第一部分：XamlUICommand 命令调用时挂载的事件

        #region 第二部分：下载管理页面——挂载的事件

        /// <summary>
        /// 搜索文本框内容发生变化的事件
        /// </summary>
        private void OnSearchDownloadTextChanged(object sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            SearchDownloadText = (sender as AutoSuggestBox).Text;

            if (string.IsNullOrEmpty(SearchDownloadText))
            {
                foreach (DownloadModel downloadItem in DownloadCollection)
                {
                    downloadItem.IsVisible = Visibility.Visible;
                }

                IsSearchEmpty = false;
            }
        }

        /// <summary>
        /// 查询搜索内容
        /// </summary>
        private void OnSearchDownloadQuerySubmitted(object sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!string.IsNullOrEmpty(SearchDownloadText))
            {
                foreach (DownloadModel downloadItem in DownloadCollection)
                {
                    downloadItem.IsVisible = downloadItem.FileName.Contains(SearchDownloadText) ? Visibility.Visible : Visibility.Collapsed;
                }

                IsSearchEmpty = DownloadCollection.All(item => item.IsVisible is Visibility.Collapsed);
            }
        }

        /// <summary>
        /// 添加任务
        /// </summary>
        private void OnAddTaskClicked(object sender, RoutedEventArgs args)
        {
            FlyoutShowOptions flyoutShowOptions = new()
            {
                Placement = FlyoutPlacementMode.Full,
                ShowMode = FlyoutShowMode.Standard,
            };
            AddDownloadTaskFlyout.ShowAt(MainWindow.Current.Content, flyoutShowOptions);
        }

        /// <summary>
        /// 开始下载全部任务
        /// </summary>
        private void OnStartDownloadClicked(object sender, RoutedEventArgs args)
        {
            foreach (DownloadModel downloadItem in DownloadCollection)
            {
                if (downloadItem.DownloadStatus is DownloadStatus.Pause)
                {
                    downloadItem.IsNotOperated = false;
                    DownloadSchedulerService.ContinueDownload(downloadItem.DownloadID);
                }
            }
        }

        /// <summary>
        /// 暂停下载全部任务
        /// </summary>
        private void OnPauseDownloadClicked(object sender, RoutedEventArgs args)
        {
            foreach (DownloadModel downloadItem in DownloadCollection)
            {
                if (downloadItem.DownloadStatus is DownloadStatus.Downloading)
                {
                    downloadItem.IsNotOperated = false;
                    DownloadSchedulerService.PauseDownload(downloadItem.DownloadID);
                }
            }
        }

        /// <summary>
        /// 删除全部下载
        /// </summary>
        private void OnDeleteDownloadClicked(object sender, RoutedEventArgs args)
        {
            for (int index = DownloadCollection.Count - 1; index >= 0; index--)
            {
                DownloadModel downloadItem = DownloadCollection[index];
                downloadItem.IsNotOperated = false;

                if (downloadItem.DownloadStatus is DownloadStatus.Downloading || downloadItem.DownloadStatus is DownloadStatus.Pause)
                {
                    DownloadSchedulerService.DeleteDownload(downloadItem.DownloadID);
                }
                else
                {
                    DownloadCollection.RemoveAt(index);
                }
            }
        }

        /// <summary>
        /// 打开默认下载目录
        /// </summary>
        private void OnOpenDefaultDownloadFolderClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Shell32Library.SHGetKnownFolderPath(new("374DE290-123F-4565-9164-39C4925E467B"), KNOWN_FOLDER_FLAG.KF_FLAG_DEFAULT, IntPtr.Zero, out string downloadFolder);
                    Process.Start(downloadFolder);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Open default download folder failed", e);
                }
            });
        }

        /// <summary>
        /// 了解传递优化
        /// </summary>
        private void OnLearnDeliveryOptimizationClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("https://learn.microsoft.com/windows/deployment/do/waas-delivery-optimization");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Open learn delivery optimization url failed", e);
                }
            });
        }

        /// <summary>
        /// 打开传递优化设置
        /// </summary>
        private void OnOpenDeliveryOptimizationSettingsClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("ms-settings:delivery-optimization");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Open delivery optimization settings failed", e);
                }
            });
        }

        /// <summary>
        /// 打开下载设置
        /// </summary>
        private void OnDownloadSettingsClicked(object sender, RoutedEventArgs args)
        {
            (MainWindow.Current.Content as MainPage).NavigateTo(typeof(SettingsPage));
        }

        #endregion 第二部分：下载管理页面——挂载的事件

        #region 第二部分：自定义事件

        /// <summary>
        /// 下载任务已创建
        /// </summary>
        private void OnDownloadCreated(Guid downloadID, DownloadSchedulerModel downloadSchedulerItem)
        {
            synchronizationContext.Post(_ =>
            {
                DownloadCollection.Add(new DownloadModel()
                {
                    DownloadID = downloadID,
                    DownloadStatus = DownloadStatus.Downloading,
                    FileName = downloadSchedulerItem.FileName,
                    FilePath = downloadSchedulerItem.FilePath,
                    FileLink = downloadSchedulerItem.FileLink,
                    FinishedSize = 0,
                    IsNotOperated = true,
                    CurrentSpeed = 0,
                    TotalSize = downloadSchedulerItem.TotalSize
                });
            }, null);
        }

        /// <summary>
        /// 下载任务已继续下载
        /// </summary>
        private void OnDownloadContinued(Guid downloadID)
        {
            synchronizationContext.Post(_ =>
            {
                foreach (DownloadModel downloadItem in DownloadCollection)
                {
                    if (downloadItem.DownloadID.Equals(downloadID))
                    {
                        downloadItem.IsNotOperated = true;
                        downloadItem.DownloadStatus = DownloadStatus.Downloading;
                    }
                }
            }, null);
        }

        /// <summary>
        /// 下载任务已暂停下载
        /// </summary>
        private void OnDownloadPaused(Guid downloadID)
        {
            synchronizationContext.Post(_ =>
            {
                foreach (DownloadModel downloadItem in DownloadCollection)
                {
                    if (downloadItem.DownloadID.Equals(downloadID))
                    {
                        downloadItem.IsNotOperated = true;
                        downloadItem.DownloadStatus = DownloadStatus.Pause;
                    }
                }
            }, null);
        }

        /// <summary>
        /// 下载任务已删除
        /// </summary>
        private void OnDownloadDeleted(Guid downloadID)
        {
            synchronizationContext.Post(_ =>
            {
                foreach (DownloadModel downloadItem in DownloadCollection)
                {
                    if (downloadItem.DownloadID.Equals(downloadID))
                    {
                        DownloadCollection.Remove(downloadItem);
                        break;
                    }
                }
            }, null);
        }

        /// <summary>
        /// 下载任务正在进行中
        /// </summary>
        private void OnDownloadProgressing(Guid downloadID, DownloadSchedulerModel downloadSchedulerItem)
        {
            synchronizationContext.Post(_ =>
            {
                foreach (DownloadModel downloadItem in DownloadCollection)
                {
                    if (downloadItem.DownloadID.Equals(downloadID))
                    {
                        downloadItem.IsNotOperated = true;
                        downloadItem.DownloadStatus = downloadSchedulerItem.DownloadStatus;
                        downloadItem.CurrentSpeed = downloadSchedulerItem.CurrentSpeed;
                        downloadItem.FinishedSize = downloadSchedulerItem.FinishedSize;
                        downloadItem.TotalSize = downloadSchedulerItem.TotalSize;
                        break;
                    }
                }
            }, null);
        }

        /// <summary>
        /// 下载任务已完成
        /// </summary>
        private void OnDownloadCompleted(Guid downloadID, DownloadSchedulerModel downloadSchedulerItem)
        {
            synchronizationContext.Post(_ =>
            {
                foreach (DownloadModel downloadItem in DownloadCollection)
                {
                    if (downloadItem.DownloadID.Equals(downloadID))
                    {
                        downloadItem.IsNotOperated = true;
                        downloadItem.DownloadStatus = downloadSchedulerItem.DownloadStatus;
                        downloadItem.CurrentSpeed = downloadSchedulerItem.CurrentSpeed;
                        downloadItem.FinishedSize = downloadSchedulerItem.FinishedSize;
                        downloadItem.TotalSize = downloadSchedulerItem.TotalSize;
                        break;
                    }
                }
            }, null);
        }

        /// <summary>
        /// 浮出控件打开时触发的事件
        /// </summary>
        private void OnOpening(object sender, object args)
        {
            DownloadLinkText = string.Empty;
            DownloadFileNameText = string.Empty;
            DownloadFolderText = DownloadOptionsService.DownloadFolder;
            IsPrimaryButtonEnabled = !string.IsNullOrEmpty(DownloadLinkText) && !string.IsNullOrEmpty(DownloadFolderText);
        }

        /// <summary>
        /// 浮出控件关闭时触发的事件
        /// </summary>
        private void OnClosing(FlyoutBase sender, FlyoutBaseClosingEventArgs args)
        {
            if (isAllowClosed)
            {
                isAllowClosed = false;
            }
            else
            {
                args.Cancel = true;
            }
        }

        private void OnFlyoutKeyDown(object sender, KeyRoutedEventArgs args)
        {
            if (args.Key is VirtualKey.Escape)
            {
                isAllowClosed = true;
                AddDownloadTaskFlyout.Hide();
            }
        }

        /// <summary>
        /// 获取输入的下载链接
        /// </summary>
        private async void OnDownloadLinkTextChanged(object sender, TextChangedEventArgs args)
        {
            DownloadLinkText = (sender as global::Windows.UI.Xaml.Controls.TextBox).Text;

            if (!string.IsNullOrEmpty(DownloadLinkText))
            {
                string createFileName = await Task.Run(() =>
                {
                    try
                    {
                        bool createSucceeded = Uri.TryCreate(DownloadLinkText, UriKind.Absolute, out Uri uri);
                        if (createSucceeded && uri.Segments.Length >= 1)
                        {
                            string fileName = uri.Segments[uri.Segments.Length - 1];
                            if (fileName is not "/")
                            {
                                return fileName;
                            }
                        }

                        return string.Empty;
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Warning, "Parse download link file name failed", e);
                        return string.Empty;
                    }
                });

                if (!string.IsNullOrEmpty(createFileName))
                {
                    DownloadFileNameText = createFileName;
                    IsPrimaryButtonEnabled = !string.IsNullOrEmpty(DownloadLinkText) && !string.IsNullOrEmpty(DownloadFolderText);
                }
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
            OpenFolderDialog dialog = new()
            {
                Description = ResourceService.DownloadManagerResource.GetString("SelectFolder"),
                RootFolder = Environment.SpecialFolder.Desktop
            };
            DialogResult result = dialog.ShowDialog();
            if (result is DialogResult.OK || result is DialogResult.Yes)
            {
                DownloadFolderText = dialog.SelectedPath;
            }
            dialog.Dispose();
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        private void OnDownloadClicked(object sender, RoutedEventArgs args)
        {
            // 检查文件路径
            if (DownloadFileNameText.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || DownloadFolderText.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                return;
            }

            string filePath = Path.Combine(DownloadFolderText, DownloadFileNameText);

            // 检查本地文件是否存在
            if (File.Exists(filePath))
            {
                synchronizationContext.Post(async (_) =>
                {
                    await ContentDialogHelper.ShowAsync(new FileCheckDialog(DownloadLinkText, filePath), MainWindow.Current.Content as FrameworkElement);
                }, null);
            }
            else
            {
                DownloadSchedulerService.CreateDownload(DownloadLinkText, Path.Combine(DownloadFolderText, DownloadFileNameText));
            }

            isAllowClosed = true;
            AddDownloadTaskFlyout.Hide();
        }

        /// <summary>
        /// 关闭对话框
        /// </summary>
        private void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            isAllowClosed = true;
            AddDownloadTaskFlyout.Hide();
        }

        #endregion 第二部分：自定义事件
    }
}
