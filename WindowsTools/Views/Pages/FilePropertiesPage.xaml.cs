using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.IO;
using System.Text;
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
using WindowsTools.Models;
using WindowsTools.Services.Root;
using WindowsTools.UI.Dialogs;
using WindowsTools.UI.TeachingTips;
using WindowsTools.WindowsAPI.ComTypes;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace WindowsTools.Views.Pages
{
    /// <summary>
    /// 文件属性页面
    /// </summary>
    public sealed partial class FilePropertiesPage : Page, INotifyPropertyChanged
    {
        private readonly SynchronizationContext synchronizationContext = SynchronizationContext.Current;
        private readonly object filePropertiesLock = new();

        private string Total { get; } = ResourceService.FilePropertiesResource.GetString("Total");

        private bool _isReadOnlyChecked = false;

        public bool IsReadOnlyChecked
        {
            get { return _isReadOnlyChecked; }

            set
            {
                if (!Equals(_isReadOnlyChecked, value))
                {
                    _isReadOnlyChecked = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsReadOnlyChecked)));
                }
            }
        }

        private bool _isArchiveChecked = false;

        public bool IsArchiveChecked
        {
            get { return _isArchiveChecked; }

            set
            {
                if (!Equals(_isArchiveChecked, value))
                {
                    _isArchiveChecked = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsArchiveChecked)));
                }
            }
        }

        private bool _isCreateDateChecked = false;

        public bool IsCreateDateChecked
        {
            get { return _isCreateDateChecked; }

            set
            {
                if (!Equals(_isCreateDateChecked, value))
                {
                    _isCreateDateChecked = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCreateDateChecked)));
                }
            }
        }

        private bool _isHideChecked = false;

        public bool IsHideChecked
        {
            get { return _isHideChecked; }

            set
            {
                if (!Equals(_isHideChecked, value))
                {
                    _isHideChecked = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsHideChecked)));
                }
            }
        }

        private bool _isSystemChecked = false;

        public bool IsSystemChecked
        {
            get { return _isSystemChecked; }

            set
            {
                if (!Equals(_isSystemChecked, value))
                {
                    _isSystemChecked = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSystemChecked)));
                }
            }
        }

        private bool _isModifyDateChecked = false;

        public bool IsModifyDateChecked
        {
            get { return _isModifyDateChecked; }

            set
            {
                if (!Equals(_isModifyDateChecked, value))
                {
                    _isModifyDateChecked = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsModifyDateChecked)));
                }
            }
        }

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

        private DateTimeOffset _createDate = DateTimeOffset.Now;

        public DateTimeOffset CreateDate
        {
            get { return _createDate; }

            set
            {
                if (!Equals(_createDate, value))
                {
                    _createDate = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CreateDate)));
                }
            }
        }

        private TimeSpan _createTime = DateTimeOffset.Now.TimeOfDay;

        public TimeSpan CreateTime
        {
            get { return _createTime; }

            set
            {
                if (!Equals(_createTime, value))
                {
                    _createTime = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CreateTime)));
                }
            }
        }

        private DateTimeOffset _modifyDate = DateTimeOffset.Now;

        public DateTimeOffset ModifyDate
        {
            get { return _modifyDate; }

            set
            {
                if (!Equals(_modifyDate, value))
                {
                    _modifyDate = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ModifyDate)));
                }
            }
        }

        private TimeSpan _modifyTime = DateTimeOffset.Now.TimeOfDay;

        public TimeSpan ModifyTime
        {
            get { return _modifyTime; }

            set
            {
                if (!Equals(_modifyTime, value))
                {
                    _modifyTime = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ModifyTime)));
                }
            }
        }

        private ObservableCollection<OldAndNewPropertiesModel> FilePropertiesCollection { get; } = [];

        private ObservableCollection<OperationFailedModel> OperationFailedCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public FilePropertiesPage()
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
            args.DragUIOverride.Caption = ResourceService.FilePropertiesResource.GetString("DragOverContent");
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
                        List<OldAndNewPropertiesModel> filePropertiesList = [];

                        foreach (IStorageItem storageItem in storageItemList)
                        {
                            try
                            {
                                FileInfo fileInfo = new(storageItem.Path);
                                if ((fileInfo.Attributes & System.IO.FileAttributes.Hidden) is System.IO.FileAttributes.Hidden)
                                {
                                    continue;
                                }

                                filePropertiesList.Add(new OldAndNewPropertiesModel()
                                {
                                    FileName = storageItem.Name,
                                    FilePath = storageItem.Path,
                                });
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(EventLevel.Error, string.Format("Read file {0} information failed", storageItem.Path), e);
                                continue;
                            }
                        }

                        AddToFilePropertiesPage(filePropertiesList);
                    });
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Warning, "Drop file in file properties page failed", e);
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

                    lock (filePropertiesLock)
                    {
                        count = FilePropertiesCollection.Count;
                    }

                    if (count is 0)
                    {
                        await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.ListEmpty));
                    }
                    else
                    {
                        PreviewChangedFileAttributes();
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

                    lock (filePropertiesLock)
                    {
                        count = FilePropertiesCollection.Count;
                    }

                    if (count is 0)
                    {
                        await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.ListEmpty));
                    }
                    else
                    {
                        PreviewChangedFileAttributes();
                        ChangeFileAttributes();
                    }
                }
                else
                {
                    await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.NoOperation));
                }
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：文件属性页面——挂载的事件

        /// <summary>
        /// 清空列表
        /// </summary>
        private void OnClearListClicked(object sender, RoutedEventArgs args)
        {
            lock (filePropertiesLock)
            {
                FilePropertiesCollection.Clear();
                OperationFailedCollection.Clear();
            }
        }

        /// <summary>
        /// 日期更改时触发的事件
        /// </summary>
        private void OnDateChanged(object sender, DatePickerValueChangedEventArgs args)
        {
            if (sender is DatePicker datePicker && datePicker.Tag is not null)
            {
                if (datePicker.Tag.ToString() == nameof(CreateDate))
                {
                    CreateDate = args.NewDate;
                }
                else if (datePicker.Tag.ToString() == nameof(ModifyDate))
                {
                    ModifyDate = args.NewDate;
                }
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

                lock (filePropertiesLock)
                {
                    count = FilePropertiesCollection.Count;
                }

                if (count is 0)
                {
                    await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.ListEmpty));
                }
                else
                {
                    PreviewChangedFileAttributes();
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

                lock (filePropertiesLock)
                {
                    count = FilePropertiesCollection.Count;
                }

                if (count is 0)
                {
                    await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.ListEmpty));
                }
                else
                {
                    PreviewChangedFileAttributes();
                    ChangeFileAttributes();
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
                Title = ResourceService.FilePropertiesResource.GetString("SelectFile")
            };
            if (dialog.ShowDialog() is DialogResult.OK)
            {
                Task.Run(() =>
                {
                    List<OldAndNewPropertiesModel> filePropertiesList = [];

                    foreach (string fileName in dialog.FileNames)
                    {
                        try
                        {
                            FileInfo fileInfo = new(fileName);
                            if ((fileInfo.Attributes & System.IO.FileAttributes.Hidden) is System.IO.FileAttributes.Hidden)
                            {
                                continue;
                            }

                            filePropertiesList.Add(new OldAndNewPropertiesModel()
                            {
                                FileName = fileInfo.Name,
                                FilePath = fileInfo.FullName,
                            });
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, string.Format("Read file {0} information failed", fileName), e);
                            continue;
                        }
                    }

                    dialog.Dispose();
                    AddToFilePropertiesPage(filePropertiesList);
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
                Description = ResourceService.FilePropertiesResource.GetString("SelectFolder"),
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
                        List<OldAndNewPropertiesModel> directoryNameList = [];
                        List<OldAndNewPropertiesModel> fileNameList = [];

                        try
                        {
                            foreach (DirectoryInfo directoryInfo in currentFolder.GetDirectories())
                            {
                                if ((directoryInfo.Attributes & System.IO.FileAttributes.Hidden) is System.IO.FileAttributes.Hidden)
                                {
                                    continue;
                                }

                                directoryNameList.Add(new OldAndNewPropertiesModel()
                                {
                                    FileName = directoryInfo.Name,
                                    FilePath = directoryInfo.FullName
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

                                fileNameList.Add(new OldAndNewPropertiesModel()
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

                        dialog.Dispose();
                        AddToFilePropertiesPage(directoryNameList);
                        AddToFilePropertiesPage(fileNameList);
                    });
                }
            }
            else
            {
                dialog.Dispose();
            }
        }

        /// <summary>
        /// 时间更改时触发的事件
        /// </summary>
        private void OnTimeChanged(object sender, TimePickerValueChangedEventArgs args)
        {
            if (sender is TimePicker timePicker && timePicker.Tag is not null)
            {
                if (timePicker.Tag.ToString() == nameof(CreateTime))
                {
                    CreateTime = args.NewTime;
                }
                else if (timePicker.Tag.ToString() == nameof(ModifyTime))
                {
                    ModifyTime = args.NewTime;
                }
            }
        }

        /// <summary>
        /// 取消选中时触发的事件
        /// </summary>
        private void OnUnchecked(object sender, RoutedEventArgs args)
        {
            if (sender is global::Windows.UI.Xaml.Controls.CheckBox checkBox)
            {
                if (checkBox.Tag.ToString() == nameof(CreateDate))
                {
                    CreateDate = DateTimeOffset.Now;
                    CreateTime = DateTimeOffset.Now.TimeOfDay;
                }
                else if (checkBox.Tag.ToString() == nameof(ModifyDate))
                {
                    ModifyDate = DateTimeOffset.Now;
                    ModifyTime = DateTimeOffset.Now.TimeOfDay;
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

        #endregion 第二部分：文件属性页面——挂载的事件

        /// <summary>
        /// 添加到文件属性页面
        /// </summary>
        public void AddToFilePropertiesPage(List<OldAndNewPropertiesModel> filePropertiesList)
        {
            synchronizationContext.Post(_ =>
            {
                lock (filePropertiesLock)
                {
                    foreach (OldAndNewPropertiesModel oldAndNewPropertiesItem in filePropertiesList)
                    {
                        FilePropertiesCollection.Add(oldAndNewPropertiesItem);
                    }
                }
            }, null);
        }

        /// <summary>
        /// 检查用户是否指定了操作过程
        /// </summary>
        private bool CheckOperationState()
        {
            return IsReadOnlyChecked is true || IsArchiveChecked is true || IsCreateDateChecked is true || IsHideChecked is true || IsSystemChecked is true || IsModifyDateChecked is true;
        }

        /// <summary>
        /// 修改文件属性
        /// </summary>
        private void PreviewChangedFileAttributes()
        {
            StringBuilder stringBuilder = new();
            if (IsReadOnlyChecked)
            {
                stringBuilder.Append(ResourceService.FilePropertiesResource.GetString("ReadOnly"));
                stringBuilder.Append(' ');
            }
            if (IsArchiveChecked)
            {
                stringBuilder.Append(ResourceService.FilePropertiesResource.GetString("Archive"));
                stringBuilder.Append(' ');
            }
            if (IsHideChecked)
            {
                stringBuilder.Append(ResourceService.FilePropertiesResource.GetString("Hide"));
                stringBuilder.Append(' ');
            }
            if (IsSystemChecked)
            {
                stringBuilder.Append(ResourceService.FilePropertiesResource.GetString("System"));
                stringBuilder.Append(' ');
            }
            if (IsCreateDateChecked)
            {
                stringBuilder.Append(ResourceService.FilePropertiesResource.GetString("CreateDate"));
                stringBuilder.Append(' ');
            }
            if (IsModifyDateChecked)
            {
                stringBuilder.Append(ResourceService.FilePropertiesResource.GetString("ModifyDate"));
                stringBuilder.Append(' ');
            }

            lock (filePropertiesLock)
            {
                foreach (OldAndNewPropertiesModel oldAndNewPropertiesItem in FilePropertiesCollection)
                {
                    oldAndNewPropertiesItem.FileProperties = stringBuilder.ToString();
                }
            }
        }

        /// <summary>
        /// 更改文件属性
        /// </summary>
        private void ChangeFileAttributes()
        {
            List<OperationFailedModel> operationFailedList = [];
            IsModifyingNow = true;
            Task.Run(async () =>
            {
                lock (filePropertiesLock)
                {
                    foreach (OldAndNewPropertiesModel oldAndNewPropertiesItem in FilePropertiesCollection)
                    {
                        if (!string.IsNullOrEmpty(oldAndNewPropertiesItem.FileName) && !string.IsNullOrEmpty(oldAndNewPropertiesItem.FilePath))
                        {
                            try
                            {
                                System.IO.FileAttributes fileAttributes = File.GetAttributes(oldAndNewPropertiesItem.FilePath);
                                if (IsReadOnlyChecked) fileAttributes |= System.IO.FileAttributes.ReadOnly;
                                if (IsArchiveChecked) fileAttributes |= System.IO.FileAttributes.Archive;
                                if (IsHideChecked) fileAttributes |= System.IO.FileAttributes.Hidden;
                                if (IsSystemChecked) fileAttributes |= System.IO.FileAttributes.System;
                                File.SetAttributes(oldAndNewPropertiesItem.FilePath, fileAttributes);

                                if (IsCreateDateChecked)
                                {
                                    File.SetCreationTime(oldAndNewPropertiesItem.FilePath, CreateDate.Date + CreateTime);
                                }
                                if (IsModifyDateChecked)
                                {
                                    File.SetLastWriteTime(oldAndNewPropertiesItem.FilePath, ModifyDate.Date + ModifyTime);
                                }
                            }
                            catch (Exception e)
                            {
                                operationFailedList.Add(new OperationFailedModel()
                                {
                                    FileName = oldAndNewPropertiesItem.FileName,
                                    FilePath = oldAndNewPropertiesItem.FilePath,
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

                    int count = FilePropertiesCollection.Count;

                    lock (filePropertiesLock)
                    {
                        FilePropertiesCollection.Clear();
                    }

                    await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.File, count - OperationFailedCollection.Count, OperationFailedCollection.Count));
                }, null);
            });
        }
    }
}
