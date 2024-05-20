using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.IO;
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
using WindowsTools.Views.Windows;

namespace WindowsTools.Views.Pages
{
    /// <summary>
    /// 大写小写页面
    /// </summary>
    public sealed partial class UpperAndLowerCasePage : Page, INotifyPropertyChanged
    {
        private readonly object upperAndLowerCaseLock = new();

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

        private UpperAndLowerSelectedKind _selectedType = UpperAndLowerSelectedKind.None;

        public UpperAndLowerSelectedKind SelectedType
        {
            get { return _selectedType; }

            set
            {
                if (!Equals(_selectedType, value))
                {
                    _selectedType = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedType)));
                }
            }
        }

        public ObservableCollection<OldAndNewNameModel> UpperAndLowerCaseCollection { get; } = [];

        private ObservableCollection<OperationFailedModel> OperationFailedCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public UpperAndLowerCasePage()
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
            args.DragUIOverride.Caption = UpperAndLowerCase.DragOverContent;
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
                        List<OldAndNewNameModel> upperAndLowerCaseList = [];

                        foreach (IStorageItem storageItem in storageItemList)
                        {
                            try
                            {
                                FileInfo fileInfo = new(storageItem.Path);
                                if ((fileInfo.Attributes & System.IO.FileAttributes.Hidden) is System.IO.FileAttributes.Hidden)
                                {
                                    continue;
                                }

                                upperAndLowerCaseList.Add(new()
                                {
                                    OriginalFileName = storageItem.Name,
                                    OriginalFilePath = storageItem.Path,
                                });
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(EventLevel.Error, string.Format("Read file {0} information failed", storageItem.Path), e);
                                continue;
                            }
                        }

                        AddtoUpperAndLowerCasePage(upperAndLowerCaseList);
                    });
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Warning, "Drop file in upper and lower case page failed", e);
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
                bool checkResult = CheckOperationState();
                if (checkResult)
                {
                    OperationFailedCollection.Clear();
                    if (UpperAndLowerCaseCollection.Count is 0)
                    {
                        TeachingTipHelper.Show(new ListEmptyTip());
                    }
                    else
                    {
                        PreviewChangedFileName();
                    }
                }
                else
                {
                    TeachingTipHelper.Show(new NoOperationTip());
                }
            }
            else if (args.Key is VirtualKey.Control && args.Key is VirtualKey.Enter)
            {
                args.Handled = true;
                bool checkResult = CheckOperationState();
                if (checkResult)
                {
                    OperationFailedCollection.Clear();
                    if (UpperAndLowerCaseCollection.Count is 0)
                    {
                        TeachingTipHelper.Show(new ListEmptyTip());
                    }
                    else
                    {
                        PreviewChangedFileName();
                        ChangeFileName();
                    }
                }
                else
                {
                    TeachingTipHelper.Show(new NoOperationTip());
                }
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：大写小写页面——挂载的事件

        /// <summary>
        /// 选中时触发的事件
        /// </summary>
        private void OnChecked(object sender, RoutedEventArgs args)
        {
            global::Windows.UI.Xaml.Controls.CheckBox checkBox = sender as global::Windows.UI.Xaml.Controls.CheckBox;
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
            lock (upperAndLowerCaseLock)
            {
                UpperAndLowerCaseCollection.Clear();
                OperationFailedCollection.Clear();
            }
        }

        /// <summary>
        /// 预览修改的内容
        /// </summary>
        private void OnPreviewClicked(object sender, RoutedEventArgs args)
        {
            bool checkResult = CheckOperationState();
            if (checkResult)
            {
                OperationFailedCollection.Clear();
                if (UpperAndLowerCaseCollection.Count is 0)
                {
                    TeachingTipHelper.Show(new ListEmptyTip());
                }
                else
                {
                    PreviewChangedFileName();
                }
            }
            else
            {
                TeachingTipHelper.Show(new NoOperationTip());
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
                OperationFailedCollection.Clear();
                if (UpperAndLowerCaseCollection.Count is 0)
                {
                    TeachingTipHelper.Show(new ListEmptyTip());
                }
                else
                {
                    PreviewChangedFileName();
                    ChangeFileName();
                }
            }
            else
            {
                TeachingTipHelper.Show(new NoOperationTip());
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
                Title = UpperAndLowerCase.SelectFile
            };
            if (dialog.ShowDialog() is DialogResult.OK)
            {
                Task.Run(() =>
                {
                    List<OldAndNewNameModel> upperAndLowerCaseList = [];

                    foreach (string fileName in dialog.FileNames)
                    {
                        try
                        {
                            FileInfo fileInfo = new(fileName);
                            if ((fileInfo.Attributes & System.IO.FileAttributes.Hidden) is System.IO.FileAttributes.Hidden)
                            {
                                continue;
                            }

                            upperAndLowerCaseList.Add(new()
                            {
                                OriginalFileName = fileInfo.Name,
                                OriginalFilePath = fileInfo.FullName
                            });
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, string.Format("Read file {0} information failed", fileName), e);
                            continue;
                        }
                    }

                    AddtoUpperAndLowerCasePage(upperAndLowerCaseList);
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
                Description = UpperAndLowerCase.SelectFolder,
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
                        List<OldAndNewNameModel> directoryNameList = [];
                        List<OldAndNewNameModel> fileNameList = [];

                        try
                        {
                            foreach (DirectoryInfo directoryInfo in currentFolder.GetDirectories())
                            {
                                if ((directoryInfo.Attributes & System.IO.FileAttributes.Hidden) is System.IO.FileAttributes.Hidden)
                                {
                                    continue;
                                }

                                directoryNameList.Add(new()
                                {
                                    OriginalFileName = directoryInfo.Name,
                                    OriginalFilePath = directoryInfo.FullName
                                });
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, string.Format("Read folder {0} directoryInfo information failed", dialog.SelectedPath), e);
                        }

                        try
                        {
                            foreach (FileInfo fileInfo in currentFolder.GetFiles())
                            {
                                if ((fileInfo.Attributes & System.IO.FileAttributes.Hidden) is System.IO.FileAttributes.Hidden)
                                {
                                    continue;
                                }

                                fileNameList.Add(new()
                                {
                                    OriginalFileName = fileInfo.Name,
                                    OriginalFilePath = fileInfo.FullName
                                });
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, string.Format("Read folder {0} fileInfo information failed", dialog.SelectedPath), e);
                        }

                        AddtoUpperAndLowerCasePage(directoryNameList);
                        AddtoUpperAndLowerCasePage(fileNameList);
                    });
                }
            }
        }

        /// <summary>
        /// 取消选中时触发的事件
        /// </summary>
        private void OnUnchecked(object sender, RoutedEventArgs args)
        {
            global::Windows.UI.Xaml.Controls.CheckBox checkBox = sender as global::Windows.UI.Xaml.Controls.CheckBox;
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
            await ContentDialogHelper.ShowAsync(new OperationFailedDialog(OperationFailedCollection), this);
        }

        #endregion 第二部分：大写小写页面——挂载的事件

        /// <summary>
        /// 添加到大写小写页面
        /// </summary>
        public void AddtoUpperAndLowerCasePage(List<OldAndNewNameModel> upperAndLowerCaseList)
        {
            MainWindow.Current.BeginInvoke(() =>
            {
                lock (upperAndLowerCaseLock)
                {
                    foreach (OldAndNewNameModel oldAndNewNameItem in upperAndLowerCaseList)
                    {
                        UpperAndLowerCaseCollection.Add(oldAndNewNameItem);
                    }
                }
            });
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
                        foreach (OldAndNewNameModel oldAndNewNameItem in UpperAndLowerCaseCollection)
                        {
                            if (!string.IsNullOrEmpty(oldAndNewNameItem.OriginalFileName))
                            {
                                oldAndNewNameItem.NewFileName = oldAndNewNameItem.OriginalFileName.ToUpper();
                                oldAndNewNameItem.NewFilePath = oldAndNewNameItem.OriginalFilePath.Replace(oldAndNewNameItem.OriginalFileName, oldAndNewNameItem.NewFileName);
                            }
                        }
                        break;
                    }
                case UpperAndLowerSelectedKind.FileNameUppercase:
                    {
                        foreach (OldAndNewNameModel oldAndNewNameItem in UpperAndLowerCaseCollection)
                        {
                            if (!string.IsNullOrEmpty(oldAndNewNameItem.OriginalFileName))
                            {
                                string fileName = Path.GetFileNameWithoutExtension(oldAndNewNameItem.OriginalFileName).ToUpper();
                                string extensionName = Path.GetExtension(oldAndNewNameItem.OriginalFileName);
                                oldAndNewNameItem.NewFileName = fileName + extensionName;
                                oldAndNewNameItem.NewFilePath = oldAndNewNameItem.OriginalFilePath.Replace(oldAndNewNameItem.OriginalFileName, oldAndNewNameItem.NewFileName);
                            }
                        }
                        break;
                    }
                case UpperAndLowerSelectedKind.ExtensionNameUppercase:
                    {
                        foreach (OldAndNewNameModel oldAndNewNameItem in UpperAndLowerCaseCollection)
                        {
                            if (!string.IsNullOrEmpty(oldAndNewNameItem.OriginalFileName))
                            {
                                string fileName = Path.GetFileNameWithoutExtension(oldAndNewNameItem.OriginalFileName);
                                string extensionName = Path.GetExtension(oldAndNewNameItem.OriginalFileName).ToUpper();
                                oldAndNewNameItem.NewFileName = fileName + extensionName;
                                oldAndNewNameItem.NewFilePath = oldAndNewNameItem.OriginalFilePath.Replace(oldAndNewNameItem.OriginalFileName, oldAndNewNameItem.NewFileName);
                            }
                        }
                        break;
                    }
                case UpperAndLowerSelectedKind.DeleteSpace:
                    {
                        foreach (OldAndNewNameModel oldAndNewNameItem in UpperAndLowerCaseCollection)
                        {
                            if (!string.IsNullOrEmpty(oldAndNewNameItem.OriginalFileName))
                            {
                                oldAndNewNameItem.NewFileName = oldAndNewNameItem.OriginalFileName.Replace(" ", string.Empty);
                                oldAndNewNameItem.NewFilePath = oldAndNewNameItem.OriginalFilePath.Replace(oldAndNewNameItem.OriginalFileName, oldAndNewNameItem.NewFileName);
                            }
                        }
                        break;
                    }
                case UpperAndLowerSelectedKind.AllLowercase:
                    {
                        foreach (OldAndNewNameModel oldAndNewNameItem in UpperAndLowerCaseCollection)
                        {
                            if (!string.IsNullOrEmpty(oldAndNewNameItem.OriginalFileName))
                            {
                                oldAndNewNameItem.NewFileName = oldAndNewNameItem.OriginalFileName.ToLower();
                                oldAndNewNameItem.NewFilePath = oldAndNewNameItem.OriginalFilePath.Replace(oldAndNewNameItem.OriginalFileName, oldAndNewNameItem.NewFileName);
                            }
                        }
                        break;
                    }
                case UpperAndLowerSelectedKind.FileNameLowercase:
                    {
                        foreach (OldAndNewNameModel oldAndNewNameItem in UpperAndLowerCaseCollection)
                        {
                            if (!string.IsNullOrEmpty(oldAndNewNameItem.OriginalFileName))
                            {
                                string fileName = Path.GetFileNameWithoutExtension(oldAndNewNameItem.OriginalFileName).ToLower();
                                string extensionName = Path.GetExtension(oldAndNewNameItem.OriginalFileName);
                                oldAndNewNameItem.NewFileName = fileName + extensionName;
                                oldAndNewNameItem.NewFilePath = oldAndNewNameItem.OriginalFilePath.Replace(oldAndNewNameItem.OriginalFileName, oldAndNewNameItem.NewFileName);
                            }
                        }
                        break;
                    }
                case UpperAndLowerSelectedKind.ExtensionNameLowercase:
                    {
                        foreach (OldAndNewNameModel oldAndNewNameItem in UpperAndLowerCaseCollection)
                        {
                            if (!string.IsNullOrEmpty(oldAndNewNameItem.OriginalFileName))
                            {
                                string fileName = Path.GetFileNameWithoutExtension(oldAndNewNameItem.OriginalFileName);
                                string extensionName = Path.GetExtension(oldAndNewNameItem.OriginalFileName).ToLower();
                                oldAndNewNameItem.NewFileName = fileName + extensionName;
                                oldAndNewNameItem.NewFilePath = oldAndNewNameItem.OriginalFilePath.Replace(oldAndNewNameItem.OriginalFileName, oldAndNewNameItem.NewFileName);
                            }
                        }
                        break;
                    }
                case UpperAndLowerSelectedKind.ReplaceSpace:
                    {
                        foreach (OldAndNewNameModel oldAndNewNameItem in UpperAndLowerCaseCollection)
                        {
                            if (!string.IsNullOrEmpty(oldAndNewNameItem.OriginalFileName))
                            {
                                oldAndNewNameItem.NewFileName = oldAndNewNameItem.OriginalFileName.Replace(" ", "_");
                                oldAndNewNameItem.NewFilePath = oldAndNewNameItem.OriginalFilePath.Replace(oldAndNewNameItem.OriginalFileName, oldAndNewNameItem.NewFileName);
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
            List<OperationFailedModel> operationFailedList = [];
            IsModifyingNow = true;
            Task.Run(async () =>
            {
                foreach (OldAndNewNameModel oldAndNewNameItem in UpperAndLowerCaseCollection)
                {
                    if (!string.IsNullOrEmpty(oldAndNewNameItem.OriginalFileName) && !string.IsNullOrEmpty(oldAndNewNameItem.OriginalFilePath))
                    {
                        if (IOHelper.IsDir(oldAndNewNameItem.OriginalFilePath))
                        {
                            try
                            {
                                Directory.Move(oldAndNewNameItem.OriginalFilePath, oldAndNewNameItem.NewFilePath);
                            }
                            catch (Exception e)
                            {
                                operationFailedList.Add(new OperationFailedModel()
                                {
                                    FileName = oldAndNewNameItem.OriginalFileName,
                                    FilePath = oldAndNewNameItem.OriginalFilePath,
                                    Exception = e
                                });
                            }
                        }
                        else
                        {
                            try
                            {
                                File.Move(oldAndNewNameItem.OriginalFilePath, oldAndNewNameItem.NewFilePath);
                            }
                            catch (Exception e)
                            {
                                operationFailedList.Add(new OperationFailedModel()
                                {
                                    FileName = oldAndNewNameItem.OriginalFileName,
                                    FilePath = oldAndNewNameItem.OriginalFilePath,
                                    Exception = e
                                });
                            }
                        }
                    }
                }

                await Task.Delay(300);

                MainWindow.Current.BeginInvoke(() =>
                {
                    IsModifyingNow = false;
                    foreach (OperationFailedModel operationFailedItem in operationFailedList)
                    {
                        OperationFailedCollection.Add(operationFailedItem);
                    }

                    TeachingTipHelper.Show(new OperationResultTip(OperationKind.File, UpperAndLowerCaseCollection.Count - OperationFailedCollection.Count, OperationFailedCollection.Count));
                    lock (upperAndLowerCaseLock)
                    {
                        UpperAndLowerCaseCollection.Clear();
                    }
                });
            });
        }
    }
}
