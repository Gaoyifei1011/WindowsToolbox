﻿using FileRenamer.Extensions.DataType.Enums;
using FileRenamer.Helpers.Root;
using FileRenamer.Models;
using FileRenamer.Services.Root;
using FileRenamer.UI.Notifications;
using FileRenamer.ViewModels.Base;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Forms;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Xaml;

namespace FileRenamer.ViewModels.Pages
{
    /// <summary>
    /// 大写小写页面视图模型
    /// </summary>
    public sealed class UpperAndLowerCaseViewModel : ViewModelBase
    {
        private UpperAndLowerSelectedType _selectedType = UpperAndLowerSelectedType.None;

        public UpperAndLowerSelectedType SelectedType
        {
            get { return _selectedType; }

            set
            {
                _selectedType = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<OldAndNewNameModel> UpperAndLowerCaseDataList { get; } = new ObservableCollection<OldAndNewNameModel>();

        public ObservableCollection<OldAndNewNameModel> OperationFailedList { get; } = new ObservableCollection<OldAndNewNameModel>();

        /// <summary>
        /// 选中时触发的事件
        /// </summary>
        public void OnChecked(object sender, RoutedEventArgs args)
        {
            Windows.UI.Xaml.Controls.CheckBox checkBox = sender as Windows.UI.Xaml.Controls.CheckBox;
            if (checkBox is not null)
            {
                SelectedType = (UpperAndLowerSelectedType)Convert.ToInt32(checkBox.Tag);
            }
        }

        /// <summary>
        /// 清空列表
        /// </summary>
        public void OnClearListClicked(object sender, RoutedEventArgs args)
        {
            UpperAndLowerCaseDataList.Clear();
            OperationFailedList.Clear();
        }

        /// <summary>
        /// 设置拖动的数据的可视表示形式
        /// </summary>
        public void OnDragOver(object sender, Windows.UI.Xaml.DragEventArgs args)
        {
            args.AcceptedOperation = DataPackageOperation.Copy;
            args.DragUIOverride.IsCaptionVisible = true;
            args.DragUIOverride.IsContentVisible = false;
            args.DragUIOverride.IsGlyphVisible = true;
            args.DragUIOverride.Caption = ResourceService.GetLocalized("UpperAndLowerCase/DragOverContent");
            args.Handled = true;
        }

        /// <summary>
        /// 拖动文件完成后获取文件信息
        /// </summary>
        public async void OnDrop(object sender, Windows.UI.Xaml.DragEventArgs args)
        {
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
        /// 预览修改的内容
        /// </summary>
        public void OnPreviewClicked(object sender, RoutedEventArgs args)
        {
            bool checkResult = CheckOperationState();
            if (checkResult)
            {
                OperationFailedList.Clear();
                if (UpperAndLowerCaseDataList.Count is 0)
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
                if (UpperAndLowerCaseDataList.Count is 0)
                {
                    new ListEmptyNotification().Show();
                }
                else
                {
                    PreviewChangedFileName();
                    ChangeFileName();
                    new OperationResultNotification(UpperAndLowerCaseDataList.Count - OperationFailedList.Count, OperationFailedList.Count).Show();
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
            dialog.Description = ResourceService.GetLocalized("UpperAndLowerCase/SelectFolder");
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
                            if ((subFile.Attributes & System.IO.FileAttributes.Hidden) == System.IO.FileAttributes.Hidden)
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
        public void OnUnchecked(object sender, RoutedEventArgs args)
        {
            Windows.UI.Xaml.Controls.CheckBox checkBox = sender as Windows.UI.Xaml.Controls.CheckBox;
            if (checkBox is not null)
            {
                if (SelectedType == (UpperAndLowerSelectedType)Convert.ToInt32(checkBox.Tag))
                {
                    SelectedType = UpperAndLowerSelectedType.None;
                }
            }
        }

        /// <summary>
        /// 检查用户是否指定了操作过程
        /// </summary>
        private bool CheckOperationState()
        {
            if (SelectedType == UpperAndLowerSelectedType.None)
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
                case UpperAndLowerSelectedType.AllUppercase:
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
                case UpperAndLowerSelectedType.FileNameUppercase:
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
                case UpperAndLowerSelectedType.ExtensionNameUppercase:
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
                case UpperAndLowerSelectedType.DeleteSpace:
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
                case UpperAndLowerSelectedType.AllLowercase:
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
                case UpperAndLowerSelectedType.FileNameLowercase:
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
                case UpperAndLowerSelectedType.ExtensionNameLowercase:
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
                case UpperAndLowerSelectedType.ReplaceSpace:
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
            foreach (OldAndNewNameModel item in UpperAndLowerCaseDataList)
            {
                if (!string.IsNullOrEmpty(item.OriginalFileName) && !string.IsNullOrEmpty(item.OriginalFilePath))
                {
                    if (IOHelper.IsDir(item.OriginalFilePath))
                    {
                        try
                        {
                            FileSystem.RenameDirectory(item.OriginalFilePath, item.NewFileName);
                        }
                        catch (Exception)
                        {
                            OperationFailedList.Add(item);
                        }
                    }
                    else
                    {
                        try
                        {
                            FileSystem.RenameFile(item.OriginalFilePath, item.NewFileName);
                        }
                        catch (Exception)
                        {
                            OperationFailedList.Add(item);
                        }
                    }
                }
            }
        }
    }
}
