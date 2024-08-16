using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using WindowsTools.Extensions.DataType.Enums;
using WindowsTools.Extensions.ShellMenu;
using WindowsTools.Models;
using WindowsTools.Services.Root;
using WindowsTools.Services.Shell;
using WindowsTools.Strings;
using WindowsTools.Views.Windows;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace WindowsTools.Views.Pages
{
    /// <summary>
    /// 自定义扩展菜单页面
    /// </summary>
    public sealed partial class ShellMenuPage : Page, INotifyPropertyChanged
    {
        private readonly SynchronizationContext synchronizationContext = SynchronizationContext.Current;
        private Guid editMenuGuid;
        private string editMenuKey;
        private int editMenuIndex;
        private string selectedDefaultIconPath = string.Empty;
        private string selectedLightThemeIconPath = string.Empty;
        private string selectedDarkThemeIconPath = string.Empty;

        private ShellMenuItemModel selectedItem;

        private bool _isAddMenuEnabled;

        public bool IsAddMenuEnabled
        {
            get { return _isAddMenuEnabled; }

            set
            {
                if (!Equals(_isAddMenuEnabled, value))
                {
                    _isAddMenuEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAddMenuEnabled)));
                }
            }
        }

        private bool _isEditMenuEnabled;

        public bool IsEditMenuEnabled
        {
            get { return _isEditMenuEnabled; }

            set
            {
                if (!Equals(_isEditMenuEnabled, value))
                {
                    _isEditMenuEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEditMenuEnabled)));
                }
            }
        }

        private bool _isMoveUpEnabled;

        public bool IsMoveUpEnabled
        {
            get { return _isMoveUpEnabled; }

            set
            {
                if (!Equals(_isMoveUpEnabled, value))
                {
                    _isMoveUpEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsMoveUpEnabled)));
                }
            }
        }

        private bool _isMoveDownEnabled;

        public bool IsMoveDownEnabled
        {
            get { return _isMoveDownEnabled; }

            set
            {
                if (!Equals(_isMoveDownEnabled, value))
                {
                    _isMoveDownEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsMoveDownEnabled)));
                }
            }
        }

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

        private bool _shouldUseIcon;

        public bool ShouldUseIcon
        {
            get { return _shouldUseIcon; }

            set
            {
                if (!Equals(_shouldUseIcon, value))
                {
                    _shouldUseIcon = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShouldUseIcon)));
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

        private bool _shouldUseThemeIcon;

        public bool ShouldUseThemeIcon
        {
            get { return _shouldUseThemeIcon; }

            set
            {
                if (!Equals(_shouldUseThemeIcon, value))
                {
                    _shouldUseThemeIcon = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShouldUseThemeIcon)));
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

        private List<DictionaryEntry> FileMatchRuleList { get; } =
        [
            new DictionaryEntry(ShellMenu.None, "None"),
            new DictionaryEntry(ShellMenu.Name, "Name"),
            new DictionaryEntry(ShellMenu.NameRegex, "NameRegex"),
            new DictionaryEntry(ShellMenu.Extension, "Extension"),
            new DictionaryEntry(ShellMenu.All, "All")
        ];

        public ObservableCollection<DictionaryEntry> BreadCollection { get; } =
        [
            new DictionaryEntry(ShellMenu.Title, "Title")
        ];

        private ObservableCollection<ShellMenuItemModel> ShellMenuItemCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public ShellMenuPage()
        {
            InitializeComponent();
            SelectedFileMatchRule = FileMatchRuleList[4];

            Task.Run(() =>
            {
                // 获取所有菜单项信息
                ShellMenuItem rootShellMenuItem = ShellMenuService.GetShellMenuItem();

                synchronizationContext.Send(_ =>
                {
                    if (rootShellMenuItem is not null)
                    {
                        ShellMenuItemCollection.Add(EnumShellMenuItem(rootShellMenuItem, MenuType.RootMenu));

                        IsAddMenuEnabled = true;
                        IsMoveUpEnabled = false;
                        IsMoveDownEnabled = false;

                        if (ShellMenuItemCollection.Count is 0)
                        {
                            selectedItem = null;
                            IsEditMenuEnabled = false;
                        }
                        else
                        {
                            ShellMenuItemCollection[0].IsSelected = true;
                            selectedItem = ShellMenuItemCollection[0];
                            IsEditMenuEnabled = true;
                        }
                    }
                    else
                    {
                        IsAddMenuEnabled = true;
                        selectedItem = null;
                        IsEditMenuEnabled = false;
                        IsMoveUpEnabled = false;
                        IsMoveDownEnabled = false;
                    }
                }, null);
            });
        }

        #region 第一部分：重写父类事件

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);

            string parameter = args.Parameter as string;

            if (!string.IsNullOrEmpty(parameter))
            {
                while (BreadCollection.Count > 1)
                {
                    BreadCollection.RemoveAt(1);
                }
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：根菜单页面——挂载的事件

        /// <summary>
        /// 当前应用主题发生变化时对应的事件
        /// </summary>
        private void OnActualThemeChanged(FrameworkElement sender, object args)
        {
            foreach (ShellMenuItemModel shellMenuItem in ShellMenuItemCollection)
            {
                EnumModifyShellMenuItemTheme(shellMenuItem);
            }
        }

        /// <summary>
        /// 单击痕迹栏条目时发生的事件
        /// </summary>
        private void OnItemClicked(object sender, BreadcrumbBarItemClickedEventArgs args)
        {
            DictionaryEntry breadItem = (DictionaryEntry)args.Item;

            if (BreadCollection.Count is 2)
            {
                if (breadItem.Value.Equals(BreadCollection[0].Value))
                {
                    BreadCollection.RemoveAt(1);
                }
            }
        }

        /// <summary>
        /// 打开菜单设置
        /// </summary>
        private void OnMenuSettingsClicked(object sender, RoutedEventArgs args)
        {
            (MainWindow.Current.Content as MainPage).NavigateTo(typeof(SettingsPage));
        }

        /// <summary>
        /// 保存更改
        /// </summary>
        private void OnSaveClicked(object sender, RoutedEventArgs args)
        {
            if (BreadCollection.Count is 2)
            {
                Task.Run(() =>
                {
                    // 保存指定菜单项信息
                    ShellMenuItem shellMenuItem = new()
                    {
                        MenuGuid = editMenuGuid,
                        MenuIndex = editMenuIndex,
                        MenuTitleText = MenuTitleText,
                        ShouldUseIcon = ShouldUseIcon,
                        ShouldUseProgramIcon = ShouldUseProgramIcon,
                        ShouldUseThemeIcon = ShouldUseThemeIcon,
                        DefaultIconPath = DefaultIconPath,
                        LightThemeIconPath = LightThemeIconPath,
                        DarkThemeIconPath = DarkThemeIconPath,
                        MenuProgramPathText = MenuProgramPathText,
                        MenuParameter = MenuParameterText,
                        FolderBackground = FolderBackgroundMatch,
                        FolderDesktop = FolderDesktopMatch,
                        FolderDirectory = FolderDirectoryMatch,
                        FolderDrive = FolderDriveMatch,
                        MenuFileMatchRule = SelectedFileMatchRule.Value.ToString(),
                        MenuFileMatchFormatText = MenuFileMatchFormatText
                    };

                    ShellMenuService.SaveShellMenuItem(editMenuKey, shellMenuItem);
                    ShellMenuItem rootShellMenuItem = ShellMenuService.GetShellMenuItem();

                    synchronizationContext.Send(_ =>
                    {
                        ShellMenuItemCollection.Clear();

                        if (rootShellMenuItem is not null)
                        {
                            synchronizationContext.Send(_ =>
                            {
                                ShellMenuItemCollection.Add(EnumShellMenuItem(rootShellMenuItem, MenuType.RootMenu));

                                IsAddMenuEnabled = true;
                                IsMoveUpEnabled = false;
                                IsMoveDownEnabled = false;

                                if (ShellMenuItemCollection.Count is 0)
                                {
                                    selectedItem = null;
                                    IsEditMenuEnabled = false;
                                }
                                else
                                {
                                    ShellMenuItemCollection[0].IsSelected = true;
                                    selectedItem = ShellMenuItemCollection[0];
                                    IsEditMenuEnabled = true;
                                }
                            }, null);
                        }
                        else
                        {
                            IsAddMenuEnabled = true;
                            selectedItem = null;
                            IsEditMenuEnabled = false;
                            IsMoveUpEnabled = false;
                            IsMoveDownEnabled = false;
                        }

                        BreadCollection.RemoveAt(1);
                    }, null);
                });
            }
        }

        /// <summary>
        /// 添加菜单
        /// </summary>
        private void OnAddMenuItemClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                Guid menuGuid = Guid.NewGuid();
                string menuKey = string.Empty;
                int menuIndex = 0;

                if (selectedItem is null)
                {
                    menuKey = menuGuid.ToString();
                }
                else if (selectedItem.MenuType is MenuType.RootMenu || selectedItem.MenuType is MenuType.FirstLevelMenu)
                {
                    menuKey = Path.Combine(selectedItem.MenuKey, menuGuid.ToString());
                    menuIndex = selectedItem.SubMenuItemCollection.Count;
                }

                synchronizationContext.Send(_ =>
                {
                    // 添加菜单项信息
                    editMenuKey = menuKey;
                    editMenuGuid = menuGuid;
                    editMenuIndex = menuIndex;
                    selectedDefaultIconPath = string.Empty;
                    selectedLightThemeIconPath = string.Empty;
                    selectedDarkThemeIconPath = string.Empty;
                    MenuTitleText = string.Empty;
                    ShouldUseIcon = true;
                    ShouldUseProgramIcon = true;
                    ShouldUseThemeIcon = true;
                    DefaultIconPath = string.Empty;
                    LightThemeIconPath = string.Empty;
                    DarkThemeIconPath = string.Empty;
                    MenuProgramPathText = string.Empty;
                    MenuParameterText = string.Empty;
                    FolderBackgroundMatch = false;
                    FolderDesktopMatch = false;
                    FolderDirectoryMatch = false;
                    FolderDriveMatch = false;
                    SelectedFileMatchRule = FileMatchRuleList[4];
                    NeedInputMatchFormat = false;
                    MenuFileMatchFormatPHText = string.Empty;
                    MenuFileMatchFormatText = string.Empty;

                    BreadCollection.Add(new DictionaryEntry(ShellMenu.EditMenu, "EditMenu"));
                }, null);
            });
        }

        /// <summary>
        /// 清空菜单项
        /// </summary>
        private void OnClearMenuClicked(object sender, RoutedEventArgs args)
        {
            string rootMenuKey = ShellMenuItemCollection[0].MenuKey;

            Task.Run(() =>
            {
                // 清空所有菜单项信息
                ShellMenuService.RemoveShellMenuItem(rootMenuKey);

                synchronizationContext.Send(_ =>
                {
                    ShellMenuItemCollection.Clear();
                    selectedItem = null;
                    IsAddMenuEnabled = true;
                    IsEditMenuEnabled = false;
                    IsMoveUpEnabled = false;
                    IsMoveDownEnabled = false;
                }, null);
            });
        }

        /// <summary>
        /// 刷新列表
        /// </summary>
        private void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            ShellMenuItemCollection.Clear();

            Task.Run(() =>
            {
                // 获取所有菜单项信息
                ShellMenuItem rootShellMenuItem = ShellMenuService.GetShellMenuItem();

                synchronizationContext.Send(_ =>
                {
                    if (rootShellMenuItem is not null)
                    {
                        ShellMenuItemCollection.Add(EnumShellMenuItem(rootShellMenuItem, MenuType.RootMenu));

                        IsAddMenuEnabled = true;
                        IsMoveUpEnabled = false;
                        IsMoveDownEnabled = false;

                        if (ShellMenuItemCollection.Count is 0)
                        {
                            selectedItem = null;
                            IsEditMenuEnabled = false;
                        }
                        else
                        {
                            ShellMenuItemCollection[0].IsSelected = true;
                            selectedItem = ShellMenuItemCollection[0];
                            IsEditMenuEnabled = true;
                        }
                    }
                    else
                    {
                        IsAddMenuEnabled = true;
                        selectedItem = null;
                        IsEditMenuEnabled = false;
                        IsMoveUpEnabled = false;
                        IsMoveDownEnabled = false;
                    }
                }, null);
            });
        }

        /// <summary>
        /// 编辑菜单
        /// </summary>
        private void OnEditMenuClicked(object sender, RoutedEventArgs args)
        {
            // 获取选中项的菜单信息
            if (selectedItem is not null && BreadCollection.Count is 1)
            {
                editMenuGuid = selectedItem.MenuGuid;
                MenuTitleText = selectedItem.MenuTitleText;
                ShouldUseIcon = selectedItem.ShouldUseIcon;
                ShouldUseProgramIcon = selectedItem.ShouldUseProgramIcon;
                ShouldUseThemeIcon = selectedItem.ShouldUseThemeIcon;
                DefaultIconPath = selectedItem.DefaultIconPath;
                LightThemeIconPath = selectedItem.LightThemeIconPath;
                DarkThemeIconPath = selectedItem.DarkThemeIconPath;
                MenuProgramPathText = selectedItem.MenuProgramPathText;
                MenuParameterText = selectedItem.MenuParameter;
                FolderBackgroundMatch = selectedItem.FolderBackground;
                FolderDesktopMatch = selectedItem.FolderDesktop;
                FolderDirectoryMatch = selectedItem.FolderDirectory;
                FolderDriveMatch = selectedItem.FolderDrive;
                MenuFileMatchFormatText = selectedItem.MenuFileMatchFormatText;

                for (int index = 0; index < FileMatchRuleList.Count; index++)
                {
                    if (selectedItem.MenuFileMatchRule.Equals(FileMatchRuleList[index].Value))
                    {
                        SelectedFileMatchRule = FileMatchRuleList[index];
                    }
                }

                if (File.Exists(DefaultIconPath))
                {
                    try
                    {
                        Icon defaultIcon = Icon.ExtractAssociatedIcon(DefaultIconPath);
                        MemoryStream memoryStream = new();
                        defaultIcon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        DefaultIconImage.SetSource(memoryStream.AsRandomAccessStream());
                        memoryStream.Dispose();
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, "Load default icon image failed", e);
                    }
                }

                if (File.Exists(LightThemeIconPath))
                {
                    try
                    {
                        Icon defaultIcon = Icon.ExtractAssociatedIcon(LightThemeIconPath);
                        MemoryStream memoryStream = new();
                        defaultIcon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        DefaultIconImage.SetSource(memoryStream.AsRandomAccessStream());
                        memoryStream.Dispose();
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, "Load light theme icon image failed", e);
                    }
                }

                if (File.Exists(LightThemeIconPath))
                {
                    try
                    {
                        Icon defaultIcon = Icon.ExtractAssociatedIcon(DarkThemeIconPath);
                        MemoryStream memoryStream = new();
                        defaultIcon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        DefaultIconImage.SetSource(memoryStream.AsRandomAccessStream());
                        memoryStream.Dispose();
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, "Load dark theme icon image failed", e);
                    }
                }

                BreadCollection.Add(new DictionaryEntry(ShellMenu.EditMenu, "EditMenu"));
            }
        }

        /// <summary>
        /// 向上移动菜单项
        /// </summary>
        private void OnMoveUpClicked(object sender, RoutedEventArgs args)
        {
            EnumMoveUpShellMenuItem(selectedItem, ShellMenuItemCollection);
        }

        /// <summary>
        /// 向下移动菜单项
        /// </summary>
        private void OnMoveDownClicked(object sender, RoutedEventArgs args)
        {
            EnumMoveDownShellMenuItem(selectedItem, ShellMenuItemCollection);
        }

        /// <summary>
        /// 点击选中项触发的事件
        /// </summary>
        private void OnItemInvoked(Microsoft.UI.Xaml.Controls.TreeView sender, Microsoft.UI.Xaml.Controls.TreeViewItemInvokedEventArgs args)
        {
            selectedItem = args.InvokedItem as ShellMenuItemModel;

            if (selectedItem is not null)
            {
                EnumModifySelectedItem(selectedItem, ShellMenuItemCollection);
            }
        }

        /// <summary>
        /// 菜单标题内容发生更改时的事件
        /// </summary>
        private void OnTitleTextChanged(object sender, TextChangedEventArgs args)
        {
            MenuTitleText = (sender as global::Windows.UI.Xaml.Controls.TextBox).Text;
        }

        /// <summary>
        /// 是否使用图标修改时触发的事件
        /// </summary>
        private void OnShouldUseIconToggled(object sender, RoutedEventArgs args)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;

            if (toggleSwitch is not null)
            {
                ShouldUseIcon = toggleSwitch.IsOn;
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
        /// 是否启用主题图标按钮修改时触发的事件
        /// </summary>
        private void OnShouldUseThemeIconToggled(object sender, RoutedEventArgs args)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;

            if (toggleSwitch is not null)
            {
                ShouldUseThemeIcon = toggleSwitch.IsOn;
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
                    selectedDefaultIconPath = dialog.FileName;
                    DefaultIconPath = Path.Combine(ShellMenuService.ShellMenuConfigDirectory.FullName, editMenuGuid.ToString(), string.Format("{0} - DefualtIcon.ico", Path.GetFileName(selectedDefaultIconPath)));
                    Icon defaultIcon = Icon.ExtractAssociatedIcon(dialog.FileName);
                    MemoryStream memoryStream = new();
                    defaultIcon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    DefaultIconImage.SetSource(memoryStream.AsRandomAccessStream());
                    memoryStream.Dispose();
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Set Default lightThemeIcon failed", e);
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
                    selectedLightThemeIconPath = dialog.FileName;
                    LightThemeIconPath = Path.Combine(ShellMenuService.ShellMenuConfigDirectory.FullName, editMenuGuid.ToString(), string.Format("{0} - LightThemeIcon.ico", Path.GetFileName(selectedLightThemeIconPath)));
                    Icon lightThemeIcon = Icon.ExtractAssociatedIcon(dialog.FileName);
                    MemoryStream memoryStream = new();
                    lightThemeIcon.ToBitmap().Save(memoryStream, ImageFormat.Png);
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
                    selectedDarkThemeIconPath = dialog.FileName;
                    DarkThemeIconPath = Path.Combine(ShellMenuService.ShellMenuConfigDirectory.FullName, editMenuGuid.ToString(), string.Format("{0} - DarkThemeIcon.ico", Path.GetFileName(selectedDarkThemeIconPath)));
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
            MenuParameterText = (sender as global::Windows.UI.Xaml.Controls.TextBox).Text;
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
            MenuFileMatchFormatText = (sender as global::Windows.UI.Xaml.Controls.TextBox).Text;
        }

        #endregion 第二部分：根菜单页面——挂载的事件

        #region 第三部分：递归遍历

        /// <summary>
        /// 枚举并递归菜单项信息
        /// </summary>
        private ShellMenuItemModel EnumShellMenuItem(ShellMenuItem menuItem, MenuType menuType)
        {
            // 读取遍历到的当前项的信息
            ShellMenuItemModel shellMenuItem = new()
            {
                MenuKey = menuItem.MenuKey,
                IsSelected = menuType is MenuType.RootMenu,
                MenuType = menuType,
                MenuTitleText = menuItem.MenuTitleText,
                MenuGuid = menuItem.MenuGuid,
                DefaultIconPath = menuItem.DefaultIconPath,
                LightThemeIconPath = menuItem.LightThemeIconPath,
                DarkThemeIconPath = menuItem.DarkThemeIconPath,
                MenuProgramPathText = menuItem.MenuProgramPathText,
                MenuParameter = menuItem.MenuParameter,
                FolderBackground = menuItem.FolderBackground,
                FolderDesktop = menuItem.FolderDesktop,
                FolderDirectory = menuItem.FolderDirectory,
                FolderDrive = menuItem.FolderDrive,
                MenuFileMatchRule = menuItem.MenuFileMatchRule,
                MenuFileMatchFormatText = menuItem.MenuFileMatchFormatText,
                MenuIndex = menuItem.MenuIndex,
                ShouldUseIcon = menuItem.ShouldUseIcon,
                ShouldUseThemeIcon = menuItem.ShouldUseThemeIcon,
                ShouldUseProgramIcon = menuItem.ShouldUseProgramIcon,
            };

            if (shellMenuItem.ShouldUseIcon)
            {
                // 使用应用程序图标
                if (shellMenuItem.ShouldUseProgramIcon)
                {
                    if (File.Exists(shellMenuItem.MenuProgramPathText) && Path.GetExtension(shellMenuItem.MenuProgramPathText).Equals(".exe"))
                    {
                        try
                        {
                            Icon icon = Icon.ExtractAssociatedIcon(shellMenuItem.MenuProgramPathText);
                            MemoryStream memoryStream = new();
                            icon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            shellMenuItem.MenuIcon.SetSource(memoryStream.AsRandomAccessStream());
                            memoryStream.Dispose();
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, "Get program icon failed", e);
                        }
                    }
                }
                else
                {
                    // 使用主题菜单图标
                    if (shellMenuItem.ShouldUseThemeIcon)
                    {
                        // 浅色主题图标
                        if (ActualTheme is ElementTheme.Light && File.Exists(shellMenuItem.LightThemeIconPath))
                        {
                            try
                            {
                                Icon icon = Icon.ExtractAssociatedIcon(shellMenuItem.LightThemeIconPath);
                                MemoryStream memoryStream = new();
                                icon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                                memoryStream.Seek(0, SeekOrigin.Begin);
                                shellMenuItem.MenuIcon.SetSource(memoryStream.AsRandomAccessStream());
                                memoryStream.Dispose();
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(EventLevel.Error, "Get light theme icon failed", e);
                            }
                        }
                        // 深色主题图标
                        else if (ActualTheme is ElementTheme.Dark && File.Exists(shellMenuItem.DarkThemeIconPath))
                        {
                            try
                            {
                                Icon icon = Icon.ExtractAssociatedIcon(shellMenuItem.DarkThemeIconPath);
                                MemoryStream memoryStream = new();
                                icon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                                memoryStream.Seek(0, SeekOrigin.Begin);
                                shellMenuItem.MenuIcon.SetSource(memoryStream.AsRandomAccessStream());
                                memoryStream.Dispose();
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(EventLevel.Error, "Get light theme icon failed", e);
                            }
                        }
                    }
                    else
                    {
                        // 默认图标
                        if (File.Exists(shellMenuItem.DefaultIconPath))
                        {
                            try
                            {
                                Icon icon = Icon.ExtractAssociatedIcon(shellMenuItem.DefaultIconPath);
                                MemoryStream memoryStream = new();
                                icon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                                memoryStream.Seek(0, SeekOrigin.Begin);
                                shellMenuItem.MenuIcon.SetSource(memoryStream.AsRandomAccessStream());
                                memoryStream.Dispose();
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(EventLevel.Error, "Get default icon failed", e);
                            }
                        }
                    }
                }
            }

            // 递归遍历子项
            foreach (ShellMenuItem subMenuItem in menuItem.SubShellMenuItem)
            {
                if (menuType is MenuType.RootMenu)
                {
                    shellMenuItem.SubMenuItemCollection.Add(EnumShellMenuItem(subMenuItem, MenuType.FirstLevelMenu));
                }
                else if (menuType is MenuType.FirstLevelMenu)
                {
                    shellMenuItem.SubMenuItemCollection.Add(EnumShellMenuItem(subMenuItem, MenuType.SecondLevelMenu));
                }
            }

            return shellMenuItem;
        }

        /// <summary>
        /// 枚举并修改选中项
        /// </summary>
        private void EnumModifySelectedItem(ShellMenuItemModel selectedItem, ObservableCollection<ShellMenuItemModel> shellMenuItemCollection)
        {
            // 递归遍历列表，修改遍历到的当前项
            for (int index = 0; index < shellMenuItemCollection.Count; index++)
            {
                // 修改遍历到的当前项
                shellMenuItemCollection[index].IsSelected = false;
                if (selectedItem is not null && selectedItem.MenuKey.Equals(shellMenuItemCollection[index].MenuKey))
                {
                    selectedItem.IsSelected = true;
                    IsEditMenuEnabled = true;
                    IsAddMenuEnabled = selectedItem.MenuType is MenuType.RootMenu || selectedItem.MenuType is MenuType.FirstLevelMenu;
                    bool isFirstOrLast = false;

                    // 第一项，不可向上移动
                    if (selectedItem.MenuIndex is 0)
                    {
                        IsMoveUpEnabled = false;
                        isFirstOrLast = true;
                    }

                    // 最后一项，不可向下移动
                    if (selectedItem.MenuIndex.Equals(shellMenuItemCollection.Count - 1))
                    {
                        IsMoveDownEnabled = false;
                        isFirstOrLast = true;
                    }

                    // 不是首项也不是最后一项，可以向上移动，也可以向下移动
                    if (!isFirstOrLast)
                    {
                        IsMoveUpEnabled = true;
                        IsMoveDownEnabled = true;
                    }
                }
            }

            // 递归遍历子项
            foreach (ShellMenuItemModel subMenuItem in shellMenuItemCollection)
            {
                EnumModifySelectedItem(selectedItem, subMenuItem.SubMenuItemCollection);
            }
        }

        /// <summary>
        /// 枚举并递归修改带主题的菜单项
        /// </summary>
        private void EnumModifyShellMenuItemTheme(ShellMenuItemModel shellMenuItem)
        {
            // 修改遍历到的当前项
            if (shellMenuItem.ShouldUseIcon && !shellMenuItem.ShouldUseProgramIcon && shellMenuItem.ShouldUseThemeIcon)
            {
                if (ActualTheme is ElementTheme.Light)
                {
                    if (File.Exists(shellMenuItem.LightThemeIconPath))
                    {
                        try
                        {
                            Icon icon = Icon.ExtractAssociatedIcon(shellMenuItem.LightThemeIconPath);
                            MemoryStream memoryStream = new();
                            icon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            DarkThemeIconImage.SetSource(memoryStream.AsRandomAccessStream());
                            memoryStream.Dispose();
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, "Get light theme icon failed", e);
                        }
                    }
                }
                else if (ActualTheme is ElementTheme.Dark)
                {
                    if (File.Exists(shellMenuItem.DarkThemeIconPath))
                    {
                        try
                        {
                            Icon icon = Icon.ExtractAssociatedIcon(shellMenuItem.DarkThemeIconPath);
                            MemoryStream memoryStream = new();
                            icon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            DarkThemeIconImage.SetSource(memoryStream.AsRandomAccessStream());
                            memoryStream.Dispose();
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, "Get light theme icon failed", e);
                        }
                    }
                }
            }

            // 递归遍历子项
            foreach (ShellMenuItemModel subMenuItem in shellMenuItem.SubMenuItemCollection)
            {
                EnumModifyShellMenuItemTheme(subMenuItem);
            }
        }

        /// <summary>
        /// 枚举并递归获取选中项，向上移动
        /// </summary>
        private void EnumMoveUpShellMenuItem(ShellMenuItemModel selectedItem, ObservableCollection<ShellMenuItemModel> shellMenuItemCollection)
        {
            // 递归遍历列表，修改选中项顺序
            for (int index = 1; index < shellMenuItemCollection.Count; index++)
            {
                if (selectedItem.MenuKey.Equals(shellMenuItemCollection[index].MenuKey))
                {
                    shellMenuItemCollection[index].MenuIndex = index - 1;
                    shellMenuItemCollection[index - 1].MenuIndex = index;

                    AutoResetEvent autoResetEvent = new(false);

                    // 修改已保存的数据
                    Task.Run(() =>
                    {
                        // 修改顺序后的菜单项信息
                        ShellMenuItem swappedMenuItem = new()
                        {
                            MenuGuid = shellMenuItemCollection[index - 1].MenuGuid,
                            MenuIndex = shellMenuItemCollection[index - 1].MenuIndex,
                            MenuTitleText = shellMenuItemCollection[index - 1].MenuTitleText,
                            ShouldUseIcon = shellMenuItemCollection[index - 1].ShouldUseIcon,
                            ShouldUseProgramIcon = shellMenuItemCollection[index - 1].ShouldUseProgramIcon,
                            ShouldUseThemeIcon = shellMenuItemCollection[index - 1].ShouldUseThemeIcon,
                            DefaultIconPath = shellMenuItemCollection[index - 1].DefaultIconPath,
                            LightThemeIconPath = shellMenuItemCollection[index - 1].LightThemeIconPath,
                            DarkThemeIconPath = shellMenuItemCollection[index - 1].DarkThemeIconPath,
                            MenuProgramPathText = shellMenuItemCollection[index - 1].MenuProgramPathText,
                            MenuParameter = shellMenuItemCollection[index - 1].MenuParameter,
                            FolderBackground = shellMenuItemCollection[index - 1].FolderBackground,
                            FolderDesktop = shellMenuItemCollection[index - 1].FolderDesktop,
                            FolderDirectory = shellMenuItemCollection[index - 1].FolderDirectory,
                            FolderDrive = shellMenuItemCollection[index - 1].FolderDrive,
                            MenuFileMatchRule = shellMenuItemCollection[index - 1].MenuFileMatchRule,
                            MenuFileMatchFormatText = shellMenuItemCollection[index - 1].MenuFileMatchFormatText,
                        };

                        // 选中项菜单信息
                        ShellMenuItem selectedMenuItem = new()
                        {
                            MenuGuid = shellMenuItemCollection[index].MenuGuid,
                            MenuIndex = shellMenuItemCollection[index].MenuIndex,
                            MenuTitleText = shellMenuItemCollection[index].MenuTitleText,
                            ShouldUseIcon = shellMenuItemCollection[index].ShouldUseIcon,
                            ShouldUseProgramIcon = shellMenuItemCollection[index].ShouldUseProgramIcon,
                            ShouldUseThemeIcon = shellMenuItemCollection[index].ShouldUseThemeIcon,
                            DefaultIconPath = shellMenuItemCollection[index].DefaultIconPath,
                            LightThemeIconPath = shellMenuItemCollection[index].LightThemeIconPath,
                            DarkThemeIconPath = shellMenuItemCollection[index].DarkThemeIconPath,
                            MenuProgramPathText = shellMenuItemCollection[index].MenuProgramPathText,
                            MenuParameter = shellMenuItemCollection[index].MenuParameter,
                            FolderBackground = shellMenuItemCollection[index].FolderBackground,
                            FolderDesktop = shellMenuItemCollection[index].FolderDesktop,
                            FolderDirectory = shellMenuItemCollection[index].FolderDirectory,
                            FolderDrive = shellMenuItemCollection[index].FolderDrive,
                            MenuFileMatchRule = shellMenuItemCollection[index].MenuFileMatchRule,
                            MenuFileMatchFormatText = shellMenuItemCollection[index].MenuFileMatchFormatText,
                        };

                        ShellMenuService.SaveShellMenuItem(shellMenuItemCollection[index - 1].MenuKey, swappedMenuItem);
                        ShellMenuService.SaveShellMenuItem(shellMenuItemCollection[index].MenuKey, selectedMenuItem);
                        autoResetEvent.Set();
                    });

                    autoResetEvent.WaitOne();
                    autoResetEvent.Dispose();
                    autoResetEvent = null;

                    (shellMenuItemCollection[index], shellMenuItemCollection[index - 1]) = (shellMenuItemCollection[index - 1], shellMenuItemCollection[index]);
                }
            }

            // 递归遍历子项
            foreach (ShellMenuItemModel shellMenuItem in shellMenuItemCollection)
            {
                EnumMoveUpShellMenuItem(selectedItem, shellMenuItem.SubMenuItemCollection);
            }
        }

        /// <summary>
        /// 枚举并递归获取选中项，向下移动
        /// </summary>
        private void EnumMoveDownShellMenuItem(ShellMenuItemModel selectedItem, ObservableCollection<ShellMenuItemModel> shellMenuItemCollection)
        {
            // 递归遍历列表，修改选中项顺序
            for (int index = shellMenuItemCollection.Count - 1; index > 0; index--)
            {
                if (selectedItem.MenuKey.Equals(shellMenuItemCollection[index].MenuKey))
                {
                    shellMenuItemCollection[index].MenuIndex = index + 1;
                    shellMenuItemCollection[index + 1].MenuIndex = index;

                    AutoResetEvent autoResetEvent = new(false);

                    Task.Run(() =>
                    {
                        // 保存修改顺序后的菜单项信息
                        ShellMenuItem swappedMenuItem = new()
                        {
                            MenuGuid = shellMenuItemCollection[index + 1].MenuGuid,
                            MenuIndex = shellMenuItemCollection[index + 1].MenuIndex,
                            MenuTitleText = shellMenuItemCollection[index + 1].MenuTitleText,
                            ShouldUseIcon = shellMenuItemCollection[index + 1].ShouldUseIcon,
                            ShouldUseProgramIcon = shellMenuItemCollection[index + 1].ShouldUseProgramIcon,
                            ShouldUseThemeIcon = shellMenuItemCollection[index + 1].ShouldUseThemeIcon,
                            DefaultIconPath = shellMenuItemCollection[index + 1].DefaultIconPath,
                            LightThemeIconPath = shellMenuItemCollection[index + 1].LightThemeIconPath,
                            DarkThemeIconPath = shellMenuItemCollection[index + 1].DarkThemeIconPath,
                            MenuProgramPathText = shellMenuItemCollection[index + 1].MenuProgramPathText,
                            MenuParameter = shellMenuItemCollection[index + 1].MenuParameter,
                            FolderBackground = shellMenuItemCollection[index + 1].FolderBackground,
                            FolderDesktop = shellMenuItemCollection[index + 1].FolderDesktop,
                            FolderDirectory = shellMenuItemCollection[index + 1].FolderDirectory,
                            FolderDrive = shellMenuItemCollection[index + 1].FolderDrive,
                            MenuFileMatchRule = shellMenuItemCollection[index + 1].MenuFileMatchRule,
                            MenuFileMatchFormatText = shellMenuItemCollection[index + 1].MenuFileMatchFormatText,
                        };

                        // 选中项菜单信息
                        ShellMenuItem selectedMenuItem = new()
                        {
                            MenuGuid = shellMenuItemCollection[index].MenuGuid,
                            MenuIndex = shellMenuItemCollection[index].MenuIndex,
                            MenuTitleText = shellMenuItemCollection[index].MenuTitleText,
                            ShouldUseIcon = shellMenuItemCollection[index].ShouldUseIcon,
                            ShouldUseProgramIcon = shellMenuItemCollection[index].ShouldUseProgramIcon,
                            ShouldUseThemeIcon = shellMenuItemCollection[index].ShouldUseThemeIcon,
                            DefaultIconPath = shellMenuItemCollection[index].DefaultIconPath,
                            LightThemeIconPath = shellMenuItemCollection[index].LightThemeIconPath,
                            DarkThemeIconPath = shellMenuItemCollection[index].DarkThemeIconPath,
                            MenuProgramPathText = shellMenuItemCollection[index].MenuProgramPathText,
                            MenuParameter = shellMenuItemCollection[index].MenuParameter,
                            FolderBackground = shellMenuItemCollection[index].FolderBackground,
                            FolderDesktop = shellMenuItemCollection[index].FolderDesktop,
                            FolderDirectory = shellMenuItemCollection[index].FolderDirectory,
                            FolderDrive = shellMenuItemCollection[index].FolderDrive,
                            MenuFileMatchRule = shellMenuItemCollection[index].MenuFileMatchRule,
                            MenuFileMatchFormatText = shellMenuItemCollection[index].MenuFileMatchFormatText,
                        };

                        ShellMenuService.SaveShellMenuItem(shellMenuItemCollection[index + 1].MenuKey, swappedMenuItem);
                        ShellMenuService.SaveShellMenuItem(shellMenuItemCollection[index].MenuKey, selectedMenuItem);
                        autoResetEvent.Set();
                    });

                    autoResetEvent.WaitOne();
                    autoResetEvent.Dispose();
                    autoResetEvent = null;

                    // 修改已保存的数据
                    (shellMenuItemCollection[index], shellMenuItemCollection[index + 1]) = (shellMenuItemCollection[index + 1], shellMenuItemCollection[index]);
                }
            }

            // 递归遍历子项
            foreach (ShellMenuItemModel shellMenuItem in shellMenuItemCollection)
            {
                EnumMoveDownShellMenuItem(selectedItem, shellMenuItem.SubMenuItemCollection);
            }
        }

        #endregion 第三部分：递归遍历
    }
}
