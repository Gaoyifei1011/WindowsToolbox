using Microsoft.UI.Xaml.Controls;
using PowerTools.Extensions.DataType.Enums;
using PowerTools.Models;
using PowerTools.Services.Root;
using PowerTools.Views.Dialogs;
using PowerTools.Views.TeachingTips;
using PowerTools.Views.Windows;
using PowerTools.WindowsAPI.ComTypes;
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

// 抑制 CA1806，IDE0060 警告
#pragma warning disable CA1806,IDE0060

namespace PowerTools.Views.Pages
{
    /// <summary>
    /// 文件名称页面
    /// </summary>
    public sealed partial class FileNamePage : Page, INotifyPropertyChanged
    {
        private readonly string ChangeRuleString = ResourceService.FileNameResource.GetString("ChangeRule");
        private readonly string DragOverContentString = ResourceService.FileNameResource.GetString("DragOverContent");
        private readonly string NameChangeRule1String = ResourceService.FileNameResource.GetString("NameChangeRule1");
        private readonly string NameChangeRule2String = ResourceService.FileNameResource.GetString("NameChangeRule2");
        private readonly string NameChangeRule3String = ResourceService.FileNameResource.GetString("NameChangeRule3");
        private readonly string NameChangeRule4String = ResourceService.FileNameResource.GetString("NameChangeRule4");
        private readonly string NameChangeOriginalName1String = ResourceService.FileNameResource.GetString("NameChangeOriginalName1");
        private readonly string NameChangeOriginalName2String = ResourceService.FileNameResource.GetString("NameChangeOriginalName2");
        private readonly string NameChangeOriginalName3String = ResourceService.FileNameResource.GetString("NameChangeOriginalName3");
        private readonly string NameChangeOriginalName4String = ResourceService.FileNameResource.GetString("NameChangeOriginalName4");
        private readonly string NameChangeList1ChangedName1String = ResourceService.FileNameResource.GetString("NameChangeList1ChangedName1");
        private readonly string NameChangeList1ChangedName2String = ResourceService.FileNameResource.GetString("NameChangeList1ChangedName2");
        private readonly string NameChangeList1ChangedName3String = ResourceService.FileNameResource.GetString("NameChangeList1ChangedName3");
        private readonly string NameChangeList1ChangedName4String = ResourceService.FileNameResource.GetString("NameChangeList1ChangedName4");
        private readonly string NameChangeList2ChangedName1String = ResourceService.FileNameResource.GetString("NameChangeList2ChangedName1");
        private readonly string NameChangeList2ChangedName2String = ResourceService.FileNameResource.GetString("NameChangeList2ChangedName2");
        private readonly string NameChangeList2ChangedName3String = ResourceService.FileNameResource.GetString("NameChangeList2ChangedName3");
        private readonly string NameChangeList2ChangedName4String = ResourceService.FileNameResource.GetString("NameChangeList2ChangedName4");
        private readonly string NameChangeList3ChangedName1String = ResourceService.FileNameResource.GetString("NameChangeList3ChangedName1");
        private readonly string NameChangeList3ChangedName2String = ResourceService.FileNameResource.GetString("NameChangeList3ChangedName2");
        private readonly string NameChangeList3ChangedName3String = ResourceService.FileNameResource.GetString("NameChangeList3ChangedName3");
        private readonly string NameChangeList3ChangedName4String = ResourceService.FileNameResource.GetString("NameChangeList3ChangedName4");
        private readonly string NameChangeList4ChangedName1String = ResourceService.FileNameResource.GetString("NameChangeList4ChangedName1");
        private readonly string NameChangeList4ChangedName2String = ResourceService.FileNameResource.GetString("NameChangeList4ChangedName2");
        private readonly string NameChangeList4ChangedName3String = ResourceService.FileNameResource.GetString("NameChangeList4ChangedName3");
        private readonly string NameChangeList4ChangedName4String = ResourceService.FileNameResource.GetString("NameChangeList4ChangedName4");
        private readonly string SelectFileString = ResourceService.FileNameResource.GetString("SelectFile");
        private readonly string SelectFolderString = ResourceService.FileNameResource.GetString("SelectFolder");
        private readonly string TotalString = ResourceService.FileNameResource.GetString("Total");
        private readonly object fileNameLock = new();

        private bool _isChecked = false;

        public bool IsChecked
        {
            get { return _isChecked; }

            set
            {
                if (!Equals(_isChecked, value))
                {
                    _isChecked = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsChecked)));
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

        private int _currentIndex = 0;

        public int CurrentIndex
        {
            get { return _currentIndex; }

            set
            {
                if (!Equals(_currentIndex, value))
                {
                    _currentIndex = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentIndex)));
                }
            }
        }

        private string _renameRule = "<#>";

        public string RenameRule
        {
            get { return _renameRule; }

            set
            {
                if (!string.Equals(_renameRule, value))
                {
                    _renameRule = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RenameRule)));
                }
            }
        }

        private string _startNumber = "1";

        public string StartNumber
        {
            get { return _startNumber; }

            set
            {
                if (!string.Equals(_startNumber, value))
                {
                    _startNumber = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StartNumber)));
                }
            }
        }

        private string _extensionName;

        public string ExtensionName
        {
            get { return _extensionName; }

            set
            {
                if (!string.Equals(_extensionName, value))
                {
                    _extensionName = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ExtensionName)));
                }
            }
        }

        private string _lookUpString = string.Empty;

        public string LookUpString
        {
            get { return _lookUpString; }

            set
            {
                if (!string.Equals(_lookUpString, value))
                {
                    _lookUpString = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LookUpString)));
                }
            }
        }

        private string _replaceString = string.Empty;

        public string ReplaceString
        {
            get { return _replaceString; }

            set
            {
                if (!string.Equals(_replaceString, value))
                {
                    _replaceString = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReplaceString)));
                }
            }
        }

        private KeyValuePair<string, string> _selectedNumberFormat;

        public KeyValuePair<string, string> SelectedNumberFormat
        {
            get { return _selectedNumberFormat; }

            set
            {
                if (!Equals(_selectedNumberFormat, value))
                {
                    _selectedNumberFormat = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedNumberFormat)));
                }
            }
        }

        private List<string> NameChangeRuleList { get; } = [];

        private readonly List<KeyValuePair<string, string>> NumberFormatList =
        [
            new KeyValuePair<string,string>( "Auto", ResourceService.FileNameResource.GetString("Auto")),
            new KeyValuePair<string,string>( "0", "0"),
            new KeyValuePair<string,string>( "00", "00"),
            new KeyValuePair<string,string>( "000", "000"),
            new KeyValuePair<string,string>( "0000", "0000"),
            new KeyValuePair<string,string>( "00000", "00000"),
            new KeyValuePair<string,string>( "000000", "000000"),
            new KeyValuePair<string,string>( "0000000", "0000000"),
        ];

        private readonly List<OldAndNewNameModel> NameChangeList =
        [
            new(){ OriginalFileName = string.Empty, NewFileName = string.Empty },
            new(){ OriginalFileName = string.Empty, NewFileName = string.Empty },
            new(){ OriginalFileName = string.Empty, NewFileName = string.Empty },
            new(){ OriginalFileName = string.Empty, NewFileName = string.Empty },
        ];

        private readonly Dictionary<int, List<OldAndNewNameModel>> NameChangeDict = [];

        private ObservableCollection<OldAndNewNameModel> FileNameCollection { get; } = [];

        private ObservableCollection<OperationFailedModel> OperationFailedCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public FileNamePage()
        {
            InitializeComponent();
            NameChangeRuleList.Add(NameChangeRule1String);
            NameChangeRuleList.Add(NameChangeRule2String);
            NameChangeRuleList.Add(NameChangeRule3String);
            NameChangeRuleList.Add(NameChangeRule4String);
            NameChangeDict.Add(0,
            [
                new(){ OriginalFileName = NameChangeOriginalName1String, NewFileName = NameChangeList1ChangedName1String },
                new(){ OriginalFileName = NameChangeOriginalName2String, NewFileName = NameChangeList1ChangedName2String },
                new(){ OriginalFileName = NameChangeOriginalName3String, NewFileName = NameChangeList1ChangedName3String },
                new(){ OriginalFileName = NameChangeOriginalName4String, NewFileName = NameChangeList1ChangedName4String },
            ]);
            NameChangeDict.Add(1,
            [
                new(){ OriginalFileName = NameChangeOriginalName1String, NewFileName = NameChangeList2ChangedName1String },
                new(){ OriginalFileName = NameChangeOriginalName2String, NewFileName = NameChangeList2ChangedName2String },
                new(){ OriginalFileName = NameChangeOriginalName3String, NewFileName = NameChangeList2ChangedName3String },
                new(){ OriginalFileName = NameChangeOriginalName4String, NewFileName = NameChangeList2ChangedName4String },
            ]);
            NameChangeDict.Add(2,
            [
                new(){ OriginalFileName = NameChangeOriginalName1String, NewFileName = NameChangeList3ChangedName1String },
                new(){ OriginalFileName = NameChangeOriginalName2String, NewFileName = NameChangeList3ChangedName2String },
                new(){ OriginalFileName = NameChangeOriginalName3String, NewFileName = NameChangeList3ChangedName3String },
                new(){ OriginalFileName = NameChangeOriginalName4String, NewFileName = NameChangeList3ChangedName4String },
            ]);
            NameChangeDict.Add(3,
            [
                new(){ OriginalFileName = NameChangeOriginalName1String, NewFileName = NameChangeList4ChangedName1String },
                new(){ OriginalFileName = NameChangeOriginalName2String, NewFileName = NameChangeList4ChangedName2String },
                new(){ OriginalFileName = NameChangeOriginalName3String, NewFileName = NameChangeList4ChangedName3String },
                new(){ OriginalFileName = NameChangeOriginalName4String, NewFileName = NameChangeList4ChangedName4String },
            ]);
            SelectedNumberFormat = NumberFormatList[0];
            CurrentIndex = 0;

            for (int index = 0; index < NameChangeList.Count; index++)
            {
                NameChangeList[index].OriginalFileName = NameChangeDict[CurrentIndex][index].OriginalFileName;
                NameChangeList[index].NewFileName = NameChangeDict[CurrentIndex][index].NewFileName;
            }
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
            args.DragUIOverride.Caption = DragOverContentString;
            args.Handled = true;
        }

        /// <summary>
        /// 拖动文件完成后获取文件信息
        /// </summary>
        protected override async void OnDrop(global::Windows.UI.Xaml.DragEventArgs args)
        {
            base.OnDrop(args);
            DragOperationDeferral dragOperationDeferral = args.GetDeferral();
            try
            {
                DataPackageView view = args.DataView;
                if (view.Contains(StandardDataFormats.StorageItems))
                {
                    List<OldAndNewNameModel> fileNameList = await Task.Run(async () =>
                    {
                        List<OldAndNewNameModel> fileNameList = [];
                        IReadOnlyList<IStorageItem> storageItemList = await view.GetStorageItemsAsync();

                        foreach (IStorageItem storageItem in storageItemList)
                        {
                            try
                            {
                                FileInfo fileInfo = new(storageItem.Path);
                                if ((fileInfo.Attributes & System.IO.FileAttributes.Hidden) is System.IO.FileAttributes.Hidden)
                                {
                                    continue;
                                }

                                fileNameList.Add(new()
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

                        return fileNameList;
                    });

                    AddToFileNamePage(fileNameList);
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Warning, "Drop file in file name page failed", e);
            }
            finally
            {
                dragOperationDeferral.Complete();
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

                    lock (fileNameLock)
                    {
                        count = FileNameCollection.Count;
                    }

                    if (count is 0)
                    {
                        await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.ListEmpty));
                    }
                    else
                    {
                        PreviewChangedFileName();
                    }
                }
                else
                {
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.NoOperation));
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

                    lock (fileNameLock)
                    {
                        count = FileNameCollection.Count;
                    }

                    if (count is 0)
                    {
                        await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.ListEmpty));
                    }
                    else
                    {
                        PreviewChangedFileName();
                        await ChangeFileNameAsync();
                    }
                }
                else
                {
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.NoOperation));
                }
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：文件名称页面——挂载的事件

        /// <summary>
        /// 当文本框中的内容发生更改时发生的事件。
        /// </summary>
        private void OnTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is global::Windows.UI.Xaml.Controls.TextBox textBox && textBox.Tag is string tag)
            {
                if (tag is "RenameRule")
                {
                    RenameRule = textBox.Text;
                }
                else if (tag is "StartNumber")
                {
                    StartNumber = textBox.Text;
                }
                else if (tag is "ExtensionName")
                {
                    ExtensionName = textBox.Text;
                }
                else if (tag is "LookUpString")
                {
                    LookUpString = textBox.Text;
                }
                else if (tag is "ReplaceString")
                {
                    ReplaceString = textBox.Text;
                }
            }
        }

        /// <summary>
        /// 清空列表
        /// </summary>
        private void OnClearListClicked(object sender, RoutedEventArgs args)
        {
            lock (fileNameLock)
            {
                FileNameCollection.Clear();
                OperationFailedCollection.Clear();
            }
        }

        /// <summary>
        /// 关闭改名示例提示
        /// </summary>
        private void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            if (NameChangeFlyout.IsOpen)
            {
                NameChangeFlyout.Hide();
            }
        }

        /// <summary>
        /// 向前导航
        /// </summary>
        private void OnForwardNavigateClicked(object sender, RoutedEventArgs args)
        {
            CurrentIndex = CurrentIndex is 0 ? 3 : CurrentIndex - 1;

            for (int index = 0; index < NameChangeList.Count; index++)
            {
                NameChangeList[index].OriginalFileName = NameChangeDict[CurrentIndex][index].OriginalFileName;
                NameChangeList[index].NewFileName = NameChangeDict[CurrentIndex][index].NewFileName;
            }
        }

        /// <summary>
        /// 向后导航
        /// </summary>
        private void OnNextNavigateClicked(object sender, RoutedEventArgs args)
        {
            CurrentIndex = CurrentIndex is 3 ? 0 : CurrentIndex + 1;

            for (int index = 0; index < NameChangeList.Count; index++)
            {
                NameChangeList[index].OriginalFileName = NameChangeDict[CurrentIndex][index].OriginalFileName;
                NameChangeList[index].NewFileName = NameChangeDict[CurrentIndex][index].NewFileName;
            }
        }

        /// <summary>
        /// 选择编号格式
        /// </summary>
        private void OnNumberFormatClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is KeyValuePair<string, string> numberFormat)
            {
                SelectedNumberFormat = numberFormat;
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

                lock (fileNameLock)
                {
                    count = FileNameCollection.Count;
                }

                if (count is 0)
                {
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.ListEmpty));
                }
                else
                {
                    PreviewChangedFileName();
                }
            }
            else
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.NoOperation));
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

                lock (fileNameLock)
                {
                    count = FileNameCollection.Count;
                }

                if (count is 0)
                {
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.ListEmpty));
                }
                else
                {
                    PreviewChangedFileName();
                    await ChangeFileNameAsync();
                }
            }
            else
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.NoOperation));
            }
        }

        /// <summary>
        /// 选择文件
        /// </summary>
        private async void OnSelectFileClicked(object sender, RoutedEventArgs args)
        {
            OpenFileDialog dialog = new()
            {
                Multiselect = true,
                Title = SelectFileString
            };
            if (dialog.ShowDialog() is DialogResult.OK)
            {
                List<OldAndNewNameModel> fileNameList = await Task.Run(() =>
                {
                    List<OldAndNewNameModel> fileNameList = [];

                    foreach (string fileName in dialog.FileNames)
                    {
                        try
                        {
                            FileInfo fileInfo = new(fileName);
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
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, string.Format("Read file {0} information failed", fileName), e);
                            continue;
                        }
                    }

                    return fileNameList;
                });

                dialog.Dispose();
                AddToFileNamePage(fileNameList);
            }
            dialog.Dispose();
        }

        /// <summary>
        /// 选择文件夹
        /// </summary>
        private async void OnSelectFolderClicked(object sender, RoutedEventArgs args)
        {
            OpenFolderDialog dialog = new()
            {
                Description = SelectFolderString,
            };
            DialogResult result = dialog.ShowDialog();
            if (result is DialogResult.OK || result is DialogResult.Yes)
            {
                OperationFailedCollection.Clear();
                if (!string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    List<OldAndNewNameModel> directoryNameList = [];
                    List<OldAndNewNameModel> fileNameList = [];

                    await Task.Run(() =>
                    {
                        DirectoryInfo currentFolder = new(dialog.SelectedPath);

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
                            LogService.WriteLog(EventLevel.Error, string.Format("Read folder {0} information failed", dialog.SelectedPath), e);
                        }

                        dialog.Dispose();
                    });

                    AddToFileNamePage(directoryNameList);
                    AddToFileNamePage(fileNameList);
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
            ExtensionName = string.Empty;
        }

        /// <summary>
        /// 查看修改失败的文件错误信息
        /// </summary>
        private async void OnViewErrorInformationClicked(object sender, RoutedEventArgs args)
        {
            await MainWindow.Current.ShowDialogAsync(new OperationFailedDialog(OperationFailedCollection));
        }

        #endregion 第二部分：文件名称页面——挂载的事件

        /// <summary>
        /// 添加到文件名称页面
        /// </summary>
        public void AddToFileNamePage(List<OldAndNewNameModel> filenameList)
        {
            lock (fileNameLock)
            {
                foreach (OldAndNewNameModel oldAndNewNameItem in filenameList)
                {
                    FileNameCollection.Add(oldAndNewNameItem);
                }
            }
        }

        private string GetChangeRule(int index)
        {
            return string.Format(ChangeRuleString, NameChangeRuleList[index]);
        }

        /// <summary>
        /// 检查用户是否指定了操作过程
        /// </summary>
        private bool CheckOperationState()
        {
            return !string.IsNullOrEmpty(RenameRule) || !string.IsNullOrEmpty(StartNumber) || IsChecked || !string.IsNullOrEmpty(LookUpString) || !string.IsNullOrEmpty(ReplaceString);
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

            lock (fileNameLock)
            {
                int endIndex = FileNameCollection.Count + startIndex;
                int numberLength = endIndex.ToString().Length;

                foreach (OldAndNewNameModel oldAndNewNameItem in FileNameCollection)
                {
                    string tempNewFileName = oldAndNewNameItem.OriginalFileName;
                    // 根据改名规则替换
                    if (!string.IsNullOrEmpty(RenameRule))
                    {
                        try
                        {
                            string tempFileName = RenameRule;
                            if (tempFileName.Contains("<#>"))
                            {
                                string formattedIndex = string.Empty;
                                if (Equals(SelectedNumberFormat, NumberFormatList[0]))
                                {
                                    formattedIndex = startIndex.ToString().PadLeft(numberLength, '0');
                                }
                                else if (Equals(SelectedNumberFormat, NumberFormatList[1]))
                                {
                                    formattedIndex = startIndex.ToString().PadLeft(1, '0');
                                }
                                else if (Equals(SelectedNumberFormat, NumberFormatList[2]))
                                {
                                    formattedIndex = startIndex.ToString().PadLeft(2, '0');
                                }
                                else if (Equals(SelectedNumberFormat, NumberFormatList[3]))
                                {
                                    formattedIndex = startIndex.ToString().PadLeft(3, '0');
                                }
                                else if (Equals(SelectedNumberFormat, NumberFormatList[4]))
                                {
                                    formattedIndex = startIndex.ToString().PadLeft(4, '0');
                                }
                                else if (Equals(SelectedNumberFormat, NumberFormatList[5]))
                                {
                                    formattedIndex = startIndex.ToString().PadLeft(5, '0');
                                }
                                else if (Equals(SelectedNumberFormat, NumberFormatList[6]))
                                {
                                    formattedIndex = startIndex.ToString().PadLeft(6, '0');
                                }
                                else if (Equals(SelectedNumberFormat, NumberFormatList[7]))
                                {
                                    formattedIndex = startIndex.ToString().PadLeft(7, '0');
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
                                tempFileName = tempFileName.Replace("<&>", oldAndNewNameItem.OriginalFileName);
                            }
                            if (tempFileName.Contains("<N>"))
                            {
                                if ((new FileInfo(oldAndNewNameItem.OriginalFilePath).Attributes & System.IO.FileAttributes.Directory) is not 0)
                                {
                                    DirectoryInfo directoryInfo = new(oldAndNewNameItem.OriginalFilePath);
                                    tempFileName = tempFileName.Replace("<N>", directoryInfo.LastWriteTime.ToString("yyyy-MM-dd"));
                                }
                                else
                                {
                                    FileInfo fileInfo = new(oldAndNewNameItem.OriginalFilePath);
                                    tempFileName = tempFileName.Replace("<N>", fileInfo.LastWriteTime.ToString("yyyy-MM-dd"));
                                }
                            }
                            if (tempFileName.Contains("<C>"))
                            {
                                if ((new FileInfo(oldAndNewNameItem.OriginalFilePath).Attributes & System.IO.FileAttributes.Directory) is not 0)
                                {
                                    DirectoryInfo directoryInfo = new(oldAndNewNameItem.OriginalFilePath);
                                    tempFileName = tempFileName.Replace("<C>", directoryInfo.CreationTime.ToString("yyyy-MM-dd"));
                                }
                                else
                                {
                                    FileInfo fileInfo = new(oldAndNewNameItem.OriginalFilePath);
                                    tempFileName = tempFileName.Replace("<C>", fileInfo.CreationTime.ToString("yyyy-MM-dd"));
                                }
                            }
                            tempNewFileName = tempFileName + Path.GetExtension(oldAndNewNameItem.OriginalFileName);
                        }
                        catch (Exception)
                        {
                            tempNewFileName = oldAndNewNameItem.OriginalFileName;
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

                    oldAndNewNameItem.NewFileName = tempNewFileName;
                    oldAndNewNameItem.NewFilePath = oldAndNewNameItem.OriginalFilePath.Replace(oldAndNewNameItem.OriginalFileName, oldAndNewNameItem.NewFileName);
                }
            }
        }

        /// <summary>
        /// 更改文件名称
        /// </summary>
        private async Task ChangeFileNameAsync()
        {
            IsModifyingNow = true;
            List<OperationFailedModel> operationFailedList = await Task.Run(() =>
            {
                List<OperationFailedModel> operationFailedList = [];

                lock (fileNameLock)
                {
                    foreach (OldAndNewNameModel oldAndNewNameItem in FileNameCollection)
                    {
                        if (!string.IsNullOrEmpty(oldAndNewNameItem.OriginalFileName) && !string.IsNullOrEmpty(oldAndNewNameItem.OriginalFilePath))
                        {
                            if ((new FileInfo(oldAndNewNameItem.OriginalFilePath).Attributes & System.IO.FileAttributes.Directory) is not 0)
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
                }

                return operationFailedList;
            });

            IsModifyingNow = false;
            foreach (OperationFailedModel operationFailedItem in operationFailedList)
            {
                OperationFailedCollection.Add(operationFailedItem);
            }

            int count = FileNameCollection.Count;

            lock (fileNameLock)
            {
                FileNameCollection.Clear();
            }

            await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.File, count - OperationFailedCollection.Count, OperationFailedCollection.Count));
        }
    }
}
