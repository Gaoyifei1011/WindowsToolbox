using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.IO;
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
using WindowsTools.Helpers.Root;
using WindowsTools.Models;
using WindowsTools.Services.Root;
using WindowsTools.Strings;
using WindowsTools.UI.Dialogs;
using WindowsTools.UI.TeachingTips;
using WindowsTools.WindowsAPI.ComTypes;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace WindowsTools.Views.Pages
{
    /// <summary>
    /// 扩展名称页面
    /// </summary>
    public sealed partial class ExtensionNamePage : Page, INotifyPropertyChanged
    {
        private readonly SynchronizationContext synchronizationContext = SynchronizationContext.Current;
        private readonly object extensionNameLock = new();

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

        private ExtensionNameSelectedKind _selectedType = ExtensionNameSelectedKind.None;

        public ExtensionNameSelectedKind SelectedType
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

        private string _changeToText;

        public string ChangeToText
        {
            get { return _changeToText; }

            set
            {
                if (!Equals(_changeToText, value))
                {
                    _changeToText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ChangeToText)));
                }
            }
        }

        private string _searchText;

        public string SearchText
        {
            get { return _searchText; }

            set
            {
                if (!Equals(_searchText, value))
                {
                    _searchText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchText)));
                }
            }
        }

        private string _replaceText;

        public string ReplaceText
        {
            get { return _replaceText; }

            set
            {
                if (!Equals(_replaceText, value))
                {
                    _replaceText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReplaceText)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<OldAndNewNameModel> ExtensionNameCollection { get; } = [];

        private ObservableCollection<OperationFailedModel> OperationFailedCollection { get; } = [];

        public ExtensionNamePage()
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
            args.DragUIOverride.Caption = ExtensionName.DragOverContent;
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
                        List<OldAndNewNameModel> extensionNameList = [];
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

                                    extensionNameList.Add(new()
                                    {
                                        OriginalFileName = storageItem.Name,
                                        OriginalFilePath = storageItem.Path
                                    });
                                }
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(EventLevel.Error, string.Format("Read file {0} information failed", storageItem.Path), e);
                                continue;
                            }
                        }

                        AddToExtensionNamePage(extensionNameList);
                    });
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Warning, "Drop file in extension name page failed", e);
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
        protected override async void OnKeyDown(KeyRoutedEventArgs args)
        {
            base.OnKeyDown(args);
            if (args.Key is VirtualKey.Enter)
            {
                args.Handled = true;
                bool checkResult = CheckOperationState();
                if (checkResult)
                {
                    OperationFailedCollection.Clear();
                    int count = 0;

                    lock (extensionNameLock)
                    {
                        count = ExtensionNameCollection.Count;
                    }

                    if (count is 0)
                    {
                        await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.ListEmpty));
                    }
                    else
                    {
                        PreviewChangedFileName();
                    }
                }
                else
                {
                    await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.NoOperation));
                }
            }
            else if (args.Key is VirtualKey.Control && args.Key is VirtualKey.Enter)
            {
                args.Handled = true;
                bool checkResult = CheckOperationState();
                if (checkResult)
                {
                    OperationFailedCollection.Clear();
                    int count = 0;

                    lock (extensionNameLock)
                    {
                        count = ExtensionNameCollection.Count;
                    }

                    if (count is 0)
                    {
                        await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.ListEmpty));
                    }
                    else
                    {
                        PreviewChangedFileName();
                        ChangeFileName();
                    }
                }
                else
                {
                    await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.NoOperation));
                }
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：扩展名称页面——挂载的事件

        /// <summary>
        /// 当文本框中的内容发生更改时发生的事件。
        /// </summary>
        private void OnTextChanged(object sender, TextChangedEventArgs args)
        {
            global::Windows.UI.Xaml.Controls.TextBox textBox = sender as global::Windows.UI.Xaml.Controls.TextBox;

            if (textBox is not null)
            {
                string tag = Convert.ToString(textBox.Tag);

                if (tag is "ChangeToText")
                {
                    ChangeToText = textBox.Text;
                }
                else if (tag is "SearchText")
                {
                    SearchText = textBox.Text;
                }
                else if (tag is "ReplaceText")
                {
                    ReplaceText = textBox.Text;
                }
            }
        }

        /// <summary>
        /// 选中时触发的事件
        /// </summary>
        private void OnChecked(object sender, RoutedEventArgs args)
        {
            global::Windows.UI.Xaml.Controls.CheckBox checkBox = sender as global::Windows.UI.Xaml.Controls.CheckBox;
            if (checkBox is not null)
            {
                SelectedType = (ExtensionNameSelectedKind)Convert.ToInt32(checkBox.Tag);
            }
        }

        /// <summary>
        /// 清空列表
        /// </summary>
        private void OnClearListClicked(object sender, RoutedEventArgs args)
        {
            lock (extensionNameLock)
            {
                ExtensionNameCollection.Clear();
                OperationFailedCollection.Clear();
            }
        }

        /// <summary>
        /// 预览修改的内容
        /// </summary>
        private async void OnPreviewClicked(object sender, RoutedEventArgs args)
        {
            bool checkResult = CheckOperationState();
            if (checkResult)
            {
                OperationFailedCollection.Clear();
                int count = 0;

                lock (extensionNameLock)
                {
                    count = ExtensionNameCollection.Count;
                }

                if (count is 0)
                {
                    await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.ListEmpty));
                }
                else
                {
                    PreviewChangedFileName();
                }
            }
            else
            {
                await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.NoOperation));
            }
        }

        /// <summary>
        /// 修改内容
        /// </summary>
        private async void OnModifyClicked(object sender, RoutedEventArgs args)
        {
            bool checkResult = CheckOperationState();
            if (checkResult)
            {
                OperationFailedCollection.Clear();
                int count = 0;

                lock (extensionNameLock)
                {
                    count = ExtensionNameCollection.Count;
                }

                if (count is 0)
                {
                    await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.ListEmpty));
                }
                else
                {
                    PreviewChangedFileName();
                    ChangeFileName();
                }
            }
            else
            {
                await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.NoOperation));
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
                Title = ExtensionName.SelectFile
            };
            if (dialog.ShowDialog() is DialogResult.OK)
            {
                Task.Run(() =>
                {
                    List<OldAndNewNameModel> extensionNameList = [];

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
                                extensionNameList.Add(new()
                                {
                                    OriginalFileName = fileInfo.Name,
                                    OriginalFilePath = fileInfo.FullName
                                });
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, string.Format("Read file {0} information failed", fileName), e);
                            continue;
                        }
                    }

                    dialog.Dispose();
                    AddToExtensionNamePage(extensionNameList);
                });
            }
            else
            {
                dialog.Dispose();
            }
        }

        /// <summary>
        /// 选择文件夹
        /// </summary>
        private void OnSelectFolderClicked(object sender, RoutedEventArgs args)
        {
            OpenFolderDialog dialog = new()
            {
                Description = ExtensionName.SelectFolder,
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
                        List<OldAndNewNameModel> fileNameList = [];

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
                            LogService.WriteLog(EventLevel.Error, string.Format("Read folder {0} information failed", dialog.SelectedPath), e);
                        }

                        dialog.Dispose();
                        AddToExtensionNamePage(fileNameList);
                    });
                }
            }
            else
            {
                dialog.Dispose();
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
                if (SelectedType == (ExtensionNameSelectedKind)Convert.ToInt32(checkBox.Tag))
                {
                    SelectedType = ExtensionNameSelectedKind.None;
                }

                if ((ExtensionNameSelectedKind)Convert.ToInt32(checkBox.Tag) is ExtensionNameSelectedKind.IsSameExtensionName)
                {
                    ChangeToText = string.Empty;
                }
                else if ((ExtensionNameSelectedKind)Convert.ToInt32(checkBox.Tag) is ExtensionNameSelectedKind.IsFindAndReplaceExtensionName)
                {
                    SearchText = string.Empty;
                    ReplaceText = string.Empty;
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

        #endregion 第二部分：扩展名称页面——挂载的事件

        /// <summary>
        /// 添加到扩展名称页面
        /// </summary>
        public void AddToExtensionNamePage(List<OldAndNewNameModel> extensionNameList)
        {
            synchronizationContext.Post(_ =>
            {
                lock (extensionNameLock)
                {
                    foreach (OldAndNewNameModel oldAndNewNameItem in extensionNameList)
                    {
                        ExtensionNameCollection.Add(oldAndNewNameItem);
                    }
                }
            }, null);
        }

        /// <summary>
        /// 检查用户是否指定了操作过程
        /// </summary>
        private bool CheckOperationState()
        {
            return SelectedType is not ExtensionNameSelectedKind.None;
        }

        /// <summary>
        /// 预览修改后的文件名称
        /// </summary>
        private void PreviewChangedFileName()
        {
            switch (SelectedType)
            {
                case ExtensionNameSelectedKind.IsSameExtensionName:
                    {
                        lock (extensionNameLock)
                        {
                            foreach (OldAndNewNameModel oldAndNewNameItem in ExtensionNameCollection)
                            {
                                if (!string.IsNullOrEmpty(oldAndNewNameItem.OriginalFileName))
                                {
                                    string fileName = Path.GetFileNameWithoutExtension(oldAndNewNameItem.OriginalFileName);
                                    oldAndNewNameItem.NewFileName = fileName + "." + ChangeToText;
                                    oldAndNewNameItem.NewFilePath = oldAndNewNameItem.OriginalFilePath.Replace(oldAndNewNameItem.OriginalFileName, oldAndNewNameItem.NewFileName);
                                }
                            }
                        }

                        break;
                    }
                case ExtensionNameSelectedKind.IsFindAndReplaceExtensionName:
                    {
                        lock (extensionNameLock)
                        {
                            foreach (OldAndNewNameModel oldAndNewNameItem in ExtensionNameCollection)
                            {
                                if (!string.IsNullOrEmpty(oldAndNewNameItem.OriginalFileName))
                                {
                                    string fileName = Path.GetFileNameWithoutExtension(oldAndNewNameItem.OriginalFileName);
                                    string extensionName = Path.GetExtension(oldAndNewNameItem.OriginalFileName);
                                    if (extensionName.Contains(SearchText))
                                    {
                                        extensionName = extensionName.Replace(SearchText, ReplaceText);
                                    }
                                    oldAndNewNameItem.NewFileName = fileName + extensionName;
                                    oldAndNewNameItem.NewFilePath = oldAndNewNameItem.OriginalFilePath.Replace(oldAndNewNameItem.OriginalFileName, oldAndNewNameItem.NewFileName);
                                }
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
                lock (extensionNameLock)
                {
                    foreach (OldAndNewNameModel oldAndNewNameItem in ExtensionNameCollection)
                    {
                        if (!string.IsNullOrEmpty(oldAndNewNameItem.OriginalFileName) && !string.IsNullOrEmpty(oldAndNewNameItem.OriginalFilePath))
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

                synchronizationContext.Post(async (_) =>
                {
                    IsModifyingNow = false;
                    foreach (OperationFailedModel operationFailedItem in operationFailedList)
                    {
                        OperationFailedCollection.Add(operationFailedItem);
                    }

                    lock (extensionNameLock)
                    {
                        ExtensionNameCollection.Clear();
                    }

                    await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.File, ExtensionNameCollection.Count - OperationFailedCollection.Count, OperationFailedCollection.Count));
                }, null);
            });
        }
    }
}
