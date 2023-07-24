using FileRenamer.Extensions.DataType.Enums;
using FileRenamer.Helpers.Root;
using FileRenamer.Models;
using FileRenamer.Services.Root;
using FileRenamer.UI.Notifications;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FileRenamer.Views.Pages
{
    /// <summary>
    /// 扩展名称页面
    /// </summary>
    public sealed partial class ExtensionNamePage : Page, INotifyPropertyChanged
    {
        private ExtensionNameSelectedType _selectedType = ExtensionNameSelectedType.None;

        public ExtensionNameSelectedType SelectedType
        {
            get { return _selectedType; }

            set
            {
                _selectedType = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedType)));
            }
        }

        private string _changeToText;

        public string ChangeToText
        {
            get { return _changeToText; }

            set
            {
                _changeToText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ChangeToText)));
            }
        }

        private string _searchText;

        public string SearchText
        {
            get { return _searchText; }

            set
            {
                _searchText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchText)));
            }
        }

        private string _replaceText;

        public string ReplaceText
        {
            get { return _replaceText; }

            set
            {
                _replaceText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReplaceText)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<OldAndNewNameModel> ExtensionNameDataList { get; } = new ObservableCollection<OldAndNewNameModel>();

        public ObservableCollection<Tuple<OldAndNewNameModel, Exception>> OperationFailedList { get; } = new ObservableCollection<Tuple<OldAndNewNameModel, Exception>>();

        public ExtensionNamePage()
        {
            InitializeComponent();
        }

        public bool IsItemChecked(ExtensionNameSelectedType selectedType, ExtensionNameSelectedType comparedType)
        {
            return selectedType == comparedType;
        }

        public string LocalizeTotal(int count)
        {
            return string.Format(ResourceService.GetLocalized("ExtensionName/Total"), ExtensionNameDataList.Count);
        }

        /// <summary>
        /// 选中时触发的事件
        /// </summary>
        public void OnChecked(object sender, RoutedEventArgs args)
        {
            Windows.UI.Xaml.Controls.CheckBox checkBox = sender as Windows.UI.Xaml.Controls.CheckBox;
            if (checkBox is not null)
            {
                SelectedType = (ExtensionNameSelectedType)Convert.ToInt32(checkBox.Tag);
            }
        }

        /// <summary>
        /// 清空列表
        /// </summary>
        public void OnClearListClicked(object sender, RoutedEventArgs args)
        {
            ExtensionNameDataList.Clear();
            OperationFailedList.Clear();
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
            args.DragUIOverride.Caption = ResourceService.GetLocalized("ExtensionName/DragOverContent");
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
                        ExtensionNameDataList.Add(new OldAndNewNameModel()
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
        /// 预览修改的内容
        /// </summary>
        public void OnPreviewClicked(object sender, RoutedEventArgs args)
        {
            bool checkResult = CheckOperationState();
            if (checkResult)
            {
                OperationFailedList.Clear();
                if (ExtensionNameDataList.Count is 0)
                {
                    new ListEmptyNotification().Show();
                }
                else
                {
                    PreviewChangedFileName();
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
                if (ExtensionNameDataList.Count is 0)
                {
                    new ListEmptyNotification().Show();
                }
                else
                {
                    PreviewChangedFileName();
                    ChangeFileName();
                    new OperationResultNotification(ExtensionNameDataList.Count - OperationFailedList.Count, OperationFailedList.Count).Show();
                    ExtensionNameDataList.Clear();
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
            dialog.Description = ResourceService.GetLocalized("ExtensionName/SelectFolder");
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
                            ExtensionNameDataList.Add(new OldAndNewNameModel()
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
                            if ((subFile.Attributes & System.IO.FileAttributes.Hidden) == System.IO.FileAttributes.Hidden)
                            {
                                continue;
                            }
                            ExtensionNameDataList.Add(new OldAndNewNameModel()
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
        public void OnUnchecked(object sender, RoutedEventArgs args)
        {
            Windows.UI.Xaml.Controls.CheckBox checkBox = sender as Windows.UI.Xaml.Controls.CheckBox;
            if (checkBox is not null)
            {
                if (SelectedType == (ExtensionNameSelectedType)Convert.ToInt32(checkBox.Tag))
                {
                    SelectedType = ExtensionNameSelectedType.None;
                }

                if ((ExtensionNameSelectedType)Convert.ToInt32(checkBox.Tag) == ExtensionNameSelectedType.IsSameExtensionName)
                {
                    ChangeToText = string.Empty;
                }
                else if ((ExtensionNameSelectedType)Convert.ToInt32(checkBox.Tag) == ExtensionNameSelectedType.IsFindAndReplaceExtensionName)
                {
                    SearchText = string.Empty;
                    ReplaceText = string.Empty;
                }
            }
        }

        /// <summary>
        /// 检查用户是否指定了操作过程
        /// </summary>
        private bool CheckOperationState()
        {
            if (SelectedType == ExtensionNameSelectedType.None)
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
                case ExtensionNameSelectedType.IsSameExtensionName:
                    {
                        foreach (OldAndNewNameModel item in ExtensionNameDataList)
                        {
                            if (!string.IsNullOrEmpty(item.OriginalFileName))
                            {
                                string fileName = Path.GetFileNameWithoutExtension(item.OriginalFileName);
                                item.NewFileName = fileName + "." + ChangeToText;
                                item.NewFilePath = item.OriginalFilePath.Replace(item.OriginalFileName, item.NewFileName);
                            }
                        }
                        break;
                    }
                case ExtensionNameSelectedType.IsFindAndReplaceExtensionName:
                    {
                        foreach (OldAndNewNameModel item in ExtensionNameDataList)
                        {
                            if (!string.IsNullOrEmpty(item.OriginalFileName))
                            {
                                string fileName = Path.GetFileNameWithoutExtension(item.OriginalFileName);
                                string extensionName = Path.GetExtension(item.OriginalFileName);
                                if (extensionName.Contains(SearchText))
                                {
                                    extensionName = extensionName.Replace(SearchText, ReplaceText);
                                }
                                item.NewFileName = fileName + extensionName;
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
            foreach (OldAndNewNameModel item in ExtensionNameDataList)
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
                            OperationFailedList.Add(new Tuple<OldAndNewNameModel, Exception>(item, e));
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
                            OperationFailedList.Add(new Tuple<OldAndNewNameModel, Exception>(item, e));
                        }
                    }
                }
            }
        }
    }
}
