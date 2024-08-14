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

        private List<DictionaryEntry> FileMatchRuleList { get; } =
        [
            new DictionaryEntry(ShellMenu.None,"None"),
            new DictionaryEntry(ShellMenu.Name,"Name"),
            new DictionaryEntry(ShellMenu.NameRegex,"NameRegex"),
            new DictionaryEntry(ShellMenu.Extension,"Extension"),
            new DictionaryEntry(ShellMenu.All,"All")
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
                ShellMenuItem rootShellMenuItem = ShellMenuService.GetShellMenuItem();

                synchronizationContext.Send(_ =>
                {
                    if (rootShellMenuItem is not null)
                    {
                        ShellMenuItemCollection.Add(EnumShellMenuItem(rootShellMenuItem, MenuType.RootMenu));

                        IsAddMenuEnabled = true;

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
            if (ShellMenuItemCollection.Count is 1)
            {
                EnumModifyShellMenuItemTheme(ShellMenuItemCollection[0]);
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
                    ShellMenuItem shellMenuItem = new()
                    {
                        MenuGuid = editMenuGuid,
                        MenuTitleText = MenuTitleText,
                        ShouldUseProgramIcon = ShouldUseProgramIcon,
                        ShouldEnableThemeIcon = ShouldEnableThemeIcon,
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
            Guid menuGuid = Guid.NewGuid();
            string menuKey = string.Empty;

            if (selectedItem is null)
            {
                menuKey = menuGuid.ToString();
            }
            else if (selectedItem.MenuType is MenuType.RootMenu || selectedItem.MenuType is MenuType.FirstLevelMenu)
            {
                menuKey = Path.Combine(selectedItem.MenuKey, menuGuid.ToString());
            }

            AddShellMenu(menuKey, Guid.NewGuid());
            BreadCollection.Add(new DictionaryEntry(ShellMenu.EditMenu, "EditMenu"));
        }

        /// <summary>
        /// 清空菜单项
        /// </summary>
        private void OnClearMenuClicked(object sender, RoutedEventArgs args)
        {
            // 更新删除操作
            ShellMenuItemCollection.Clear();
            selectedItem = null;
            IsAddMenuEnabled = true;
            IsEditMenuEnabled = false;
        }

        /// <summary>
        /// 刷新列表
        /// </summary>
        private void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            ShellMenuItemCollection.Clear();

            Task.Run(() =>
            {
                ShellMenuItem rootShellMenuItem = ShellMenuService.GetShellMenuItem();

                synchronizationContext.Send(_ =>
                {
                    if (rootShellMenuItem is not null)
                    {
                        ShellMenuItemCollection.Add(EnumShellMenuItem(rootShellMenuItem, MenuType.RootMenu));

                        IsAddMenuEnabled = true;

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
                    }
                }, null);
            });
        }

        /// <summary>
        /// 编辑菜单
        /// </summary>
        private void OnEditMenuClicked(object sender, RoutedEventArgs args)
        {
            if (selectedItem is not null && BreadCollection.Count is 1)
            {
                editMenuGuid = selectedItem.MenuGuid;
                MenuTitleText = selectedItem.MenuTitleText;
                ShouldUseProgramIcon = selectedItem.ShouldUseProgramIcon;
                ShouldEnableThemeIcon = selectedItem.ShouldEnableThemeIcon;
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
        /// 点击选中项触发的事件
        /// </summary>
        private void OnItemInvoked(Microsoft.UI.Xaml.Controls.TreeView sender, Microsoft.UI.Xaml.Controls.TreeViewItemInvokedEventArgs args)
        {
            if (selectedItem is not null)
            {
                selectedItem.IsSelected = false;
            }

            selectedItem = args.InvokedItem as ShellMenuItemModel;

            if (selectedItem is not null)
            {
                selectedItem.IsSelected = true;
                IsEditMenuEnabled = true;
                IsAddMenuEnabled = selectedItem.MenuType is MenuType.RootMenu || selectedItem.MenuType is MenuType.FirstLevelMenu;
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

        /// <summary>
        /// 添加菜单项信息
        /// </summary>
        private void AddShellMenu(string menuKey, Guid menuGuid)
        {
            editMenuKey = menuKey;
            editMenuGuid = menuGuid;
            selectedDefaultIconPath = string.Empty;
            selectedLightThemeIconPath = string.Empty;
            selectedDarkThemeIconPath = string.Empty;
            MenuTitleText = string.Empty;
            ShouldUseProgramIcon = true;
            ShouldEnableThemeIcon = true;
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
        }

        /// <summary>
        /// 枚举并递归菜单项信息
        /// </summary>
        private ShellMenuItemModel EnumShellMenuItem(ShellMenuItem menuItem, MenuType menuType)
        {
            ShellMenuItemModel shellMenuItem = new()
            {
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
                MenuFileMatchFormatText = menuItem.MenuFileMatchFormatText
            };

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
                        DarkThemeIconImage.SetSource(memoryStream.AsRandomAccessStream());
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
                if (shellMenuItem.ShouldEnableThemeIcon)
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
                else
                {
                    if (File.Exists(shellMenuItem.DefaultIconPath))
                    {
                        try
                        {
                            Icon icon = Icon.ExtractAssociatedIcon(shellMenuItem.DefaultIconPath);
                            MemoryStream memoryStream = new();
                            icon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            DarkThemeIconImage.SetSource(memoryStream.AsRandomAccessStream());
                            memoryStream.Dispose();
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, "Get default icon failed", e);
                        }
                    }
                }
            }

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
        /// 枚举并递归修改带主题的菜单项
        /// </summary>
        private void EnumModifyShellMenuItemTheme(ShellMenuItemModel shellMenuItem)
        {
            if (!shellMenuItem.ShouldUseProgramIcon && shellMenuItem.ShouldEnableThemeIcon)
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
        }

        /// <summary>
        /// 枚举并递归修改主题项信息
        /// </summary>
        private bool EnumModifyShellMenuItemData(ShellMenuItemModel shellMenuItem, ShellMenuItem menuItem)
        {
            if (shellMenuItem.MenuKey.Equals(menuItem.MenuKey))
            {
                shellMenuItem.MenuTitleText = menuItem.MenuTitleText;
                shellMenuItem.MenuGuid = menuItem.MenuGuid;
                shellMenuItem.DefaultIconPath = menuItem.DefaultIconPath;
                shellMenuItem.LightThemeIconPath = menuItem.LightThemeIconPath;
                shellMenuItem.DarkThemeIconPath = menuItem.DarkThemeIconPath;
                shellMenuItem.MenuProgramPathText = menuItem.MenuProgramPathText;
                shellMenuItem.MenuParameter = menuItem.MenuParameter;
                shellMenuItem.FolderBackground = menuItem.FolderBackground;
                shellMenuItem.FolderDesktop = menuItem.FolderDesktop;
                shellMenuItem.FolderDirectory = menuItem.FolderDirectory;
                shellMenuItem.FolderDrive = menuItem.FolderDrive;
                shellMenuItem.MenuFileMatchRule = menuItem.MenuFileMatchRule;
                shellMenuItem.MenuFileMatchFormatText = menuItem.MenuFileMatchFormatText;
                return true;
            }
            else
            {
                foreach (ShellMenuItemModel subMenuItem in shellMenuItem.SubMenuItemCollection)
                {
                    return EnumModifyShellMenuItemData(subMenuItem, menuItem);
                }
            }

            return false;
        }

        /// <summary>
        /// 获取 ToggleSwitch 的文字转向
        /// </summary>
        private global::Windows.UI.Xaml.FlowDirection GetToggleSwitchDirection(RightToLeft rightToLeft)
        {
            return rightToLeft is RightToLeft.Yes ? global::Windows.UI.Xaml.FlowDirection.LeftToRight : global::Windows.UI.Xaml.FlowDirection.RightToLeft;
        }
    }
}
