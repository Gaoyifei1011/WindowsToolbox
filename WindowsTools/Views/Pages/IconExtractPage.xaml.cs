using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using WindowsTools.Extensions.DataType.Enums;
using WindowsTools.Helpers.Controls.Extensions;
using WindowsTools.Models;
using WindowsTools.Services.Root;
using WindowsTools.Strings;
using WindowsTools.UI.TeachingTips;
using WindowsTools.Views.Windows;
using WindowsTools.WindowsAPI.PInvoke.User32;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace WindowsTools.Views.Pages
{
    /// <summary>
    /// 提取图标页面
    /// </summary>
    public sealed partial class IconExtractPage : Page, INotifyPropertyChanged
    {
        private readonly object iconExtractLock = new();

        private string filePath;

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }

            set
            {
                if (!Equals(_isSelected, value))
                {
                    _isSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
                }
            }
        }

        private bool _isImageEmpty;

        public bool IsImageEmpty
        {
            get { return _isImageEmpty; }

            set
            {
                if (!Equals(_isImageEmpty, value))
                {
                    _isImageEmpty = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsImageEmpty)));
                }
            }
        }

        private bool _isSaving;

        public bool IsSaving
        {
            get { return _isSaving; }

            set
            {
                if (!Equals(_isSaving, value))
                {
                    _isSaving = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSaving)));
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

        private string _noResources;

        public string NoResources
        {
            get { return _noResources; }

            set
            {
                if (!Equals(_noResources, value))
                {
                    _noResources = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NoResources)));
                }
            }
        }

        private DictionaryEntry _selectedIconFormat;

        public DictionaryEntry SelectedIconFormat
        {
            get { return _selectedIconFormat; }

            set
            {
                if (!Equals(_selectedIconFormat, value))
                {
                    _selectedIconFormat = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedIconFormat)));
                }
            }
        }

        private DictionaryEntry _selectedIconSize;

        public DictionaryEntry SelectedIconSize
        {
            get { return _selectedIconSize; }

            set
            {
                if (!Equals(_selectedIconSize, value))
                {
                    _selectedIconSize = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedIconSize)));
                }
            }
        }

        private ImageSource _imageSource;

        public ImageSource ImageSource
        {
            get { return _imageSource; }

            set
            {
                if (!Equals(_imageSource, value))
                {
                    _imageSource = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImageSource)));
                }
            }
        }

        private List<DictionaryEntry> IconFormatList { get; } =
        [
            new DictionaryEntry() { Key = ".ico", Value = ".ico" },
            new DictionaryEntry() { Key = ".png", Value = ".png" }
        ];

        private List<DictionaryEntry> IconSizeList { get; set; } =
        [
            new DictionaryEntry() { Key = "16 * 16", Value = 16 },
            new DictionaryEntry() { Key = "24 * 24", Value = 24 },
            new DictionaryEntry() { Key = "32 * 32", Value = 32 },
            new DictionaryEntry() { Key = "48 * 48", Value = 48 },
            new DictionaryEntry() { Key = "64 * 64", Value = 64 },
            new DictionaryEntry() { Key = "96 * 96", Value = 96 },
            new DictionaryEntry() { Key = "128 * 128", Value = 128 },
            new DictionaryEntry() { Key = "256 * 256", Value = 256 }
        ];

        private ObservableCollection<IconModel> IconCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public IconExtractPage()
        {
            InitializeComponent();
            SelectedIconFormat = IconFormatList[1];
            SelectedIconSize = IconSizeList[7];
            IsSelected = false;
            IsImageEmpty = true;

            GetResults = IconExtract.NoSelectedFile;
            NoResources = IconExtract.PleaseSelectFile;
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

                if (extensionName.Equals(".exe", StringComparison.OrdinalIgnoreCase) || extensionName.Equals(".dll", StringComparison.OrdinalIgnoreCase))
                {
                    args.AcceptedOperation = DataPackageOperation.Copy;
                    args.DragUIOverride.IsCaptionVisible = true;
                    args.DragUIOverride.IsContentVisible = false;
                    args.DragUIOverride.IsGlyphVisible = true;
                    args.DragUIOverride.Caption = IconExtract.DragOverContent;
                }
                else
                {
                    args.AcceptedOperation = DataPackageOperation.None;
                    args.DragUIOverride.IsCaptionVisible = true;
                    args.DragUIOverride.IsContentVisible = false;
                    args.DragUIOverride.IsGlyphVisible = true;
                    args.DragUIOverride.Caption = IconExtract.NoOtherExtensionNameFile;
                }
            }
            else
            {
                args.AcceptedOperation = DataPackageOperation.None;
                args.DragUIOverride.IsCaptionVisible = true;
                args.DragUIOverride.IsContentVisible = false;
                args.DragUIOverride.IsGlyphVisible = true;
                args.DragUIOverride.Caption = IconExtract.NoMultiFile;
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
                                ParseIconFile(filesList[0].Path);
                            });
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Warning, "Drop file in icon extract page failed", e);
                }
            });
            deferral.Complete();
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：提取图标页面——挂载的事件

        /// <summary>
        /// 网格控件选中项发生改变时的事件
        /// </summary>
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            IList<object> selectedItemsList = (sender as GridView).SelectedItems;
            if (selectedItemsList.Count > 0)
            {
                IsSelected = true;

                IntPtr[] phicon = new IntPtr[1];
                int[] piconid = new int[1];
                int iconIndex = Convert.ToInt32((selectedItemsList.Last() as IconModel).DisplayIndex);
                int nIcons = User32Library.PrivateExtractIcons(filePath, iconIndex, Convert.ToInt32(SelectedIconSize.Value), Convert.ToInt32(SelectedIconSize.Value), phicon, piconid, 1, 0);

                if (nIcons is 0)
                {
                    ImageSource = null;
                    IsImageEmpty = true;
                }
                else
                {
                    try
                    {
                        Icon icon = Icon.FromHandle(phicon[0]);
                        MemoryStream memoryStream = new();
                        icon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        BitmapImage bitmapImage = new();
                        bitmapImage.SetSource(memoryStream.AsRandomAccessStream());
                        ImageSource = bitmapImage;
                        IsImageEmpty = false;
                        icon.Dispose();
                        memoryStream.Dispose();
                        User32Library.DestroyIcon(phicon[0]);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, string.Format("Display {0} index {1} image failed", filePath, iconIndex), e);
                    }
                }
            }
            else
            {
                IsSelected = false;
            }
        }

        /// <summary>
        /// 选择图标格式
        /// </summary>
        private void OnIconFormatClicked(object sender, RoutedEventArgs args)
        {
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            if (item.Tag is not null)
            {
                SelectedIconFormat = IconFormatList[Convert.ToInt32(item.Tag)];
            }
        }

        /// <summary>
        /// 选择图标尺寸
        /// </summary>
        private void OnIconSizeClicked(object sender, RoutedEventArgs args)
        {
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            if (item.Tag is not null)
            {
                SelectedIconSize = IconSizeList[Convert.ToInt32(item.Tag)];

                if (IconsGridView.SelectedItem is not null)
                {
                    IntPtr[] phicon = new IntPtr[1];
                    int[] piconid = new int[1];
                    int iconIndex = Convert.ToInt32((IconsGridView.SelectedItem as IconModel).DisplayIndex);

                    int nIcons = User32Library.PrivateExtractIcons(filePath, iconIndex, Convert.ToInt32(SelectedIconSize.Value), Convert.ToInt32(SelectedIconSize.Value), phicon, piconid, 1, 0);

                    if (nIcons is 0)
                    {
                        ImageSource = null;
                        IsImageEmpty = true;
                    }
                    else
                    {
                        try
                        {
                            Icon icon = Icon.FromHandle(phicon[0]);
                            MemoryStream memoryStream = new();
                            icon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            BitmapImage bitmapImage = new();
                            bitmapImage.SetSource(memoryStream.AsRandomAccessStream());
                            ImageSource = bitmapImage;
                            IsImageEmpty = false;
                            icon.Dispose();
                            memoryStream.Dispose();
                            User32Library.DestroyIcon(phicon[0]);
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, string.Format("Display {0} index {1} image failed", filePath, iconIndex), e);
                        }
                    }
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
                Filter = IconExtract.FilterCondition,
                Title = IconExtract.SelectFile
            };
            if (dialog.ShowDialog() is DialogResult.OK && !string.IsNullOrEmpty(dialog.FileName))
            {
                ParseIconFile(dialog.FileName);
            }
        }

        /// <summary>
        /// 导出选中的图标
        /// </summary>
        private void OnExportSelectedIconsClicked(object sender, RoutedEventArgs args)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                IList<object> selectedItemsList = IconsGridView.SelectedItems;

                FolderBrowserDialog dialog = new()
                {
                    Description = IconExtract.SelectFolder,
                    ShowNewFolderButton = true,
                    RootFolder = Environment.SpecialFolder.Desktop
                };
                DialogResult result = dialog.ShowDialog();
                if (result is DialogResult.OK || result is DialogResult.Yes)
                {
                    IsSaving = true;
                    Task.Run(async () =>
                    {
                        int saveFailedCount = 0;

                        for (int index = 0; index < selectedItemsList.Count; index++)
                        {
                            object selectedItem = selectedItemsList[index];

                            if (selectedItem is not null)
                            {
                                IntPtr[] phicon = new IntPtr[1];
                                int[] piconid = new int[1];
                                int iconIndex = Convert.ToInt32((selectedItem as IconModel).DisplayIndex);

                                int nIcons = User32Library.PrivateExtractIcons(filePath, iconIndex, Convert.ToInt32(SelectedIconSize.Value), Convert.ToInt32(SelectedIconSize.Value), phicon, piconid, 1, 0);

                                if (nIcons is not 0)
                                {
                                    try
                                    {
                                        Icon icon = Icon.FromHandle(phicon[0]);

                                        if (icon is not null)
                                        {
                                            if (SelectedIconFormat.Value == IconFormatList[0].Value)
                                            {
                                                bool result = SaveIcon(icon, Path.Combine(dialog.SelectedPath, string.Format("{0} - {1} - {2}.ico", Path.GetFileName(filePath), iconIndex, Convert.ToInt32(SelectedIconSize.Value))));
                                                if (!result)
                                                {
                                                    saveFailedCount++;
                                                }
                                            }
                                            else
                                            {
                                                icon.ToBitmap().Save(Path.Combine(dialog.SelectedPath, string.Format("{0} - {1} - {2}.png", Path.GetFileName(filePath), iconIndex, Convert.ToInt32(SelectedIconSize.Value))), ImageFormat.Png);
                                            }
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        saveFailedCount++;
                                        LogService.WriteLog(EventLevel.Error, string.Format("Save icon {0} failed", Path.Combine(dialog.SelectedPath, string.Format("{0} - {1} - {2}", Path.GetFileName(filePath), iconIndex, Convert.ToInt32(SelectedIconSize.Value)))), e);
                                    }
                                }
                            }
                        }

                        await Task.Delay(300);

                        MainWindow.Current.BeginInvoke(() =>
                        {
                            IsSaving = false;
                            TeachingTipHelper.Show(new OperationResultTip(OperationKind.IconExtract, selectedItemsList.Count - saveFailedCount, saveFailedCount));
                        });
                    });
                }
            }
        }

        /// <summary>
        /// 导出所有图标
        /// </summary>
        private void OnExportAllIconsClicked(object sender, RoutedEventArgs args)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                FolderBrowserDialog dialog = new()
                {
                    Description = IconExtract.SelectFolder,
                    ShowNewFolderButton = true,
                    RootFolder = Environment.SpecialFolder.Desktop
                };
                DialogResult result = dialog.ShowDialog();
                if (result is DialogResult.OK || result is DialogResult.Yes)
                {
                    IsSaving = true;
                    Task.Run(async () =>
                    {
                        int saveFailedCount = 0;

                        lock (iconExtractLock)
                        {
                            for (int index = 0; index < IconCollection.Count; index++)
                            {
                                IconModel iconItem = IconCollection[index];

                                if (iconItem is not null)
                                {
                                    IntPtr[] phicon = new IntPtr[1];
                                    int[] piconid = new int[1];
                                    int iconIndex = Convert.ToInt32(iconItem.DisplayIndex);

                                    int nIcons = User32Library.PrivateExtractIcons(filePath, iconIndex, Convert.ToInt32(SelectedIconSize.Value), Convert.ToInt32(SelectedIconSize.Value), phicon, piconid, 1, 0);

                                    if (nIcons is not 0)
                                    {
                                        try
                                        {
                                            Icon icon = Icon.FromHandle(phicon[0]);

                                            if (icon is not null)
                                            {
                                                if (SelectedIconFormat.Value == IconFormatList[0].Value)
                                                {
                                                    bool result = SaveIcon(icon, Path.Combine(dialog.SelectedPath, string.Format("{0} - {1} - {2}.ico", Path.GetFileName(filePath), iconIndex, Convert.ToInt32(SelectedIconSize.Value))));
                                                    if (!result)
                                                    {
                                                        saveFailedCount++;
                                                    }
                                                }
                                                else
                                                {
                                                    icon.ToBitmap().Save(Path.Combine(dialog.SelectedPath, string.Format("{0} - {1} - {2}.png", Path.GetFileName(filePath), iconIndex, Convert.ToInt32(SelectedIconSize.Value))), ImageFormat.Png);
                                                }
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            saveFailedCount++;
                                            LogService.WriteLog(EventLevel.Error, string.Format("Save icon {0} failed", Path.Combine(dialog.SelectedPath, string.Format("{0} - {1} - {2}", Path.GetFileName(filePath), iconIndex, Convert.ToInt32(SelectedIconSize.Value)))), e);
                                        }
                                    }
                                }
                            }
                        }

                        await Task.Delay(300);

                        MainWindow.Current.BeginInvoke(() =>
                        {
                            lock (iconExtractLock)
                            {
                                IsSaving = false;
                                TeachingTipHelper.Show(new OperationResultTip(OperationKind.IconExtract, IconCollection.Count - saveFailedCount, saveFailedCount));
                            }
                        });
                    });
                }
            }
        }

        #endregion 第二部分：提取图标页面——挂载的事件

        /// <summary>
        /// 解析带有图标的二进制文件
        /// </summary>
        public void ParseIconFile(string iconFilePath)
        {
            lock (iconExtractLock)
            {
                IconCollection.Clear();
            }

            Task.Run(() =>
            {
                try
                {
                    filePath = iconFilePath;
                    // 图标个数
                    int iconsNum = User32Library.PrivateExtractIcons(filePath, 0, 0, 0, null, null, 0, 0);

                    // 显示图标
                    IntPtr[] phicon = new IntPtr[iconsNum];
                    int[] piconid = new int[iconsNum];
                    List<IconModel> iconsList = [];
                    int nIcons = User32Library.PrivateExtractIcons(filePath, 0, 48, 48, phicon, piconid, iconsNum, 0);
                    for (int index = 0; index < iconsNum; index++)
                    {
                        Icon icon = Icon.FromHandle(phicon[index]);
                        MemoryStream memoryStream = new();
                        icon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                        memoryStream.Seek(0, SeekOrigin.Begin);

                        iconsList.Add(new IconModel()
                        {
                            DisplayIndex = Convert.ToString(index),
                            IconMemoryStream = memoryStream,
                        });

                        icon.Dispose();
                        User32Library.DestroyIcon(phicon[index]);
                    }

                    MainWindow.Current.BeginInvoke(() =>
                    {
                        try
                        {
                            GetResults = string.Format(IconExtract.GetResults, Path.GetFileName(filePath), iconsNum);
                            NoResources = string.Format(IconExtract.NoResources, Path.GetFileName(filePath));
                            ImageSource = null;
                            IsImageEmpty = true;

                            foreach (IconModel iconItem in iconsList)
                            {
                                BitmapImage bitmapImage = new();
                                bitmapImage.SetSource(iconItem.IconMemoryStream.AsRandomAccessStream());

                                lock (iconExtractLock)
                                {
                                    IconCollection.Add(new IconModel()
                                    {
                                        DisplayIndex = iconItem.DisplayIndex,
                                        IconImage = bitmapImage
                                    });
                                }

                                iconItem.IconMemoryStream.Dispose();
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, string.Format("Display {0} icons failed", iconsList), e);
                        }
                    });
                }
                catch (Exception e)
                {
                    MainWindow.Current.BeginInvoke(() =>
                    {
                        GetResults = string.Format(IconExtract.GetResults, Path.GetFileName(filePath), 0);
                        NoResources = string.Format(IconExtract.NoResources, Path.GetFileName(filePath));
                        ImageSource = null;
                        IsImageEmpty = true;
                    });

                    LogService.WriteLog(EventLevel.Error, string.Format("Parse {0} file icons failed", filePath), e);
                }
            });
        }

        /// <summary>
        /// 保存获取到的 ico 图片到 Icon 文件
        /// </summary>
        private bool SaveIcon(Icon rawIcon, string destination)
        {
            try
            {
                MemoryStream bitMapStream = new(); //存原图的内存流
                MemoryStream iconStream = new(); //存图标的内存流
                rawIcon.ToBitmap().Save(bitMapStream, ImageFormat.Png); //将原图读取为png格式并存入原图内存流
                BinaryWriter iconWriter = new(iconStream); //新建二进制写入器以写入目标图标内存流

                // 下面是根据原图信息，进行文件头写入
                iconWriter.Write((short)0);
                iconWriter.Write((short)1);
                iconWriter.Write((short)1);
                iconWriter.Write((byte)0); // 图片宽度，由于 bytes 最大数值为 255，图片大小为 256 时，数值异常
                iconWriter.Write((byte)0); // 图片高度，由于 bytes 最大数值为 255，图片大小为 256 时，数值异常
                iconWriter.Write((short)0);
                iconWriter.Write((short)0);
                iconWriter.Write((short)32);
                iconWriter.Write((int)bitMapStream.Length);
                iconWriter.Write(22);
                //写入图像体至目标图标内存流
                iconWriter.Write(bitMapStream.ToArray());
                //保存流，并将流指针定位至头部以Icon对象进行读取输出为文件
                iconWriter.Flush();
                iconWriter.Seek(0, SeekOrigin.Begin);
                Stream iconFileStream = new FileStream(destination, FileMode.Create);
                Icon icon = new(iconStream);
                icon.Save(iconFileStream); //储存图像

                // 释放资源
                iconFileStream.Close();
                iconWriter.Close();
                iconStream.Close();
                bitMapStream.Close();
                icon.Dispose();
                return File.Exists(destination);
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, string.Format("Save icon {0} failed", destination), e);
                return false;
            }
        }
    }
}
