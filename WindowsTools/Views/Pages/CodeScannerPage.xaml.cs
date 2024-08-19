using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using WindowsTools.Extensions.DataType.Enums;
using WindowsTools.Helpers.Controls;
using WindowsTools.Helpers.Root;
using WindowsTools.Services.Controls.Settings;
using WindowsTools.Services.Root;
using WindowsTools.Strings;
using WindowsTools.UI.TeachingTips;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace WindowsTools.Views.Pages
{
    /// <summary>
    /// 条码扫描页面
    /// </summary>
    public sealed partial class CodeScannerPage : Page, INotifyPropertyChanged
    {
        private readonly BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Static;
        private readonly SynchronizationContext synchronizationContext = SynchronizationContext.Current;
        private PrintPreviewDialog printPreviewDialog;
        private PrintDialog printDialog;
        private PrintDocument printDocument;

        private int _selectedIndex = 0;

        public int SelectedIndex
        {
            get { return _selectedIndex; }

            set
            {
                if (!Equals(_selectedIndex, value))
                {
                    _selectedIndex = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedIndex)));
                }
            }
        }

        private string _generateText;

        public string GenerateText
        {
            get { return _generateText; }

            set
            {
                if (!Equals(_generateText, value))
                {
                    _generateText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GenerateText)));
                }
            }
        }

        private BitmapImage _generatedImage;

        public BitmapImage GeneratedImage
        {
            get { return _generatedImage; }

            set
            {
                if (!Equals(_generatedImage, value))
                {
                    _generatedImage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GeneratedImage)));
                }
            }
        }

        private DictionaryEntry _selectedGenerateType;

        public DictionaryEntry SelectedGenerateType
        {
            get { return _selectedGenerateType; }

            set
            {
                if (!Equals(_selectedGenerateType, value))
                {
                    _selectedGenerateType = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedGenerateType)));
                }
            }
        }

        private DictionaryEntry _selectedSaveCodeType;

        public DictionaryEntry SelectedSaveCodeType
        {
            get { return _selectedSaveCodeType; }

            set
            {
                if (!Equals(_selectedSaveCodeType, value))
                {
                    _selectedSaveCodeType = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedSaveCodeType)));
                }
            }
        }

        private bool _isSquare = true;

        public bool IsSquare
        {
            get { return _isSquare; }

            set
            {
                if (!Equals(_isSquare, value))
                {
                    _isSquare = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSquare)));
                }
            }
        }

        private double _photoWidth = 512;

        public double PhotoWidth
        {
            get { return _photoWidth; }

            set
            {
                if (!Equals(_photoWidth, value))
                {
                    _photoWidth = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PhotoWidth)));
                }
            }
        }

        private double _photoHeight = 512;

        public double PhotoHeight
        {
            get { return _photoHeight; }

            set
            {
                if (!Equals(_photoHeight, value))
                {
                    _photoHeight = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PhotoHeight)));
                }
            }
        }

        private bool _isReserveBarCodeText;

        public bool IsReserveBarCodeText
        {
            get { return _isReserveBarCodeText; }

            set
            {
                _isReserveBarCodeText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsReserveBarCodeText)));
            }
        }

        private string _recognizeText;

        public string RecognizeText
        {
            get { return _recognizeText; }

            set
            {
                _recognizeText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RecognizeText)));
            }
        }

        private List<DictionaryEntry> GenerateTypeList { get; } =
        [
            new DictionaryEntry(CodeScanner.BarCode, "BarCode"),
            new DictionaryEntry(CodeScanner.QRCode, "QRCode")
        ];

        public event PropertyChangedEventHandler PropertyChanged;

        public CodeScannerPage()
        {
            InitializeComponent();

            if (SelectedIndex is 0)
            {
                GenerateGrid.Visibility = Visibility.Visible;
                ParseGrid.Visibility = Visibility.Collapsed;
            }

            SelectedGenerateType = GenerateTypeList[1];
        }

        #region 第一部分：重写父类事件

        /// <summary>
        /// 设置拖动的数据的可视表示形式
        /// </summary>
        protected override void OnDragEnter(global::Windows.UI.Xaml.DragEventArgs args)
        {
            base.OnDragEnter(args);

            if (SelectedIndex is 1)
            {
                IReadOnlyList<IStorageItem> dragItemsList = args.DataView.GetStorageItemsAsync().AsTask().Result;

                if (dragItemsList.Count is 1)
                {
                    args.AcceptedOperation = DataPackageOperation.Copy;
                    args.DragUIOverride.IsCaptionVisible = true;
                    args.DragUIOverride.IsContentVisible = false;
                    args.DragUIOverride.IsGlyphVisible = true;
                    args.DragUIOverride.Caption = CodeScanner.DragOverContent;
                }
                else
                {
                    args.AcceptedOperation = DataPackageOperation.None;
                    args.DragUIOverride.IsCaptionVisible = true;
                    args.DragUIOverride.IsContentVisible = false;
                    args.DragUIOverride.IsGlyphVisible = true;
                    args.DragUIOverride.Caption = CodeScanner.NoMultiFile;
                }
            }
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
                            System.Drawing.Image image = System.Drawing.Image.FromFile(filesList[0].Path);

                            if (image is not null)
                            {
                                ParseCodeImage(image);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Warning, "Drop file in code scanner page failed", e);
                }
            });
            deferral.Complete();
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：条码扫描页面——挂载的事件

        /// <summary>
        /// 工具类型选择
        /// </summary>
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            RadioButtons radioButtons = sender as RadioButtons;

            if (radioButtons is not null && radioButtons.SelectedIndex is not -1)
            {
                SelectedIndex = radioButtons.SelectedIndex;
                if (SelectedIndex is 0)
                {
                    GenerateGrid.Visibility = Visibility.Visible;
                    ParseGrid.Visibility = Visibility.Collapsed;
                }
                else
                {
                    GenerateGrid.Visibility = Visibility.Collapsed;
                    ParseGrid.Visibility = Visibility.Visible;
                }
            }
        }

        /// <summary>
        /// 生成文本框文本发生变化的事件
        /// </summary>
        private void OnGenerateTextChanged(object sender, TextChangedEventArgs args)
        {
            GenerateText = (sender as global::Windows.UI.Xaml.Controls.TextBox).Text;
        }

        /// <summary>
        /// 保存图片
        /// </summary>
        private async void OnSavePhotoClicked(object sender, RoutedEventArgs args)
        {
            if (string.IsNullOrEmpty(GenerateText))
            {
                await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.GenerateTextEmpty));
                return;
            }

            SaveFileDialog saveFileDialog = new()
            {
                Filter = CodeScanner.FilterCondition
            };

            if (saveFileDialog.ShowDialog() is DialogResult.OK)
            {
                await Task.Run(() =>
                {
                    // 条形码
                    if (SelectedGenerateType.Equals(GenerateTypeList[0]))
                    {
                        Bitmap barCodeBitmap = GenerateBarCode(GenerateText, Convert.ToInt32(PhotoWidth), Convert.ToInt32(PhotoHeight));
                        string extensionName = Path.GetExtension(saveFileDialog.FileName);
                        if (extensionName.Equals(".png", StringComparison.OrdinalIgnoreCase))
                        {
                            barCodeBitmap.Save(saveFileDialog.FileName, ImageFormat.Png);
                        }
                        else if (extensionName.Equals(".jpg", StringComparison.OrdinalIgnoreCase))
                        {
                            barCodeBitmap.Save(saveFileDialog.FileName, ImageFormat.Jpeg);
                        }
                        else if (extensionName.Equals(".bmp", StringComparison.OrdinalIgnoreCase))
                        {
                            barCodeBitmap.Save(saveFileDialog.FileName, ImageFormat.Bmp);
                        }
                    }
                    // 二维码
                    else if (SelectedGenerateType.Equals(GenerateTypeList[1]))
                    {
                        Bitmap qrCodeBitmap = GenerateQRCode(GenerateText, Convert.ToInt32(PhotoWidth), Convert.ToInt32(PhotoHeight));
                        string extensionName = Path.GetExtension(saveFileDialog.FileName);
                        if (extensionName.Equals(".png", StringComparison.OrdinalIgnoreCase))
                        {
                            qrCodeBitmap.Save(saveFileDialog.FileName, ImageFormat.Png);
                        }
                        else if (extensionName.Equals(".jpg", StringComparison.OrdinalIgnoreCase))
                        {
                            qrCodeBitmap.Save(saveFileDialog.FileName, ImageFormat.Jpeg);
                        }
                        else if (extensionName.Equals(".bmp", StringComparison.OrdinalIgnoreCase))
                        {
                            qrCodeBitmap.Save(saveFileDialog.FileName, ImageFormat.Bmp);
                        }
                    }
                    saveFileDialog.Dispose();
                });
            }
        }

        /// <summary>
        /// 打印图片
        /// </summary>
        private async void OnPrintPhotoClicked(object sender, RoutedEventArgs args)
        {
            if (string.IsNullOrEmpty(GenerateText))
            {
                await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.GenerateTextEmpty));
                return;
            }

            AutoResetEvent autoResetEvent = new(false);
            await Task.Run(() =>
            {
                // 条形码
                if (SelectedGenerateType.Equals(GenerateTypeList[0]))
                {
                    Bitmap barCodeBitmap = GenerateBarCode(GenerateText, 300, 150);
                    if (barCodeBitmap is not null)
                    {
                        MemoryStream memoryStream = new();
                        barCodeBitmap.Save(memoryStream, ImageFormat.Png);
                        memoryStream.Position = 0;
                        autoResetEvent.Set();

                        synchronizationContext.Post(async (_) =>
                        {
                            try
                            {
                                BitmapImage bitmapImage = new();
                                bitmapImage.SetSource(memoryStream.AsRandomAccessStream());
                                GeneratedImage = bitmapImage;
                                barCodeBitmap.Dispose();
                                memoryStream.Dispose();
                            }
                            catch (Exception e)
                            {
                                await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.GenerateBarCodeFailed));
                                LogService.WriteLog(EventLevel.Error, "Display generated bar code photo failed", e);
                            }
                        }, null);
                    }
                }
                // 二维码
                else if (SelectedGenerateType.Equals(GenerateTypeList[1]))
                {
                    Bitmap qrCodeBitmap = GenerateQRCode(GenerateText, 200, 200);

                    if (qrCodeBitmap is not null)
                    {
                        MemoryStream memoryStream = new();
                        qrCodeBitmap.Save(memoryStream, ImageFormat.Png);
                        memoryStream.Position = 0;
                        autoResetEvent.Set();

                        synchronizationContext.Post(async (_) =>
                        {
                            try
                            {
                                BitmapImage bitmapImage = new();
                                bitmapImage.SetSource(memoryStream.AsRandomAccessStream());
                                GeneratedImage = bitmapImage;
                                qrCodeBitmap.Dispose();
                                memoryStream.Dispose();
                            }
                            catch (Exception e)
                            {
                                await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.GenerateQRCodeFailed));
                                LogService.WriteLog(EventLevel.Error, "Display generated qr code photo failed", e);
                            }
                        }, null);
                    }
                }
            });

            autoResetEvent.WaitOne();
            autoResetEvent.Dispose();
            autoResetEvent = null;

            // 未初始化打印预览对话框，初始化打印预览对话框
            if (printPreviewDialog is null)
            {
                try
                {
                    printPreviewDialog = new PrintPreviewDialog
                    {
                        RightToLeft = LanguageService.RightToLeft,
                        RightToLeftLayout = LanguageService.RightToLeft is RightToLeft.Yes
                    };
                    Type printPreviewDialogType = typeof(PrintPreviewDialog);

                    FieldInfo printToolStripButtonFieldInfo = printPreviewDialogType.GetField("printToolStripButton", bindingFlags);
                    if (printToolStripButtonFieldInfo is not null)
                    {
                        // 获取打印按钮，并清除打印按钮默认的点击事件
                        ToolStripButton printToolStripButton = (ToolStripButton)printToolStripButtonFieldInfo.GetValue(printPreviewDialog);
                        EventInfo[] eventsInfoArray = printToolStripButton.GetType().GetEvents(bindingFlags);

                        foreach (EventInfo eventInfo in eventsInfoArray)
                        {
                            try
                            {
                                FieldInfo eventClickFieldInfo = eventInfo.DeclaringType.GetField("EventClick", bindingFlags);

                                eventClickFieldInfo?.SetValue(printToolStripButton, null);
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(EventLevel.Error, "Clear print preview dialog default print button click event failed", e);
                                continue;
                            }
                        }

                        printToolStripButton.Click += OnPrintToolStripButtonClick;
                        printDocument = new PrintDocument();
                        printDocument.PrintPage += OnPrintPage;
                        printPreviewDialog.Document = printDocument;
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Initialize print preview dialog failed", e);
                }
            }

            try
            {
                printPreviewDialog?.ShowDialog();
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, "Open print preview dialog failed", e);
            }
        }

        /// <summary>
        /// 生成图片
        /// </summary>
        private async void OnGeneratePhotoClicked(object sender, RoutedEventArgs args)
        {
            if (string.IsNullOrEmpty(GenerateText))
            {
                await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.GenerateTextEmpty));
            }

            await Task.Run(() =>
            {
                // 条形码
                if (SelectedGenerateType.Equals(GenerateTypeList[0]))
                {
                    Bitmap barCodeBitmap = GenerateBarCode(GenerateText, 300, 150);
                    if (barCodeBitmap is not null)
                    {
                        MemoryStream memoryStream = new();
                        barCodeBitmap.Save(memoryStream, ImageFormat.Png);
                        memoryStream.Position = 0;

                        synchronizationContext.Post(async (_) =>
                        {
                            try
                            {
                                BitmapImage bitmapImage = new();
                                bitmapImage.SetSource(memoryStream.AsRandomAccessStream());
                                GeneratedImage = bitmapImage;
                                barCodeBitmap.Dispose();
                                memoryStream.Dispose();
                            }
                            catch (Exception e)
                            {
                                await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.GenerateBarCodeFailed));
                                LogService.WriteLog(EventLevel.Error, "Display generated bar code photo failed", e);
                            }
                        }, null);
                    }
                }
                // 二维码
                else if (SelectedGenerateType.Equals(GenerateTypeList[1]))
                {
                    Bitmap qrCodeBitmap = GenerateQRCode(GenerateText, 200, 200);

                    if (qrCodeBitmap is not null)
                    {
                        MemoryStream memoryStream = new();
                        qrCodeBitmap.Save(memoryStream, ImageFormat.Png);
                        memoryStream.Position = 0;

                        synchronizationContext.Post(async (_) =>
                        {
                            try
                            {
                                BitmapImage bitmapImage = new();
                                bitmapImage.SetSource(memoryStream.AsRandomAccessStream());
                                GeneratedImage = bitmapImage;
                                qrCodeBitmap.Dispose();
                                memoryStream.Dispose();
                            }
                            catch (Exception e)
                            {
                                await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.GenerateQRCodeFailed));
                                LogService.WriteLog(EventLevel.Error, "Display generated qr code photo failed", e);
                            }
                        }, null);
                    }
                }
            });
        }

        /// <summary>
        /// 选择生成码的类型
        /// </summary>
        private void OnGenerateTypeClicked(object sender, RoutedEventArgs args)
        {
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            if (item.Tag is not null)
            {
                SelectedGenerateType = GenerateTypeList[Convert.ToInt32(item.Tag)];
                if (SelectedGenerateType.Equals(GenerateTypeList[0]))
                {
                    IsSquare = false;
                    PhotoHeight = Convert.ToInt32(PhotoWidth / 2);
                }
                else if (SelectedGenerateType.Equals(GenerateTypeList[1]))
                {
                    PhotoHeight = PhotoWidth;
                }
            }
        }

        /// <summary>
        /// 生成的图片是否为正方形
        /// </summary>
        private void OnIsSquareToggled(object sender, RoutedEventArgs args)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch is not null)
            {
                IsSquare = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 图片大小输入的值发生变化时触发的事件
        /// </summary>
        private void OnValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            string tag = Convert.ToString(sender.Tag);

            if (tag is "PhotoWidthNumberBox")
            {
                PhotoWidth = args.NewValue;
                if (IsSquare)
                {
                    PhotoHeight = args.NewValue;
                }
            }
            else if (tag is "PhotoHeightNumberBox")
            {
                PhotoHeight = args.NewValue;
                if (IsSquare)
                {
                    PhotoWidth = args.NewValue;
                }
            }

            GenerateText = string.Empty;
        }

        /// <summary>
        /// 是否保留条形码底部数字
        /// </summary>
        private void OnReserveBarCodeTextToggled(object sender, RoutedEventArgs args)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch is not null)
            {
                IsReserveBarCodeText = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 打开图片
        /// </summary>
        private void OnOpenPhotoClicked(object sender, RoutedEventArgs args)
        {
            OpenFileDialog dialog = new()
            {
                Multiselect = false,
                Title = CodeScanner.SelectFile
            };
            if (dialog.ShowDialog() is DialogResult.OK && !string.IsNullOrEmpty(dialog.FileName))
            {
                Task.Run(() =>
                {
                    try
                    {
                        System.Drawing.Image image = System.Drawing.Image.FromFile(dialog.FileName);
                        if (image is not null)
                        {
                            ParseCodeImage(image);
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, string.Format("Open file {0} failed", dialog.FileName), e);
                    }

                    dialog.Dispose();
                });
            }
            else
            {
                dialog.Dispose();
            }
        }

        /// <summary>
        /// 清除已经识别到的文本
        /// </summary>
        private void OnClearRecognizeTextClicked(object sender, RoutedEventArgs args)
        {
            RecognizeText = string.Empty;
        }

        /// <summary>
        /// 读取剪贴板图片
        /// </summary>
        private async void OnReadClipboardPhotoClicked(object sender, RoutedEventArgs args)
        {
            System.Drawing.Image clipboardImage = CopyPasteHelper.ReadClipboardImage();

            if (clipboardImage is not null)
            {
                ParseCodeImage(clipboardImage);
            }
            else
            {
                await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.ReadClipboardImageFailed));
            }
        }

        #endregion 第二部分：条码扫描页面——挂载的事件

        #region 第三部分：自定义事件

        /// <summary>
        /// 当需要为当前页打印的输出时发生的事件
        /// </summary>
        private void OnPrintPage(object sender, PrintPageEventArgs args)
        {
            // 条形码
            if (SelectedGenerateType.Equals(GenerateTypeList[0]))
            {
                Bitmap barCodeBitmap = GenerateBarCode(GenerateText, Convert.ToInt32(PhotoWidth), Convert.ToInt32(PhotoHeight));
                float x = (args.PageBounds.Width - barCodeBitmap.Width) / 2;
                float y = (args.PageBounds.Height - barCodeBitmap.Height) / 2;
                args.Graphics.DrawImage(barCodeBitmap, x, y, barCodeBitmap.Width, barCodeBitmap.Height);
            }
            // 二维码
            else if (SelectedGenerateType.Equals(GenerateTypeList[1]))
            {
                Bitmap qrCodeBitmap = GenerateQRCode(GenerateText, Convert.ToInt32(PhotoWidth), Convert.ToInt32(PhotoHeight));
                float x = (args.PageBounds.Width - qrCodeBitmap.Width) / 2;
                float y = (args.PageBounds.Height - qrCodeBitmap.Height) / 2;
                args.Graphics.DrawImage(qrCodeBitmap, x, y, qrCodeBitmap.Width, qrCodeBitmap.Height);
            }
        }

        /// <summary>
        /// 自定义打印预览控件打印按钮的事件，显示选择打印机对话框
        /// </summary>
        private void OnPrintToolStripButtonClick(object sender, EventArgs args)
        {
            printPreviewDialog?.Close();
            printDialog ??= new PrintDialog();

            if (printDialog.ShowDialog() is DialogResult.OK)
            {
                printDocument.PrinterSettings.PrinterName = printDialog.PrinterSettings.PrinterName;
                printDocument.Print();
            }
        }

        #endregion 第三部分：自定义事件

        /// <summary>
        /// 获取生成的条形码
        /// </summary>
        private Bitmap GenerateBarCode(string content, int width, int height)
        {
            BarcodeWriter barcodeWriter = new()
            {
                Format = BarcodeFormat.CODE_128
            };

            EncodingOptions encodingOptions = new()
            {
                Width = width,
                Height = height,
                Margin = 2,
                PureBarcode = IsReserveBarCodeText
            };

            barcodeWriter.Options = encodingOptions;
            return barcodeWriter.Write(content);
        }

        /// <summary>
        /// 获取生成的二维码
        /// </summary>
        private Bitmap GenerateQRCode(string content, int width, int height)
        {
            BarcodeWriter barcodeWriter = new()
            {
                Format = BarcodeFormat.QR_CODE
            };

            QrCodeEncodingOptions qrCodeEncodingOptions = new()
            {
                DisableECI = true,
                CharacterSet = "UTF-8",
                Width = width,
                Height = height,
                Margin = 1
            };

            barcodeWriter.Options = qrCodeEncodingOptions;
            return barcodeWriter.Write(content);
        }

        /// <summary>
        /// 解析图片文件
        /// </summary>
        public void ParseCodeImage(System.Drawing.Image image)
        {
            Task.Run(() =>
            {
                try
                {
                    BarcodeReader reader = new();
                    Bitmap codeBitmap = new(image);
                    Result result = reader.Decode(codeBitmap);

                    if (result is not null)
                    {
                        synchronizationContext.Post(_ =>
                        {
                            RecognizeText = result.Text;
                        }, null);
                    }
                    else
                    {
                        synchronizationContext.Post(async (_) =>
                        {
                            await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.ParsePhotoFailed));
                        }, null);
                    }
                }
                catch (Exception e)
                {
                    synchronizationContext.Post(async (_) =>
                    {
                        await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.ParsePhotoFailed));
                    }, null);
                    LogService.WriteLog(EventLevel.Error, string.Format("Parse code file failed"), e);
                }
            });
        }
    }
}
