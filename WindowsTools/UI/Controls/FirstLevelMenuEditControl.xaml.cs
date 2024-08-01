using System;
using System.Collections;
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
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using WindowsTools.Models;
using WindowsTools.Services.Root;
using WindowsTools.Strings;
using WindowsTools.WindowsAPI.PInvoke.Shell32;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace WindowsTools.UI.Controls
{
    /// <summary>
    /// 一级菜单编辑控件
    /// </summary>
    public sealed partial class FirstLevelMenuEditControl : Grid, INotifyPropertyChanged
    {
        private Guid firstLevelMenuGuid = Guid.Empty;

        private string _menuTitleText;

        public string MenuTitleText
        {
            get { return _menuTitleText; }

            set
            {
                if (!Equals(_menuTitleText, value))
                {
                    _menuTitleText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MenuTitleText)));
                }
            }
        }

        private bool _shouldUseProgramIcon;

        public bool ShouldUseProgramIcon
        {
            get { return _shouldUseProgramIcon; }

            set
            {
                if (!Equals(_shouldUseProgramIcon, value))
                {
                    _shouldUseProgramIcon = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShouldUseProgramIcon)));
                }
            }
        }

        private bool _shouldEnableThemeIcon;

        public bool ShouldEnableThemeIcon
        {
            get { return _shouldEnableThemeIcon; }

            set
            {
                if (!Equals(_shouldEnableThemeIcon, value))
                {
                    _shouldEnableThemeIcon = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShouldEnableThemeIcon)));
                }
            }
        }

        private BitmapImage _defaultIconImage = new();

        public BitmapImage DefaultIconImage
        {
            get { return _defaultIconImage; }

            set
            {
                if (!Equals(_defaultIconImage, value))
                {
                    _defaultIconImage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DefaultIconImage)));
                }
            }
        }

        private string _defaultIconPath;

        public string DefaultIconPath
        {
            get { return _defaultIconPath; }

            set
            {
                if (!Equals(_defaultIconPath, value))
                {
                    _defaultIconPath = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DefaultIconPath)));
                }
            }
        }

        private BitmapImage _lightThemeIconImage = new();

        public BitmapImage LightThemeIconImage
        {
            get { return _lightThemeIconImage; }

            set
            {
                if (!Equals(_lightThemeIconImage, value))
                {
                    _lightThemeIconImage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LightThemeIconImage)));
                }
            }
        }

        private string _lightThemeIconPath;

        public string LightThemeIconPath
        {
            get { return _lightThemeIconPath; }

            set
            {
                if (!Equals(_lightThemeIconPath, value))
                {
                    _lightThemeIconPath = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LightThemeIconPath)));
                }
            }
        }

        private BitmapImage _darkThemeIconImage = new();

        public BitmapImage DarkThemeIconImage
        {
            get { return _darkThemeIconImage; }

            set
            {
                if (!Equals(_darkThemeIconImage, value))
                {
                    _darkThemeIconImage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DarkThemeIconImage)));
                }
            }
        }

        private string _darkThemeIconPath;

        public string DarkThemeIconPath
        {
            get { return _darkThemeIconPath; }

            set
            {
                if (!Equals(_darkThemeIconPath, value))
                {
                    _darkThemeIconPath = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DarkThemeIconPath)));
                }
            }
        }

        private string _menuProgramPathText;

        public string MenuProgramPathText
        {
            get { return _menuProgramPathText; }

            set
            {
                if (!Equals(_menuProgramPathText, value))
                {
                    _menuProgramPathText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MenuProgramPathText)));
                }
            }
        }

        private string _menuParameterText;

        public string MenuParameterText
        {
            get { return _menuParameterText; }

            set
            {
                if (!Equals(_menuParameterText, value))
                {
                    _menuParameterText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MenuParameterText)));
                }
            }
        }

        private bool _folderBackgroundMatch;

        public bool FolderBackgroundMatch
        {
            get { return _folderBackgroundMatch; }

            set
            {
                if (!Equals(_folderBackgroundMatch, value))
                {
                    _folderBackgroundMatch = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FolderBackgroundMatch)));
                }
            }
        }

        private bool _folderDesktopMatch;

        public bool FolderDesktopMatch
        {
            get { return _folderDesktopMatch; }

            set
            {
                if (!Equals(_folderDesktopMatch, value))
                {
                    _folderDesktopMatch = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FolderDesktopMatch)));
                }
            }
        }

        private bool _folderDirectoryMatch;

        public bool FolderDirectoryMatch
        {
            get { return _folderDirectoryMatch; }

            set
            {
                if (!Equals(_folderDirectoryMatch, value))
                {
                    _folderDirectoryMatch = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FolderDirectoryMatch)));
                }
            }
        }

        private bool _folderDriveMatch;

        public bool FolderDriveMatch
        {
            get { return _folderDriveMatch; }

            set
            {
                if (!Equals(_folderDriveMatch, value))
                {
                    _folderDriveMatch = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FolderDriveMatch)));
                }
            }
        }

        private DictionaryEntry _selectedFileMatchRule;

        public DictionaryEntry SelectedFileMatchRule
        {
            get { return _selectedFileMatchRule; }

            set
            {
                if (!Equals(_selectedFileMatchRule, value))
                {
                    _selectedFileMatchRule = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedFileMatchRule)));
                }
            }
        }

        private bool _needInputMatchFormat;

        public bool NeedInputMatchFormat
        {
            get { return _needInputMatchFormat; }

            set
            {
                if (!Equals(_needInputMatchFormat, value))
                {
                    _needInputMatchFormat = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NeedInputMatchFormat)));
                }
            }
        }

        private string _menuFileMatchFormatPHText;

        public string MenuFileMatchFormatPHText
        {
            get { return _menuFileMatchFormatPHText; }

            set
            {
                if (!Equals(_menuFileMatchFormatPHText, value))
                {
                    _menuFileMatchFormatPHText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MenuFileMatchFormatPHText)));
                }
            }
        }

        private string _menuFileMatchFormatText;

        public string MenuFileMatchFormatText
        {
            get { return _menuFileMatchFormatText; }

            set
            {
                if (!Equals(_menuFileMatchFormatText, value))
                {
                    _menuFileMatchFormatText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MenuFileMatchFormatText)));
                }
            }
        }

        private bool _shouldUseSecondLevelMenu;

        public bool ShouldUseSecondLevelMenu
        {
            get { return _shouldUseSecondLevelMenu; }

            set
            {
                if (!Equals(_shouldUseSecondLevelMenu, value))
                {
                    _shouldUseSecondLevelMenu = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShouldUseSecondLevelMenu)));
                }
            }
        }

        private List<DictionaryEntry> FileMatchRuleList { get; } =
        [
            new DictionaryEntry(ShellMenu.None,"None"),
            new DictionaryEntry(ShellMenu.Name,"Name"),
            new DictionaryEntry(ShellMenu.NameRegex,"NameRegex"),
            new DictionaryEntry(ShellMenu.Extension,"Extension"),
            new DictionaryEntry(ShellMenu.All,"All")
        ];

        private ObservableCollection<ShellMenuItemModel> MenuOrderCollection { get; } = [];

        private ObservableCollection<ShellMenuItemModel> SecondLevelMenuCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public FirstLevelMenuEditControl()
        {
            InitializeComponent();
            ShouldUseProgramIcon = false;
            SelectedFileMatchRule = FileMatchRuleList[0];

            MenuOrderCollection.Add(new ShellMenuItemModel()
            {
                Title = "Windows 工具箱",
                MenuIndex = 0,
                ProgramPath = System.Windows.Forms.Application.ExecutablePath,
            });
        }

        #region 第二部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 菜单顺序上移
        /// </summary>
        private void OnMoveUpExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            int menuIndex = Convert.ToInt32(args.Parameter);
        }

        /// <summary>
        /// 菜单顺序下移
        /// </summary>
        private void OnMoveDownExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            int menuIndex = Convert.ToInt32(args.Parameter);
        }

        /// <summary>
        /// 编辑菜单项
        /// </summary>
        private void OnEditExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            int menuIndex = Convert.ToInt32(args.Parameter);

            //if (BreadCollection.Count is 2)
            //{
            //    BreadCollection.Add(new DictionaryEntry(ShellMenu.SecondLevelMenu, "SecondLevelMenu"));
            //}
        }

        /// <summary>
        /// 定位菜单项
        /// </summary>
        private void OnOpenProgramPathRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            string filePath = args.Parameter as string;

            if (!string.IsNullOrEmpty(filePath))
            {
                Task.Run(() =>
                {
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        if (File.Exists(filePath))
                        {
                            IntPtr pidlList = Shell32Library.ILCreateFromPath(filePath);
                            if (pidlList != IntPtr.Zero)
                            {
                                Shell32Library.SHOpenFolderAndSelectItems(pidlList, 0, IntPtr.Zero, 0);
                                Shell32Library.ILFree(pidlList);
                            }
                        }
                        else
                        {
                            string directoryPath = Path.GetDirectoryName(filePath);

                            if (Directory.Exists(directoryPath))
                            {
                                Process.Start(directoryPath);
                            }
                            else
                            {
                                Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                            }
                        }
                    }
                });
            }
        }

        #endregion 第二部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 菜单标题内容发生更改时的事件
        /// </summary>
        private void OnTitleTextChanged(object sender, TextChangedEventArgs args)
        {
            MenuTitleText = (sender as Windows.UI.Xaml.Controls.TextBox).Text;
        }

        /// <summary>
        /// 是否启用主题图标按钮修改时触发的事件
        /// </summary>
        private void OnShouldEnableThemeIconToggled(object sender, RoutedEventArgs args)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;

            if (toggleSwitch is not null)
            {
                ShouldEnableThemeIcon = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 是否使用应用程序图标修改时触发的事件
        /// </summary>
        private void OnShouldUseProgramIconToggled(object sender, RoutedEventArgs args)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;

            if (toggleSwitch is not null)
            {
                ShouldUseProgramIcon = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 默认图标修改
        /// </summary>
        private void OnDefaultIconModifyClicked(object sender, RoutedEventArgs args)
        {
            OpenFileDialog dialog = new()
            {
                Multiselect = false,
                Filter = ShellMenu.IconFilterCondition,
                Title = ShellMenu.SelectIcon
            };
            if (dialog.ShowDialog() is DialogResult.OK && !string.IsNullOrEmpty(dialog.FileName))
            {
                try
                {
                    DefaultIconPath = dialog.FileName;
                    Icon icon = Icon.ExtractAssociatedIcon(dialog.FileName);
                    MemoryStream memoryStream = new();
                    icon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    DefaultIconImage.SetSource(memoryStream.AsRandomAccessStream());
                    memoryStream.Dispose();
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Set Default icon failed", e);
                }
            }
        }

        /// <summary>
        /// 浅色主题图标修改
        /// </summary>
        private void OnLightThemeIconModifyClicked(object sender, RoutedEventArgs args)
        {
            OpenFileDialog dialog = new()
            {
                Multiselect = false,
                Filter = ShellMenu.IconFilterCondition,
                Title = ShellMenu.SelectIcon
            };
            if (dialog.ShowDialog() is DialogResult.OK && !string.IsNullOrEmpty(dialog.FileName))
            {
                try
                {
                    LightThemeIconPath = dialog.FileName;
                    Icon icon = Icon.ExtractAssociatedIcon(dialog.FileName);
                    MemoryStream memoryStream = new();
                    icon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    LightThemeIconImage.SetSource(memoryStream.AsRandomAccessStream());
                    memoryStream.Dispose();
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Set light theme icon failed", e);
                }
            }
        }

        /// <summary>
        /// 深色主题图标修改
        /// </summary>
        private void OnDarkThemeIconModifyClicked(object sender, RoutedEventArgs args)
        {
            OpenFileDialog dialog = new()
            {
                Multiselect = false,
                Filter = ShellMenu.IconFilterCondition,
                Title = ShellMenu.SelectIcon
            };
            if (dialog.ShowDialog() is DialogResult.OK && !string.IsNullOrEmpty(dialog.FileName))
            {
                try
                {
                    DarkThemeIconPath = dialog.FileName;
                    Icon icon = Icon.ExtractAssociatedIcon(dialog.FileName);
                    MemoryStream memoryStream = new();
                    icon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    DarkThemeIconImage.SetSource(memoryStream.AsRandomAccessStream());
                    memoryStream.Dispose();
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Set dark theme icon failed", e);
                }
            }
        }

        /// <summary>
        /// 修改菜单程序文件路径
        /// </summary>
        private void OnMenuProgramPathModifyClicked(object sender, RoutedEventArgs args)
        {
            OpenFileDialog dialog = new()
            {
                Multiselect = false,
                Filter = ShellMenu.ProgramFilterCondition,
                Title = ShellMenu.SelectProgram
            };
            if (dialog.ShowDialog() is DialogResult.OK && !string.IsNullOrEmpty(dialog.FileName))
            {
                MenuProgramPathText = dialog.FileName;
            }
        }

        /// <summary>
        /// 菜单参数内容发生更改时的事件
        /// </summary>
        private void OnMenuParameterTextChanged(object sender, TextChangedEventArgs args)
        {
            MenuParameterText = (sender as Windows.UI.Xaml.Controls.TextBox).Text;
        }

        /// <summary>
        /// 修改菜单文件匹配规则
        /// </summary>
        private void OnFileMatchRuleClicked(object sender, RoutedEventArgs args)
        {
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            if (item.Tag is not null)
            {
                SelectedFileMatchRule = FileMatchRuleList[Convert.ToInt32(item.Tag)];
                NeedInputMatchFormat = SelectedFileMatchRule.Equals(FileMatchRuleList[1]) || SelectedFileMatchRule.Equals(FileMatchRuleList[2]) || SelectedFileMatchRule.Equals(FileMatchRuleList[3]);

                if (SelectedFileMatchRule.Equals(FileMatchRuleList[0]) || SelectedFileMatchRule.Equals(4))
                {
                    NeedInputMatchFormat = false;
                    MenuFileMatchFormatPHText = string.Empty;
                }
                else if (SelectedFileMatchRule.Equals(FileMatchRuleList[1]))
                {
                    NeedInputMatchFormat = true;
                    MenuFileMatchFormatPHText = ShellMenu.MenuFileNameFormat;
                }
                else if (SelectedFileMatchRule.Equals(FileMatchRuleList[2]))
                {
                    NeedInputMatchFormat = true;
                    MenuFileMatchFormatPHText = ShellMenu.MenuFileNameRegexFormat;
                }
                else if (SelectedFileMatchRule.Equals(FileMatchRuleList[3]))
                {
                    NeedInputMatchFormat = true;
                    MenuFileMatchFormatPHText = ShellMenu.MenuFileExtensionFormat;
                }
            }
        }

        /// <summary>
        /// 菜单文件匹配格式内容发生更改时的事件
        /// </summary>
        private void OnMenuFileMatchFormatTextChanged(object sender, TextChangedEventArgs args)
        {
            MenuFileMatchFormatText = (sender as Windows.UI.Xaml.Controls.TextBox).Text;
        }

        /// <summary>
        /// 是否启用二级菜单
        /// </summary>
        private void OnShouldUseSecondLevelMenuToggled(object sender, RoutedEventArgs args)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;

            if (toggleSwitch is not null)
            {
                ShouldUseSecondLevelMenu = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 添加菜单
        /// </summary>
        private void OnAddMenuClicked(object sender, RoutedEventArgs args)
        {
        }

        /// <summary>
        /// 清空菜单项
        /// </summary>
        private void OnClearMenuClicked(object sender, RoutedEventArgs args)
        {
        }

        /// <summary>
        /// 刷新列表
        /// </summary>
        private void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
        }
    }
}
