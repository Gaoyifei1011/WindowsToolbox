using Microsoft.Windows.ApplicationModel.Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using WindowsTools.Extensions.DataType.Enums;
using WindowsTools.Helpers.Controls.Extensions;
using WindowsTools.Helpers.Root;
using WindowsTools.Models;
using WindowsTools.Services.Root;
using WindowsTools.Strings;
using WindowsTools.UI.TeachingTips;
using WindowsTools.Views.Windows;
using WindowsTools.WindowsAPI.PInvoke.Shell32;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace WindowsTools.Views.Pages
{
    /// <summary>
    /// 包资源索引提取页面
    /// </summary>
    public sealed partial class PriExtractPage : Page, INotifyPropertyChanged
    {
        private bool isStringAllSelect = false;
        private bool isFilePathAllSelect = false;
        private bool isEmbeddedDataAllSelect = false;
        private string stringFileName;
        private string filePathFileName;
        private ResourceManager resourceManager;
        private ResourceContext resourceContext;

        private bool _isExtractSaveSamely;

        public bool IsExtractSaveSamely
        {
            get { return _isExtractSaveSamely; }

            set
            {
                if (!Equals(_isExtractSaveSamely, value))
                {
                    _isExtractSaveSamely = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsExtractSaveSamely)));
                }
            }
        }

        private bool _isExtractSaveString;

        public bool IsExtractSaveString
        {
            get { return _isExtractSaveString; }

            set
            {
                if (!Equals(_isExtractSaveString, value))
                {
                    _isExtractSaveString = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsExtractSaveString)));
                }
            }
        }

        private bool _isExtractSaveFilePath;

        public bool IsExtractSaveFilePath
        {
            get { return _isExtractSaveFilePath; }

            set
            {
                if (!Equals(_isExtractSaveFilePath, value))
                {
                    _isExtractSaveFilePath = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsExtractSaveFilePath)));
                }
            }
        }

        private bool _isExtractSaveEmbeddedData;

        public bool IsExtractSaveEmbeddedData
        {
            get { return _isExtractSaveEmbeddedData; }

            set
            {
                if (!Equals(_isExtractSaveEmbeddedData, value))
                {
                    _isExtractSaveEmbeddedData = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsExtractSaveEmbeddedData)));
                }
            }
        }

        private bool _isProcessing;

        public bool IsProcessing
        {
            get { return _isProcessing; }

            set
            {
                if (!Equals(_isProcessing, value))
                {
                    _isProcessing = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsProcessing)));
                }
            }
        }

        private string _getResults;

        public string GetResults
        {
            get { return _getResults; }

            set
            {
                if (!Equals(_getResults, value))
                {
                    _getResults = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GetResults)));
                }
            }
        }

        private string _selectedSaveFolder;

        public string SelectedSaveFolder
        {
            get { return _selectedSaveFolder; }

            set
            {
                if (!Equals(_selectedSaveFolder, value))
                {
                    _selectedSaveFolder = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedSaveFolder)));
                }
            }
        }

        private string _inputLanguage;

        public string InputLanguage
        {
            get { return _inputLanguage; }

            set
            {
                if (!Equals(_inputLanguage, value))
                {
                    _inputLanguage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InputLanguage)));
                }
            }
        }

        private DictionaryEntry _selectedResourceCandidateKind;

        public DictionaryEntry SelectedResourceCandidateKind
        {
            get { return _selectedResourceCandidateKind; }

            set
            {
                if (!Equals(_selectedResourceCandidateKind, value))
                {
                    _selectedResourceCandidateKind = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedResourceCandidateKind)));
                }
            }
        }

        private List<DictionaryEntry> ResourceCandidateKindList { get; } =
        [
            new DictionaryEntry(PriExtract.String,ResourceCandidateKind.String),
            new DictionaryEntry(PriExtract.FilePath,ResourceCandidateKind.FilePath),
            new DictionaryEntry(PriExtract.EmbeddedData,ResourceCandidateKind.EmbeddedData)
        ];

        private ObservableCollection<StringModel> StringCollection { get; } = [];

        private ObservableCollection<FilePathModel> FilePathCollection { get; } = [];

        private ObservableCollection<EmbeddedDataModel> EmbeddedDataCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public PriExtractPage()
        {
            InitializeComponent();
            GetResults = PriExtract.NoSelectedFile;
            SelectedResourceCandidateKind = ResourceCandidateKindList[0];
            Shell32Library.SHGetKnownFolderPath(new("374DE290-123F-4565-9164-39C4925E467B"), KNOWN_FOLDER_FLAG.KF_FLAG_DEFAULT, IntPtr.Zero, out string downloadFolder);
            SelectedSaveFolder = downloadFolder;
        }

        #region 第一部分：重写父类事件

        /// <summary>
        /// 设置拖动的数据的可视表示形式
        /// </summary>
        protected override void OnDragOver(global::Windows.UI.Xaml.DragEventArgs args)
        {
            base.OnDragOver(args);

            IReadOnlyList<IStorageItem> dragItemsList = args.DataView.GetStorageItemsAsync().AsTask().Result;

            if (dragItemsList.Count is 1)
            {
                string extensionName = Path.GetExtension(dragItemsList[0].Name);

                if (extensionName.Equals(".pri", StringComparison.OrdinalIgnoreCase))
                {
                    args.AcceptedOperation = DataPackageOperation.Copy;
                    args.DragUIOverride.IsCaptionVisible = true;
                    args.DragUIOverride.IsContentVisible = false;
                    args.DragUIOverride.IsGlyphVisible = true;
                    args.DragUIOverride.Caption = PriExtract.DragOverContent;
                }
                else
                {
                    args.AcceptedOperation = DataPackageOperation.None;
                    args.DragUIOverride.IsCaptionVisible = true;
                    args.DragUIOverride.IsContentVisible = false;
                    args.DragUIOverride.IsGlyphVisible = true;
                    args.DragUIOverride.Caption = PriExtract.NoOtherExtensionNameFile;
                }
            }
            else
            {
                args.AcceptedOperation = DataPackageOperation.None;
                args.DragUIOverride.IsCaptionVisible = true;
                args.DragUIOverride.IsContentVisible = false;
                args.DragUIOverride.IsGlyphVisible = true;
                args.DragUIOverride.Caption = PriExtract.NoMultiFile;
            }

            args.Handled = true;
        }

        /// <summary>
        /// 拖动文件完成后获取文件信息
        /// </summary>
        protected override void OnDrop(global::Windows.UI.Xaml.DragEventArgs args)
        {
            base.OnDrop(args);
            DragOperationDeferral deferral = args.GetDeferral();
            DataPackageView view = args.DataView;

            Task.Run(async () =>
            {
                try
                {
                    if (view.Contains(StandardDataFormats.StorageItems))
                    {
                        IReadOnlyList<IStorageItem> filesList = await view.GetStorageItemsAsync();

                        if (filesList.Count is 1)
                        {
                            MainWindow.Current.BeginInvoke(() =>
                            {
                                ParseResourceFile(filesList[0].Path);
                            });
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Warning, "Drop file in pri extract page failed", e);
                    MainWindow.Current.BeginInvoke(() =>
                    {
                        IsProcessing = false;
                    });
                }
            });
            deferral.Complete();
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 复制字符串到剪贴板
        /// </summary>
        private void OnStringExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            StringModel stringItem = args.Parameter as StringModel;

            if (stringItem is not null)
            {
                bool copyResult = CopyPasteHelper.CopyToClipboard(string.Format("Key:{0}, Content:{1}", stringItem.Key, stringItem.Content));
                TeachingTipHelper.Show(new DataCopyTip(DataCopyKind.String, copyResult, false));
            }
        }

        /// <summary>
        /// 复制文件路径到剪贴板
        /// </summary>
        private void OnFilePathExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            FilePathModel filePathItem = args.Parameter as FilePathModel;

            if (filePathItem is not null)
            {
                bool copyResult = CopyPasteHelper.CopyToClipboard(string.Format("Key:{0}, AbsolutePath:{1}", filePathItem.Key, filePathItem.AbsolutePath));
                TeachingTipHelper.Show(new DataCopyTip(DataCopyKind.String, copyResult, false));
            }
        }

        /// <summary>
        /// 导出嵌入数据
        /// </summary>
        private void OnEmbeddedDataExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            EmbeddedDataModel embeddedDataItem = args.Parameter as EmbeddedDataModel;

            if (embeddedDataItem is not null)
            {
                FolderBrowserDialog dialog = new()
                {
                    Description = PriExtract.SelectFolder,
                    ShowNewFolderButton = true,
                    RootFolder = Environment.SpecialFolder.Desktop
                };
                DialogResult result = dialog.ShowDialog();
                if (result is DialogResult.OK || result is DialogResult.Yes)
                {
                    Task.Run(() =>
                    {
                        try
                        {
                            if (resourceManager is not null && resourceContext is not null)
                            {
                                byte[] byteArray = resourceManager.MainResourceMap.GetValueByIndex(embeddedDataItem.EmbeddedDataIndex, resourceContext).Value.ValueAsBytes;

                                FileStream fileStream = new(Path.Combine(dialog.SelectedPath, Path.GetFileName(embeddedDataItem.Key)), FileMode.OpenOrCreate, FileAccess.Write);
                                fileStream.Write(byteArray, 0, byteArray.Length);
                                fileStream.Close();
                                fileStream.Dispose();
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, string.Format("Save resourceCandidate embedded data(key:{0}) failed", embeddedDataItem.Key), e);
                        }

                        try
                        {
                            Process process = new();
                            process.StartInfo.FileName = "explorer.exe";
                            process.StartInfo.Arguments = "/select," + Path.Combine(dialog.SelectedPath, Path.GetFileName(embeddedDataItem.Key));
                            process.Start();
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, string.Format("Open saved embedded data folder {0} failed", dialog.SelectedPath), e);
                        }

                        MainWindow.Current.BeginInvoke(() =>
                        {
                            IsProcessing = false;
                        });
                    });
                }
            }
        }

        #endregion 第二部分：XamlUICommand 命令调用时挂载的事件

        #region 第三部分：包资源索引提取——挂载的事件

        /// <summary>
        /// 字符串列表全选和全部不选
        /// </summary>
        private void OnStringSelectTapped(object sender, TappedRoutedEventArgs args)
        {
            if (!isStringAllSelect)
            {
                isStringAllSelect = true;

                foreach (StringModel stringItem in StringCollection)
                {
                    stringItem.IsSelected = true;
                }
            }
            else
            {
                isStringAllSelect = false;

                foreach (StringModel stringItem in StringCollection)
                {
                    stringItem.IsSelected = false;
                }
            }
        }

        /// <summary>
        /// 文件路径列表全选和全部不选
        /// </summary>
        private void OnFilePathSelectTapped(object sender, TappedRoutedEventArgs args)
        {
            if (!isFilePathAllSelect)
            {
                isFilePathAllSelect = true;

                foreach (FilePathModel filePathItem in FilePathCollection)
                {
                    filePathItem.IsSelected = true;
                }
            }
            else
            {
                isFilePathAllSelect = false;

                foreach (FilePathModel filePathItem in FilePathCollection)
                {
                    filePathItem.IsSelected = false;
                }
            }
        }

        /// <summary>
        /// 嵌入数据列表全选和全部不选
        /// </summary>
        private void OnEmbeddedDataSelectTapped(object sender, TappedRoutedEventArgs args)
        {
            if (!isEmbeddedDataAllSelect)
            {
                isEmbeddedDataAllSelect = true;

                foreach (EmbeddedDataModel embeddedDataItem in EmbeddedDataCollection)
                {
                    embeddedDataItem.IsSelected = true;
                }
            }
            else
            {
                isEmbeddedDataAllSelect = false;

                foreach (EmbeddedDataModel embeddedDataItem in EmbeddedDataCollection)
                {
                    embeddedDataItem.IsSelected = false;
                }
            }
        }

        /// <summary>
        /// 提取时同时保存单选框点击时触发的事件
        /// </summary>
        private void OnExtractSaveSamelyClicked(object sender, RoutedEventArgs args)
        {
            if (!IsExtractSaveSamely)
            {
                IsExtractSaveString = false;
                IsExtractSaveEmbeddedData = false;
            }
        }

        /// <summary>
        /// 自动保存时选择保存的文件夹
        /// </summary>
        private void OnSelectSaveFolderClicked(object sender, RoutedEventArgs args)
        {
            FolderBrowserDialog dialog = new()
            {
                Description = PriExtract.SelectFolder,
                ShowNewFolderButton = true,
                RootFolder = Environment.SpecialFolder.Desktop,
                SelectedPath = SelectedSaveFolder
            };
            DialogResult result = dialog.ShowDialog();
            if (result is DialogResult.OK || result is DialogResult.Yes)
            {
                SelectedSaveFolder = dialog.SelectedPath;
            }
        }

        /// <summary>
        /// 输入的语言文本框内容发生变化时触发的事件
        /// </summary>
        private void OnInputLanguageTextChanged(object sender, TextChangedEventArgs args)
        {
            InputLanguage = (sender as global::Windows.UI.Xaml.Controls.TextBox).Text;
        }

        /// <summary>
        /// 选择文件
        /// </summary>
        private void OnSelectFileClicked(object sender, RoutedEventArgs args)
        {
            OpenFileDialog dialog = new()
            {
                Multiselect = false,
                Filter = PriExtract.FilterCondition,
                Title = PriExtract.SelectFile
            };
            if (dialog.ShowDialog() is DialogResult.OK && !string.IsNullOrEmpty(dialog.FileName))
            {
                ParseResourceFile(dialog.FileName);
            }
        }

        /// <summary>
        /// 选择要显示的封装的资源类型数据的格式
        /// </summary>
        private void OnSelectedResourceCandidateKindClicked(object sender, RoutedEventArgs args)
        {
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            if (item.Tag is not null)
            {
                SelectedResourceCandidateKind = ResourceCandidateKindList[Convert.ToInt32(item.Tag)];
            }
        }

        /// <summary>
        /// 复制选中的字符串
        /// </summary>
        private void OnCopySelectedStringClicked(object sender, RoutedEventArgs args)
        {
            IsProcessing = true;
            List<StringModel> selectedStringList = StringCollection.Where(item => item.IsSelected == true).ToList();
            if (selectedStringList.Count > 0)
            {
                StringBuilder copyStringBuilder = new();
                foreach (StringModel stringItem in selectedStringList)
                {
                    copyStringBuilder.AppendLine(string.Format("Key:{0}, Content:{1}", stringItem.Key, stringItem.Content));
                }
                bool copyResult = CopyPasteHelper.CopyToClipboard(copyStringBuilder.ToString());
                TeachingTipHelper.Show(new DataCopyTip(DataCopyKind.String, copyResult, true, selectedStringList.Count));
            }
            IsProcessing = false;
        }

        /// <summary>
        /// 复制选中的文件路径
        /// </summary>
        private void OnCopySelectedFilePathClicked(object sender, RoutedEventArgs args)
        {
            IsProcessing = true;
            List<FilePathModel> selectedFilePathList = FilePathCollection.Where(item => item.IsSelected is true).ToList();
            if (selectedFilePathList.Count > 0)
            {
                StringBuilder copyFilePathBuilder = new();
                foreach (FilePathModel filePathItem in selectedFilePathList)
                {
                    copyFilePathBuilder.AppendLine(string.Format("Key:{0}, AbsolutePath:{1}", filePathItem.Key, filePathItem.AbsolutePath));
                }
                bool copyResult = CopyPasteHelper.CopyToClipboard(copyFilePathBuilder.ToString());
                TeachingTipHelper.Show(new DataCopyTip(DataCopyKind.String, copyResult, true, selectedFilePathList.Count));
            }
            IsProcessing = false;
        }

        /// <summary>
        /// 复制选中的嵌入数据
        /// </summary>
        private void OnExportSelectedEmbeddedDataClicked(object sender, RoutedEventArgs args)
        {
            List<EmbeddedDataModel> selectedEmbeddedDataList = EmbeddedDataCollection.Where(item => item.IsSelected is true).ToList();
            if (selectedEmbeddedDataList.Count > 0)
            {
                IsProcessing = true;
                FolderBrowserDialog dialog = new()
                {
                    Description = PriExtract.SelectFolder,
                    ShowNewFolderButton = true,
                    RootFolder = Environment.SpecialFolder.Desktop
                };
                DialogResult result = dialog.ShowDialog();
                if (result is DialogResult.OK || result is DialogResult.Yes)
                {
                    Task.Run(() =>
                    {
                        foreach (EmbeddedDataModel embeddedDataItem in selectedEmbeddedDataList)
                        {
                            try
                            {
                                if (resourceManager is not null && resourceContext is not null)
                                {
                                    byte[] byteArray = resourceManager.MainResourceMap.GetValueByIndex(embeddedDataItem.EmbeddedDataIndex, resourceContext).Value.ValueAsBytes;

                                    FileStream fileStream = new(Path.Combine(dialog.SelectedPath, Path.GetFileName(embeddedDataItem.Key)), FileMode.OpenOrCreate, FileAccess.Write);
                                    fileStream.Write(byteArray, 0, byteArray.Length);
                                    fileStream.Close();
                                    fileStream.Dispose();
                                }
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(EventLevel.Error, string.Format("Save resourceCandidate embedded data(key:{0}) failed", embeddedDataItem.Key), e);
                                continue;
                            }
                        }

                        try
                        {
                            Process.Start(dialog.SelectedPath);
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, string.Format("Open saved embedded data folder {0} failed", dialog.SelectedPath), e);
                        }

                        MainWindow.Current.BeginInvoke(() =>
                        {
                            IsProcessing = false;
                        });
                    });
                }
                else
                {
                    IsProcessing = false;
                }
            }
        }

        /// <summary>
        /// 复制所有的字符串
        /// </summary>
        private void OnCopyAllStringClicked(object sender, RoutedEventArgs args)
        {
            IsProcessing = true;
            List<StringModel> stringList = [.. StringCollection];
            if (stringList.Count > 0)
            {
                StringBuilder copyStringBuilder = new();
                foreach (StringModel stringItem in stringList)
                {
                    copyStringBuilder.AppendLine(string.Format("Key:{0}, Content:{1}", stringItem.Key, stringItem.Content));
                }
                bool copyResult = CopyPasteHelper.CopyToClipboard(copyStringBuilder.ToString());
                TeachingTipHelper.Show(new DataCopyTip(DataCopyKind.String, copyResult, true, stringList.Count));
            }
            IsProcessing = false;
        }

        /// <summary>
        /// 复制所有的文件路径
        /// </summary>
        private void OnCopyAllFilePathClicked(object sender, RoutedEventArgs args)
        {
            IsProcessing = true;
            List<FilePathModel> filePathList = [.. FilePathCollection];
            if (filePathList.Count > 0)
            {
                StringBuilder copyFilePathBuilder = new();
                foreach (FilePathModel filePathItem in filePathList)
                {
                    copyFilePathBuilder.AppendLine(string.Format("Key:{0}, AbsolutePath:{1}", filePathItem.Key, filePathItem.AbsolutePath));
                }
                bool copyResult = CopyPasteHelper.CopyToClipboard(copyFilePathBuilder.ToString());
                TeachingTipHelper.Show(new DataCopyTip(DataCopyKind.String, copyResult, true, filePathList.Count));
            }
            IsProcessing = false;
        }

        /// <summary>
        /// 导出所有的嵌入数据
        /// </summary>
        private void OnExportAllEmbeddedDataClicked(object sender, RoutedEventArgs args)
        {
            FolderBrowserDialog dialog = new()
            {
                Description = PriExtract.SelectFolder,
                ShowNewFolderButton = true,
                RootFolder = Environment.SpecialFolder.Desktop
            };
            DialogResult result = dialog.ShowDialog();
            if (result is DialogResult.OK || result is DialogResult.Yes)
            {
                IsProcessing = true;
                List<EmbeddedDataModel> embeddedDataList = [.. EmbeddedDataCollection];
                if (embeddedDataList.Count > 0)
                {
                    Task.Run(() =>
                    {
                        foreach (EmbeddedDataModel embeddedDataItem in embeddedDataList)
                        {
                            try
                            {
                                if (resourceManager is not null && resourceContext is not null)
                                {
                                    byte[] byteArray = resourceManager.MainResourceMap.GetValueByIndex(embeddedDataItem.EmbeddedDataIndex, resourceContext).Value.ValueAsBytes;

                                    FileStream fileStream = new(Path.Combine(dialog.SelectedPath, Path.GetFileName(embeddedDataItem.Key)), FileMode.OpenOrCreate, FileAccess.Write);
                                    fileStream.Write(byteArray, 0, byteArray.Length);
                                    fileStream.Close();
                                    fileStream.Dispose();
                                }
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(EventLevel.Error, string.Format("Save resourceCandidate embedded data(key:{0}) failed", embeddedDataItem.Key), e);
                                continue;
                            }
                        }

                        try
                        {
                            Process.Start(dialog.SelectedPath);
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, string.Format("Open saved embedded data folder {0} failed", dialog.SelectedPath), e);
                        }

                        MainWindow.Current.BeginInvoke(() =>
                        {
                            IsProcessing = false;
                        });
                    });
                }
            }
        }

        #endregion 第三部分：包资源索引提取——挂载的事件

        /// <summary>
        /// 解析 PRI 资源文件
        /// </summary>
        public void ParseResourceFile(string filePath)
        {
            IsProcessing = true;
            StringCollection.Clear();
            FilePathCollection.Clear();
            EmbeddedDataCollection.Clear();

            Task.Run(() =>
            {
                try
                {
                    resourceManager = null;
                    stringFileName = string.Format("{0} - {1}.txt", Path.GetFileName(filePath), "Strings.txt");
                    filePathFileName = string.Format("{0} - {1}.txt", Path.GetFileName(filePath), "FilePath.txt");
                    resourceManager = new ResourceManager(filePath);

                    if (resourceManager is not null)
                    {
                        resourceContext = resourceManager.CreateResourceContext();
                        if (!string.IsNullOrEmpty(InputLanguage))
                        {
                            resourceContext.QualifierValues["language"] = InputLanguage;
                        }

                        ResourceMap mainResourceMap = resourceManager.MainResourceMap;
                        int collectedIndex = 0;

                        List<StringModel> stringList = [];
                        List<FilePathModel> filePathList = [];
                        List<EmbeddedDataModel> embeddedDataList = [];

                        for (uint index = 0; index < mainResourceMap.ResourceCount; index++)
                        {
                            try
                            {
                                KeyValuePair<string, ResourceCandidate> resourceCandidateItem = mainResourceMap.GetValueByIndex(index, resourceContext);

                                // 资源是字符串
                                if (resourceCandidateItem.Value.Kind is ResourceCandidateKind.String)
                                {
                                    StringModel stringItem = new()
                                    {
                                        IsSelected = false,
                                        Key = resourceCandidateItem.Key,
                                        Content = resourceCandidateItem.Value.ValueAsString,
                                        StringIndex = index
                                    };
                                    stringList.Add(stringItem);

                                    // 自动保存字符串
                                    if (IsExtractSaveString)
                                    {
                                        try
                                        {
                                            File.AppendAllText(Path.Combine(SelectedSaveFolder, stringFileName), string.Format("Key: {0} - Content:{1}{2}", stringItem.Key, stringItem.Content, Environment.NewLine));
                                        }
                                        catch (Exception e)
                                        {
                                            LogService.WriteLog(EventLevel.Error, string.Format("Save resourceCandidate string(key:{0},Content:{1}) failed", stringItem.Key, stringItem.Content), e);
                                        }
                                    }
                                }

                                // 资源是位于指定位置的文件
                                else if (resourceCandidateItem.Value.Kind is ResourceCandidateKind.FilePath)
                                {
                                    FilePathModel filePathItem = new()
                                    {
                                        IsSelected = false,
                                        Key = resourceCandidateItem.Key,
                                        AbsolutePath = resourceCandidateItem.Value.ValueAsString,
                                        FilePathIndex = index
                                    };
                                    filePathList.Add(filePathItem);

                                    // 自动保存文件路径
                                    if (IsExtractSaveFilePath)
                                    {
                                        try
                                        {
                                            File.AppendAllText(Path.Combine(SelectedSaveFolder, filePathFileName), string.Format("Key: {0} - AbsolutePath:{1}{2}", filePathItem.Key, filePathItem.AbsolutePath, Environment.NewLine));
                                        }
                                        catch (Exception e)
                                        {
                                            LogService.WriteLog(EventLevel.Error, string.Format("Save resourceCandidate filePath(key:{0},AbsolutePath:{1}) failed", filePathItem.Key, filePathItem.AbsolutePath), e);
                                        }
                                    }
                                }

                                // 资源是某些包含资源文件 (（如 .resw 文件) ）中的嵌入数据
                                else if (resourceCandidateItem.Value.Kind is ResourceCandidateKind.EmbeddedData)
                                {
                                    EmbeddedDataModel embeddedDataItem = new()
                                    {
                                        IsSelected = false,
                                        Key = resourceCandidateItem.Key,
                                        EmbeddedDataIndex = index
                                    };
                                    embeddedDataList.Add(embeddedDataItem);

                                    // 自动保存资源文件嵌入数据
                                    if (IsExtractSaveEmbeddedData)
                                    {
                                        try
                                        {
                                            byte[] byteArray = resourceCandidateItem.Value.ValueAsBytes;

                                            FileStream fileStream = new(Path.Combine(SelectedSaveFolder, Path.GetFileName(embeddedDataItem.Key)), FileMode.OpenOrCreate, FileAccess.Write);
                                            fileStream.Write(byteArray, 0, byteArray.Length);
                                            fileStream.Close();
                                            fileStream.Dispose();
                                        }
                                        catch (Exception e)
                                        {
                                            LogService.WriteLog(EventLevel.Error, string.Format("Save resourceCandidate embedded data(key:{0}) failed", resourceCandidateItem.Key), e);
                                        }
                                    }
                                }

                                collectedIndex++;
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                        }

                        Task.Delay(300);
                        MainWindow.Current.BeginInvoke(() =>
                        {
                            try
                            {
                                GetResults = string.Format(PriExtract.GetResults, Path.GetFileName(filePath), stringList.Count + filePathList.Count + embeddedDataList.Count);

                                foreach (StringModel stringItem in stringList)
                                {
                                    StringCollection.Add(stringItem);
                                }

                                foreach (FilePathModel filePathItem in filePathList)
                                {
                                    FilePathCollection.Add(filePathItem);
                                }

                                foreach (EmbeddedDataModel embeddedDataItem in embeddedDataList)
                                {
                                    EmbeddedDataCollection.Add(embeddedDataItem);
                                }

                                IsProcessing = false;
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(EventLevel.Error, string.Format("Get pri data from {0} file failed", filePath), e);
                            }
                        });
                    }
                    else
                    {
                        MainWindow.Current.BeginInvoke(() =>
                        {
                            IsProcessing = false;
                            GetResults = string.Format(PriExtract.GetResults, Path.GetFileName(filePath), 0);
                        });
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, string.Format("Parse file {0} resources failed", filePath), e);
                }
            });
        }
    }
}
