using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using WindowsTools.Extensions.DataType.Enums;
using WindowsTools.Services.Root;
using WindowsTools.Strings;
using WindowsTools.Views.Windows;

namespace WindowsTools.Views.Pages
{
    /// <summary>
    /// 摸鱼页面
    /// </summary>
    public sealed partial class LoafPage : Page, INotifyPropertyChanged
    {
        private bool _loadImageCompleted = false;

        public bool LoadImageCompleted
        {
            get { return _loadImageCompleted; }

            set
            {
                if (!Equals(_loadImageCompleted, value))
                {
                    _loadImageCompleted = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LoadImageCompleted)));
                }
            }
        }

        private BitmapImage _loafImage;

        public BitmapImage LoafImage
        {
            get { return _loafImage; }

            set
            {
                if (!Equals(_loafImage, value))
                {
                    _loafImage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LoafImage)));
                }
            }
        }

        private bool _isLoafing;

        public bool IsLoafing
        {
            get { return _isLoafing; }

            set
            {
                if (!Equals(_isLoafing, value))
                {
                    _isLoafing = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoafing)));
                }
            }
        }

        private bool _blockAllKeys = true;

        public bool BlockAllKeys
        {
            get { return _blockAllKeys; }

            set
            {
                if (!Equals(_blockAllKeys, value))
                {
                    _blockAllKeys = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BlockAllKeys)));
                }
            }
        }

        private DictionaryEntry _selectedUpdateStyle;

        public DictionaryEntry SelectedUpdateStyle
        {
            get { return _selectedUpdateStyle; }

            set
            {
                if (!Equals(_selectedUpdateStyle, value))
                {
                    _selectedUpdateStyle = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedUpdateStyle)));
                }
            }
        }

        private TimeSpan _durationTime = new TimeSpan(0, 30, 0);

        public TimeSpan DurationTime
        {
            get { return _durationTime; }

            set
            {
                if (!Equals(_durationTime, value))
                {
                    _durationTime = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DurationTime)));
                }
            }
        }

        private bool _lockScreenAutomaticly = true;

        public bool LockScreenAutomaticly
        {
            get { return _lockScreenAutomaticly; }

            set
            {
                if (!Equals(_lockScreenAutomaticly, value))
                {
                    _lockScreenAutomaticly = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LockScreenAutomaticly)));
                }
            }
        }

        private List<DictionaryEntry> UpdateStyleList { get; } = new List<DictionaryEntry>()
        {
            new DictionaryEntry(Loaf.Windows11Style, UpdatingKind.Windows11),
            new DictionaryEntry(Loaf.Windows10Style, UpdatingKind.Windows10),
        };

        public event PropertyChangedEventHandler PropertyChanged;

        public LoafPage()
        {
            InitializeComponent();
            SelectedUpdateStyle = UpdateStyleList[0];
        }

        #region 第一部分：摸鱼页面——挂载的事件

        /// <summary>
        /// 加载每日必应壁纸图片
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            Task.Run(async () =>
            {
                try
                {
                    HttpClient httpClient = new HttpClient();
                    httpClient.Timeout = TimeSpan.FromSeconds(5);

                    HttpResponseMessage responseMessage = await httpClient.GetAsync("https://bing.biturl.top/?resolution=1920&format=image");

                    if (responseMessage.IsSuccessStatusCode)
                    {
                        Stream stream = await responseMessage.Content.ReadAsStreamAsync();
                        IRandomAccessStream randomAccessStream = stream.AsRandomAccessStream();

                        MainWindow.Current.BeginInvoke(async () =>
                        {
                            try
                            {
                                BitmapImage bitmapImage = new BitmapImage();
                                await bitmapImage.SetSourceAsync(randomAccessStream);
                                LoafImage = bitmapImage;
                                LoadImageCompleted = true;
                                responseMessage.Dispose();
                                httpClient.Dispose();
                            }
                            catch (Exception e)
                            {
                                LoafImage = new BitmapImage(new Uri("ms-appx:///Assets/Images/LoafWallpaper.jpg"));
                                LoadImageCompleted = true;
                                LogService.WriteLog(EventLevel.Error, "Load bing wallpaper image failed", e);
                            }
                        });
                    }
                    else
                    {
                        MainWindow.Current.BeginInvoke(() =>
                        {
                            LoafImage = new BitmapImage(new Uri("ms-appx:///Assets/Images/LoafWallpaper.jpg"));
                            LoadImageCompleted = true;
                        });
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Load bing wallpaper image failed", e);
                    MainWindow.Current.BeginInvoke(() =>
                    {
                        LoafImage = new BitmapImage(new Uri("ms-appx:///Assets/Images/LoafWallpaper.jpg"));
                        LoadImageCompleted = true;
                    });
                }
            });
        }

        /// <summary>
        /// 开始摸鱼
        /// </summary>
        private void OnStartLoafClicked(object sender, RoutedEventArgs args)
        {
            new LoafWindow((UpdatingKind)SelectedUpdateStyle.Value, DurationTime, BlockAllKeys, LockScreenAutomaticly).Show();
            LoafWindow.Current.FormClosed += OnClosed;
            IsLoafing = true;
        }

        /// <summary>
        /// 停止模拟更新后触发的事件
        /// </summary>
        private void OnClosed(object sender, FormClosedEventArgs args)
        {
            LoafWindow.Current.FormClosed -= OnClosed;
            IsLoafing = false;
        }

        /// <summary>
        /// 选择模拟更新的界面风格
        /// </summary>
        private void OnUpdateStyleClicked(object sender, RoutedEventArgs args)
        {
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            if (item.Tag is not null)
            {
                SelectedUpdateStyle = UpdateStyleList[Convert.ToInt32(item.Tag)];
            }
        }

        /// <summary>
        /// 是否在模拟更新的时候屏蔽所有键盘按键
        /// </summary>
        private void OnBlockAllKeysToggled(object sender, RoutedEventArgs args)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch is not null)
            {
                BlockAllKeys = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 时间更改时触发的事件
        /// </summary>
        private void OnTimeChanged(object sender, TimePickerValueChangedEventArgs args)
        {
            TimePicker timePicker = sender as TimePicker;
            if (timePicker is not null)
            {
                DurationTime = args.NewTime;
            }
        }

        /// <summary>
        /// 模拟更新结束后是否自动锁屏
        /// </summary>
        private void OnLockScreenAutomaticlyToggled(object sender, RoutedEventArgs args)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch is not null)
            {
                LockScreenAutomaticly = toggleSwitch.IsOn;
            }
        }

        #endregion 第一部分：摸鱼页面——挂载的事件
    }
}
