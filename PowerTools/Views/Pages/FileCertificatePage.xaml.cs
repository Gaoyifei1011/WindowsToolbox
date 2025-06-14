using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using PowerTools.Extensions.DataType.Enums;
using PowerTools.Models;
using PowerTools.Services.Root;
using PowerTools.Views.Dialogs;
using PowerTools.Views.TeachingTips;
using PowerTools.Views.Windows;
using PowerTools.WindowsAPI.ComTypes;
using PowerTools.WindowsAPI.PInvoke.Imagehlp;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace PowerTools.Views.Pages
{
    /// <summary>
    /// 数字签名页面
    /// </summary>
    public sealed partial class FileCertificatePage : Page, INotifyPropertyChanged
    {
        private readonly string DragOverContentString = ResourceService.FileCertificateResource.GetString("DragOverContent");
        private readonly string SelectFileString = ResourceService.FileCertificateResource.GetString("SelectFile");
        private readonly string SelectFolderString = ResourceService.FileCertificateResource.GetString("SelectFolder");
        private readonly string TotalString = ResourceService.FileCertificateResource.GetString("Total");
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
            args.DragUIOverride.Caption = DragOverContentString;
            args.Handled = true;
        }

        /// <summary>
        /// 拖动文件完成后获取文件信息
        /// </summary>
        protected override async void OnDrop(global::Windows.UI.Xaml.DragEventArgs args)
        {
            base.OnDrop(args);
            DragOperationDeferral dragOperationDeferral = args.GetDeferral();
            try
            {
                DataPackageView view = args.DataView;
                if (view.Contains(StandardDataFormats.StorageItems))
                {
                    List<CertificateResultModel> fileCertificateList = await Task.Run(async () =>
                    {
                        List<CertificateResultModel> fileCertificateList = [];
                        IReadOnlyList<IStorageItem> storageItemList = await view.GetStorageItemsAsync();

                        foreach (IStorageItem storageItem in storageItemList)
                        {
                            try
                            {
                                FileInfo fileInfo = new(storageItem.Path);
                                if ((fileInfo.Attributes & System.IO.FileAttributes.Hidden) is System.IO.FileAttributes.Hidden)
                                {
                                    continue;
                                }

                                if ((fileInfo.Attributes & System.IO.FileAttributes.Directory) is 0)
                                {
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

                        return fileCertificateList;
                    });

                    AddToFileCertificatePage(fileCertificateList);
                }
            }
            finally
            {
                dragOperationDeferral.Complete();
                OperationFailedCollection.Clear();
            }
        }

        /// <summary>
        /// 按下 Enter 键发生的事件（预览修改内容）
        /// 按下 Ctrl + Enter 键发生的事件（修改内容）
        /// </summary>
        protected override async void OnKeyDown(KeyRoutedEventArgs args)
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
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.ListEmpty));
                }
                else
                {
                    await RemoveFileCertificatesAsync();
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
        private async void OnModifyClicked(object sender, RoutedEventArgs args)
        {
            OperationFailedCollection.Clear();
            int count = 0;

            lock (fileCertificateLock)
            {
                count = FileCertificateCollection.Count;
            }

            if (count is 0)
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.ListEmpty));
            }
            else
            {
                await RemoveFileCertificatesAsync();
            }
        }

        /// <summary>
        /// 选择文件
        /// </summary>
        private async void OnSelectFileClicked(object sender, RoutedEventArgs args)
        {
            OpenFileDialog dialog = new()
            {
                Multiselect = true,
                Title = SelectFileString
            };
            if (dialog.ShowDialog() is DialogResult.OK)
            {
                List<CertificateResultModel> fileCertificateList = await Task.Run(() =>
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

                            if ((fileInfo.Attributes & System.IO.FileAttributes.Directory) is 0)
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

                    return fileCertificateList;
                });

                dialog.Dispose();
                AddToFileCertificatePage(fileCertificateList);
            }
            else
            {
                dialog.Dispose();
            }
        }

        /// <summary>
        /// 选择文件夹
        /// </summary>
        private async void OnSelectFolderClicked(object sender, RoutedEventArgs args)
        {
            OpenFolderDialog dialog = new()
            {
                Description = SelectFolderString,
                RootFolder = Environment.SpecialFolder.Desktop
            };
            DialogResult result = dialog.ShowDialog();
            if (result is DialogResult.OK || result is DialogResult.Yes)
            {
                OperationFailedCollection.Clear();
                if (!string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    List<CertificateResultModel> fileNameList = await Task.Run(() =>
                    {
                        List<CertificateResultModel> fileNameList = [];
                        DirectoryInfo currentFolder = new(dialog.SelectedPath);

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

                        return fileNameList;
                    });

                    dialog.Dispose();
                    AddToFileCertificatePage(fileNameList);
                }
            }
            else
            {
                dialog.Dispose();
            }
        }

        /// <summary>
        /// 查看修改失败的文件错误信息
        /// </summary>
        private async void OnViewErrorInformationClicked(object sender, RoutedEventArgs args)
        {
            await MainWindow.Current.ShowDialogAsync(new OperationFailedDialog(OperationFailedCollection));
        }

        #endregion 第二部分：文件证书页面——挂载的事件

        /// <summary>
        /// 添加到数字签名页面
        /// </summary>
        public void AddToFileCertificatePage(List<CertificateResultModel> fileCertificateList)
        {
            lock (fileCertificateLock)
            {
                foreach (CertificateResultModel certificateResultItem in fileCertificateList)
                {
                    FileCertificateCollection.Add(certificateResultItem);
                }
            }
        }

        /// <summary>
        /// 移除文件证书
        /// </summary>
        private async Task RemoveFileCertificatesAsync()
        {
            IsModifyingNow = true;
            List<OperationFailedModel> operationFailedList = await Task.Run(() =>
            {
                List<OperationFailedModel> operationFailedList = [];

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

                return operationFailedList;
            });

            IsModifyingNow = false;
            foreach (OperationFailedModel operationFailedItem in operationFailedList)
            {
                OperationFailedCollection.Add(operationFailedItem);
            }

            int count = FileCertificateCollection.Count;

            lock (fileCertificateLock)
            {
                FileCertificateCollection.Clear();
            }

            await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.File, count - OperationFailedCollection.Count, OperationFailedCollection.Count));
        }
    }
}
