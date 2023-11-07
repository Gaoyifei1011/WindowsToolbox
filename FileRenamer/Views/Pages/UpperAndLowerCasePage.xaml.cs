using FileRenamer.Extensions.DataType.Enums;
using FileRenamer.Helpers.Controls;
using FileRenamer.Helpers.Root;
using FileRenamer.Models;
using FileRenamer.Strings;
using FileRenamer.UI.Dialogs;
using FileRenamer.UI.Notifications;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace FileRenamer.Views.Pages
{
    /// <summary>
    /// 大写小写页面
    /// </summary>
    public sealed partial class UpperAndLowerCasePage : Page, INotifyPropertyChanged
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

        private UpperAndLowerSelectedKind _selectedType = UpperAndLowerSelectedKind.None;

        public UpperAndLowerSelectedKind SelectedType
        {
            get { return _selectedType; }

            set
            {
                _selectedType = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<OldAndNewNameModel> UpperAndLowerCaseDataList { get; } = new ObservableCollection<OldAndNewNameModel>();

        private ObservableCollection<OperationFailedModel> OperationFailedList { get; } = new ObservableCollection<OperationFailedModel>();

        public event PropertyChangedEventHandler PropertyChanged;

        public UpperAndLowerCasePage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 设置拖动的数据的可视表示形式
        /// </summary>
        protected override void OnDragOver(Windows.UI.Xaml.DragEventArgs args)
        {
            base.OnDragOver(args);
            args.AcceptedOperation = DataPackageOperation.Copy;
            args.DragUIOverride.IsCaptionVisible = true;
            args.DragUIOverride.IsContentVisible = false;
            args.DragUIOverride.IsGlyphVisible = true;
            args.DragUIOverride.Caption = UpperAndLowerCase.DragOverContent;
            args.Handled = true;
        }

        /// <summary>
        /// 拖动文件完成后获取文件信息
        /// </summary>
        protected override async void OnDrop(Windows.UI.Xaml.DragEventArgs args)
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
                        UpperAndLowerCaseDataList.Add(new OldAndNewNameModel()
                        {
                            OriginalFileName = item.Name,
                            OriginalFilePath = item.Path
                        });
                    }
                }
            }
            finally
            {
                deferral.Complete();
                OperationFailedList.Clear();
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
                bool checkResult = CheckOperationState();
                if (checkResult)
                {
                    OperationFailedList.Clear();
                    if (UpperAndLowerCaseDataList.Count is 0)
                    {
                        new ListEmptyNotification(this).Show();
                    }
                    else
                    {
                        PreviewChangedFileName();
                    }
                }
                else
                {
                    new NoOperationNotification(this).Show();
                }
            }
            else if (args.Key is VirtualKey.Control && args.Key is VirtualKey.Enter)
            {
                args.Handled = true;
                bool checkResult = CheckOperationState();
                if (checkResult)
                {
                    OperationFailedList.Clear();
                    if (UpperAndLowerCaseDataList.Count is 0)
                    {
                        new ListEmptyNotification(this).Show();
                    }
                    else
                    {
                        PreviewChangedFileName();
                        ChangeFileName();
                    }
                }
                else
                {
                    new NoOperationNotification(this).Show();
                }
            }
        }

        /// <summary>
        /// 选中时触发的事件
        /// </summary>
        private void OnChecked(object sender, RoutedEventArgs args)
        {
            Windows.UI.Xaml.Controls.CheckBox checkBox = sender as Windows.UI.Xaml.Controls.CheckBox;
            if (checkBox is not null)
            {
                SelectedType = (UpperAndLowerSelectedKind)Convert.ToInt32(checkBox.Tag);
            }
        }

        /// <summary>
        /// 清空列表
        /// </summary>
        private void OnClearListClicked(object sender, RoutedEventArgs args)
        {
            UpperAndLowerCaseDataList.Clear();
            OperationFailedList.Clear();
        }

        /// <summary>
        /// 预览修改的内容
        /// </summary>
        private void OnPreviewClicked(object sender, RoutedEventArgs args)
        {
            bool checkResult = CheckOperationState();
            if (checkResult)
            {
                OperationFailedList.Clear();
                if (UpperAndLowerCaseDataList.Count is 0)
                {
                    new ListEmptyNotification(this).Show();
                }
                else
                {
                    PreviewChangedFileName();
                }
            }
            else
            {
                new NoOperationNotification(this).Show();
            }
        }

        /// <summary>
        /// 修改内容
        /// </summary>
        private void OnModifyClicked(object sender, RoutedEventArgs args)
        {
            bool checkResult = CheckOperationState();
            if (checkResult)
            {
                OperationFailedList.Clear();
                if (UpperAndLowerCaseDataList.Count is 0)
                {
                    new ListEmptyNotification(this).Show();
                }
                else
                {
                    PreviewChangedFileName();
                    ChangeFileName();
                }
            }
            else
            {
                new NoOperationNotification(this).Show();
            }
        }

        /// <summary>
        /// 选择文件
        /// </summary>
        private void OnSelectFileClicked(object sender, RoutedEventArgs args)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            dialog.Title = UpperAndLowerCase.SelectFile;
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
                        UpperAndLowerCaseDataList.Add(new OldAndNewNameModel()
                        {
                            OriginalFileName = file.Name,
                            OriginalFilePath = file.FullName
                        });
                    }
                    catch (Exception)
                    {
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
            dialog.Description = UpperAndLowerCase.SelectFolder;
            dialog.ShowNewFolderButton = true;
            dialog.RootFolder = Environment.SpecialFolder.Desktop;
            DialogResult result = dialog.ShowDialog();
            if (result is DialogResult.OK || result is DialogResult.Yes)
            {
                OperationFailedList.Clear();
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
                            UpperAndLowerCaseDataList.Add(new OldAndNewNameModel()
                            {
                                OriginalFileName = subFolder.Name,
                                OriginalFilePath = subFolder.FullName
                            });
                        }
                    }
                    catch (Exception)
                    {
                    }

                    try
                    {
                        foreach (FileInfo subFile in currentFolder.GetFiles())
                        {
                            if ((subFile.Attributes & System.IO.FileAttributes.Hidden) is System.IO.FileAttributes.Hidden)
                            {
                                continue;
                            }
                            UpperAndLowerCaseDataList.Add(new OldAndNewNameModel()
                            {
                                OriginalFileName = subFile.Name,
                                OriginalFilePath = subFile.FullName
                            });
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        /// <summary>
        /// 取消选中时触发的事件
        /// </summary>
        private void OnUnchecked(object sender, RoutedEventArgs args)
        {
            Windows.UI.Xaml.Controls.CheckBox checkBox = sender as Windows.UI.Xaml.Controls.CheckBox;
            if (checkBox is not null)
            {
                if (SelectedType == (UpperAndLowerSelectedKind)Convert.ToInt32(checkBox.Tag))
                {
                    SelectedType = UpperAndLowerSelectedKind.None;
                }
            }
        }

        /// <summary>
        /// 查看修改失败的文件错误信息
        /// </summary>
        private async void OnViewErrorInformationClicked(object sender, RoutedEventArgs args)
        {
            await ContentDialogHelper.ShowAsync(new OperationFailedDialog(OperationFailedList), this);
        }

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string LocalizeTotal(int count)
        {
            return string.Format(UpperAndLowerCase.Total, UpperAndLowerCaseDataList.Count);
        }

        /// <summary>
        /// 检查用户是否指定了操作过程
        /// </summary>
        private bool CheckOperationState()
        {
            if (SelectedType is UpperAndLowerSelectedKind.None)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 预览修改后的文件名称
        /// </summary>
        private void PreviewChangedFileName()
        {
            switch (SelectedType)
            {
                case UpperAndLowerSelectedKind.AllUppercase:
                    {
                        foreach (OldAndNewNameModel item in UpperAndLowerCaseDataList)
                        {
                            if (!string.IsNullOrEmpty(item.OriginalFileName))
                            {
                                item.NewFileName = item.OriginalFileName.ToUpper();
                                item.NewFilePath = item.OriginalFilePath.Replace(item.OriginalFileName, item.NewFileName);
                            }
                        }
                        break;
                    }
                case UpperAndLowerSelectedKind.FileNameUppercase:
                    {
                        foreach (OldAndNewNameModel item in UpperAndLowerCaseDataList)
                        {
                            if (!string.IsNullOrEmpty(item.OriginalFileName))
                            {
                                string fileName = Path.GetFileNameWithoutExtension(item.OriginalFileName).ToUpper();
                                string extensionName = Path.GetExtension(item.OriginalFileName);
                                item.NewFileName = fileName + extensionName;
                                item.NewFilePath = item.OriginalFilePath.Replace(item.OriginalFileName, item.NewFileName);
                            }
                        }
                        break;
                    }
                case UpperAndLowerSelectedKind.ExtensionNameUppercase:
                    {
                        foreach (OldAndNewNameModel item in UpperAndLowerCaseDataList)
                        {
                            if (!string.IsNullOrEmpty(item.OriginalFileName))
                            {
                                string fileName = Path.GetFileNameWithoutExtension(item.OriginalFileName);
                                string extensionName = Path.GetExtension(item.OriginalFileName).ToUpper();
                                item.NewFileName = fileName + extensionName;
                                item.NewFilePath = item.OriginalFilePath.Replace(item.OriginalFileName, item.NewFileName);
                            }
                        }
                        break;
                    }
                case UpperAndLowerSelectedKind.DeleteSpace:
                    {
                        foreach (OldAndNewNameModel item in UpperAndLowerCaseDataList)
                        {
                            if (!string.IsNullOrEmpty(item.OriginalFileName))
                            {
                                item.NewFileName = item.OriginalFileName.Replace(" ", string.Empty);
                                item.NewFilePath = item.OriginalFilePath.Replace(item.OriginalFileName, item.NewFileName);
                            }
                        }
                        break;
                    }
                case UpperAndLowerSelectedKind.AllLowercase:
                    {
                        foreach (OldAndNewNameModel item in UpperAndLowerCaseDataList)
                        {
                            if (!string.IsNullOrEmpty(item.OriginalFileName))
                            {
                                item.NewFileName = item.OriginalFileName.ToLower();
                                item.NewFilePath = item.OriginalFilePath.Replace(item.OriginalFileName, item.NewFileName);
                            }
                        }
                        break;
                    }
                case UpperAndLowerSelectedKind.FileNameLowercase:
                    {
                        foreach (OldAndNewNameModel item in UpperAndLowerCaseDataList)
                        {
                            if (!string.IsNullOrEmpty(item.OriginalFileName))
                            {
                                string fileName = Path.GetFileNameWithoutExtension(item.OriginalFileName).ToLower();
                                string extensionName = Path.GetExtension(item.OriginalFileName);
                                item.NewFileName = fileName + extensionName;
                                item.NewFilePath = item.OriginalFilePath.Replace(item.OriginalFileName, item.NewFileName);
                            }
                        }
                        break;
                    }
                case UpperAndLowerSelectedKind.ExtensionNameLowercase:
                    {
                        foreach (OldAndNewNameModel item in UpperAndLowerCaseDataList)
                        {
                            if (!string.IsNullOrEmpty(item.OriginalFileName))
                            {
                                string fileName = Path.GetFileNameWithoutExtension(item.OriginalFileName);
                                string extensionName = Path.GetExtension(item.OriginalFileName).ToLower();
                                item.NewFileName = fileName + extensionName;
                                item.NewFilePath = item.OriginalFilePath.Replace(item.OriginalFileName, item.NewFileName);
                            }
                        }
                        break;
                    }
                case UpperAndLowerSelectedKind.ReplaceSpace:
                    {
                        foreach (OldAndNewNameModel item in UpperAndLowerCaseDataList)
                        {
                            if (!string.IsNullOrEmpty(item.OriginalFileName))
                            {
                                item.NewFileName = item.OriginalFileName.Replace(" ", "_");
                                item.NewFilePath = item.OriginalFilePath.Replace(item.OriginalFileName, item.NewFileName);
                            }
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// 更改文件名称
        /// </summary>
        private void ChangeFileName()
        {
            List<OperationFailedModel> operationFailedList = new List<OperationFailedModel>();
            IsModifyingNow = true;
            Task.Run(async () =>
            {
                foreach (OldAndNewNameModel item in UpperAndLowerCaseDataList)
                {
                    if (!string.IsNullOrEmpty(item.OriginalFileName) && !string.IsNullOrEmpty(item.OriginalFilePath))
                    {
                        if (IOHelper.IsDir(item.OriginalFilePath))
                        {
                            try
                            {
                                Directory.Move(item.OriginalFilePath, item.NewFilePath);
                            }
                            catch (Exception e)
                            {
                                operationFailedList.Add(new OperationFailedModel()
                                {
                                    FileName = item.OriginalFileName,
                                    FilePath = item.OriginalFilePath,
                                    Exception = e
                                });
                            }
                        }
                        else
                        {
                            try
                            {
                                File.Move(item.OriginalFilePath, item.NewFilePath);
                            }
                            catch (Exception e)
                            {
                                operationFailedList.Add(new OperationFailedModel()
                                {
                                    FileName = item.OriginalFileName,
                                    FilePath = item.OriginalFilePath,
                                    Exception = e
                                });
                            }
                        }
                    }
                }

                await Task.Delay(300);

                Program.MainWindow.BeginInvoke(() =>
                {
                    IsModifyingNow = false;
                    foreach (OperationFailedModel item in operationFailedList)
                    {
                        OperationFailedList.Add(item);
                    }

                    new OperationResultNotification(this, UpperAndLowerCaseDataList.Count - OperationFailedList.Count, OperationFailedList.Count).Show();
                    UpperAndLowerCaseDataList.Clear();
                });
            });
        }
    }
}
