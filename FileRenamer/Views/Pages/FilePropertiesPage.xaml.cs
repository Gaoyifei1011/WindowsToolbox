﻿using FileRenamer.Models;
using FileRenamer.Services.Root;
using FileRenamer.UI.Notifications;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FileRenamer.Views.Pages
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsReadOnlyChecked)));
            }
        }

        private bool _isArchiveChecked = false;

        public bool IsArchiveChecked
        {
            get { return _isArchiveChecked; }

            set
            {
                _isArchiveChecked = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsArchiveChecked)));
            }
        }

        private bool _isCreateDateChecked = false;

        public bool IsCreateDateChecked
        {
            get { return _isCreateDateChecked; }

            set
            {
                _isCreateDateChecked = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCreateDateChecked)));
            }
        }

        private bool _isHideChecked = false;

        public bool IsHideChecked
        {
            get { return _isHideChecked; }

            set
            {
                _isHideChecked = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsHideChecked)));
            }
        }

        private bool _isSystemChecked = false;

        public bool IsSystemChecked
        {
            get { return _isSystemChecked; }

            set
            {
                _isSystemChecked = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSystemChecked)));
            }
        }

        private bool _isModifyDateChecked = false;

        public bool IsModifyDateChecked
        {
            get { return _isModifyDateChecked; }

            set
            {
                _isModifyDateChecked = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsModifyDateChecked)));
            }
        }

        private DateTimeOffset _createDate = DateTimeOffset.Now;

        public DateTimeOffset CreateDate
        {
            get { return _createDate; }

            set
            {
                _createDate = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CreateDate)));
            }
        }

        private TimeSpan _createTime = DateTimeOffset.Now.TimeOfDay;

        public TimeSpan CreateTime
        {
            get { return _createTime; }

            set
            {
                _createTime = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CreateTime)));
            }
        }

        private DateTimeOffset _modifyDate = DateTimeOffset.Now;

        public DateTimeOffset ModifyDate
        {
            get { return _modifyDate; }

            set
            {
                _modifyDate = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ModifyDate)));
            }
        }

        private TimeSpan _modifyTime = DateTimeOffset.Now.TimeOfDay;

        public TimeSpan ModifyTime
        {
            get { return _modifyTime; }

            set
            {
                _modifyTime = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ModifyTime)));
            }
        }

        public ObservableCollection<OldAndNewPropertiesModel> FilePropertiesDataList { get; } = new ObservableCollection<OldAndNewPropertiesModel>();

        public ObservableCollection<Tuple<OldAndNewPropertiesModel, Exception>> OperationFailedList { get; } = new ObservableCollection<Tuple<OldAndNewPropertiesModel, Exception>>();

        public event PropertyChangedEventHandler PropertyChanged;

        public FilePropertiesPage()
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
            args.DragUIOverride.Caption = ResourceService.GetLocalized("FileProperties/DragOverContent");
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
                        FilePropertiesDataList.Add(new OldAndNewPropertiesModel()
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
                OperationFailedList.Clear();
            }
        }

        public string LocalizeTotal(int count)
        {
            return string.Format(ResourceService.GetLocalized("FileProperties/Total"), FilePropertiesDataList.Count);
        }

        /// <summary>
        /// 清空列表
        /// </summary>
        public void OnClearListClicked(object sender, RoutedEventArgs args)
        {
            FilePropertiesDataList.Clear();
            OperationFailedList.Clear();
        }

        /// <summary>
        /// 日期更改时触发的事件
        /// </summary>
        public void OnDateChanged(object sender, DatePickerValueChangedEventArgs args)
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
        public void OnPreviewClicked(object sender, RoutedEventArgs args)
        {
            bool checkResult = CheckOperationState();
            if (checkResult)
            {
                OperationFailedList.Clear();
                if (FilePropertiesDataList.Count is 0)
                {
                    new ListEmptyNotification().Show();
                }
                else
                {
                    PreviewChangedFileAttributes();
                }
            }
            else
            {
                new NoOperationNotification().Show();
            }
        }

        /// <summary>
        /// 修改内容
        /// </summary>
        public void OnModifyClicked(object sender, RoutedEventArgs args)
        {
            bool checkResult = CheckOperationState();
            if (checkResult)
            {
                OperationFailedList.Clear();
                if (FilePropertiesDataList.Count is 0)
                {
                    new ListEmptyNotification().Show();
                }
                else
                {
                    PreviewChangedFileAttributes();
                    ChangeFileAttributes();
                    new OperationResultNotification(FilePropertiesDataList.Count - OperationFailedList.Count, OperationFailedList.Count).Show();
                    FilePropertiesDataList.Clear();
                }
            }
            else
            {
                new NoOperationNotification().Show();
            }
        }

        /// <summary>
        /// 选择文件夹
        /// </summary>
        public void OnSelectFolderClicked(object sender, RoutedEventArgs args)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = ResourceService.GetLocalized("FileProperties/SelectFolder");
            dialog.ShowNewFolderButton = true;
            dialog.RootFolder = Environment.SpecialFolder.Desktop;
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK || result == DialogResult.Yes)
            {
                OperationFailedList.Clear();
                if (!string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    DirectoryInfo currentFolder = new DirectoryInfo(dialog.SelectedPath);

                    try
                    {
                        foreach (DirectoryInfo subFolder in currentFolder.GetDirectories())
                        {
                            if ((subFolder.Attributes & System.IO.FileAttributes.Hidden) == System.IO.FileAttributes.Hidden)
                            {
                                continue;
                            }
                            FilePropertiesDataList.Add(new OldAndNewPropertiesModel()
                            {
                                FileName = subFolder.Name,
                                FilePath = subFolder.FullName
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
                            if ((subFile.Attributes & System.IO.FileAttributes.Hidden) == System.IO.FileAttributes.Hidden)
                            {
                                continue;
                            }
                            FilePropertiesDataList.Add(new OldAndNewPropertiesModel()
                            {
                                FileName = subFile.Name,
                                FilePath = subFile.FullName
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
        /// 时间更改时触发的事件
        /// </summary>
        public void OnTimeChanged(object sender, TimePickerValueChangedEventArgs args)
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
        public void OnUnchecked(object sender, RoutedEventArgs args)
        {
            Windows.UI.Xaml.Controls.CheckBox checkBox = sender as Windows.UI.Xaml.Controls.CheckBox;
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
        /// 检查用户是否指定了操作过程
        /// </summary>
        private bool CheckOperationState()
        {
            if (IsReadOnlyChecked == false && IsArchiveChecked == false && IsCreateDateChecked == false && IsHideChecked == false && IsSystemChecked == false && IsModifyDateChecked == false)
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
                stringBuilder.Append(ResourceService.GetLocalized("FileProperties/ReadOnly"));
                stringBuilder.Append(" ");
            }
            if (IsArchiveChecked)
            {
                stringBuilder.Append(ResourceService.GetLocalized("FileProperties/Archive"));
                stringBuilder.Append(" ");
            }
            if (IsHideChecked)
            {
                stringBuilder.Append(ResourceService.GetLocalized("FileProperties/Hide"));
                stringBuilder.Append(" ");
            }
            if (IsSystemChecked)
            {
                stringBuilder.Append(ResourceService.GetLocalized("FileProperties/System"));
                stringBuilder.Append(" ");
            }
            if (IsCreateDateChecked)
            {
                stringBuilder.Append(ResourceService.GetLocalized("FileProperties/CreateDate"));
                stringBuilder.Append(" ");
            }
            if (IsModifyDateChecked)
            {
                stringBuilder.Append(ResourceService.GetLocalized("FileProperties/ModifyDate"));
                stringBuilder.Append(" ");
            }

            foreach (OldAndNewPropertiesModel item in FilePropertiesDataList)
            {
                item.FileProperties = stringBuilder.ToString();
            }
        }

        /// <summary>
        /// 更改文件属性
        /// </summary>
        private void ChangeFileAttributes()
        {
            foreach (OldAndNewPropertiesModel item in FilePropertiesDataList)
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
                        OperationFailedList.Add(new Tuple<OldAndNewPropertiesModel, Exception>(item, e));
                    }
                }
            }
        }
    }
}
