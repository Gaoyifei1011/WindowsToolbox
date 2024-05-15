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
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using WindowsTools.Extensions.DataType.Enums;
using WindowsTools.Models;
using WindowsTools.Models.Controls.Download;
using WindowsTools.Services.Controls.Download;
using WindowsTools.Services.Root;
using WindowsTools.Strings;
using WindowsTools.Views.Windows;
using WindowsTools.WindowsAPI.ComTypes;
using WindowsTools.WindowsAPI.PInvoke.Shell32;

namespace WindowsTools.Views.Pages
{
    /// <summary>
    /// 下载管理页面
    /// </summary>
    public sealed partial class DownloadManagerPage : Page, INotifyPropertyChanged
    {
        private readonly object downloadLock = new object();

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

        private ObservableCollection<DownloadModel> DownloadCollection { get; } = new ObservableCollection<DownloadModel>();

        public event PropertyChangedEventHandler PropertyChanged;

        public DownloadManagerPage()
        {
            InitializeComponent();
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
                lock (downloadLock)
                {
                    foreach (DownloadModel downloadItem in DownloadCollection)
                    {
                        if (downloadItem.DownloadID.Equals(downloadID))
                        {
                            downloadItem.IsNotOperated = false;
                            break;
                        }
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
                lock (downloadLock)
                {
                    foreach (DownloadModel downloadItem in DownloadCollection)
                    {
                        if (downloadItem.DownloadID.Equals(downloadID))
                        {
                            downloadItem.IsNotOperated = false;
                            break;
                        }
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
            string filePath = args.Parameter as string;

            Task.Run(() =>
            {
                if (!string.IsNullOrEmpty(filePath))
                {
                    if (File.Exists(filePath))
                    {
                        IntPtr pidlList = Shell32Library.ILCreateFromPath(filePath);
                        if (pidlList != IntPtr.Zero)
                        {
                            Shell32Library.SHOpenFolderAndSelectItems(pidlList, 0, IntPtr.Zero, 0);
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
            });
        }

        /// <summary>
        /// 删除当前任务
        /// </summary>
        private void OnDeleteExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            DownloadModel downloadItem = args.Parameter as DownloadModel;

            if (downloadItem.DownloadID != Guid.Empty)
            {
                lock (downloadLock)
                {
                    foreach (DownloadModel item in DownloadCollection)
                    {
                        if (downloadItem.DownloadID.Equals(item.DownloadID))
                        {
                            downloadItem.IsNotOperated = false;
                            break;
                        }
                    }
                }

                if (downloadItem.DownloadStatus is DownloadStatus.Downloading || downloadItem.DownloadStatus is DownloadStatus.Pause)
                {
                    DownloadSchedulerService.DeleteDownload(downloadItem.DownloadID);
                }
                else
                {
                    lock (downloadLock)
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
        }

        /// <summary>
        /// 删除下载（包括文件）
        /// </summary>
        private void OnDeleteWithFileExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            DownloadModel downloadItem = args.Parameter as DownloadModel;

            Task.Run(() =>
            {
                if (!string.IsNullOrEmpty(downloadItem.FilePath))
                {
                    if (File.Exists(downloadItem.FilePath))
                    {
                        try
                        {
                            File.Delete(downloadItem.FilePath);
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Warning, string.Format("Delete file {0} failed", downloadItem.FilePath), e);
                        }
                    }

                    MainWindow.Current.BeginInvoke(() =>
                    {
                        lock (downloadLock)
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
                    });
                }
            });
        }

        /// <summary>
        /// 文件共享
        /// </summary>
        private void OnShareFileExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            string filePath = args.Parameter as string;

            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                try
                {
                    IDataTransferManagerInterop dataTransferManagerInterop = (IDataTransferManagerInterop)WindowsRuntimeMarshal.GetActivationFactory(typeof(DataTransferManager));

                    DataTransferManager dataTransferManager = dataTransferManagerInterop.GetForWindow((IntPtr)MainWindow.Current.AppWindow.Id.Value, new Guid("A5CAEE9B-8708-49D1-8D36-67D25A8DA00C"));

                    dataTransferManager.DataRequested += async (sender, args) =>
                    {
                        DataRequestDeferral deferral = args.Request.GetDeferral();

                        args.Request.Data.Properties.Title = string.Format(DownloadManager.ShareFileTitle);
                        args.Request.Data.SetStorageItems(new List<StorageFile>() { await StorageFile.GetFileFromPathAsync(filePath) });
                        deferral.Complete();
                    };

                    dataTransferManagerInterop.ShowShareUIForWindow((IntPtr)MainWindow.Current.AppWindow.Id.Value);
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
            string filePath = args.Parameter as string;

            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                SHELLEXECUTEINFO info = new SHELLEXECUTEINFO();
                info.cbSize = Marshal.SizeOf<SHELLEXECUTEINFO>();
                info.lpVerb = "properties";
                info.lpFile = filePath;
                info.nShow = 5;
                info.fMask = 0x50c;
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
            SearchDownload = (sender as AutoSuggestBox).Text;

            if (string.IsNullOrEmpty(SearchDownload))
            {
                lock (downloadLock)
                {
                    foreach (DownloadModel downloadItem in DownloadCollection)
                    {
                        downloadItem.IsVisible = Visibility.Visible;
                    }
                }

                IsSearchEmpty = false;
            }
        }

        /// <summary>
        /// 查询搜索内容
        /// </summary>
        private void OnSearchDownloadQuerySubmitted(object sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!string.IsNullOrEmpty(SearchDownload))
            {
                lock (downloadLock)
                {
                    foreach (DownloadModel downloadItem in DownloadCollection)
                    {
                        downloadItem.IsVisible = downloadItem.FileName.Contains(SearchDownload) ? Visibility.Visible : Visibility.Collapsed;
                    }

                    IsSearchEmpty = DownloadCollection.All(item => item.IsVisible is Visibility.Collapsed);
                }
            }
        }

        /// <summary>
        /// 添加任务
        /// </summary>
        private void OnAddTaskClicked(object sender, RoutedEventArgs args)
        {
            if (AddDownloadTaskWindow.Current is null)
            {
                new AddDownloadTaskWindow();
                AddDownloadTaskWindow.Current.Left = MainWindow.Current.Left + (MainWindow.Current.Width - AddDownloadTaskWindow.Current.Width) / 2;
                AddDownloadTaskWindow.Current.Top = MainWindow.Current.Top + (MainWindow.Current.Height - AddDownloadTaskWindow.Current.Height) / 2;
                AddDownloadTaskWindow.Current.Show(MainWindow.Current);
            }
        }

        /// <summary>
        /// 开始下载全部任务
        /// </summary>
        private void OnStartDownloadClicked(object sender, RoutedEventArgs args)
        {
            lock (downloadLock)
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
        }

        /// <summary>
        /// 暂停下载全部任务
        /// </summary>
        private void OnPauseDownloadClicked(object sender, RoutedEventArgs args)
        {
            lock (downloadLock)
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
        }

        /// <summary>
        /// 删除全部下载
        /// </summary>
        private void OnDeleteDownloadClicked(object sender, RoutedEventArgs args)
        {
            lock (downloadLock)
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

        #endregion 第二部分：下载管理页面——挂载的事件

        #region 第二部分：自定义事件

        /// <summary>
        /// 下载任务已创建
        /// </summary>
        private void OnDownloadCreated(Guid downloadID, DownloadSchedulerModel downloadSchedulerItem)
        {
            MainWindow.Current.BeginInvoke(() =>
            {
                lock (downloadLock)
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
                }
            });
        }

        /// <summary>
        /// 下载任务已继续下载
        /// </summary>
        private void OnDownloadContinued(Guid downloadID)
        {
            MainWindow.Current.BeginInvoke(() =>
            {
                lock (downloadLock)
                {
                    foreach (DownloadModel downloadItem in DownloadCollection)
                    {
                        if (downloadItem.DownloadID.Equals(downloadID))
                        {
                            downloadItem.IsNotOperated = true;
                            downloadItem.DownloadStatus = DownloadStatus.Downloading;
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 下载任务已暂停下载
        /// </summary>
        private void OnDownloadPaused(Guid downloadID)
        {
            MainWindow.Current.BeginInvoke(() =>
            {
                lock (downloadLock)
                {
                    foreach (DownloadModel downloadItem in DownloadCollection)
                    {
                        if (downloadItem.DownloadID.Equals(downloadID))
                        {
                            downloadItem.IsNotOperated = true;
                            downloadItem.DownloadStatus = DownloadStatus.Pause;
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 下载任务已删除
        /// </summary>
        private void OnDownloadDeleted(Guid downloadID)
        {
            MainWindow.Current.BeginInvoke(() =>
            {
                lock (downloadLock)
                {
                    foreach (DownloadModel downloadItem in DownloadCollection)
                    {
                        if (downloadItem.DownloadID.Equals(downloadID))
                        {
                            DownloadCollection.Remove(downloadItem);
                            break;
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 下载任务正在进行中
        /// </summary>
        private void OnDownloadProgressing(Guid downloadID, DownloadSchedulerModel downloadSchedulerItem)
        {
            MainWindow.Current.BeginInvoke(() =>
            {
                lock (downloadLock)
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
                }
            });
        }

        /// <summary>
        /// 下载任务已完成
        /// </summary>
        private void OnDownloadCompleted(Guid downloadID, DownloadSchedulerModel downloadSchedulerItem)
        {
            MainWindow.Current.BeginInvoke(() =>
            {
                lock (downloadLock)
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
                }
            });
        }

        #endregion 第二部分：自定义事件
    }
}
