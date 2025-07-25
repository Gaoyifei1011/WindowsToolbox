using Microsoft.UI.Xaml.Controls;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using PowerToolbox.WindowsAPI.ComTypes;
using PowerToolbox.WindowsAPI.PInvoke.User32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 文件恢复页面
    /// </summary>
    public sealed partial class WinFRPage : Page, INotifyPropertyChanged
    {
        private bool isInitialized;
        private readonly string DriveImagePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.SystemX86), "Imageres.dll");
        private readonly string ExtensiveModeString = ResourceService.WinFRResource.GetString("ExtensiveMode");
        private readonly string KeepBothString = ResourceService.WinFRResource.GetString("KeepBoth");
        private readonly string NeverOverrideString = ResourceService.WinFRResource.GetString("NeverOverride");
        private readonly string NTFSModeString = ResourceService.WinFRResource.GetString("NTFSMode");
        private readonly string OverrideString = ResourceService.WinFRResource.GetString("Override");
        private readonly string RegularModeString = ResourceService.WinFRResource.GetString("RegularMode");
        private readonly string SegmentModeString = ResourceService.WinFRResource.GetString("SegmentMode");
        private readonly string SelectFolderString = ResourceService.WinFRResource.GetString("SelectFolder");
        private readonly string SignatureModeString = ResourceService.WinFRResource.GetString("SignatureMode");

        private ImageSource SystemDriveSource;
        private ImageSource StandardDriveSource;

        private string _saveFolder;

        public string SaveFolder
        {
            get { return _saveFolder; }

            set
            {
                if (!string.Equals(_saveFolder, value))
                {
                    _saveFolder = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SaveFolder)));
                }
            }
        }

        private KeyValuePair<string, string> _selectedRecoveryMode;

        public KeyValuePair<string, string> SelectedRecoveryMode
        {
            get { return _selectedRecoveryMode; }

            set
            {
                if (!Equals(_saveFolder, value))
                {
                    _selectedRecoveryMode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedRecoveryMode)));
                }
            }
        }

        private KeyValuePair<string, string> _selectedDuplicatedFileOption;

        public KeyValuePair<string, string> SelectedDuplicatedFileOption
        {
            get { return _selectedDuplicatedFileOption; }

            set
            {
                if (!Equals(_selectedDuplicatedFileOption, value))
                {
                    _selectedDuplicatedFileOption = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedDuplicatedFileOption)));
                }
            }
        }

        private bool _useCustomLogFolder;

        public bool UseCustomLogFolder
        {
            get { return _useCustomLogFolder; }

            set
            {
                if (!Equals(_useCustomLogFolder, value))
                {
                    _useCustomLogFolder = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UseCustomLogFolder)));
                }
            }
        }

        private string _logSaveFolder;

        public string LogSaveFolder
        {
            get { return _logSaveFolder; }

            set
            {
                if (!Equals(_logSaveFolder, value))
                {
                    _logSaveFolder = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LogSaveFolder)));
                }
            }
        }

        private List<KeyValuePair<string, string>> RecoveryModeList { get; } = [];

        private List<KeyValuePair<string, string>> DuplicatedFileOptionList { get; } = [];

        public ObservableCollection<DriveModel> DriveCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public WinFRPage()
        {
            InitializeComponent();
            RecoveryModeList.Add(new KeyValuePair<string, string>("RegularMode", RegularModeString));
            RecoveryModeList.Add(new KeyValuePair<string, string>("ExtensiveMode", ExtensiveModeString));
            RecoveryModeList.Add(new KeyValuePair<string, string>("NTFSModeMode", NTFSModeString));
            RecoveryModeList.Add(new KeyValuePair<string, string>("SegmentMode", SegmentModeString));
            RecoveryModeList.Add(new KeyValuePair<string, string>("Signature", SignatureModeString));
            SelectedRecoveryMode = RecoveryModeList[0];

            DuplicatedFileOptionList.Add(new KeyValuePair<string, string>("Override", OverrideString));
            DuplicatedFileOptionList.Add(new KeyValuePair<string, string>("NeverOverride", NeverOverrideString));
            DuplicatedFileOptionList.Add(new KeyValuePair<string, string>("KeepBoth", KeepBothString));
            SelectedDuplicatedFileOption = DuplicatedFileOptionList[0];
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

        #region 第二部分：文件恢复页面——挂载的事件

        /// <summary>
        /// 开始恢复
        /// </summary>
        /// TODO：未完成
        private void OnRecoveryClicked(Microsoft.UI.Xaml.Controls.SplitButton sender, Microsoft.UI.Xaml.Controls.SplitButtonClickEventArgs args)
        {
        }

        /// <summary>
        /// 了解文件恢复
        /// </summary>
        private void OnLearnWinFRClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("https://aka.ms/winfrhelp");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(WinFRPage), nameof(OnLearnWinFRClicked), 1, e);
                }
            });
        }

        /// <summary>
        /// 选择保存目录
        /// </summary>
        private void OnSelectFolderClicked(object sender, RoutedEventArgs args)
        {
            OpenFolderDialog openFolderDialog = new()
            {
                Description = SelectFolderString,
                RootFolder = Environment.SpecialFolder.Desktop
            };
            DialogResult dialogResult = openFolderDialog.ShowDialog();
            if (dialogResult is DialogResult.OK || dialogResult is DialogResult.Yes)
            {
                SaveFolder = openFolderDialog.SelectedPath;
            }
            openFolderDialog.Dispose();
        }

        /// <summary>
        /// 打开目录
        /// </summary>
        private void OnOpenSaveFolderClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start(SaveFolder);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(WinFRPage), nameof(OnLearnWinFRClicked), 1, e);
                }
            });
        }

        /// <summary>
        /// 修改恢复模式
        /// </summary>
        private void OnRecoveryModeClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is KeyValuePair<string, string> recoveryMode)
            {
                SelectedRecoveryMode = recoveryMode;
            }
        }

        private void OnDuplicatedFileOptionClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is KeyValuePair<string, string> duplicatedFileOption)
            {
                SelectedDuplicatedFileOption = duplicatedFileOption;
            }
        }

        /// <summary>
        /// 使用自定义日志文件目录
        /// </summary>
        private void OnUseCustomLogFolderToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                UseCustomLogFolder = toggleSwitch.IsOn;
            }
        }

        #endregion 第二部分：文件恢复页面——挂载的事件

        /// <summary>
        /// 获取驱动器信息
        /// </summary>
        /// TODO：未完成
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
