using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
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
using WindowsTools.Models;
using WindowsTools.Services.Root;
using WindowsTools.Strings;
using WindowsTools.UI.Dialogs;
using WindowsTools.UI.TeachingTips;
using WindowsTools.Views.Windows;

namespace WindowsTools.Views.Pages
{
    /// <summary>
    /// 文件属性页面
    /// </summary>
    public sealed partial class FilePropertiesPage : Page, INotifyPropertyChanged
    {
        private bool _isReadOnlyChecked = false;

        public bool IsReadOnlyChecked
        {
            get { return _isReadOnlyChecked; }

            set
            {
                _isReadOnlyChecked = value;
                OnPropertyChanged();
            }
        }

        private bool _isArchiveChecked = false;

        public bool IsArchiveChecked
        {
            get { return _isArchiveChecked; }

            set
            {
                _isArchiveChecked = value;
                OnPropertyChanged();
            }
        }

        private bool _isCreateDateChecked = false;

        public bool IsCreateDateChecked
        {
            get { return _isCreateDateChecked; }

            set
            {
                _isCreateDateChecked = value;
                OnPropertyChanged();
            }
        }

        private bool _isHideChecked = false;

        public bool IsHideChecked
        {
            get { return _isHideChecked; }

            set
            {
                _isHideChecked = value;
                OnPropertyChanged();
            }
        }

        private bool _isSystemChecked = false;

        public bool IsSystemChecked
        {
            get { return _isSystemChecked; }

            set
            {
                _isSystemChecked = value;
                OnPropertyChanged();
            }
        }

        private bool _isModifyDateChecked = false;

        public bool IsModifyDateChecked
        {
            get { return _isModifyDateChecked; }

            set
            {
                _isModifyDateChecked = value;
                OnPropertyChanged();
            }
        }

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

        private DateTimeOffset _createDate = DateTimeOffset.Now;

        public DateTimeOffset CreateDate
        {
            get { return _createDate; }

            set
            {
                _createDate = value;
                OnPropertyChanged();
            }
        }

        private TimeSpan _createTime = DateTimeOffset.Now.TimeOfDay;

        public TimeSpan CreateTime
        {
            get { return _createTime; }

            set
            {
                _createTime = value;
                OnPropertyChanged();
            }
        }

        private DateTimeOffset _modifyDate = DateTimeOffset.Now;

        public DateTimeOffset ModifyDate
        {
            get { return _modifyDate; }

            set
            {
                _modifyDate = value;
                OnPropertyChanged();
            }
        }

        private TimeSpan _modifyTime = DateTimeOffset.Now.TimeOfDay;

        public TimeSpan ModifyTime
        {
            get { return _modifyTime; }

            set
            {
                _modifyTime = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<OldAndNewPropertiesModel> FilePropertiesCollection { get; } = new ObservableCollection<OldAndNewPropertiesModel>();

        private ObservableCollection<OperationFailedModel> OperationFailedCollection { get; } = new ObservableCollection<OperationFailedModel>();

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
            args.DragUIOverride.Caption = FileProperties.DragOverContent;
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
                        FilePropertiesCollection.Add(new OldAndNewPropertiesModel()
                        {
                            FileName = item.Name,
                            FilePath = item.Path
                        });
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
                bool checkResult = CheckOperationState();
                if (checkResult)
                {
                    OperationFailedCollection.Clear();
                    if (FilePropertiesCollection.Count is 0)
                    {
                        TeachingTipHelper.Show(new ListEmptyTip());
                    }
                    else
                    {
                        PreviewChangedFileAttributes();
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
                    if (FilePropertiesCollection.Count is 0)
                    {
                        TeachingTipHelper.Show(new ListEmptyTip());
                    }
                    else
                    {
                        PreviewChangedFileAttributes();
                        ChangeFileAttributes();
                    }
                }
                else
                {
                    TeachingTipHelper.Show(new NoOperationTip());
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
            FilePropertiesCollection.Clear();
            OperationFailedCollection.Clear();
        }

        /// <summary>
        /// 日期更改时触发的事件
        /// </summary>
        private void OnDateChanged(object sender, DatePickerValueChangedEventArgs args)
        {
            DatePicker datePicker = sender as DatePicker;
            if (datePicker is not null && datePicker.Tag is not null)
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
        private void OnPreviewClicked(object sender, RoutedEventArgs args)
        {
            bool checkResult = CheckOperationState();
            if (checkResult)
            {
                OperationFailedCollection.Clear();
                if (FilePropertiesCollection.Count is 0)
                {
                    TeachingTipHelper.Show(new ListEmptyTip());
                }
                else
                {
                    PreviewChangedFileAttributes();
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
                if (FilePropertiesCollection.Count is 0)
                {
                    TeachingTipHelper.Show(new ListEmptyTip());
                }
                else
                {
                    PreviewChangedFileAttributes();
                    ChangeFileAttributes();
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
                        FilePropertiesCollection.Add(new OldAndNewPropertiesModel()
                        {
                            FileName = file.Name,
                            FilePath = file.FullName
                        });
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
                            FilePropertiesCollection.Add(new OldAndNewPropertiesModel()
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
                            FilePropertiesCollection.Add(new OldAndNewPropertiesModel()
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
        /// 时间更改时触发的事件
        /// </summary>
        private void OnTimeChanged(object sender, TimePickerValueChangedEventArgs args)
        {
            TimePicker timePicker = sender as TimePicker;
            if (timePicker is not null && timePicker.Tag is not null)
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
            global::Windows.UI.Xaml.Controls.CheckBox checkBox = sender as global::Windows.UI.Xaml.Controls.CheckBox;
            if (checkBox is not null)
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
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string LocalizeTotal(int count)
        {
            return string.Format(FileProperties.Total, FilePropertiesCollection.Count);
        }

        /// <summary>
        /// 检查用户是否指定了操作过程
        /// </summary>
        private bool CheckOperationState()
        {
            if (IsReadOnlyChecked is false && IsArchiveChecked is false && IsCreateDateChecked is false && IsHideChecked is false && IsSystemChecked is false && IsModifyDateChecked is false)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 修改文件属性
        /// </summary>
        private void PreviewChangedFileAttributes()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (IsReadOnlyChecked)
            {
                stringBuilder.Append(FileProperties.ReadOnly);
                stringBuilder.Append(" ");
            }
            if (IsArchiveChecked)
            {
                stringBuilder.Append(FileProperties.Archive);
                stringBuilder.Append(" ");
            }
            if (IsHideChecked)
            {
                stringBuilder.Append(FileProperties.Hide);
                stringBuilder.Append(" ");
            }
            if (IsSystemChecked)
            {
                stringBuilder.Append(FileProperties.System);
                stringBuilder.Append(" ");
            }
            if (IsCreateDateChecked)
            {
                stringBuilder.Append(FileProperties.CreateDate);
                stringBuilder.Append(" ");
            }
            if (IsModifyDateChecked)
            {
                stringBuilder.Append(FileProperties.ModifyDate);
                stringBuilder.Append(" ");
            }

            foreach (OldAndNewPropertiesModel item in FilePropertiesCollection)
            {
                item.FileProperties = stringBuilder.ToString();
            }
        }

        /// <summary>
        /// 更改文件属性
        /// </summary>
        private void ChangeFileAttributes()
        {
            List<OperationFailedModel> operationFailedList = new List<OperationFailedModel>();
            IsModifyingNow = true;
            Task.Run(async () =>
            {
                foreach (OldAndNewPropertiesModel item in FilePropertiesCollection)
                {
                    if (!string.IsNullOrEmpty(item.FileName) && !string.IsNullOrEmpty(item.FilePath))
                    {
                        try
                        {
                            System.IO.FileAttributes fileAttributes = File.GetAttributes(item.FilePath);
                            if (IsReadOnlyChecked) fileAttributes |= System.IO.FileAttributes.ReadOnly;
                            if (IsArchiveChecked) fileAttributes |= System.IO.FileAttributes.Archive;
                            if (IsHideChecked) fileAttributes |= System.IO.FileAttributes.Hidden;
                            if (IsSystemChecked) fileAttributes |= System.IO.FileAttributes.System;
                            File.SetAttributes(item.FilePath, fileAttributes);

                            if (IsCreateDateChecked)
                            {
                                File.SetCreationTime(item.FilePath, CreateDate.Date + CreateTime);
                            }
                            if (IsModifyDateChecked)
                            {
                                File.SetLastWriteTime(item.FilePath, ModifyDate.Date + ModifyTime);
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

                    TeachingTipHelper.Show(new OperationResultTip(OperationKind.File, FilePropertiesCollection.Count - OperationFailedCollection.Count, OperationFailedCollection.Count));
                    FilePropertiesCollection.Clear();
                });
            });
        }
    }
}
