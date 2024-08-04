using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using WindowsTools.Extensions.DataType.Enums;
using WindowsTools.Helpers.Controls;
using WindowsTools.Helpers.Controls.Extensions;
using WindowsTools.Helpers.Root;
using WindowsTools.Models;
using WindowsTools.Services.Root;
using WindowsTools.Strings;
using WindowsTools.UI.Dialogs;
using WindowsTools.UI.TeachingTips;
using WindowsTools.WindowsAPI.PInvoke.Imagehlp;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace WindowsTools.Views.Pages
{
    /// <summary>
    /// 数字签名页面
    /// </summary>
    public sealed partial class FileCertificatePage : Page, INotifyPropertyChanged
    {
        private readonly SynchronizationContext synchronizationContext = SynchronizationContext.Current;
        private readonly object fileCertificateLock = new();

        private bool _isModifyingNow = false;

        public bool IsModifyingNow
        {
            get { return _isModifyingNow; }

            set
            {
                if (!Equals(_isModifyingNow, value))
                {
                    _isModifyingNow = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsModifyingNow)));
                }
            }
        }

        private ObservableCollection<CertificateResultModel> FileCertificateCollection { get; } = [];

        private ObservableCollection<OperationFailedModel> OperationFailedCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public FileCertificatePage()
        {
            InitializeComponent();
        }

        #region 第一部分：重写父类事件

        /// <summary>
        /// 设置拖动的数据的可视表示形式
        /// </summary>
        protected override void OnDragOver(global::Windows.UI.Xaml.DragEventArgs args)
        {
            base.OnDragOver(args);
            args.AcceptedOperation = DataPackageOperation.Copy;
            args.DragUIOverride.IsCaptionVisible = true;
            args.DragUIOverride.IsContentVisible = false;
            args.DragUIOverride.IsGlyphVisible = true;
            args.DragUIOverride.Caption = FileCertificate.DragOverContent;
            args.Handled = true;
        }

        /// <summary>
        /// 拖动文件完成后获取文件信息
        /// </summary>
        protected override void OnDrop(global::Windows.UI.Xaml.DragEventArgs args)
        {
            base.OnDrop(args);
            DragOperationDeferral deferral = args.GetDeferral();
            try
            {
                DataPackageView view = args.DataView;
                if (view.Contains(StandardDataFormats.StorageItems))
                {
                    Task.Run(async () =>
                    {
                        IReadOnlyList<IStorageItem> storageItemList = await view.GetStorageItemsAsync();
                        List<CertificateResultModel> fileCertificateList = [];
                        foreach (IStorageItem storageItem in storageItemList)
                        {
                            try
                            {
                                if (!IOHelper.IsDir(storageItem.Path))
                                {
                                    FileInfo fileInfo = new(storageItem.Path);
                                    if ((fileInfo.Attributes & System.IO.FileAttributes.Hidden) is System.IO.FileAttributes.Hidden)
                                    {
                                        continue;
                                    }

                                    fileCertificateList.Add(new CertificateResultModel()
                                    {
                                        FileName = storageItem.Name,
                                        FilePath = storageItem.Path
                                    });
                                }
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(EventLevel.Error, string.Format("Read file {0} information failed", storageItem.Path), e);
                                continue;
                            }
                        }

                        AddToFileCertificatePage(fileCertificateList);
                    });
                }
            }
            finally
            {
                deferral.Complete();
                OperationFailedCollection.Clear();
            }
        }

        /// <summary>
        /// 按下 Enter 键发生的事件（预览修改内容）
        /// 按下 Ctrl + Enter 键发生的事件（修改内容）
        /// </summary>
        protected override void OnKeyDown(KeyRoutedEventArgs args)
        {
            base.OnKeyDown(args);
            if (args.Key is VirtualKey.Enter)
            {
                args.Handled = true;
                OperationFailedCollection.Clear();
                int count = 0;

                lock (fileCertificateLock)
                {
                    count = FileCertificateCollection.Count;
                }

                if (count is 0)
                {
                    TeachingTipHelper.Show(new OperationResultTip(OperationKind.ListEmpty));
                }
                else
                {
                    RemoveFileCertificates();
                }
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：文件证书页面——挂载的事件

        /// <summary>
        /// 清空列表
        /// </summary>
        private void OnClearListClicked(object sender, RoutedEventArgs args)
        {
            lock (fileCertificateLock)
            {
                FileCertificateCollection.Clear();
                OperationFailedCollection.Clear();
            }
        }

        /// <summary>
        /// 修改内容
        /// </summary>
        private void OnModifyClicked(object sender, RoutedEventArgs args)
        {
            OperationFailedCollection.Clear();
            int count = 0;

            lock (fileCertificateLock)
            {
                count = FileCertificateCollection.Count;
            }

            if (count is 0)
            {
                TeachingTipHelper.Show(new OperationResultTip(OperationKind.ListEmpty));
            }
            else
            {
                RemoveFileCertificates();
            }
        }

        /// <summary>
        /// 选择文件
        /// </summary>
        private void OnSelectFileClicked(object sender, RoutedEventArgs args)
        {
            OpenFileDialog dialog = new()
            {
                Multiselect = true,
                Title = FileProperties.SelectFile
            };
            if (dialog.ShowDialog() is DialogResult.OK)
            {
                Task.Run(() =>
                {
                    List<CertificateResultModel> fileCertificateList = [];

                    foreach (string fileName in dialog.FileNames)
                    {
                        try
                        {
                            FileInfo fileInfo = new(fileName);
                            if ((fileInfo.Attributes & System.IO.FileAttributes.Hidden) is System.IO.FileAttributes.Hidden)
                            {
                                continue;
                            }

                            if (!IOHelper.IsDir(fileInfo.FullName))
                            {
                                fileCertificateList.Add(new CertificateResultModel()
                                {
                                    FileName = fileInfo.Name,
                                    FilePath = fileInfo.FullName
                                });
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, string.Format("Read file {0} information failed", fileName), e);
                            continue;
                        }
                    }

                    AddToFileCertificatePage(fileCertificateList);
                });
            }
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
                OperationFailedCollection.Clear();
                if (!string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    Task.Run(() =>
                    {
                        DirectoryInfo currentFolder = new(dialog.SelectedPath);
                        List<CertificateResultModel> fileNameList = [];

                        try
                        {
                            foreach (FileInfo fileInfo in currentFolder.GetFiles())
                            {
                                if ((fileInfo.Attributes & System.IO.FileAttributes.Hidden) is System.IO.FileAttributes.Hidden)
                                {
                                    continue;
                                }

                                fileNameList.Add(new CertificateResultModel()
                                {
                                    FileName = fileInfo.Name,
                                    FilePath = fileInfo.FullName
                                });
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, string.Format("Read folder {0} fileInfo information failed", dialog.SelectedPath), e);
                        }

                        AddToFileCertificatePage(fileNameList);
                    });
                }
            }
        }

        /// <summary>
        /// 查看修改失败的文件错误信息
        /// </summary>
        private async void OnViewErrorInformationClicked(object sender, RoutedEventArgs args)
        {
            await ContentDialogHelper.ShowAsync(new OperationFailedDialog(OperationFailedCollection), this);
        }

        #endregion 第二部分：文件证书页面——挂载的事件

        /// <summary>
        /// 添加到数字签名页面
        /// </summary>
        public void AddToFileCertificatePage(List<CertificateResultModel> fileCertificateList)
        {
            synchronizationContext.Post(_ =>
            {
                lock (fileCertificateLock)
                {
                    foreach (CertificateResultModel certificateResultItem in fileCertificateList)
                    {
                        FileCertificateCollection.Add(certificateResultItem);
                    }
                }
            }, null);
        }

        /// <summary>
        /// 移除文件证书
        /// </summary>
        private void RemoveFileCertificates()
        {
            List<OperationFailedModel> operationFailedList = [];
            IsModifyingNow = true;
            Task.Run(async () =>
            {
                lock (fileCertificateLock)
                {
                    foreach (CertificateResultModel certificateResultItem in FileCertificateCollection)
                    {
                        if (!string.IsNullOrEmpty(certificateResultItem.FileName) && !string.IsNullOrEmpty(certificateResultItem.FilePath))
                        {
                            try
                            {
                                using FileStream fileStream = new(certificateResultItem.FilePath, FileMode.Open, FileAccess.ReadWrite);
                                bool result = ImagehlpLibrary.ImageRemoveCertificate(fileStream.SafeFileHandle.DangerousGetHandle(), 0);

                                if (!result)
                                {
                                    operationFailedList.Add(new OperationFailedModel()
                                    {
                                        FileName = certificateResultItem.FileName,
                                        FilePath = certificateResultItem.FilePath,
                                        Exception = Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error())
                                    });
                                }
                            }
                            catch (Exception e)
                            {
                                operationFailedList.Add(new OperationFailedModel()
                                {
                                    FileName = certificateResultItem.FileName,
                                    FilePath = certificateResultItem.FilePath,
                                    Exception = e
                                });
                            }
                        }
                    }
                }

                await Task.Delay(300);

                synchronizationContext.Post(_ =>
                {
                    IsModifyingNow = false;
                    foreach (OperationFailedModel operationFailedItem in operationFailedList)
                    {
                        OperationFailedCollection.Add(operationFailedItem);
                    }

                    lock (fileCertificateLock)
                    {
                        TeachingTipHelper.Show(new OperationResultTip(OperationKind.File, FileCertificateCollection.Count - OperationFailedCollection.Count, OperationFailedCollection.Count));

                        FileCertificateCollection.Clear();
                    }
                }, null);
            });
        }
    }
}
