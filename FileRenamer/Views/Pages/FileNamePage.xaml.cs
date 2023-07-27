﻿using FileRenamer.Helpers.Controls;
using FileRenamer.Helpers.Root;
using FileRenamer.Models;
using FileRenamer.Services.Root;
using FileRenamer.UI.Dialogs;
using FileRenamer.UI.Notifications;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FileRenamer.Views.Pages
{
    /// <summary>
    /// 文件名称页面
    /// </summary>
    public sealed partial class FileNamePage : Page, INotifyPropertyChanged
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

        public ObservableCollection<OperationFailedModel> OperationFailedList { get; } = new ObservableCollection<OperationFailedModel>();

        public event PropertyChangedEventHandler PropertyChanged;

        public FileNamePage()
        {
            InitializeComponent();
            SelectedNumberFormat = NumberFormatList[0];

            CurrentIndex = 0;

            for (int index = 0; index < NameChangeList.Count; index++)
            {
                NameChangeList[index].OriginalFileName = NameChangeDict[CurrentIndex][index].OriginalFileName;
                NameChangeList[index].NewFileName = NameChangeDict[CurrentIndex][index].NewFileName;
            }
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
            args.DragUIOverride.Caption = ResourceService.GetLocalized("FileName/DragOverContent");
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

        public string GetChangeRule(int index)
        {
            return string.Format(ResourceService.GetLocalized("Dialog/ChangeRule"), NameChangeRuleList[index]);
        }

        public bool IsItemChecked(string selectedInternalName, string internalName)
        {
            return selectedInternalName == internalName;
        }

        public string LocalizeTotal(int count)
        {
            return string.Format(ResourceService.GetLocalized("FileName/Total"), FileNameDataList.Count);
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
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
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
        public void OnModifyClicked(object sender, RoutedEventArgs args)
        {
            bool checkResult = CheckOperationState();
            if (checkResult)
            {
                OperationFailedList.Clear();
                if (FileNameDataList.Count is 0)
                {
                    new ListEmptyNotification(this).Show();
                }
                else
                {
                    PreviewChangedFileName();
                    ChangeFileName();
                    new OperationResultNotification(this, FileNameDataList.Count - OperationFailedList.Count, OperationFailedList.Count).Show();
                    FileNameDataList.Clear();
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
        public void OnSelectFileClicked(object sender, RoutedEventArgs args)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            dialog.Title = ResourceService.GetLocalized("FileName/SelectFile");
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string fileName in dialog.FileNames)
                {
                    try
                    {
                        FileInfo file = new FileInfo(fileName);
                        if ((file.Attributes & System.IO.FileAttributes.Hidden) == System.IO.FileAttributes.Hidden)
                        {
                            continue;
                        }
                        FileNameDataList.Add(new OldAndNewNameModel()
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
                    catch (Exception) { }

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
                    catch (Exception) { }
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
        /// 查看修改失败的文件错误信息
        /// </summary>
        public async void OnViewErrorInformationClicked(object sender, RoutedEventArgs args)
        {
            await ContentDialogHelper.ShowAsync(new OperationFailedDialog(OperationFailedList), this);
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

        /// <summary>
        /// 预览修改后的文件名称
        /// </summary>
        private void PreviewChangedFileName()
        {
            int startIndex = 0;
            if (!string.IsNullOrEmpty(StartNumber))
            {
                int.TryParse(StartNumber, out startIndex);
            }

            foreach (OldAndNewNameModel item in FileNameDataList)
            {
                string tempNewFileName = item.OriginalFileName;
                // 根据改名规则替换
                if (!string.IsNullOrEmpty(RenameRule))
                {
                    try
                    {
                        string tempFileName = RenameRule;
                        if (tempFileName.Contains("<#>"))
                        {
                            string formattedIndex = string.Empty;
                            if (SelectedNumberFormat.InternalName == NumberFormatList[0].InternalName)
                            {
                                formattedIndex = startIndex.ToString();
                            }
                            else if (SelectedNumberFormat.InternalName == NumberFormatList[1].InternalName)
                            {
                                formattedIndex = string.Format("{0:D1}", startIndex);
                            }
                            else if (SelectedNumberFormat.InternalName == NumberFormatList[2].InternalName)
                            {
                                formattedIndex = string.Format("{0:D2}", startIndex);
                            }
                            else if (SelectedNumberFormat.InternalName == NumberFormatList[3].InternalName)
                            {
                                formattedIndex = string.Format("{0:D3}", startIndex);
                            }
                            else if (SelectedNumberFormat.InternalName == NumberFormatList[4].InternalName)
                            {
                                formattedIndex = string.Format("{0:D4}", startIndex);
                            }
                            else if (SelectedNumberFormat.InternalName == NumberFormatList[5].InternalName)
                            {
                                formattedIndex = string.Format("{0:D5}", startIndex);
                            }
                            else if (SelectedNumberFormat.InternalName == NumberFormatList[6].InternalName)
                            {
                                formattedIndex = string.Format("{0:D6}", startIndex);
                            }
                            else if (SelectedNumberFormat.InternalName == NumberFormatList[7].InternalName)
                            {
                                formattedIndex = string.Format("{0:D7}", startIndex);
                            }

                            tempFileName = tempFileName.Replace("<#>", formattedIndex);
                            startIndex++;
                        }
                        if (tempFileName.Contains("<$>"))
                        {
                            tempFileName = tempFileName.Replace("<$>", DateTime.Now.ToString("yyyy-MM-dd"));
                        }
                        if (tempFileName.Contains("<&>"))
                        {
                            tempFileName = tempFileName.Replace("<&>", item.OriginalFileName);
                        }
                        if (tempFileName.Contains("<N>"))
                        {
                            if (IOHelper.IsDir(item.OriginalFilePath))
                            {
                                DirectoryInfo directoryInfo = new DirectoryInfo(item.OriginalFilePath);
                                tempFileName = tempFileName.Replace("<N>", directoryInfo.LastWriteTime.ToString("yyyy-MM-dd"));
                            }
                            else
                            {
                                FileInfo fileInfo = new FileInfo(item.OriginalFilePath);
                                tempFileName = tempFileName.Replace("<N>", fileInfo.LastWriteTime.ToString("yyyy-MM-dd"));
                            }
                        }
                        if (tempFileName.Contains("<C>"))
                        {
                            if (IOHelper.IsDir(item.OriginalFilePath))
                            {
                                DirectoryInfo directoryInfo = new DirectoryInfo(item.OriginalFilePath);
                                tempFileName = tempFileName.Replace("<C>", directoryInfo.CreationTime.ToString("yyyy-MM-dd"));
                            }
                            else
                            {
                                FileInfo fileInfo = new FileInfo(item.OriginalFilePath);
                                tempFileName = tempFileName.Replace("<C>", fileInfo.CreationTime.ToString("yyyy-MM-dd"));
                            }
                        }
                        tempNewFileName = tempFileName + Path.GetExtension(item.OriginalFileName);
                    }
                    catch (Exception)
                    {
                        tempNewFileName = item.OriginalFileName;
                    }
                }

                // 修改文件扩展名
                if (IsChecked)
                {
                    string fileName = Path.GetFileNameWithoutExtension(tempNewFileName);
                    tempNewFileName = fileName + ExtensionName;
                }

                // 查找并替换字符串
                if (!string.IsNullOrEmpty(LookUpString) && tempNewFileName.Contains(LookUpString))
                {
                    tempNewFileName = tempNewFileName.Replace(LookUpString, ReplaceString);
                }

                item.NewFileName = tempNewFileName;
                item.NewFilePath = item.OriginalFilePath.Replace(item.OriginalFileName, item.NewFileName);
            }
        }

        /// <summary>
        /// 更改文件名称
        /// </summary>
        private void ChangeFileName()
        {
            foreach (OldAndNewNameModel item in FileNameDataList)
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
                            OperationFailedList.Add(new OperationFailedModel()
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
                            OperationFailedList.Add(new OperationFailedModel()
                            {
                                FileName = item.OriginalFileName,
                                FilePath = item.OriginalFilePath,
                                Exception = e
                            });
                        }
                    }
                }
            }
        }
    }
}
