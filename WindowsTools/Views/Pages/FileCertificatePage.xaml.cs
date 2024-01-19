using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using WindowsTools.Helpers.Controls;
using WindowsTools.Helpers.Controls.Extensions;
using WindowsTools.Helpers.Root;
using WindowsTools.Models;
using WindowsTools.Services.Root;
using WindowsTools.Strings;
using WindowsTools.UI.Dialogs;
using WindowsTools.UI.TeachingTips;
using WindowsTools.Views.Windows;
using WindowsTools.WindowsAPI.PInvoke.Imagehlp;

namespace WindowsTools.Views.Pages
{
    /// <summary>
    /// 数字签名页面
    /// </summary>
    public sealed partial class FileCertificatePage : Page, INotifyPropertyChanged
    {
        private bool _isModifyingNow = false;

        public bool IsModifyingNow
        {
            get { return _isModifyingNow; }

            set
            {
                _isModifyingNow = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<CertificateResultModel> FileCertificateCollection { get; } = new ObservableCollection<CertificateResultModel>();

        private ObservableCollection<OperationFailedModel> OperationFailedCollection { get; } = new ObservableCollection<OperationFailedModel>();

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
        protected override async void OnDrop(global::Windows.UI.Xaml.DragEventArgs args)
        {
            base.OnDrop(args);
            DragOperationDeferral deferral = args.GetDeferral();
            try
            {
                DataPackageView view = args.DataView;
                if (view.Contains(StandardDataFormats.StorageItems))
                {
                    IReadOnlyList<IStorageItem> filesList = await view.GetStorageItemsAsync();
                    foreach (IStorageItem item in filesList)
                    {
                        if (!IOHelper.IsDir(item.Path))
                        {
                            FileCertificateCollection.Add(new CertificateResultModel()
                            {
                                FileName = item.Name,
                                FilePath = item.Path
                            });
                        }
                    }
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
                if (FileCertificateCollection.Count is 0)
                {
                    TeachingTipHelper.Show(new ListEmptyTip());
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
            FileCertificateCollection.Clear();
            OperationFailedCollection.Clear();
        }

        /// <summary>
        /// 修改内容
        /// </summary>
        private void OnModifyClicked(object sender, RoutedEventArgs args)
        {
            OperationFailedCollection.Clear();
            if (FileCertificateCollection.Count is 0)
            {
                TeachingTipHelper.Show(new ListEmptyTip());
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
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            dialog.Title = FileProperties.SelectFile;
            if (dialog.ShowDialog() is DialogResult.OK)
            {
                foreach (string fileName in dialog.FileNames)
                {
                    try
                    {
                        FileInfo file = new FileInfo(fileName);
                        if ((file.Attributes & System.IO.FileAttributes.Hidden) is System.IO.FileAttributes.Hidden)
                        {
                            continue;
                        }
                        if (!IOHelper.IsDir(file.FullName))
                        {
                            FileCertificateCollection.Add(new CertificateResultModel()
                            {
                                FileName = file.Name,
                                FilePath = file.FullName
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLogEntryType.Error, string.Format("Read file {0} information failed", fileName), e);
                        continue;
                    }
                }
            }
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
                OperationFailedCollection.Clear();
                if (!string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    DirectoryInfo currentFolder = new DirectoryInfo(dialog.SelectedPath);

                    try
                    {
                        foreach (DirectoryInfo subFolder in currentFolder.GetDirectories())
                        {
                            if ((subFolder.Attributes & System.IO.FileAttributes.Hidden) is System.IO.FileAttributes.Hidden)
                            {
                                continue;
                            }
                            FileCertificateCollection.Add(new CertificateResultModel()
                            {
                                FileName = subFolder.Name,
                                FilePath = subFolder.FullName
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLogEntryType.Error, string.Format("Read folder {0} subFolder information failed", dialog.SelectedPath), e);
                    }

                    try
                    {
                        foreach (FileInfo subFile in currentFolder.GetFiles())
                        {
                            if ((subFile.Attributes & System.IO.FileAttributes.Hidden) is System.IO.FileAttributes.Hidden)
                            {
                                continue;
                            }
                            FileCertificateCollection.Add(new CertificateResultModel()
                            {
                                FileName = subFile.Name,
                                FilePath = subFile.FullName
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLogEntryType.Error, string.Format("Read folder {0} subFile information failed", dialog.SelectedPath), e);
                    }
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
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string LocalizeTotal(int count)
        {
            return string.Format(FileCertificate.Total, FileCertificateCollection.Count);
        }

        /// <summary>
        /// 移除文件证书
        /// </summary>
        private void RemoveFileCertificates()
        {
            List<OperationFailedModel> operationFailedList = new List<OperationFailedModel>();
            IsModifyingNow = true;
            Task.Run(async () =>
            {
                foreach (CertificateResultModel item in FileCertificateCollection)
                {
                    if (!string.IsNullOrEmpty(item.FileName) && !string.IsNullOrEmpty(item.FilePath))
                    {
                        try
                        {
                            using FileStream fileStream = new FileStream(item.FilePath, FileMode.Open, FileAccess.ReadWrite);
                            bool result = ImagehlpLibrary.ImageRemoveCertificate(fileStream.SafeFileHandle.DangerousGetHandle(), 0);
                            fileStream.Close();

                            if (!result)
                            {
                                operationFailedList.Add(new OperationFailedModel()
                                {
                                    FileName = item.FileName,
                                    FilePath = item.FilePath,
                                    Exception = Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error())
                                });
                            }
                        }
                        catch (Exception e)
                        {
                            operationFailedList.Add(new OperationFailedModel()
                            {
                                FileName = item.FileName,
                                FilePath = item.FilePath,
                                Exception = e
                            });
                        }
                    }
                }

                await Task.Delay(300);

                MainWindow.Current.BeginInvoke(() =>
                {
                    IsModifyingNow = false;
                    foreach (OperationFailedModel item in operationFailedList)
                    {
                        OperationFailedCollection.Add(item);
                    }

                    TeachingTipHelper.Show(new OperationResultTip(FileCertificateCollection.Count - OperationFailedCollection.Count, OperationFailedCollection.Count));
                    FileCertificateCollection.Clear();
                });
            });
        }
    }
}
