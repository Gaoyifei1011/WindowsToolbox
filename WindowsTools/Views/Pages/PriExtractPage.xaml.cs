using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Globalization;
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
using WindowsTools.Extensions.DataType.Methods;
using WindowsTools.Extensions.PriExtract;
using WindowsTools.Helpers.Controls;
using WindowsTools.Helpers.Root;
using WindowsTools.Models;
using WindowsTools.Services.Root;
using WindowsTools.UI.TeachingTips;
using WindowsTools.WindowsAPI.ComTypes;
using WindowsTools.WindowsAPI.PInvoke.Shell32;

// 抑制 CA1806，IDE0060 警告
#pragma warning disable CA1806,IDE0060

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
        private bool isLoadCompleted = false;
        private string stringFileName;
        private string filePathFileName;

        private string SelectedFolder { get; } = ResourceService.PriExtractResource.GetString("SelectedFolder");

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

        private KeyValuePair<string, string> _selectedLanguage;

        public KeyValuePair<string, string> SelectedLanguage
        {
            get { return _selectedLanguage; }

            set
            {
                if (!Equals(_selectedLanguage, value))
                {
                    _selectedLanguage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedLanguage)));
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

        private string _searchText;

        public string SearchText
        {
            get { return _searchText; }

            set
            {
                if (!Equals(_searchText, value))
                {
                    _searchText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchText)));
                }
            }
        }

        private KeyValuePair<string, string> _selectedResourceCandidateKind;

        public KeyValuePair<string, string> SelectedResourceCandidateKind
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

        private List<KeyValuePair<string, string>> ResourceCandidateKindList { get; } =
        [
            new KeyValuePair<string,string>("String", ResourceService.PriExtractResource.GetString("String")),
            new KeyValuePair<string,string>("FilePath", ResourceService.PriExtractResource.GetString("FilePath")),
            new KeyValuePair<string,string>("EmbeddedData", ResourceService.PriExtractResource.GetString("EmbeddedData"))
        ];

        private readonly List<StringModel> stringList = [];
        private readonly List<FilePathModel> filePathList = [];
        private readonly List<EmbeddedDataModel> embeddedDataList = [];

        private ObservableCollection<LanguageModel> LanguageCollection { get; } = [];

        private ObservableCollection<StringModel> StringCollection { get; } = [];

        private ObservableCollection<FilePathModel> FilePathCollection { get; } = [];

        private ObservableCollection<EmbeddedDataModel> EmbeddedDataCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public PriExtractPage()
        {
            InitializeComponent();
            GetResults = ResourceService.PriExtractResource.GetString("NoSelectedFile");
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
                    args.DragUIOverride.Caption = ResourceService.PriExtractResource.GetString("DragOverContent");
                }
                else
                {
                    args.AcceptedOperation = DataPackageOperation.None;
                    args.DragUIOverride.IsCaptionVisible = true;
                    args.DragUIOverride.IsContentVisible = false;
                    args.DragUIOverride.IsGlyphVisible = true;
                    args.DragUIOverride.Caption = ResourceService.PriExtractResource.GetString("NoOtherExtensionNameFile");
                }
            }
            else
            {
                args.AcceptedOperation = DataPackageOperation.None;
                args.DragUIOverride.IsCaptionVisible = true;
                args.DragUIOverride.IsContentVisible = false;
                args.DragUIOverride.IsGlyphVisible = true;
                args.DragUIOverride.Caption = ResourceService.PriExtractResource.GetString("NoMultiFile");
            }

            args.Handled = true;
        }

        /// <summary>
        /// 拖动文件完成后获取文件信息
        /// </summary>
        protected override async void OnDrop(global::Windows.UI.Xaml.DragEventArgs args)
        {
            base.OnDrop(args);
            DragOperationDeferral deferral = args.GetDeferral();
            DataPackageView view = args.DataView;

            IReadOnlyList<IStorageItem> filesList = await Task.Run(async () =>
            {
                try
                {
                    if (view.Contains(StandardDataFormats.StorageItems))
                    {
                        return await view.GetStorageItemsAsync();
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Warning, "Drop file in pri extract page failed", e);
                }

                return null;
            });

            if (filesList is not null && filesList.Count is 1)
            {
                ParseResourceFile(filesList[0].Path);
            }
            else
            {
                IsProcessing = false;
            }

            deferral.Complete();
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：XamlUICommand 命令调用时挂载的事件

        private async void OnLanguageExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (LanguageFlyout.IsOpen)
            {
                LanguageFlyout.Hide();
            }

            if (args.Parameter is LanguageModel languageItem && isLoadCompleted)
            {
                foreach (LanguageModel item in LanguageCollection)
                {
                    item.IsChecked = false;
                    if (languageItem.LangaugeInfo.Key.Equals(item.LangaugeInfo.Key))
                    {
                        SelectedLanguage = item.LangaugeInfo;
                        item.IsChecked = true;
                    }
                }

                StringCollection.Clear();

                if (SelectedLanguage.Equals(LanguageCollection[0]))
                {
                    foreach (StringModel stringItem in stringList)
                    {
                        StringCollection.Add(stringItem);
                    }
                }
                else
                {
                    List<StringModel> coincidentStringList = await Task.Run(() =>
                    {
                        return stringList.Where(item => item.Language.Equals(SelectedLanguage.Key, StringComparison.OrdinalIgnoreCase)).ToList();
                    });

                    foreach (StringModel stringItem in coincidentStringList)
                    {
                        StringCollection.Add(stringItem);
                    }
                }

                foreach (StringModel stringItem in stringList)
                {
                    stringItem.IsSelected = false;
                }
            }
        }

        /// <summary>
        /// 复制字符串到剪贴板
        /// </summary>
        private async void OnStringExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is StringModel stringItem)
            {
                bool copyResult = CopyPasteHelper.CopyToClipboard(string.Format("Key:{0}, Content:{1}", stringItem.Key, stringItem.Content));
                await TeachingTipHelper.ShowAsync(new DataCopyTip(DataCopyKind.String, copyResult, false));
            }
        }

        /// <summary>
        /// 复制文件路径到剪贴板
        /// </summary>
        private async void OnFilePathExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is FilePathModel filePathItem)
            {
                bool copyResult = CopyPasteHelper.CopyToClipboard(string.Format("Key:{0}, FilePath:{1}", filePathItem.Key, filePathItem.AbsolutePath));
                await TeachingTipHelper.ShowAsync(new DataCopyTip(DataCopyKind.String, copyResult, false));
            }
        }

        /// <summary>
        /// 导出嵌入数据
        /// </summary>
        private async void OnEmbeddedDataExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is EmbeddedDataModel embeddedDataItem)
            {
                OpenFolderDialog dialog = new()
                {
                    Description = ResourceService.PriExtractResource.GetString("SelectFolder"),
                    RootFolder = Environment.SpecialFolder.Desktop
                };

                DialogResult result = dialog.ShowDialog();
                if (result is DialogResult.OK || result is DialogResult.Yes)
                {
                    await Task.Run(() =>
                    {
                        try
                        {
                            File.WriteAllBytes(Path.Combine(dialog.SelectedPath, Path.GetFileName(embeddedDataItem.Key)), embeddedDataItem.EmbeddedData);

                            IntPtr pidlList = Shell32Library.ILCreateFromPath(Path.Combine(dialog.SelectedPath, Path.GetFileName(embeddedDataItem.Key)));
                            if (pidlList != IntPtr.Zero)
                            {
                                Shell32Library.SHOpenFolderAndSelectItems(pidlList, 0, IntPtr.Zero, 0);
                                Shell32Library.ILFree(pidlList);
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, string.Format("Open saved embedded data folder {0} failed", dialog.SelectedPath), e);
                        }

                        dialog.Dispose();
                    });

                    IsProcessing = false;
                }
                else
                {
                    dialog.Dispose();
                }
            }
        }

        #endregion 第二部分：XamlUICommand 命令调用时挂载的事件

        #region 第三部分：包资源索引提取——挂载的事件

        /// <summary>
        /// 单击时修改字符串项选中值
        /// </summary>
        private void OnStringItemClicked(object sender, ItemClickEventArgs args)
        {
            if (args.ClickedItem is StringModel stringItem)
            {
                int clickedIndex = StringCollection.IndexOf(stringItem);
                StringCollection[clickedIndex].IsSelected = !StringCollection[clickedIndex].IsSelected;
            }
        }

        /// <summary>
        /// 单击时修改文件路径项选中值
        /// </summary>
        private void OnFilePathItemClicked(object sender, ItemClickEventArgs args)
        {
            if (args.ClickedItem is FilePathModel filePathItem)
            {
                int clickedIndex = FilePathCollection.IndexOf(filePathItem);
                FilePathCollection[clickedIndex].IsSelected = !FilePathCollection[clickedIndex].IsSelected;
            }
        }

        /// <summary>
        /// 单击时修改嵌入的数据项选中值
        /// </summary>
        private void OnEmbeddedDataItemClicked(object sender, ItemClickEventArgs args)
        {
            if (args.ClickedItem is EmbeddedDataModel embeddedDataItem)
            {
                int clickedIndex = EmbeddedDataCollection.IndexOf(embeddedDataItem);
                EmbeddedDataCollection[clickedIndex].IsSelected = !EmbeddedDataCollection[clickedIndex].IsSelected;
            }
        }

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
                IsExtractSaveFilePath = false;
                IsExtractSaveEmbeddedData = false;
            }
        }

        /// <summary>
        /// 自动保存时选择保存的文件夹
        /// </summary>
        private void OnSelectSaveFolderClicked(object sender, RoutedEventArgs args)
        {
            OpenFolderDialog dialog = new()
            {
                Description = ResourceService.PriExtractResource.GetString("SelectFolder"),
                RootFolder = Environment.SpecialFolder.Desktop
            };
            DialogResult result = dialog.ShowDialog();
            if (result is DialogResult.OK || result is DialogResult.Yes)
            {
                SelectedSaveFolder = dialog.SelectedPath;
            }
            dialog.Dispose();
        }

        /// <summary>
        /// 语言选择菜单打开时自动定位到选中项
        /// </summary>
        private void OnOpened(object sender, object args)
        {
            foreach (LanguageModel languageItem in LanguageCollection)
            {
                if (languageItem.IsChecked)
                {
                    LanguageListView.ScrollIntoView(languageItem);
                    break;
                }
            }
        }

        /// <summary>
        /// 选择文件
        /// </summary>
        private void OnSelectFileClicked(object sender, RoutedEventArgs args)
        {
            OpenFileDialog dialog = new()
            {
                Multiselect = false,
                Filter = ResourceService.PriExtractResource.GetString("FilterCondition"),
                Title = ResourceService.PriExtractResource.GetString("SelectFile")
            };

            if (dialog.ShowDialog() is DialogResult.OK && !string.IsNullOrEmpty(dialog.FileName))
            {
                ParseResourceFile(dialog.FileName);
            }
            dialog.Dispose();
        }

        /// <summary>
        /// 选择要显示的封装的资源类型数据的格式
        /// </summary>
        private void OnSelectedResourceCandidateKindClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is not null)
            {
                SelectedResourceCandidateKind = ResourceCandidateKindList[Convert.ToInt32(radioMenuFlyoutItem.Tag)];
            }
        }

        /// <summary>
        /// 复制选中的字符串
        /// </summary>
        private async void OnCopySelectedStringClicked(object sender, RoutedEventArgs args)
        {
            IsProcessing = true;
            List<StringModel> selectedStringList = StringCollection.Where(item => item.IsSelected is true).ToList();
            if (selectedStringList.Count > 0)
            {
                StringBuilder copyStringBuilder = new();
                foreach (StringModel stringItem in selectedStringList)
                {
                    copyStringBuilder.AppendLine(string.Format("Key:{0}, Content:{1}", stringItem.Key, stringItem.Content));
                }
                bool copyResult = CopyPasteHelper.CopyToClipboard(copyStringBuilder.ToString());
                await TeachingTipHelper.ShowAsync(new DataCopyTip(DataCopyKind.String, copyResult, true, selectedStringList.Count));
            }
            IsProcessing = false;
        }

        /// <summary>
        /// 复制选中的文件路径
        /// </summary>
        private async void OnCopySelectedFilePathClicked(object sender, RoutedEventArgs args)
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
                await TeachingTipHelper.ShowAsync(new DataCopyTip(DataCopyKind.String, copyResult, true, selectedFilePathList.Count));
            }
            IsProcessing = false;
        }

        /// <summary>
        /// 导出选中的嵌入数据
        /// </summary>
        private async void OnExportSelectedEmbeddedDataClicked(object sender, RoutedEventArgs args)
        {
            List<EmbeddedDataModel> selectedEmbeddedDataList = EmbeddedDataCollection.Where(item => item.IsSelected is true).ToList();
            if (selectedEmbeddedDataList.Count > 0)
            {
                IsProcessing = true;
                OpenFolderDialog dialog = new()
                {
                    Description = ResourceService.PriExtractResource.GetString("SelectFolder"),
                    RootFolder = Environment.SpecialFolder.Desktop
                };
                DialogResult result = dialog.ShowDialog();
                if (result is DialogResult.OK || result is DialogResult.Yes)
                {
                    await Task.Run(() =>
                    {
                        try
                        {
                            foreach (EmbeddedDataModel embeddedDataItem in selectedEmbeddedDataList)
                            {
                                File.WriteAllBytes(Path.Combine(dialog.SelectedPath, Path.GetFileName(embeddedDataItem.Key)), embeddedDataItem.EmbeddedData);
                            }

                            Process.Start(dialog.SelectedPath);
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, string.Format("Open saved embedded data folder {0} failed", dialog.SelectedPath), e);
                        }

                        dialog.Dispose();
                    });

                    IsProcessing = false;
                }
                else
                {
                    IsProcessing = false;
                    dialog.Dispose();
                }
            }
        }

        /// <summary>
        /// 复制所有的字符串
        /// </summary>
        private async void OnCopyAllStringClicked(object sender, RoutedEventArgs args)
        {
            IsProcessing = true;
            List<StringModel> copyAllStringList = [.. StringCollection];
            if (copyAllStringList.Count > 0)
            {
                StringBuilder copyStringBuilder = new();
                foreach (StringModel stringItem in copyAllStringList)
                {
                    copyStringBuilder.AppendLine(string.Format("Key:{0}, Content:{1}", stringItem.Key, stringItem.Content));
                }
                bool copyResult = CopyPasteHelper.CopyToClipboard(copyStringBuilder.ToString());
                await TeachingTipHelper.ShowAsync(new DataCopyTip(DataCopyKind.String, copyResult, true, copyAllStringList.Count));
            }
            IsProcessing = false;
        }

        /// <summary>
        /// 复制所有的文件路径
        /// </summary>
        private async void OnCopyAllFilePathClicked(object sender, RoutedEventArgs args)
        {
            IsProcessing = true;
            List<FilePathModel> copyAllFilePathList = [.. FilePathCollection];
            if (copyAllFilePathList.Count > 0)
            {
                StringBuilder copyFilePathBuilder = new();
                foreach (FilePathModel filePathItem in copyAllFilePathList)
                {
                    copyFilePathBuilder.AppendLine(string.Format("Key:{0}, AbsolutePath:{1}", filePathItem.Key, filePathItem.AbsolutePath));
                }
                bool copyResult = CopyPasteHelper.CopyToClipboard(copyFilePathBuilder.ToString());
                await TeachingTipHelper.ShowAsync(new DataCopyTip(DataCopyKind.String, copyResult, true, copyAllFilePathList.Count));
            }
            IsProcessing = false;
        }

        /// <summary>
        /// 导出所有的嵌入数据
        /// </summary>
        private async void OnExportAllEmbeddedDataClicked(object sender, RoutedEventArgs args)
        {
            OpenFolderDialog dialog = new()
            {
                Description = ResourceService.PriExtractResource.GetString("SelectFolder"),
                RootFolder = Environment.SpecialFolder.Desktop
            };

            DialogResult result = dialog.ShowDialog();
            if (result is DialogResult.OK || result is DialogResult.Yes)
            {
                IsProcessing = true;
                List<EmbeddedDataModel> exportAllEmbeddedDataList = [.. EmbeddedDataCollection];
                if (exportAllEmbeddedDataList.Count > 0)
                {
                    await Task.Run(() =>
                    {
                        try
                        {
                            foreach (EmbeddedDataModel embeddedDataItem in exportAllEmbeddedDataList)
                            {
                                File.WriteAllBytes(Path.Combine(dialog.SelectedPath, Path.GetFileName(embeddedDataItem.Key)), embeddedDataItem.EmbeddedData);
                            }

                            Process.Start(dialog.SelectedPath);
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, string.Format("Open saved embedded data folder {0} failed", dialog.SelectedPath), e);
                        }

                        dialog.Dispose();
                    });

                    IsProcessing = false;
                }
            }
            else
            {
                dialog.Dispose();
            }
        }

        #endregion 第三部分：包资源索引提取——挂载的事件

        /// <summary>
        /// 解析 PRI 资源文件
        /// </summary>
        public async void ParseResourceFile(string filePath)
        {
            isLoadCompleted = false;
            IsProcessing = true;
            LanguageCollection.Clear();
            StringCollection.Clear();
            FilePathCollection.Clear();
            EmbeddedDataCollection.Clear();
            List<string> languageList = [];

            bool result = await Task.Run(() =>
            {
                stringFileName = string.Format("{0} - {1}.txt", Path.GetFileName(filePath), "Strings.txt");
                filePathFileName = string.Format("{0} - {1}.txt", Path.GetFileName(filePath), "FilePath.txt");

                stringList.Clear();
                filePathList.Clear();
                embeddedDataList.Clear();

                try
                {
                    // 尝试读取文件二进制流
                    using FileStream priFileStream = File.OpenRead(filePath);

                    // 读取并检查文件类型
                    BinaryReader priBinaryReader = new(priFileStream, Encoding.ASCII, true);
                    long fileStartOffset = priBinaryReader.BaseStream.Position;
                    string priType = new(priBinaryReader.ReadChars(8));

                    if (priType is not "mrm_pri0" && priType is not "mrm_pri1" && priType is not "mrm_pri2" && priType is not "mrm_prif")
                    {
                        throw new InvalidDataException("Data does not start with a PRI file header.");
                    }

                    priBinaryReader.ExpectUInt16(0);
                    priBinaryReader.ExpectUInt16(1);
                    uint totalFileSize = priBinaryReader.ReadUInt32();
                    uint tocOffset = priBinaryReader.ReadUInt32();
                    uint sectionStartOffset = priBinaryReader.ReadUInt32();
                    uint numSections = priBinaryReader.ReadUInt16();
                    priBinaryReader.ExpectUInt16(0xFFFF);
                    priBinaryReader.ExpectUInt32(0);
                    priBinaryReader.BaseStream.Seek(fileStartOffset + totalFileSize - 16, SeekOrigin.Begin);
                    priBinaryReader.ExpectUInt32(0xDEFFFADE);
                    priBinaryReader.ExpectUInt32(totalFileSize);
                    priBinaryReader.ExpectString(priType);
                    priBinaryReader.BaseStream.Seek(tocOffset, SeekOrigin.Begin);

                    // 读取内容列表
                    List<TocEntry> tocList = new((int)numSections);

                    for (int index = 0; index < numSections; index++)
                    {
                        tocList.Add(new TocEntry()
                        {
                            SectionIdentifier = new(priBinaryReader.ReadChars(16)),
                            Flags = priBinaryReader.ReadUInt16(),
                            SectionFlags = priBinaryReader.ReadUInt16(),
                            SectionQualifier = priBinaryReader.ReadUInt32(),
                            SectionOffset = priBinaryReader.ReadUInt32(),
                            SectionLength = priBinaryReader.ReadUInt32(),
                        });
                    }

                    // 读取分段列表
                    object[] sectionArray = new object[numSections];

                    for (int index = 0; index < sectionArray.Length; index++)
                    {
                        if (sectionArray[index] is null)
                        {
                            priBinaryReader.BaseStream.Seek(sectionStartOffset + tocList[index].SectionOffset, SeekOrigin.Begin);

                            switch (tocList[index].SectionIdentifier)
                            {
                                case "[mrm_pridescex]\0":
                                    {
                                        PriDescriptorSection section = new("[mrm_pridescex]\0", priBinaryReader);
                                        sectionArray[index] = section;
                                        break;
                                    }
                                case "[mrm_hschema]  \0":
                                    {
                                        HierarchicalSchemaSection section = new("[mrm_hschema]  \0", priBinaryReader, false);
                                        sectionArray[index] = section;
                                        break;
                                    }
                                case "[mrm_hschemaex] ":
                                    {
                                        HierarchicalSchemaSection section = new("[mrm_hschemaex] ", priBinaryReader, true);
                                        sectionArray[index] = section;
                                        break;
                                    }
                                case "[mrm_decn_info]\0":
                                    {
                                        DecisionInfoSection section = new("[mrm_decn_info]\0", priBinaryReader);
                                        sectionArray[index] = section;
                                        break;
                                    }
                                case "[mrm_res_map__]\0":
                                    {
                                        ResourceMapSection section = new("[mrm_res_map__]\0", priBinaryReader, false, ref sectionArray);
                                        sectionArray[index] = section;
                                        break;
                                    }
                                case "[mrm_res_map2_]\0":
                                    {
                                        ResourceMapSection section = new("[mrm_res_map2_]\0", priBinaryReader, true, ref sectionArray);
                                        sectionArray[index] = section;
                                        break;
                                    }
                                case "[mrm_dataitem] \0":
                                    {
                                        DataItemSection section = new("[mrm_dataitem] \0", priBinaryReader);
                                        sectionArray[index] = section;
                                        break;
                                    }
                                case "[mrm_rev_map]  \0":
                                    {
                                        ReverseMapSection section = new("[mrm_rev_map]  \0", priBinaryReader);
                                        sectionArray[index] = section;
                                        break;
                                    }
                                case "[def_file_list]\0":
                                    {
                                        ReferencedFileSection section = new("[def_file_list]\0", priBinaryReader);
                                        sectionArray[index] = section;
                                        break;
                                    }
                                default:
                                    {
                                        UnknownSection section = new(null, priBinaryReader);
                                        sectionArray[index] = section;
                                        break;
                                    }
                            }
                        }
                    }

                    // 根据分段列表获取相应的内容
                    List<PriDescriptorSection> priDescriptorSectionList = sectionArray.OfType<PriDescriptorSection>().ToList();

                    foreach (PriDescriptorSection priDescriptorSection in priDescriptorSectionList)
                    {
                        foreach (int resourceMapIndex in priDescriptorSection.ResourceMapSectionsList)
                        {
                            if (sectionArray[resourceMapIndex] is ResourceMapSection resourceMapSection)
                            {
                                if (resourceMapSection.HierarchicalSchemaReference is not null)
                                {
                                    continue;
                                }

                                DecisionInfoSection decisionInfoSection = sectionArray[resourceMapSection.DecisionInfoSectionIndex] as DecisionInfoSection;

                                foreach (CandidateSet candidateSet in resourceMapSection.CandidateSetsDict.Values)
                                {
                                    if (sectionArray[candidateSet.ResourceMapSectionAndIndex.Item1] is HierarchicalSchemaSection hierarchicalSchemaSection)
                                    {
                                        ResourceMapScopeAndItem resourceMapScopeAndItem = hierarchicalSchemaSection.ItemsList[candidateSet.ResourceMapSectionAndIndex.Item2];

                                        string key = string.Empty;

                                        if (resourceMapScopeAndItem.Name is not null && resourceMapScopeAndItem.Parent is not null)
                                        {
                                            key = Path.Combine(resourceMapScopeAndItem.Parent.Name, resourceMapScopeAndItem.Name);
                                        }
                                        else if (resourceMapScopeAndItem.Name is not null)
                                        {
                                            key = resourceMapScopeAndItem.Name;
                                        }

                                        if (string.IsNullOrEmpty(key))
                                        {
                                            continue;
                                        }

                                        foreach (Candidate candidate in candidateSet.CandidatesList)
                                        {
                                            string value = string.Empty;

                                            if (candidate.SourceFileIndex is null)
                                            {
                                                ByteSpan byteSpan = null;

                                                if (candidate.DataItemSectionAndIndex is not null)
                                                {
                                                    DataItemSection dataItemSection = sectionArray[candidate.DataItemSectionAndIndex.Item1] as DataItemSection;
                                                    byteSpan = dataItemSection is not null ? dataItemSection.DataItemsList[candidate.DataItemSectionAndIndex.Item2] : candidate.Data;
                                                }

                                                if (byteSpan is not null)
                                                {
                                                    priFileStream.Seek(byteSpan.Offset, SeekOrigin.Begin);
                                                    using BinaryReader binaryReader = new(priFileStream, Encoding.Default, true);
                                                    byte[] data = binaryReader.ReadBytes((int)byteSpan.Length);

                                                    switch (candidate.Type)
                                                    {
                                                        // ASCII 格式路径内容
                                                        case ResourceValueType.AsciiPath:
                                                            {
                                                                string absolutePath = Path.Combine(Path.GetDirectoryName(filePath), Encoding.ASCII.GetString(data).TrimEnd('\0'));

                                                                // 自动保存文件路径
                                                                if (IsExtractSaveFilePath)
                                                                {
                                                                    try
                                                                    {
                                                                        File.AppendAllText(Path.Combine(SelectedSaveFolder, filePathFileName), string.Format("Key: {0} - AbsolutePath:{1}{2}", key, absolutePath, Environment.NewLine));
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        LogService.WriteLog(EventLevel.Error, string.Format("Save resourceCandidate filePath(key:{0},AbsolutePath:{1}) failed", key, absolutePath), e);
                                                                    }
                                                                }

                                                                filePathList.Add(new FilePathModel()
                                                                {
                                                                    Key = key,
                                                                    IsSelected = false,
                                                                    AbsolutePath = absolutePath,
                                                                });
                                                                break;
                                                            }
                                                        // ASCII 格式字符串内容
                                                        case ResourceValueType.AsciiString:
                                                            {
                                                                string content = Encoding.ASCII.GetString(data).TrimEnd('\0');

                                                                // 自动保存字符串
                                                                if (IsExtractSaveString)
                                                                {
                                                                    try
                                                                    {
                                                                        File.AppendAllText(Path.Combine(SelectedSaveFolder, stringFileName), string.Format("Key: {0} - Content:{1}{2}", key, content, Environment.NewLine));
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        LogService.WriteLog(EventLevel.Error, string.Format("Save resourceCandidate string(key:{0},Content:{1}) failed", key, content), e);
                                                                    }
                                                                }

                                                                stringList.Add(new StringModel()
                                                                {
                                                                    Key = key,
                                                                    Content = content,
                                                                    Language = decisionInfoSection.QualifierSetsList[candidate.QualifierSet].QualifiersList.Count > 0 ? decisionInfoSection.QualifierSetsList[candidate.QualifierSet].QualifiersList[0].Value : string.Empty,
                                                                    IsSelected = false
                                                                });
                                                                break;
                                                            }
                                                        // UTF8 格式路径内容
                                                        case ResourceValueType.Utf8Path:
                                                            {
                                                                string absolutePath = Path.Combine(Path.GetDirectoryName(filePath), Encoding.UTF8.GetString(data).TrimEnd('\0'));

                                                                // 自动保存文件路径
                                                                if (IsExtractSaveFilePath)
                                                                {
                                                                    try
                                                                    {
                                                                        File.AppendAllText(Path.Combine(SelectedSaveFolder, filePathFileName), string.Format("Key: {0} - AbsolutePath:{1}{2}", key, absolutePath, Environment.NewLine));
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        LogService.WriteLog(EventLevel.Error, string.Format("Save resourceCandidate filePath(key:{0},AbsolutePath:{1}) failed", key, absolutePath), e);
                                                                    }
                                                                }

                                                                filePathList.Add(new FilePathModel()
                                                                {
                                                                    Key = key,
                                                                    IsSelected = false,
                                                                    AbsolutePath = absolutePath,
                                                                });
                                                                break;
                                                            }
                                                        // UTF8 格式字符串内容
                                                        case ResourceValueType.Utf8String:
                                                            {
                                                                string content = Encoding.UTF8.GetString(data).TrimEnd('\0');

                                                                // 自动保存字符串
                                                                if (IsExtractSaveString)
                                                                {
                                                                    try
                                                                    {
                                                                        File.AppendAllText(Path.Combine(SelectedSaveFolder, stringFileName), string.Format("Key: {0} - Content:{1}{2}", key, content, Environment.NewLine));
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        LogService.WriteLog(EventLevel.Error, string.Format("Save resourceCandidate string(key:{0},Content:{1}) failed", key, content), e);
                                                                    }
                                                                }

                                                                stringList.Add(new StringModel()
                                                                {
                                                                    Key = key,
                                                                    Content = content,
                                                                    Language = decisionInfoSection.QualifierSetsList[candidate.QualifierSet].QualifiersList.Count > 0 ? decisionInfoSection.QualifierSetsList[candidate.QualifierSet].QualifiersList[0].Value : string.Empty,
                                                                    IsSelected = false
                                                                });
                                                                break;
                                                            }
                                                        // Unicode 格式路径内容
                                                        case ResourceValueType.UnicodePath:
                                                            {
                                                                string absolutePath = Path.Combine(Path.GetDirectoryName(filePath), Encoding.Unicode.GetString(data).TrimEnd('\0'));

                                                                // 自动保存文件路径
                                                                if (IsExtractSaveFilePath)
                                                                {
                                                                    try
                                                                    {
                                                                        File.AppendAllText(Path.Combine(SelectedSaveFolder, filePathFileName), string.Format("Key: {0} - AbsolutePath:{1}{2}", key, absolutePath, Environment.NewLine));
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        LogService.WriteLog(EventLevel.Error, string.Format("Save resourceCandidate filePath(key:{0},AbsolutePath:{1}) failed", key, absolutePath), e);
                                                                    }
                                                                }

                                                                filePathList.Add(new FilePathModel()
                                                                {
                                                                    Key = key,
                                                                    IsSelected = false,
                                                                    AbsolutePath = absolutePath,
                                                                });
                                                                break;
                                                            }
                                                        // Unicode 格式字符串内容
                                                        case ResourceValueType.UnicodeString:
                                                            {
                                                                string content = Encoding.Unicode.GetString(data).TrimEnd('\0');

                                                                // 自动保存字符串
                                                                if (IsExtractSaveString)
                                                                {
                                                                    try
                                                                    {
                                                                        File.AppendAllText(Path.Combine(SelectedSaveFolder, stringFileName), string.Format("Key: {0} - Content:{1}{2}", key, content, Environment.NewLine));
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        LogService.WriteLog(EventLevel.Error, string.Format("Save resourceCandidate string(key:{0},Content:{1}) failed", key, content), e);
                                                                    }
                                                                }

                                                                stringList.Add(new StringModel()
                                                                {
                                                                    Key = key,
                                                                    Content = content,
                                                                    Language = decisionInfoSection.QualifierSetsList[candidate.QualifierSet].QualifiersList.Count > 0 ? decisionInfoSection.QualifierSetsList[candidate.QualifierSet].QualifiersList[0].Value : string.Empty,
                                                                    IsSelected = false
                                                                });
                                                                break;
                                                            }
                                                        case ResourceValueType.EmbeddedData:
                                                            {
                                                                // 自动保存资源文件嵌入数据
                                                                if (IsExtractSaveEmbeddedData)
                                                                {
                                                                    try
                                                                    {
                                                                        File.WriteAllBytes(Path.Combine(SelectedSaveFolder, Path.GetFileName(key)), data);
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        LogService.WriteLog(EventLevel.Error, string.Format("Save resourceCandidate embedded data(key:{0}) failed", key), e);
                                                                    }
                                                                }

                                                                embeddedDataList.Add(new EmbeddedDataModel()
                                                                {
                                                                    Key = key,
                                                                    EmbeddedData = data,
                                                                    IsSelected = false,
                                                                });
                                                                break;
                                                            }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // 根据分段列表得到的内容进行归纳分类
                    languageList.AddRange(stringList.Select(item => item.Language).Distinct());
                    languageList.Sort();
                    stringList.Sort();
                    filePathList.Sort((item1, item2) => item1.Key.CompareTo(item2.Key));
                    embeddedDataList.Sort((item1, item2) => item1.Key.CompareTo(item2.Key));
                    return true;
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, string.Format("Parse file {0} resources failed", filePath), e);
                    return false;
                }
            });

            if (result)
            {
                // 显示获取到的所有内容
                LanguageCollection.Add(new LanguageModel()
                {
                    IsChecked = true,
                    LangaugeInfo = new KeyValuePair<string, string>("AllLanguage", ResourceService.PriExtractResource.GetString("AllLanguage"))
                });

                foreach (string languageItem in languageList)
                {
                    CultureInfo cultureInfo = CultureInfo.GetCultureInfo(languageItem);
                    LanguageCollection.Add(new LanguageModel()
                    {
                        IsChecked = false,
                        LangaugeInfo = new KeyValuePair<string, string>(cultureInfo.Name, string.Format("{0}[{1}]", cultureInfo.DisplayName, languageItem))
                    });
                }

                SelectedLanguage = LanguageCollection[0].LangaugeInfo;

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
                GetResults = string.Format(ResourceService.PriExtractResource.GetString("GetResults"), Path.GetFileName(filePath), stringList.Count + filePathList.Count + embeddedDataList.Count);
                isLoadCompleted = true;
            }
            else
            {
                IsProcessing = false;
                GetResults = string.Format(ResourceService.PriExtractResource.GetString("GetResults"), Path.GetFileName(filePath), 0);
                isLoadCompleted = true;
            }
        }
    }
}
