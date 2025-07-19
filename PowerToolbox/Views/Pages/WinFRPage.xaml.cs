using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using PowerToolbox.WindowsAPI.PInvoke.User32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 文件恢复页面
    /// </summary>
    public sealed partial class WinFRPage : Page
    {
        private bool isInitialized;
        private readonly string DriveImagePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.SystemX86), "Imageres.dll");
        private ImageSource SystemDriveSource;
        private ImageSource StandardDriveSource;

        public ObservableCollection<DriveModel> DriveCollection { get; } = [];

        public WinFRPage()
        {
            InitializeComponent();
        }

        #region 第一部分：重写父类事件

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (!isInitialized)
            {
                isInitialized = true;

                try
                {
                    int iconsNum = User32Library.PrivateExtractIcons(DriveImagePath, 0, 0, 0, null, null, 0, 0);
                    IntPtr[] phicon = new IntPtr[iconsNum];
                    int[] piconid = new int[iconsNum];
                    int nIcons = User32Library.PrivateExtractIcons(DriveImagePath, 31, 256, 256, phicon, piconid, 1, 0);

                    Icon icon = Icon.FromHandle(phicon[0]);
                    MemoryStream memoryStream = new();
                    icon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    BitmapImage bitmapImage = new();
                    bitmapImage.SetSource(memoryStream.AsRandomAccessStream());
                    SystemDriveSource = bitmapImage;
                    icon.Dispose();
                    memoryStream.Dispose();
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(WinFRPage), nameof(OnNavigatedTo), 1, e);
                }

                try
                {
                    int iconsNum = User32Library.PrivateExtractIcons(DriveImagePath, 0, 0, 0, null, null, 0, 0);
                    IntPtr[] phicon = new IntPtr[iconsNum];
                    int[] piconid = new int[iconsNum];
                    int nIcons = User32Library.PrivateExtractIcons(DriveImagePath, 30, 256, 256, phicon, piconid, 1, 0);

                    Icon icon = Icon.FromHandle(phicon[0]);
                    MemoryStream memoryStream = new();
                    icon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    BitmapImage bitmapImage = new();
                    bitmapImage.SetSource(memoryStream.AsRandomAccessStream());
                    StandardDriveSource = bitmapImage;
                    icon.Dispose();
                    memoryStream.Dispose();
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(WinFRPage), nameof(OnNavigatedTo), 1, e);
                }

                await GetDriverInfoAsync();
            }
        }

        #endregion 第一部分：重写父类事件

        /// <summary>
        /// 获取磁盘信息
        /// </summary>
        private async Task GetDriverInfoAsync()
        {
            List<DriverModel> driverList = await Task.Run(() =>
            {
                List<DriverModel> driverList = [];
                DriveInfo[] driverInfoArray = DriveInfo.GetDrives();

                foreach (DriveInfo driveInfo in driverInfoArray)
                {
                    DriveModel driveItem = new()
                    {
                        IsSelected = false,
                        Name = driveInfo.Name,
                        Space = driveInfo.TotalFreeSpace.ToString(),
                        IsSytemDrive = false,
                        DriveUsedPercentage = driveInfo.AvailableFreeSpace / driveInfo.TotalSize
                    };
                }

                return driverList;
            });

            DriveCollection.Clear();
            DriveCollection.Add(new DriveModel()
            {
                IsSelected = false,
                Name = "本地磁盘 (C:)",
                DriveUsedPercentage = 30.4,
                IsSytemDrive = true,
                DiskImage = SystemDriveSource,
                IsAvailableSpaceError = false,
                IsAvailableSpaceWarning = false,
                Space = "194 GB 可用，共 279 GB"
            });

            DriveCollection.Add(new DriveModel()
            {
                IsSelected = false,
                Name = "新加卷 (D:)",
                DriveUsedPercentage = 82.7,
                IsSytemDrive = false,
                DiskImage = StandardDriveSource,
                IsAvailableSpaceError = false,
                IsAvailableSpaceWarning = false,
                Space = "32.6 GB 可用，共 185 GB"
            });

            DriveCollection.Add(new DriveModel()
            {
                IsSelected = false,
                Name = "本地磁盘 (E:)",
                DriveUsedPercentage = 24.5,
                IsSytemDrive = false,
                DiskImage = StandardDriveSource,
                IsAvailableSpaceError = false,
                IsAvailableSpaceWarning = false,
                Space = "88.9 GB 可用，共 118 GB"
            });
        }
    }
}
