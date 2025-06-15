using PowerTools.Extensions.DataType.Enums;
using PowerTools.Helpers.Root;
using PowerTools.Models;
using PowerTools.Services.Download;
using PowerTools.Services.Root;
using PowerTools.Services.Settings;
using PowerTools.Views.Dialogs;
using PowerTools.Views.Windows;
using PowerTools.WindowsAPI.ComTypes;
using PowerTools.WindowsAPI.PInvoke.Shell32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// 抑制 CA1806，CA1822，IDE0060 警告
#pragma warning disable CA1806,CA1822,IDE0060

namespace PowerTools.Views.Pages
{
    /// <summary>
    /// 下载管理页面
    /// </summary>
    public sealed partial class DownloadManagerPage : Page, INotifyPropertyChanged
    {
        private readonly string DownloadingCountInfoString = ResourceService.DownloadManagerResource.GetString("DownloadingCountInfo");
        private readonly string FileShareString = ResourceService.DownloadManagerResource.GetString("FileShare");
        private readonly string SelectFolderString = ResourceService.DownloadManagerResource.GetString("SelectFolder");
        private readonly SynchronizationContext synchronizationContext = SynchronizationContext.Current;
        private bool isInitialized = false;
        private bool isAllowClosed = false;

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
                if (!string.Equals(_downloadLinkText, value))
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
                if (!string.Equals(_downloadFileNameText, value))
                {
                    _downloadFileNameText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DownloadFileNameText)));
                }
            }
        }

        private string _downloadFolderText = DownloadOptionsService.DownloadFolder;

        public string DownloadFolderText
        {
            get { return _downloadFolderText; }

            set
            {
                if (!string.Equals(_downloadFolderText, value))
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
            IsPrimaryButtonEnabled = !string.IsNullOrEmpty(DownloadLinkText) && !string.IsNullOrEmpty(DownloadFolderText);
        }

        #region 第一部分：重载父类事件

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (!isInitialized)
            {
                isInitialized = true;

                await Task.Run(() =>
                {
                    // TODO：添加全局卸载事件
                    DownloadSchedulerService.DownloadProgress += OnDownloadProgress;
                });
            }
        }

        #endregion 第一部分：重载父类事件

        #region 第一部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 继续下载当前任务
        /// </summary>
        private void OnContinueExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is DownloadModel download && !string.IsNullOrEmpty(download.DownloadID))
            {
                download.IsOperating = true;
                DownloadSchedulerService.ContinueDownload(download.DownloadID);
            }
        }

        /// <summary>
        /// 暂停下载当前任务
        /// </summary>
        private void OnPauseExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is DownloadModel download && !string.IsNullOrEmpty(download.DownloadID))
            {
                download.IsOperating = true;
                DownloadSchedulerService.PauseDownload(download.DownloadID);
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
            if (args.Parameter is DownloadModel download && !string.IsNullOrEmpty(download.DownloadID))
            {
                if (download.DownloadProgressState is DownloadProgressState.Queued || download.DownloadProgressState is DownloadProgressState.Downloading || download.DownloadProgressState is DownloadProgressState.Paused)
                {
                    DownloadSchedulerService.DeleteDownload(download.DownloadID);
                }
                else
                {
                    DownloadCollection.Remove(download);
                }
            }
        }

        /// <summary>
        /// 删除下载（包括文件）
        /// </summary>
        private async void OnDeleteWithFileExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is DownloadModel download && !string.IsNullOrEmpty(download.DownloadID))
            {
                if (download.DownloadProgressState is DownloadProgressState.Queued || download.DownloadProgressState is DownloadProgressState.Downloading || download.DownloadProgressState is DownloadProgressState.Paused)
                {
                    DownloadSchedulerService.DeleteDownload(download.DownloadID);
                }
                else if (download.DownloadProgressState is DownloadProgressState.Finished)
                {
                    download.IsOperating = true;
                    (bool result, Exception exception) = await Task.Run(() =>
                    {
                        // 删除文件
                        try
                        {
                            if (File.Exists(download.FilePath))
                            {
                                File.Delete(download.FilePath);
                            }

                            return ValueTuple.Create<bool, Exception>(true, null);
                        }
                        catch (Exception e)
                        {
                            return ValueTuple.Create(false, e);
                        }
                    });

                    if (result)
                    {
                        DownloadCollection.Remove(download);
                    }
                    else
                    {
                        download.IsOperating = false;
                        // TODO：显示删除失败通知
                    }
                }
                else
                {
                    DownloadCollection.Remove(download);
                }
            }
        }

        /// <summary>
        /// 文件共享
        /// </summary>
        private async void OnShareFileExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string filePath && File.Exists(filePath))
            {
                if (DataTransferManager.IsSupported())
                {
                    try
                    {
                        List<StorageFile> fileList = [await StorageFile.GetFileFromPathAsync(filePath)];
                        IDataTransferManagerInterop dataTransferManagerInterop = (IDataTransferManagerInterop)WindowsRuntimeMarshal.GetActivationFactory(typeof(DataTransferManager));
                        dataTransferManagerInterop.GetForWindow(MainWindow.Current.Handle, new("A5CAEE9B-8708-49D1-8D36-67D25A8DA00C"), out DataTransferManager dataTransferManager);
                        dataTransferManager.DataRequested += (sender, args) => OnDataRequested(sender, args, fileList);
                        dataTransferManagerInterop.ShowShareUIForWindow(MainWindow.Current.Handle);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Warning, "Share file failed.", e);
                    }
                }
            }
            else
            {
                // TODO：显示文件失踪通知
                //await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.FileLost));
            }
        }

        /// <summary>
        /// 查看文件信息
        /// </summary>
        private void OnFileInformationExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string filePath && File.Exists(filePath))
            {
                Task.Run(() =>
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
                });
            }
        }

        #endregion 第一部分：XamlUICommand 命令调用时挂载的事件

        #region 第二部分：下载管理页面——挂载的事件

        /// <summary>
        /// 添加任务
        /// </summary>
        private void OnAddTaskClicked(object sender, RoutedEventArgs args)
        {
            AddDownloadTaskFlyout.ShowAt(MainWindow.Current.Content, new()
            {
                Placement = FlyoutPlacementMode.Full,
                ShowMode = FlyoutShowMode.Standard,
            });
        }

        /// <summary>
        /// 继续下载全部任务
        /// </summary>
        private void OnContinueAllClicked(object sender, RoutedEventArgs args)
        {
            foreach (DownloadModel downloadItem in DownloadCollection)
            {
                if (downloadItem.DownloadProgressState is DownloadProgressState.Paused)
                {
                    downloadItem.IsOperating = true;
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
                if (downloadItem.DownloadProgressState is DownloadProgressState.Queued || downloadItem.DownloadProgressState is DownloadProgressState.Downloading)
                {
                    downloadItem.IsOperating = true;
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
                downloadItem.IsOperating = true;

                if (downloadItem.DownloadProgressState is DownloadProgressState.Queued || downloadItem.DownloadProgressState is DownloadProgressState.Downloading || downloadItem.DownloadProgressState is DownloadProgressState.Paused)
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
        /// 打开默认保存的文件夹
        /// </summary>
        private void OnOpenFolderClicked(object sender, RoutedEventArgs args)
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

        /// <summary>
        /// 浮出控件接受屏幕按键触发的事件
        /// </summary>
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
            if (sender is global::Windows.UI.Xaml.Controls.TextBox textBox)
            {
                DownloadLinkText = textBox.Text;

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
        }

        /// <summary>
        /// 获取输入的下载链接
        /// </summary>
        private void OnDownloadFileNameTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is global::Windows.UI.Xaml.Controls.TextBox textBox)
            {
                DownloadLinkText = textBox.Text;
                DownloadFileNameText = (sender as global::Windows.UI.Xaml.Controls.TextBox).Text;
                IsPrimaryButtonEnabled = !string.IsNullOrEmpty(DownloadLinkText) && !string.IsNullOrEmpty(DownloadFileNameText) && !string.IsNullOrEmpty(DownloadFolderText);
            }
        }

        /// <summary>
        /// 获取输入的下载目录
        /// </summary>
        private void OnDownloadFolderTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is global::Windows.UI.Xaml.Controls.TextBox textBox)
            {
                DownloadLinkText = textBox.Text;
            }
        }

        /// <summary>
        /// 选择文件夹
        /// </summary>
        private void OnSelectFolderClicked(object sender, RoutedEventArgs args)
        {
            OpenFolderDialog openFolderDialog = new()
            {
                Description = SelectFolderString,
                RootFolder = Environment.SpecialFolder.Desktop
            };
            DialogResult dialogResult = openFolderDialog.ShowDialog();
            if (dialogResult is DialogResult.OK || dialogResult is DialogResult.Yes)
            {
                DownloadFolderText = openFolderDialog.SelectedPath;
            }
            openFolderDialog.Dispose();
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        private void OnDownloadClicked(object sender, RoutedEventArgs args)
        {
            isAllowClosed = true;
            AddDownloadTaskFlyout.Hide();

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
                    await MainWindow.Current.ShowDialogAsync(new FileCheckDialog(DownloadLinkText, filePath));
                }, null);
            }
            else
            {
                DownloadSchedulerService.CreateDownload(DownloadLinkText, Path.Combine(DownloadFolderText, DownloadFileNameText));
            }
        }

        /// <summary>
        /// 关闭对话框或使用说明
        /// </summary>
        private void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            if (sender is global::Windows.UI.Xaml.Controls.Button button)
            {
                if (button.Tag is Flyout flyout)
                {
                    isAllowClosed = true;
                    flyout.Hide();
                }
                else if (button.Tag is SplitView splitView && splitView.IsPaneOpen)
                {
                    splitView.IsPaneOpen = false;
                }
            }
        }

        /// <summary>
        /// 疑难解答
        /// </summary>
        private void OnTroubleShootClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            if (RuntimeHelper.IsElevated)
            {
                Task.Run(() =>
                {
                    try
                    {
                        Process.Start("ms-settings:troubleshoot");
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, "Open trouble shoot failed", e);
                    }
                });
            }
        }

        #endregion 第二部分：下载管理页面——挂载的事件

        #region 第三部分：下载管理页面——自定义事件

        /// <summary>
        /// 下载状态发生改变时触发的事件
        /// </summary>
        private void OnDownloadProgress(DownloadSchedulerModel downloadScheduler)
        {
            // 处于等待中（新添加下载任务或者已经恢复下载）
            if (downloadScheduler.DownloadProgressState is DownloadProgressState.Queued)
            {
                synchronizationContext.Post((_) =>
                {
                    // 下载任务已经存在，更新下载状态
                    foreach (DownloadModel downloadItem in DownloadCollection)
                    {
                        if (string.Equals(downloadItem.DownloadID, downloadScheduler.DownloadID))
                        {
                            downloadItem.IsOperating = false;
                            downloadItem.DownloadProgressState = downloadScheduler.DownloadProgressState;
                            return;
                        }
                    }

                    // 不存在则添加任务
                    DownloadModel download = new()
                    {
                        IsOperating = false,
                        DownloadID = downloadScheduler.DownloadID,
                        FileName = downloadScheduler.FileName,
                        FilePath = downloadScheduler.FilePath,
                        DownloadProgressState = downloadScheduler.DownloadProgressState,
                        CompletedSize = downloadScheduler.CompletedSize,
                        TotalSize = downloadScheduler.TotalSize,
                        DownloadSpeed = downloadScheduler.DownloadSpeed
                    };

                    DownloadCollection.Add(download);
                }, null);
            }
            // 下载任务正在下载中
            else if (downloadScheduler.DownloadProgressState is DownloadProgressState.Downloading)
            {
                synchronizationContext.Post((_) =>
                {
                    foreach (DownloadModel downloadItem in DownloadCollection)
                    {
                        if (string.Equals(downloadItem.DownloadID, downloadScheduler.DownloadID))
                        {
                            downloadItem.DownloadProgressState = downloadScheduler.DownloadProgressState;
                            downloadItem.DownloadSpeed = downloadScheduler.DownloadSpeed;
                            downloadItem.CompletedSize = downloadScheduler.CompletedSize;
                            downloadItem.TotalSize = downloadScheduler.TotalSize;
                            return;
                        }
                    }
                }, null);
            }
            // 下载任务已暂停或已失败
            else if (downloadScheduler.DownloadProgressState is DownloadProgressState.Paused || downloadScheduler.DownloadProgressState is DownloadProgressState.Failed)
            {
                synchronizationContext.Post((_) =>
                {
                    foreach (DownloadModel downloadItem in DownloadCollection)
                    {
                        if (string.Equals(downloadItem.DownloadID, downloadScheduler.DownloadID))
                        {
                            downloadItem.IsOperating = false;
                            downloadItem.DownloadProgressState = downloadScheduler.DownloadProgressState;
                            return;
                        }
                    }
                }, null);
            }
            // 下载任务已完成
            else if (downloadScheduler.DownloadProgressState is DownloadProgressState.Finished)
            {
                MemoryStream memoryStream = null;
                try
                {
                    Bitmap thumbnailBitmap = GetThumbnailBitmap(downloadScheduler.FilePath);

                    if (thumbnailBitmap is not null)
                    {
                        memoryStream = new();
                        thumbnailBitmap.Save(memoryStream, ImageFormat.Png);
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        thumbnailBitmap.Dispose();
                    }
                }
                catch (Exception)
                {
                    // TODO：未完成，添加异常处理
                }

                synchronizationContext.Post((_) =>
                {
                    foreach (DownloadModel downloadItem in DownloadCollection)
                    {
                        if (string.Equals(downloadItem.DownloadID, downloadScheduler.DownloadID))
                        {
                            downloadItem.DownloadProgressState = downloadScheduler.DownloadProgressState;
                            downloadItem.DownloadSpeed = downloadScheduler.DownloadSpeed;
                            downloadItem.CompletedSize = downloadScheduler.CompletedSize;
                            downloadItem.TotalSize = downloadScheduler.TotalSize;
                            downloadItem.DownloadProgressState = downloadScheduler.DownloadProgressState;

                            if (memoryStream is not null)
                            {
                                try
                                {
                                    BitmapImage bitmapImage = new();
                                    bitmapImage.SetSource(memoryStream.AsRandomAccessStream());
                                }
                                catch (Exception)
                                {
                                    // TODO：未完成，添加异常处理
                                }
                                finally
                                {
                                    memoryStream?.Dispose();
                                }
                            }

                            return;
                        }
                    }
                }, null);
            }
            // 下载任务已删除
            else if (downloadScheduler.DownloadProgressState is DownloadProgressState.Deleted)
            {
                synchronizationContext.Post((_) =>
                {
                    foreach (DownloadModel downloadItem in DownloadCollection)
                    {
                        if (string.Equals(downloadItem.DownloadID, downloadScheduler.DownloadID))
                        {
                            DownloadCollection.Remove(downloadItem);
                            return;
                        }
                    }
                }, null);
            }
        }

        /// <summary>
        /// 在共享操作启动时发生的事件
        /// </summary>
        private void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs args, List<StorageFile> fileList)
        {
            DataRequestDeferral dataRequestDeferral = args.Request.GetDeferral();

            try
            {
                args.Request.Data.Properties.Title = FileShareString;
                args.Request.Data.SetStorageItems(fileList);
            }
            catch (Exception)
            {
                return;
            }
            finally
            {
                dataRequestDeferral.Complete();
            }
        }

        #endregion 第三部分：下载管理页面——自定义事件

        /// <summary>
        /// 获取文件缩略图
        /// </summary>
        private Bitmap GetThumbnailBitmap(string filePath)
        {
            try
            {
                int result = Shell32Library.SHCreateItemFromParsingName(filePath, null, typeof(IShellItem).GUID, out IShellItem shellItem);

                if (result is 0)
                {
                    result = ((IShellItemImageFactory)shellItem).GetImage(new Size(256, 256), SIIGBF.SIIGBF_RESIZETOFIT, out IntPtr hBitmap);
                    Marshal.ReleaseComObject(shellItem);

                    if (result is 0)
                    {
                        Bitmap bitmap = System.Drawing.Image.FromHbitmap(hBitmap);

                        if (System.Drawing.Image.GetPixelFormatSize(bitmap.PixelFormat) < 32)
                        {
                            return bitmap;
                        }
                        else
                        {
                            return CreateAlphaBitmap(bitmap, PixelFormat.Format32bppArgb);
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 创建 Alpha 的 Bitmap
        /// </summary>
        private static Bitmap CreateAlphaBitmap(Bitmap srcBitmap, PixelFormat targetPixelFormat)
        {
            Bitmap bitmap = new(srcBitmap.Width, srcBitmap.Height, targetPixelFormat);
            Rectangle bitmapBound = new(0, 0, srcBitmap.Width, srcBitmap.Height);
            BitmapData srcData = srcBitmap.LockBits(bitmapBound, ImageLockMode.ReadOnly, srcBitmap.PixelFormat);
            bool isAlplaBitmap = false;

            try
            {
                for (int y = 0; y <= srcData.Height - 1; y++)
                {
                    for (int x = 0; x <= srcData.Width - 1; x++)
                    {
                        Color pixelColor = Color.FromArgb(
                            Marshal.ReadInt32(srcData.Scan0, (srcData.Stride * y) + (4 * x)));

                        if (pixelColor.A > 0 & pixelColor.A < 255)
                        {
                            isAlplaBitmap = true;
                        }

                        bitmap.SetPixel(x, y, pixelColor);
                    }
                }
            }
            finally
            {
                srcBitmap.UnlockBits(srcData);
            }

            return isAlplaBitmap ? bitmap : srcBitmap;
        }
    }
}
