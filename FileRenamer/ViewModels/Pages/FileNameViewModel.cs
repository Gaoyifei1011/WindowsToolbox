﻿using FileRenamer.Models;
using FileRenamer.Services.Root;
using FileRenamer.UI.Notifications;
using FileRenamer.ViewModels.Base;
using FileRenamer.Views.CustomControls.DialogsAndFlyouts;
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
    /// 文件名称页面视图模型
    /// </summary>
    public sealed class FileNameViewModel : ViewModelBase
    {
        private bool _isChecked = false;

        public bool IsChecked
        {
            get { return _isChecked; }

            set
            {
                _isChecked = value;
                OnPropertyChanged();
            }
        }

        private int _currentIndex = 0;

        public int CurrentIndex
        {
            get { return _currentIndex; }

            set
            {
                _currentIndex = value;
                OnPropertyChanged();
            }
        }

        private string _renameRule = "<#>";

        public string RenameRule
        {
            get { return _renameRule; }

            set
            {
                _renameRule = value;
                OnPropertyChanged();
            }
        }

        private string _startNumber = "1";

        public string StartNumber
        {
            get { return _startNumber; }

            set
            {
                _startNumber = value;
                OnPropertyChanged();
            }
        }

        private string _extensionName;

        public string ExtensionName
        {
            get { return _extensionName; }

            set
            {
                _extensionName = value;
                OnPropertyChanged();
            }
        }

        private string _lookUpString = string.Empty;

        public string LookUpString
        {
            get { return _lookUpString; }

            set
            {
                _lookUpString = value;
                OnPropertyChanged();
            }
        }

        private string _replaceString = string.Empty;

        public string ReplaceString
        {
            get { return _replaceString; }

            set
            {
                _replaceString = value;
                OnPropertyChanged();
            }
        }

        private NumberFormatModel _selectedNumberFormat;

        public NumberFormatModel SelectedNumberFormat
        {
            get { return _selectedNumberFormat; }

            set
            {
                _selectedNumberFormat = value;
                OnPropertyChanged();
            }
        }

        public List<string> NameChangeRuleList { get; } = new List<string>()
        {
            ResourceService.GetLocalized("FileName/NameChangeRule1"),
            ResourceService.GetLocalized("FileName/NameChangeRule2"),
            ResourceService.GetLocalized("FileName/NameChangeRule3"),
            ResourceService.GetLocalized("FileName/NameChangeRule4"),
        };

        public List<NumberFormatModel> NumberFormatList { get; } = new List<NumberFormatModel>
        {
            new NumberFormatModel(){ DisplayName = ResourceService.GetLocalized("FileName/Auto"), InternalName = "Auto"},
            new NumberFormatModel(){ DisplayName = "0", InternalName = "0"},
            new NumberFormatModel(){ DisplayName = "00", InternalName = "00"},
            new NumberFormatModel(){ DisplayName = "000", InternalName = "000"},
            new NumberFormatModel(){ DisplayName = "0000", InternalName = "0000"},
            new NumberFormatModel(){ DisplayName = "00000", InternalName = "00000"},
            new NumberFormatModel(){ DisplayName = "000000", InternalName = "000000"},
            new NumberFormatModel(){ DisplayName = "0000000", InternalName = "0000000"},
        };

        public List<OldAndNewNameModel> NameChangeList { get; } = new List<OldAndNewNameModel>()
        {
            new OldAndNewNameModel(){ OriginalFileName = string.Empty, NewFileName = string.Empty },
            new OldAndNewNameModel(){ OriginalFileName = string.Empty, NewFileName = string.Empty },
            new OldAndNewNameModel(){ OriginalFileName = string.Empty, NewFileName = string.Empty },
            new OldAndNewNameModel(){ OriginalFileName = string.Empty, NewFileName = string.Empty },
        };

        public Dictionary<int, List<OldAndNewNameModel>> NameChangeDict = new Dictionary<int, List<OldAndNewNameModel>>()
        {
            { 0, new List<OldAndNewNameModel>()
                {
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("FileName/NameChangeOriginalName1"), NewFileName = ResourceService.GetLocalized("FileName/NameChangeList1ChangedName1") },
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("FileName/NameChangeOriginalName2"), NewFileName = ResourceService.GetLocalized("FileName/NameChangeList1ChangedName2") },
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("FileName/NameChangeOriginalName3"), NewFileName = ResourceService.GetLocalized("FileName/NameChangeList1ChangedName3") },
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("FileName/NameChangeOriginalName4"), NewFileName = ResourceService.GetLocalized("FileName/NameChangeList1ChangedName4") },
                }
            },
            { 1, new List<OldAndNewNameModel>()
                {
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("FileName/NameChangeOriginalName1"), NewFileName = ResourceService.GetLocalized("FileName/NameChangeList2ChangedName1") },
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("FileName/NameChangeOriginalName2"), NewFileName = ResourceService.GetLocalized("FileName/NameChangeList2ChangedName2") },
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("FileName/NameChangeOriginalName3"), NewFileName = ResourceService.GetLocalized("FileName/NameChangeList2ChangedName3") },
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("FileName/NameChangeOriginalName4"), NewFileName = ResourceService.GetLocalized("FileName/NameChangeList2ChangedName4") },
                }
            },
            { 2, new List<OldAndNewNameModel>()
                {
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("FileName/NameChangeOriginalName1"), NewFileName = ResourceService.GetLocalized("FileName/NameChangeList3ChangedName1") },
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("FileName/NameChangeOriginalName2"), NewFileName = ResourceService.GetLocalized("FileName/NameChangeList3ChangedName2") },
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("FileName/NameChangeOriginalName3"), NewFileName = ResourceService.GetLocalized("FileName/NameChangeList3ChangedName3") },
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("FileName/NameChangeOriginalName4"), NewFileName = ResourceService.GetLocalized("FileName/NameChangeList3ChangedName4") },
                }
            },
            { 3, new List<OldAndNewNameModel>()
                {
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("FileName/NameChangeOriginalName1"), NewFileName = ResourceService.GetLocalized("FileName/NameChangeList4ChangedName1") },
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("FileName/NameChangeOriginalName2"), NewFileName = ResourceService.GetLocalized("FileName/NameChangeList4ChangedName2") },
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("FileName/NameChangeOriginalName3"), NewFileName = ResourceService.GetLocalized("FileName/NameChangeList4ChangedName3") },
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("FileName/NameChangeOriginalName4"), NewFileName = ResourceService.GetLocalized("FileName/NameChangeList4ChangedName4") },
                }
            },
        };

        public ObservableCollection<OldAndNewNameModel> FileNameDataList { get; } = new ObservableCollection<OldAndNewNameModel>();

        public ObservableCollection<string> OperationFailedList { get; } = new ObservableCollection<string>();

        public FileNameViewModel()
        {
            SelectedNumberFormat = NumberFormatList[0];

            CurrentIndex = 0;

            for (int index = 0; index < NameChangeList.Count; index++)
            {
                NameChangeList[index].OriginalFileName = NameChangeDict[CurrentIndex][index].OriginalFileName;
                NameChangeList[index].NewFileName = NameChangeDict[CurrentIndex][index].NewFileName;
            }
        }

        /// <summary>
        /// 清空列表
        /// </summary>
        public void OnClearListClicked(object sender, RoutedEventArgs args)
        {
            FileNameDataList.Clear();
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
            args.DragUIOverride.Caption = ResourceService.GetLocalized("FileName/DragOverContent");
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
                        FileNameDataList.Add(new OldAndNewNameModel()
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
        /// 向前导航
        /// </summary>
        public void OnForwardNavigateClicked(object sender, RoutedEventArgs args)
        {
            CurrentIndex = CurrentIndex == 0 ? 3 : CurrentIndex - 1;

            for (int index = 0; index < NameChangeList.Count; index++)
            {
                NameChangeList[index].OriginalFileName = NameChangeDict[CurrentIndex][index].OriginalFileName;
                NameChangeList[index].NewFileName = NameChangeDict[CurrentIndex][index].NewFileName;
            }
        }

        /// <summary>
        /// 向后导航
        /// </summary>
        public void OnNextNavigateClicked(object sender, RoutedEventArgs args)
        {
            CurrentIndex = CurrentIndex == 3 ? 0 : CurrentIndex + 1;

            for (int index = 0; index < NameChangeList.Count; index++)
            {
                NameChangeList[index].OriginalFileName = NameChangeDict[CurrentIndex][index].OriginalFileName;
                NameChangeList[index].NewFileName = NameChangeDict[CurrentIndex][index].NewFileName;
            }
        }

        /// <summary>
        /// 选择编号格式
        /// </summary>
        public void OnNumberFormatClicked(object sender, RoutedEventArgs args)
        {
            RadioMenuFlyoutItem item = sender as RadioMenuFlyoutItem;
            if (item.Tag is not null)
            {
                SelectedNumberFormat = NumberFormatList[Convert.ToInt32(item.Tag)];
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
                if (FileNameDataList.Count is 0)
                {
                    new ListEmptyNotification().Show();
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
                if (FileNameDataList.Count is 0)
                {
                    new ListEmptyNotification().Show();
                }
                else
                {
                    new OperationResultNotification(FileNameDataList.Count - OperationFailedList.Count, OperationFailedList.Count).Show();
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
            dialog.Description = ResourceService.GetLocalized("FileName/SelectFolder");
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
                            FileNameDataList.Add(new OldAndNewNameModel()
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
                            FileNameDataList.Add(new OldAndNewNameModel()
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
            ExtensionName = string.Empty;
        }

        /// <summary>
        /// 检查用户是否指定了操作过程
        /// </summary>
        private bool CheckOperationState()
        {
            if (RenameRule == string.Empty && StartNumber == string.Empty && IsChecked == false && LookUpString == string.Empty && ReplaceString == string.Empty)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
