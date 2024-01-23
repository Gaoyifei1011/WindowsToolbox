using Microsoft.Windows.ApplicationModel.Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WindowsTools.Models;
using WindowsTools.Services.Root;
using WindowsTools.Strings;
using WindowsTools.Views.Windows;

namespace WindowsTools.Views.Pages
{
    /// <summary>
    /// 包资源索引提取页面
    /// </summary>
    public sealed partial class PriExtractPage : Page, INotifyPropertyChanged
    {
        private ResourceManager resourceManager;

        private bool _isExtractSaveSamely;

        public bool IsExtractSaveSamely
        {
            get { return _isExtractSaveSamely; }

            set
            {
                _isExtractSaveSamely = value;
                OnPropertyChanged();
            }
        }

        private bool _isExtractSaveString;

        public bool IsExtractSaveString
        {
            get { return _isExtractSaveString; }

            set
            {
                _isExtractSaveString = value;
                OnPropertyChanged();
            }
        }

        private bool _isExtractSaveEmbeddedData;

        public bool IsExtractSaveEmbeddedData
        {
            get { return _isExtractSaveEmbeddedData; }

            set
            {
                _isExtractSaveEmbeddedData = value;
                OnPropertyChanged();
            }
        }

        private string _getResults;

        public string GetResults
        {
            get { return _getResults; }

            set
            {
                _getResults = value;
                OnPropertyChanged();
            }
        }

        private DictionaryEntry _selectedResourceCandidateKind;

        public DictionaryEntry SelectedResourceCandidateKind
        {
            get { return _selectedResourceCandidateKind; }

            set
            {
                _selectedResourceCandidateKind = value;
                OnPropertyChanged();
            }
        }

        private List<DictionaryEntry> ResourceCandidateKindList { get; } = new List<DictionaryEntry>()
        {
            new DictionaryEntry(){ Key = PriExtract.String, Value = ResourceCandidateKind.String },
            new DictionaryEntry(){ Key = PriExtract.FilePath, Value = ResourceCandidateKind.FilePath },
            new DictionaryEntry(){ Key = PriExtract.EmbeddedData, Value = ResourceCandidateKind.EmbeddedData }
        };

        private ObservableCollection<StringsModel> StringCollection { get; } = new ObservableCollection<StringsModel>();

        private ObservableCollection<FilePathModel> FilePathCollection { get; } = new ObservableCollection<FilePathModel>();

        private ObservableCollection<EmbeddedDataModel> EmbeddedDataModelCollection { get; } = new ObservableCollection<EmbeddedDataModel>();

        public event PropertyChangedEventHandler PropertyChanged;

        public PriExtractPage()
        {
            InitializeComponent();
            GetResults = PriExtract.NoSelectedFile;
            SelectedResourceCandidateKind = ResourceCandidateKindList[0];
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
                            resourceManager = new ResourceManager(filesList[0].Path);
                            ResourceMap mainResourceMap = resourceManager.MainResourceMap;
                            int collectedIndex = 0;

                            for (uint index = 0; index < mainResourceMap.ResourceCount; index++)
                            {
                                try
                                {
                                    KeyValuePair<string, ResourceCandidate> resourceCandidateItem = mainResourceMap.GetValueByIndex(index);

                                    if (resourceCandidateItem.Value.Kind is ResourceCandidateKind.String)
                                    {
                                        string resourceString = resourceCandidateItem.Value.ValueAsString;
                                    }
                                    else if (resourceCandidateItem.Value.Kind is ResourceCandidateKind.FilePath)
                                    {
                                        string resourceString = resourceCandidateItem.Value.ValueAsString;
                                    }
                                    else if (resourceCandidateItem.Value.Kind is ResourceCandidateKind.EmbeddedData)
                                    {
                                        byte[] resourceBytes = resourceCandidateItem.Value.ValueAsBytes;
                                    }

                                    collectedIndex++;
                                }
                                catch (Exception)
                                {
                                    continue;
                                }
                            }

                            if (resourceManager is not null)
                            {
                                MainWindow.Current.BeginInvoke(() =>
                                {
                                    try
                                    {
                                    }
                                    catch (Exception e)
                                    {
                                        LogService.WriteLog(EventLogEntryType.Error, string.Format("Get pri data from {0} file failed", filesList[0].Path), e);
                                    }
                                });
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLogEntryType.Error, "Read file from dragover failed", e);
                }
            });
            deferral.Complete();
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：包资源索引提取——挂载的事件

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
        /// 选择文件
        /// </summary>
        private void OnSelectFileClicked(object sender, RoutedEventArgs args)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Filter = PriExtract.FilterCondition;
            dialog.Title = PriExtract.SelectFile;
            if (dialog.ShowDialog() is DialogResult.OK)
            {
                Task.Run(() =>
                {
                    try
                    {
                        resourceManager = new ResourceManager(dialog.FileName);
                    }
                    catch (Exception)
                    {
                    }
                });
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

        #endregion 第二部分：包资源索引提取——挂载的事件

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
