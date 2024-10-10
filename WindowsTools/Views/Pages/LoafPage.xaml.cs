using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.IO;
using System.Net.Http;
using System.Threading;
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

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace WindowsTools.Views.Pages
{
    /// <summary>
    /// 摸鱼页面
    /// </summary>
    public sealed partial class LoafPage : Page, INotifyPropertyChanged
    {
        private bool isLoadWallpaperFailed;
        private readonly SynchronizationContext synchronizationContext = SynchronizationContext.Current;

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

        private KeyValuePair<UpdatingKind, string> _selectedUpdateStyle;

        public KeyValuePair<UpdatingKind, string> SelectedUpdateStyle
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

        private TimeSpan _durationTime = new(0, 30, 0);

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

        private List<KeyValuePair<UpdatingKind, string>> UpdateStyleList { get; } =
        [
            new KeyValuePair<UpdatingKind,string>(UpdatingKind.Windows11,Loaf.Windows11Style ),
            new KeyValuePair<UpdatingKind,string>( UpdatingKind.Windows10,Loaf.Windows10Style),
        ];

        public event PropertyChangedEventHandler PropertyChanged;

        public LoafPage()
        {
            InitializeComponent();
            SelectedUpdateStyle = UpdateStyleList[0];

            Task.Run(async () =>
            {
                try
                {
                    HttpClient httpClient = new()
                    {
                        Timeout = TimeSpan.FromSeconds(5)
                    };

                    HttpResponseMessage responseMessage = await httpClient.GetAsync("https://bing.biturl.top/?resolution=1920&format=image");

                    if (responseMessage.IsSuccessStatusCode)
                    {
                        Stream stream = await responseMessage.Content.ReadAsStreamAsync();
                        IRandomAccessStream randomAccessStream = stream.AsRandomAccessStream();

                        synchronizationContext.Post(async (_) =>
                        {
                            try
                            {
                                BitmapImage bitmapImage = new();
                                await bitmapImage.SetSourceAsync(randomAccessStream);
                                LoafImage = bitmapImage;
                                LoadImageCompleted = true;
                                responseMessage.Dispose();
                                httpClient.Dispose();
                            }
                            catch (Exception e)
                            {
                                isLoadWallpaperFailed = true;
                                LoafImage = ActualTheme is ElementTheme.Light ? new(new Uri("ms-appx:///Assets/Images/LoafLightWallpaper.jpg")) : new(new Uri("ms-appx:///Assets/Images/LoafDarkWallpaper.jpg"));
                                LoadImageCompleted = true;
                                LogService.WriteLog(EventLevel.Error, "Load bing wallpaper image failed", e);
                            }
                        }, null);
                    }
                    else
                    {
                        synchronizationContext.Post(_ =>
                        {
                            isLoadWallpaperFailed = true;
                            LoafImage = ActualTheme is ElementTheme.Light ? new(new Uri("ms-appx:///Assets/Images/LoafLightWallpaper.jpg")) : new(new Uri("ms-appx:///Assets/Images/LoafDarkWallpaper.jpg"));
                            LoadImageCompleted = true;
                        }, null);
                    }
                }
                catch (Exception e)
                {
                    isLoadWallpaperFailed = true;
                    LogService.WriteLog(EventLevel.Error, "Load bing wallpaper image failed", e);
                    synchronizationContext.Post(_ =>
                    {
                        LoafImage = ActualTheme is ElementTheme.Light ? new(new Uri("ms-appx:///Assets/Images/LoafLightWallpaper.jpg")) : new(new Uri("ms-appx:///Assets/Images/LoafDarkWallpaper.jpg"));
                        LoadImageCompleted = true;
                    }, null);
                }
            });
        }

        #region 第一部分：摸鱼页面——挂载的事件

        /// <summary>
        /// 在已构造 FrameworkElement 并将其添加到对象树中并准备好交互时发生的事件
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            ActualThemeChanged += OnActualThemeChanged;
        }

        /// <summary>
        /// 当此对象不再连接到主对象树时发生的事件
        /// </summary>
        private void OnUnLoaded(object sender, RoutedEventArgs args)
        {
            ActualThemeChanged -= OnActualThemeChanged;
        }

        /// <summary>
        /// 当前应用主题发生变化时对应的事件
        /// </summary>
        private void OnActualThemeChanged(FrameworkElement sender, object args)
        {
            if (isLoadWallpaperFailed)
            {
                LoafImage = ActualTheme is ElementTheme.Light ? new(new Uri("ms-appx:///Assets/Images/LoafLightWallpaper.jpg")) : new(new Uri("ms-appx:///Assets/Images/LoafDarkWallpaper.jpg"));
            }
        }

        /// <summary>
        /// 开始摸鱼
        /// </summary>
        private void OnStartLoafClicked(object sender, RoutedEventArgs args)
        {
            new LoafWindow(SelectedUpdateStyle.Key, DurationTime, BlockAllKeys, LockScreenAutomaticly).Show();
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
            if (sender is ToggleMenuFlyoutItem toggleMenuFlyoutItem && toggleMenuFlyoutItem.Tag is not null)
            {
                SelectedUpdateStyle = UpdateStyleList[Convert.ToInt32(toggleMenuFlyoutItem.Tag)];
            }
        }

        /// <summary>
        /// 是否在模拟更新的时候屏蔽所有键盘按键
        /// </summary>
        private void OnBlockAllKeysToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                BlockAllKeys = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 时间更改时触发的事件
        /// </summary>
        private void OnTimeChanged(object sender, TimePickerValueChangedEventArgs args)
        {
            if (sender is TimePicker)
            {
                DurationTime = args.NewTime;
            }
        }

        /// <summary>
        /// 模拟更新结束后是否自动锁屏
        /// </summary>
        private void OnLockScreenAutomaticlyToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                LockScreenAutomaticly = toggleSwitch.IsOn;
            }
        }

        #endregion 第一部分：摸鱼页面——挂载的事件
    }
}
