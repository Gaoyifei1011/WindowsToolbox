using System;
using System.Collections;
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
    /// 文件名称页面
    /// </summary>
    public sealed partial class FileNamePage : Page, INotifyPropertyChanged
    {
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
                if (!Equals(_renameRule, value))
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
                if (!Equals(_startNumber, value))
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
                if (!Equals(_extensionName, value))
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
                if (!Equals(_lookUpString, value))
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
                if (!Equals(_replaceString, value))
                {
                    _replaceString = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReplaceString)));
                }
            }
        }

        private DictionaryEntry _selectedNumberFormat;

        public DictionaryEntry SelectedNumberFormat
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

        private List<string> NameChangeRuleList { get; } =
        [
            FileName.NameChangeRule1,
            FileName.NameChangeRule2,
            FileName.NameChangeRule3,
            FileName.NameChangeRule4,
        ];

        private readonly List<DictionaryEntry> NumberFormatList =
        [
            new DictionaryEntry( FileName.Auto, "Auto"),
            new DictionaryEntry( "0", "0"),
            new DictionaryEntry( "00", "00"),
            new DictionaryEntry( "000", "000"),
            new DictionaryEntry( "0000", "0000"),
            new DictionaryEntry( "00000", "00000"),
            new DictionaryEntry( "000000", "000000"),
            new DictionaryEntry( "0000000", "0000000"),
        ];

        private readonly List<OldAndNewNameModel> NameChangeList =
        [
            new(){ OriginalFileName = string.Empty, NewFileName = string.Empty },
            new(){ OriginalFileName = string.Empty, NewFileName = string.Empty },
            new(){ OriginalFileName = string.Empty, NewFileName = string.Empty },
            new(){ OriginalFileName = string.Empty, NewFileName = string.Empty },
        ];

        private readonly Dictionary<int, List<OldAndNewNameModel>> NameChangeDict = new()
        {
            { 0, new List<OldAndNewNameModel>()
                {
                    new(){ OriginalFileName = FileName.NameChangeOriginalName1, NewFileName = FileName.NameChangeList1ChangedName1 },
                    new(){ OriginalFileName = FileName.NameChangeOriginalName2, NewFileName = FileName.NameChangeList1ChangedName2 },
                    new(){ OriginalFileName = FileName.NameChangeOriginalName3, NewFileName = FileName.NameChangeList1ChangedName3 },
                    new(){ OriginalFileName = FileName.NameChangeOriginalName4, NewFileName = FileName.NameChangeList1ChangedName4 },
                }
            },
            { 1, new List<OldAndNewNameModel>()
                {
                    new(){ OriginalFileName = FileName.NameChangeOriginalName1, NewFileName = FileName.NameChangeList2ChangedName1 },
                    new(){ OriginalFileName = FileName.NameChangeOriginalName2, NewFileName = FileName.NameChangeList2ChangedName2 },
                    new(){ OriginalFileName = FileName.NameChangeOriginalName3, NewFileName = FileName.NameChangeList2ChangedName3 },
                    new(){ OriginalFileName = FileName.NameChangeOriginalName4, NewFileName = FileName.NameChangeList2ChangedName4 },
                }
            },
            { 2, new List<OldAndNewNameModel>()
                {
                    new(){ OriginalFileName = FileName.NameChangeOriginalName1, NewFileName = FileName.NameChangeList3ChangedName1 },
                    new(){ OriginalFileName = FileName.NameChangeOriginalName2, NewFileName = FileName.NameChangeList3ChangedName2 },
                    new(){ OriginalFileName = FileName.NameChangeOriginalName3, NewFileName = FileName.NameChangeList3ChangedName3 },
                    new(){ OriginalFileName = FileName.NameChangeOriginalName4, NewFileName = FileName.NameChangeList3ChangedName4 },
                }
            },
            { 3, new List<OldAndNewNameModel>()
                {
                    new(){ OriginalFileName = FileName.NameChangeOriginalName1, NewFileName = FileName.NameChangeList4ChangedName1 },
                    new(){ OriginalFileName = FileName.NameChangeOriginalName2, NewFileName = FileName.NameChangeList4ChangedName2 },
                    new(){ OriginalFileName = FileName.NameChangeOriginalName3, NewFileName = FileName.NameChangeList4ChangedName3 },
                    new(){ OriginalFileName = FileName.NameChangeOriginalName4, NewFileName = FileName.NameChangeList4ChangedName4 },
                }
            },
        };

        private ObservableCollection<OldAndNewNameModel> FileNameCollection { get; } = [];

        private ObservableCollection<OperationFailedModel> OperationFailedCollection { get; } = [];

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
            args.DragUIOverride.Caption = FileName.DragOverContent;
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
                        List<OldAndNewNameModel> fileNameList = [];
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

                        AddToFileNamePage(fileNameList);
                    });
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Warning, "Drop file in file name page failed", e);
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
                    if (FileNameCollection.Count is 0)
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
                    if (FileNameCollection.Count is 0)
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

        #region 第二部分：文件名称页面——挂载的事件

        /// <summary>
        /// 当文本框中的内容发生更改时发生的事件。
        /// </summary>
        private void OnTextChanged(object sender, TextChangedEventArgs args)
        {
            global::Windows.UI.Xaml.Controls.TextBox textBox = sender as global::Windows.UI.Xaml.Controls.TextBox;

            if (textBox is not null)
            {
                string tag = Convert.ToString(textBox.Tag);

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
        private void OnNextNavigateClicked(object sender, RoutedEventArgs args)
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
        private void OnNumberFormatClicked(object sender, RoutedEventArgs args)
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
        private void OnPreviewClicked(object sender, RoutedEventArgs args)
        {
            bool checkResult = CheckOperationState();
            if (checkResult)
            {
                OperationFailedCollection.Clear();
                if (FileNameCollection.Count is 0)
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
                if (FileNameCollection.Count is 0)
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
                Title = FileName.SelectFile
            };
            if (dialog.ShowDialog() is DialogResult.OK)
            {
                Task.Run(() =>
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

                    AddToFileNamePage(fileNameList);
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
                Description = FileName.SelectFolder,
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
                            LogService.WriteLog(EventLevel.Error, string.Format("Read folder {0} information failed", dialog.SelectedPath), e);
                        }

                        AddToFileNamePage(directoryNameList);
                        AddToFileNamePage(fileNameList);
                    });
                }
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
            await ContentDialogHelper.ShowAsync(new OperationFailedDialog(OperationFailedCollection), this);
        }

        #endregion 第二部分：文件名称页面——挂载的事件

        /// <summary>
        /// 添加到文件名称页面
        /// </summary>
        public void AddToFileNamePage(List<OldAndNewNameModel> filenameList)
        {
            MainWindow.Current.BeginInvoke(() =>
            {
                lock (fileNameLock)
                {
                    foreach (OldAndNewNameModel oldAndNewNameItem in filenameList)
                    {
                        FileNameCollection.Add(oldAndNewNameItem);
                    }
                }
            });
        }

        private string GetChangeRule(int index)
        {
            return string.Format(FileName.ChangeRule, NameChangeRuleList[index]);
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

            int endIndex = FileNameCollection.Count - startIndex;
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
                            if (SelectedNumberFormat.Value.Equals(NumberFormatList[0].Value))
                            {
                                formattedIndex = startIndex.ToString().PadLeft(numberLength, '0');
                            }
                            else if (SelectedNumberFormat.Value.Equals(NumberFormatList[1].Value))
                            {
                                formattedIndex = startIndex.ToString().PadLeft(1, '0');
                            }
                            else if (SelectedNumberFormat.Value.Equals(NumberFormatList[2].Value))
                            {
                                formattedIndex = startIndex.ToString().PadLeft(2, '0');
                            }
                            else if (SelectedNumberFormat.Value.Equals(NumberFormatList[3].Value))
                            {
                                formattedIndex = startIndex.ToString().PadLeft(3, '0');
                            }
                            else if (SelectedNumberFormat.Value.Equals(NumberFormatList[4].Value))
                            {
                                formattedIndex = startIndex.ToString().PadLeft(4, '0');
                            }
                            else if (SelectedNumberFormat.Value.Equals(NumberFormatList[5].Value))
                            {
                                formattedIndex = startIndex.ToString().PadLeft(5, '0');
                            }
                            else if (SelectedNumberFormat.Value.Equals(NumberFormatList[6].Value))
                            {
                                formattedIndex = startIndex.ToString().PadLeft(6, '0');
                            }
                            else if (SelectedNumberFormat.Value.Equals(NumberFormatList[7].Value))
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
                            if (IOHelper.IsDir(oldAndNewNameItem.OriginalFilePath))
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
                            if (IOHelper.IsDir(oldAndNewNameItem.OriginalFilePath))
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

        /// <summary>
        /// 更改文件名称
        /// </summary>
        private void ChangeFileName()
        {
            List<OperationFailedModel> operationFailedList = [];
            IsModifyingNow = true;
            Task.Run(async () =>
            {
                foreach (OldAndNewNameModel oldAndNewNameItem in FileNameCollection)
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

                    TeachingTipHelper.Show(new OperationResultTip(OperationKind.File, FileNameCollection.Count - OperationFailedCollection.Count, OperationFailedCollection.Count));
                    lock (fileNameLock)
                    {
                        FileNameCollection.Clear();
                    }
                });
            });
        }
    }
}
