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
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using WindowsTools.Extensions.DataType.Enums;
using WindowsTools.Extensions.ShellMenu;
using WindowsTools.Helpers.Controls;
using WindowsTools.Helpers.Root;
using WindowsTools.Models;
using WindowsTools.Services.Root;
using WindowsTools.Services.Shell;
using WindowsTools.UI.TeachingTips;
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
        public static readonly string shellMenuConfigurationKey = @"Software\WindowsTools\ShellMenuConfigurationTest";
        private readonly InMemoryRandomAccessStream emptyStream = new();
        private string selectedDefaultIconPath = string.Empty;
        private string selectedLightThemeIconPath = string.Empty;
        private string selectedDarkThemeIconPath = string.Empty;
        private Guid editMenuGuid;
        private string editMenuKey;
        private int editMenuIndex;
        private bool isInitialized;
        private bool needToRefreshData;
        public bool isChanger;
        private ShellMenuItemModel selectedItem;

        private bool _isLoading = false;

        public bool IsLoading
        {
            get { return _isLoading; }

            set
            {
                if (!Equals(_isLoading, value))
                {
                    _isLoading = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
                }
            }
        }

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

        private bool _isRemoveMenuEnabled;

        public bool IsRemoveMenuEnabled
        {
            get { return _isRemoveMenuEnabled; }

            set
            {
                if (!Equals(_isRemoveMenuEnabled, value))
                {
                    _isRemoveMenuEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRemoveMenuEnabled)));
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

        private bool _isAlwaysRunAsAdministrator;

        public bool IsAlwaysRunAsAdministrator
        {
            get { return _isAlwaysRunAsAdministrator; }

            set
            {
                if (!Equals(_isAlwaysRunAsAdministrator, value))
                {
                    _isAlwaysRunAsAdministrator = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAlwaysRunAsAdministrator)));
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

        private KeyValuePair<string, string> _selectedFileMatchRule;

        public KeyValuePair<string, string> SelectedFileMatchRule
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

        private List<KeyValuePair<string, string>> FileMatchRuleList { get; } =
        [
            new KeyValuePair<string,string>("None", ResourceService.ShellMenuResource.GetString("None")),
            new KeyValuePair<string,string>("Name", ResourceService.ShellMenuResource.GetString("Name")),
            new KeyValuePair<string,string>("NameRegex",ResourceService.ShellMenuResource.GetString("NameRegex")),
            new KeyValuePair<string,string>("Extension",ResourceService.ShellMenuResource.GetString("Extension")),
            new KeyValuePair<string,string>("All",ResourceService.ShellMenuResource.GetString("All"))
        ];

        public ObservableCollection<DictionaryEntry> BreadCollection { get; } =
        [
            new DictionaryEntry(ResourceService.ShellMenuResource.GetString("Title"), "Title")
        ];

        private ObservableCollection<ShellMenuItemModel> ShellMenuItemCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public ShellMenuPage()
        {
            InitializeComponent();
            SelectedFileMatchRule = FileMatchRuleList[4];
        }

        #region 第一部分：重写父类事件

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (args.Parameter is string parameter && !string.IsNullOrEmpty(parameter))
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
        /// 在已构造 FrameworkElement 并将其添加到对象树中并准备好交互时发生的事件
        /// </summary>
        private async void OnLoaded(object sender, RoutedEventArgs args)
        {
            if (!isInitialized)
            {
                isInitialized = true;
                IsLoading = true;
                RegistryHelper.NotifyKeyValueChanged += OnNotifyKeyValueChanged;

                // 获取所有菜单项信息
                ShellMenuItem rootShellMenuItem = await Task.Run(ShellMenuService.GetShellMenuItem);

                await Task.Delay(500);
                IsAddMenuEnabled = true;
                IsMoveUpEnabled = false;
                IsMoveDownEnabled = false;

                if (rootShellMenuItem is not null)
                {
                    ShellMenuItemCollection.Add(EnumShellMenuItem(rootShellMenuItem, MenuType.FirstLevelMenu));

                    if (ShellMenuItemCollection.Count is 0)
                    {
                        selectedItem = null;
                        IsRemoveMenuEnabled = false;
                        IsEditMenuEnabled = false;
                    }
                    else
                    {
                        ShellMenuItemCollection[0].IsSelected = true;
                        selectedItem = ShellMenuItemCollection[0];
                        IsRemoveMenuEnabled = true;
                        IsEditMenuEnabled = true;
                    }
                }
                else
                {
                    selectedItem = null;
                    IsRemoveMenuEnabled = false;
                    IsEditMenuEnabled = false;
                }

                IsLoading = false;

                await Task.Run(() =>
                {
                    RegistryHelper.MonitorRegistryValueChange(@"Software\WindowsTools\ShellMenuTest");
                });
            }

            foreach (ShellMenuItemModel shellMenuItem in ShellMenuItemCollection)
            {
                EnumModifyShellMenuItemTheme(shellMenuItem);
            }
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
            KeyValuePair<string, string> breadItem = (KeyValuePair<string, string>)args.Item;

            if (BreadCollection.Count is 2)
            {
                if (breadItem.Key.Equals(BreadCollection[0].Key))
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
        private async void OnSaveClicked(object sender, RoutedEventArgs args)
        {
            // 菜单数据已发生更改，通知用户手动刷新
            if (needToRefreshData)
            {
                await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.ShellMenuNeedToRefreshData));
                return;
            }

            // 有部分内容是必填项，没填的内容进行提示
            if (string.IsNullOrEmpty(MenuTitleText))
            {
                MenuTitleTextBox.Focus(FocusState.Programmatic);
                await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.MenuTitleEmpty));
                return;
            }

            if (ShouldUseIcon && !ShouldUseProgramIcon)
            {
                if (ShouldUseThemeIcon)
                {
                    if (string.IsNullOrEmpty(LightThemeIconPath))
                    {
                        MenuLigthThemeIconBrowserButton.Focus(FocusState.Programmatic);
                        await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.MenuLightThemeIconPathEmpty));
                        return;
                    }

                    if (string.IsNullOrEmpty(DarkThemeIconPath))
                    {
                        MenuDarkThemeIconBrowserButton.Focus(FocusState.Programmatic);
                        await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.MenuDarkThemeIconPathEmpty));
                        return;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(DefaultIconPath))
                    {
                        MenuDefaultIconBrowserButton.Focus(FocusState.Programmatic);
                        await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.MenuDefaultIconPathEmpty));
                        return;
                    }
                }
            }

            if (string.IsNullOrEmpty(MenuProgramPathText))
            {
                MenuProgramBrowserButton.Focus(FocusState.Programmatic);
                await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.MenuProgramPathEmpty));
                return;
            }

            if ((SelectedFileMatchRule.Equals(FileMatchRuleList[1]) || SelectedFileMatchRule.Equals(FileMatchRuleList[2]) || SelectedFileMatchRule.Equals(FileMatchRuleList[3])) && string.IsNullOrEmpty(MenuFileMatchFormatText))
            {
                MenuFileMatchFormatTextBox.Focus(FocusState.Programmatic);
                await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.MenuMatchRuleEmpty));
                return;
            }

            if (BreadCollection.Count is 2)
            {
                ShellMenuItem rootShellMenuItem = await Task.Run(() =>
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
                        MenuProgramPath = MenuProgramPathText,
                        MenuParameter = MenuParameterText,
                        IsAlwaysRunAsAdministrator = IsAlwaysRunAsAdministrator,
                        FolderBackground = FolderBackgroundMatch,
                        FolderDesktop = FolderDesktopMatch,
                        FolderDirectory = FolderDirectoryMatch,
                        FolderDrive = FolderDriveMatch,
                        MenuFileMatchRule = SelectedFileMatchRule.Key,
                        MenuFileMatchFormatText = MenuFileMatchFormatText
                    };

                    isChanger = true;
                    ShellMenuService.SaveShellMenuItem(editMenuKey, shellMenuItem);

                    // 复制选中的图标文件到指定目录
                    if (File.Exists(selectedDefaultIconPath))
                    {
                        try
                        {
                            string defaultIconDirectoryPath = Path.GetDirectoryName(DefaultIconPath);
                            if (!Directory.Exists(defaultIconDirectoryPath))
                            {
                                Directory.CreateDirectory(defaultIconDirectoryPath);
                            }

                            File.Copy(selectedDefaultIconPath, DefaultIconPath, true);
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, string.Format("Copy default icon {0} failed", DefaultIconPath), e);
                        }
                    }

                    if (File.Exists(selectedLightThemeIconPath))
                    {
                        try
                        {
                            string lightThemeIconDirectoryPath = Path.GetDirectoryName(LightThemeIconPath);
                            if (!Directory.Exists(lightThemeIconDirectoryPath))
                            {
                                Directory.CreateDirectory(lightThemeIconDirectoryPath);
                            }

                            File.Copy(selectedLightThemeIconPath, LightThemeIconPath, true);
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, string.Format("Copy light theme icon {0} failed", LightThemeIconPath), e);
                        }
                    }

                    if (File.Exists(selectedDarkThemeIconPath))
                    {
                        try
                        {
                            string darkThemeIconDirectoryPath = Path.GetDirectoryName(DarkThemeIconPath);
                            if (!Directory.Exists(darkThemeIconDirectoryPath))
                            {
                                Directory.CreateDirectory(darkThemeIconDirectoryPath);
                            }

                            File.Copy(selectedDarkThemeIconPath, DarkThemeIconPath, true);
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, string.Format("Copy dark theme icon {0} failed", DarkThemeIconPath), e);
                        }
                    }

                    // 保存完成，刷新菜单列表
                    return ShellMenuService.GetShellMenuItem();
                });

                BreadCollection.RemoveAt(1);
                IsLoading = true;
                ShellMenuItemCollection.Clear();
                IsAddMenuEnabled = true;
                IsMoveUpEnabled = false;
                IsMoveDownEnabled = false;

                if (rootShellMenuItem is not null)
                {
                    ShellMenuItemCollection.Add(EnumShellMenuItem(rootShellMenuItem, MenuType.FirstLevelMenu));

                    if (ShellMenuItemCollection.Count is 0)
                    {
                        selectedItem = null;
                        IsRemoveMenuEnabled = false;
                        IsEditMenuEnabled = false;
                    }
                    else
                    {
                        ShellMenuItemCollection[0].IsSelected = true;
                        selectedItem = ShellMenuItemCollection[0];
                        IsRemoveMenuEnabled = true;
                        IsEditMenuEnabled = true;
                    }
                }
                else
                {
                    selectedItem = null;
                    IsRemoveMenuEnabled = false;
                    IsEditMenuEnabled = false;
                }

                await Task.Delay(500);
                IsLoading = false;
            }
        }

        /// <summary>
        /// 添加菜单
        /// </summary>
        private async void OnAddMenuItemClicked(object sender, RoutedEventArgs args)
        {
            // 菜单数据已发生更改，通知用户手动刷新
            if (needToRefreshData)
            {
                await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.ShellMenuNeedToRefreshData));
                return;
            }

            Guid menuGuid = Guid.NewGuid();
            string menuKey = string.Empty;
            int menuIndex = -1;

            await Task.Run(() =>
            {
                // 检查添加项是否为根菜单项
                if (selectedItem is null)
                {
                    menuKey = Path.Combine(@"Software\WindowsTools\ShellMenuTest", menuGuid.ToString());
                    menuIndex = 0;
                }
                else if (selectedItem.MenuType is MenuType.FirstLevelMenu)
                {
                    menuKey = Path.Combine(selectedItem.MenuKey, menuGuid.ToString());
                    menuIndex = selectedItem.SubMenuItemCollection.Count;
                }
            });

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
            ShouldUseThemeIcon = false;
            DefaultIconPath = string.Empty;
            DefaultIconImage.SetSource(emptyStream);
            LightThemeIconPath = string.Empty;
            LightThemeIconImage.SetSource(emptyStream);
            DarkThemeIconPath = string.Empty;
            DarkThemeIconImage.SetSource(emptyStream);
            MenuProgramPathText = string.Empty;
            MenuParameterText = string.Empty;
            FolderBackgroundMatch = false;
            IsAlwaysRunAsAdministrator = false;
            FolderDesktopMatch = false;
            FolderDirectoryMatch = false;
            FolderDriveMatch = false;
            SelectedFileMatchRule = FileMatchRuleList[4];
            NeedInputMatchFormat = false;
            MenuFileMatchFormatPHText = string.Empty;
            MenuFileMatchFormatText = string.Empty;

            BreadCollection.Add(new DictionaryEntry(ResourceService.ShellMenuResource.GetString("EditMenu"), "EditMenu"));
        }

        /// <summary>
        /// 移除菜单项
        /// </summary>

        private async void OnRemoveMenuItemClicked(object sender, RoutedEventArgs args)
        {
            // 菜单数据已发生更改，通知用户手动刷新
            if (needToRefreshData)
            {
                await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.ShellMenuNeedToRefreshData));
                return;
            }

            if (selectedItem is not null)
            {
                // 移除指定的菜单项
                await Task.Run(() =>
                {
                    ShellMenuService.RemoveShellMenuItem(selectedItem.MenuKey);
                });

                ShellMenuItemModel parentItem = EnumRemoveItem(selectedItem, null, ShellMenuItemCollection);

                // 删除的父项为空，说明被删除项为根菜单项
                if (parentItem is null)
                {
                    selectedItem = null;
                    IsAddMenuEnabled = true;
                    IsRemoveMenuEnabled = false;
                    IsEditMenuEnabled = false;
                    IsMoveUpEnabled = false;
                    IsMoveDownEnabled = false;
                }
                else
                {
                    parentItem.IsSelected = true;
                    selectedItem = parentItem;
                    IsAddMenuEnabled = true;
                    IsRemoveMenuEnabled = true;
                    IsEditMenuEnabled = true;
                    EnumModifySelectedItem(selectedItem, ShellMenuItemCollection);
                }
            }
        }

        /// <summary>
        /// 清空菜单项
        /// </summary>
        private async void OnClearMenuClicked(object sender, RoutedEventArgs args)
        {
            // 菜单数据已发生更改，通知用户手动刷新
            if (needToRefreshData)
            {
                await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.ShellMenuNeedToRefreshData));
                return;
            }

            if (ShellMenuItemCollection.Count > 0)
            {
                string rootMenuKey = ShellMenuItemCollection[0].MenuKey;

                // 清空所有菜单项信息
                await Task.Run(() =>
                {
                    ShellMenuService.RemoveShellMenuItem(rootMenuKey);
                });

                ShellMenuItemCollection.Clear();
                selectedItem = null;
                IsAddMenuEnabled = true;
                IsRemoveMenuEnabled = false;
                IsEditMenuEnabled = false;
                IsMoveUpEnabled = false;
                IsMoveDownEnabled = false;
            }
        }

        /// <summary>
        /// 刷新列表
        /// </summary>
        private async void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            IsLoading = true;
            ShellMenuItemCollection.Clear();

            // 获取所有菜单项信息
            ShellMenuItem rootShellMenuItem = await Task.Run(() =>
            {
                return ShellMenuService.GetShellMenuItem();
            });

            await Task.Delay(500);
            IsAddMenuEnabled = true;
            IsMoveUpEnabled = false;
            IsMoveDownEnabled = false;

            if (rootShellMenuItem is not null)
            {
                ShellMenuItemCollection.Add(EnumShellMenuItem(rootShellMenuItem, MenuType.FirstLevelMenu));

                if (ShellMenuItemCollection.Count is 0)
                {
                    selectedItem = null;
                    IsRemoveMenuEnabled = false;
                    IsEditMenuEnabled = false;
                }
                else
                {
                    ShellMenuItemCollection[0].IsSelected = true;
                    selectedItem = ShellMenuItemCollection[0];
                    IsRemoveMenuEnabled = true;
                    IsEditMenuEnabled = true;
                }
            }
            else
            {
                selectedItem = null;
                IsRemoveMenuEnabled = false;
                IsEditMenuEnabled = false;
            }

            IsLoading = false;
            needToRefreshData = false;
        }

        /// <summary>
        /// 编辑菜单
        /// </summary>
        private async void OnEditMenuClicked(object sender, RoutedEventArgs args)
        {
            // 菜单数据已发生更改，通知用户手动刷新
            if (needToRefreshData)
            {
                await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.ShellMenuNeedToRefreshData));
                return;
            }

            // 获取选中项的菜单信息
            if (selectedItem is not null && BreadCollection.Count is 1)
            {
                selectedDefaultIconPath = string.Empty;
                selectedLightThemeIconPath = string.Empty;
                selectedDarkThemeIconPath = string.Empty;
                editMenuKey = selectedItem.MenuKey;
                editMenuIndex = selectedItem.MenuIndex;
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
                IsAlwaysRunAsAdministrator = selectedItem.IsAlwaysRunAsAdministrator;
                FolderBackgroundMatch = selectedItem.FolderBackground;
                FolderDesktopMatch = selectedItem.FolderDesktop;
                FolderDirectoryMatch = selectedItem.FolderDirectory;
                FolderDriveMatch = selectedItem.FolderDrive;
                MenuFileMatchFormatText = selectedItem.MenuFileMatchFormatText;

                for (int index = 0; index < FileMatchRuleList.Count; index++)
                {
                    if (selectedItem.MenuFileMatchRule.Equals(FileMatchRuleList[index].Key))
                    {
                        SelectedFileMatchRule = FileMatchRuleList[index];
                    }
                }

                NeedInputMatchFormat = !SelectedFileMatchRule.Equals(FileMatchRuleList[0]) && !SelectedFileMatchRule.Equals(FileMatchRuleList[4]);

                DefaultIconImage.SetSource(emptyStream);
                LightThemeIconImage.SetSource(emptyStream);
                DarkThemeIconImage.SetSource(emptyStream);

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
                        LogService.WriteLog(EventLevel.Error, string.Format("Load default icon image {0} failed", DefaultIconPath), e);
                    }
                }

                if (File.Exists(LightThemeIconPath))
                {
                    try
                    {
                        Icon lightThemeIcon = Icon.ExtractAssociatedIcon(LightThemeIconPath);
                        MemoryStream memoryStream = new();
                        lightThemeIcon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        LightThemeIconImage.SetSource(memoryStream.AsRandomAccessStream());
                        memoryStream.Dispose();
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, string.Format("Load light theme icon image {0} failed", LightThemeIconPath), e);
                    }
                }

                if (File.Exists(DarkThemeIconPath))
                {
                    try
                    {
                        Icon darkThemeIcon = Icon.ExtractAssociatedIcon(DarkThemeIconPath);
                        MemoryStream memoryStream = new();
                        darkThemeIcon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        DarkThemeIconImage.SetSource(memoryStream.AsRandomAccessStream());
                        memoryStream.Dispose();
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, string.Format("Load dark theme icon image {0} failed", DarkThemeIconPath), e);
                    }
                }

                BreadCollection.Add(new DictionaryEntry(ResourceService.ShellMenuResource.GetString("EditMenu"), "EditMenu"));
            }
        }

        /// <summary>
        /// 向上移动菜单项
        /// </summary>
        private async void OnMoveUpClicked(object sender, RoutedEventArgs args)
        {
            // 菜单数据已发生更改，通知用户手动刷新
            if (needToRefreshData)
            {
                await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.ShellMenuNeedToRefreshData));
                return;
            }

            EnumMoveUpShellMenuItem(selectedItem, ShellMenuItemCollection);
        }

        /// <summary>
        /// 向下移动菜单项
        /// </summary>
        private async void OnMoveDownClicked(object sender, RoutedEventArgs args)
        {
            // 菜单数据已发生更改，通知用户手动刷新
            if (needToRefreshData)
            {
                await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.ShellMenuNeedToRefreshData));
                return;
            }

            EnumMoveDownShellMenuItem(selectedItem, ShellMenuItemCollection);
        }

        /// <summary>
        /// 点击选中项触发的事件
        /// </summary>
        private void OnItemInvoked(Microsoft.UI.Xaml.Controls.TreeView sender, Microsoft.UI.Xaml.Controls.TreeViewItemInvokedEventArgs args)
        {
            if (args.InvokedItem is ShellMenuItemModel selectedItem)
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
            if (sender is ToggleSwitch toggleSwitch)
            {
                ShouldUseIcon = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 是否使用应用程序图标修改时触发的事件
        /// </summary>
        private void OnShouldUseProgramIconToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                ShouldUseProgramIcon = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 是否启用主题图标按钮修改时触发的事件
        /// </summary>
        private void OnShouldUseThemeIconToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                ShouldUseThemeIcon = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 默认图标修改
        /// </summary>
        private void OnDefaultIconBrowserClicked(object sender, RoutedEventArgs args)
        {
            OpenFileDialog dialog = new()
            {
                Multiselect = false,
                Filter = ResourceService.ShellMenuResource.GetString("IconFilterCondition"),
                Title = ResourceService.ShellMenuResource.GetString("SelectIcon")
            };
            if (dialog.ShowDialog() is DialogResult.OK && !string.IsNullOrEmpty(dialog.FileName))
            {
                try
                {
                    DefaultIconImage.SetSource(emptyStream);
                    selectedDefaultIconPath = dialog.FileName;
                    DefaultIconPath = Path.Combine(ShellMenuService.ShellMenuConfigDirectory.FullName, editMenuGuid.ToString(), "DefualtIcon.ico");
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
            dialog.Dispose();
        }

        /// <summary>
        /// 浅色主题图标修改
        /// </summary>
        private void OnLightThemeIconBrowserClicked(object sender, RoutedEventArgs args)
        {
            OpenFileDialog dialog = new()
            {
                Multiselect = false,
                Filter = ResourceService.ShellMenuResource.GetString("IconFilterCondition"),
                Title = ResourceService.ShellMenuResource.GetString("SelectIcon")
            };
            if (dialog.ShowDialog() is DialogResult.OK && !string.IsNullOrEmpty(dialog.FileName))
            {
                try
                {
                    LightThemeIconImage.SetSource(emptyStream);
                    selectedLightThemeIconPath = dialog.FileName;
                    LightThemeIconPath = Path.Combine(ShellMenuService.ShellMenuConfigDirectory.FullName, editMenuGuid.ToString(), "LightThemeIcon.ico");
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
            dialog.Dispose();
        }

        /// <summary>
        /// 深色主题图标修改
        /// </summary>
        private void OnDarkThemeIconBrowserClicked(object sender, RoutedEventArgs args)
        {
            OpenFileDialog dialog = new()
            {
                Multiselect = false,
                Filter = ResourceService.ShellMenuResource.GetString("IconFilterCondition"),
                Title = ResourceService.ShellMenuResource.GetString("SelectIcon")
            };
            if (dialog.ShowDialog() is DialogResult.OK && !string.IsNullOrEmpty(dialog.FileName))
            {
                try
                {
                    DarkThemeIconImage.SetSource(emptyStream);
                    selectedDarkThemeIconPath = dialog.FileName;
                    DarkThemeIconPath = Path.Combine(ShellMenuService.ShellMenuConfigDirectory.FullName, editMenuGuid.ToString(), "DarkThemeIcon.ico");
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
            dialog.Dispose();
        }

        /// <summary>
        /// 修改菜单程序文件路径
        /// </summary>
        private void OnMenuProgramPathBrowserClicked(object sender, RoutedEventArgs args)
        {
            OpenFileDialog dialog = new()
            {
                Multiselect = false,
                Filter = ResourceService.ShellMenuResource.GetString("ProgramFilterCondition"),
                Title = ResourceService.ShellMenuResource.GetString("SelectProgram")
            };
            if (dialog.ShowDialog() is DialogResult.OK && !string.IsNullOrEmpty(dialog.FileName))
            {
                MenuProgramPathText = dialog.FileName;
            }
            dialog.Dispose();
        }

        /// <summary>
        /// 菜单参数内容发生更改时的事件
        /// </summary>
        private void OnMenuParameterTextChanged(object sender, TextChangedEventArgs args)
        {
            MenuParameterText = (sender as global::Windows.UI.Xaml.Controls.TextBox).Text;
        }

        /// <summary>
        /// 是否总是需要提权运行修改时触发的事件
        /// </summary>
        private void OnIsAlwaysRunAsAdministratorToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                IsAlwaysRunAsAdministrator = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 修改菜单文件匹配规则
        /// </summary>
        private void OnFileMatchRuleClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is not null)
            {
                SelectedFileMatchRule = FileMatchRuleList[Convert.ToInt32(radioMenuFlyoutItem.Tag)];
                MenuFileMatchFormatText = string.Empty;

                if (SelectedFileMatchRule.Equals(FileMatchRuleList[0]) || SelectedFileMatchRule.Equals(FileMatchRuleList[4]))
                {
                    NeedInputMatchFormat = false;
                    MenuFileMatchFormatPHText = string.Empty;
                }
                else if (SelectedFileMatchRule.Equals(FileMatchRuleList[1]))
                {
                    NeedInputMatchFormat = true;
                    MenuFileMatchFormatPHText = ResourceService.ShellMenuResource.GetString("MenuFileNameFormat");
                }
                else if (SelectedFileMatchRule.Equals(FileMatchRuleList[2]))
                {
                    NeedInputMatchFormat = true;
                    MenuFileMatchFormatPHText = string.Format(ResourceService.ShellMenuResource.GetString("MenuFileNameRegexFormat"), @"[\s\S]+.jpg | [\w\W]*.jpg");
                }
                else if (SelectedFileMatchRule.Equals(FileMatchRuleList[3]))
                {
                    NeedInputMatchFormat = true;
                    MenuFileMatchFormatPHText = ResourceService.ShellMenuResource.GetString("MenuFileExtensionFormat");
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

        #region 第三部分：自定义事件

        /// <summary>
        /// 注册表内容发生变更时触发的事件
        /// </summary>
        private void OnNotifyKeyValueChanged(object sender, EventArgs args)
        {
            if (!isChanger)
            {
                needToRefreshData = true;
            }

            isChanger = false;

            // 注册的变化通知在使用一次后就消失了，需要重新注册
            RegistryHelper.MonitorRegistryValueChange(@"Software\WindowsTools\ShellMenuTest");
        }

        #endregion 第三部分：自定义事件

        #region 第四部分：递归遍历

        /// <summary>
        /// 枚举并递归菜单项信息
        /// </summary>
        private ShellMenuItemModel EnumShellMenuItem(ShellMenuItem menuItem, MenuType menuType)
        {
            // 读取遍历到的当前项的信息
            ShellMenuItemModel shellMenuItem = new()
            {
                MenuKey = menuItem.MenuKey,
                IsSelected = menuType is MenuType.FirstLevelMenu,
                MenuType = menuType,
                MenuTitleText = menuItem.MenuTitleText,
                MenuGuid = menuItem.MenuGuid,
                DefaultIconPath = menuItem.DefaultIconPath,
                LightThemeIconPath = menuItem.LightThemeIconPath,
                DarkThemeIconPath = menuItem.DarkThemeIconPath,
                MenuProgramPathText = menuItem.MenuProgramPath,
                MenuParameter = menuItem.MenuParameter,
                FolderBackground = menuItem.FolderBackground,
                IsAlwaysRunAsAdministrator = menuItem.IsAlwaysRunAsAdministrator,
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

            shellMenuItem.MenuIcon.SetSource(emptyStream);
            if (shellMenuItem.ShouldUseIcon)
            {
                // 使用应用程序图标
                if (shellMenuItem.ShouldUseProgramIcon)
                {
                    try
                    {
                        if (File.Exists(shellMenuItem.MenuProgramPathText) && Path.GetExtension(shellMenuItem.MenuProgramPathText).Equals(".exe"))
                        {
                            Icon icon = Icon.ExtractAssociatedIcon(shellMenuItem.MenuProgramPathText);
                            MemoryStream memoryStream = new();
                            icon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            shellMenuItem.MenuIcon.SetSource(memoryStream.AsRandomAccessStream());
                            memoryStream.Dispose();
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, string.Format("Get program icon {0} failed", shellMenuItem.MenuProgramPathText), e);
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
                                LogService.WriteLog(EventLevel.Error, string.Format("Get light theme icon {0} failed", shellMenuItem.LightThemeIconPath), e);
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
                                LogService.WriteLog(EventLevel.Error, string.Format("Get dark theme icon {0} failed", shellMenuItem.DarkThemeIconPath), e);
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
                                LogService.WriteLog(EventLevel.Error, string.Format("Get default icon {0} failed", shellMenuItem.DefaultIconPath), e);
                            }
                        }
                    }
                }
            }

            // 递归遍历子项
            foreach (ShellMenuItem subMenuItem in menuItem.SubShellMenuItem)
            {
                if (menuType is MenuType.FirstLevelMenu)
                {
                    shellMenuItem.SubMenuItemCollection.Add(EnumShellMenuItem(subMenuItem, MenuType.SecondLevelMenu));
                }
            }

            return shellMenuItem;
        }

        /// <summary>
        /// 删除指定项
        /// </summary>
        private ShellMenuItemModel EnumRemoveItem(ShellMenuItemModel selectedItem, ShellMenuItemModel parentItem, ObservableCollection<ShellMenuItemModel> shellMenuItemCollection)
        {
            bool isRemoved = false;
            ShellMenuItemModel removedParentItem = null;

            // 递归遍历列表，删除遍历到的当前项
            for (int index = 0; index < shellMenuItemCollection.Count; index++)
            {
                // 删除遍历到的当前项
                if (selectedItem is not null && selectedItem.MenuKey.Equals(shellMenuItemCollection[index].MenuKey))
                {
                    shellMenuItemCollection.RemoveAt(index);
                    isRemoved = true;
                    break;
                }
            }

            // 删除当前项后，对列表中已有的项重新排序
            if (isRemoved)
            {
                AutoResetEvent autoResetEvent = new(false);
                for (int index = 0; index < shellMenuItemCollection.Count; index++)
                {
                    shellMenuItemCollection[index].MenuIndex = index;
                }

                Task.Run(() =>
                {
                    // 修改菜单顺序
                    foreach (ShellMenuItemModel shellMenuItem in shellMenuItemCollection)
                    {
                        ShellMenuItem selectedMenuItem = new()
                        {
                            MenuGuid = shellMenuItem.MenuGuid,
                            MenuIndex = shellMenuItem.MenuIndex,
                            MenuTitleText = shellMenuItem.MenuTitleText,
                            ShouldUseIcon = shellMenuItem.ShouldUseIcon,
                            ShouldUseProgramIcon = shellMenuItem.ShouldUseProgramIcon,
                            ShouldUseThemeIcon = shellMenuItem.ShouldUseThemeIcon,
                            DefaultIconPath = shellMenuItem.DefaultIconPath,
                            LightThemeIconPath = shellMenuItem.LightThemeIconPath,
                            DarkThemeIconPath = shellMenuItem.DarkThemeIconPath,
                            MenuProgramPath = shellMenuItem.MenuProgramPathText,
                            MenuParameter = shellMenuItem.MenuParameter,
                            IsAlwaysRunAsAdministrator = shellMenuItem.IsAlwaysRunAsAdministrator,
                            FolderBackground = shellMenuItem.FolderBackground,
                            FolderDesktop = shellMenuItem.FolderDesktop,
                            FolderDirectory = shellMenuItem.FolderDirectory,
                            FolderDrive = shellMenuItem.FolderDrive,
                            MenuFileMatchRule = shellMenuItem.MenuFileMatchRule,
                            MenuFileMatchFormatText = shellMenuItem.MenuFileMatchFormatText,
                        };

                        isChanger = true;

                        // 保存菜单新顺序
                        ShellMenuService.SaveShellMenuItem(shellMenuItem.MenuKey, selectedMenuItem);
                    }

                    autoResetEvent.Set();
                });

                autoResetEvent.WaitOne();
                autoResetEvent.Dispose();
                autoResetEvent = null;

                removedParentItem = parentItem;
                return removedParentItem;
            }

            // 递归遍历子项
            foreach (ShellMenuItemModel subMenuItem in shellMenuItemCollection)
            {
                if (EnumRemoveItem(selectedItem, subMenuItem, subMenuItem.SubMenuItemCollection) is ShellMenuItemModel searchedParentItem)
                {
                    removedParentItem = searchedParentItem;
                }
            }

            return removedParentItem;
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
                    IsAddMenuEnabled = selectedItem.MenuType is MenuType.FirstLevelMenu;

                    if (shellMenuItemCollection.Count is 1)
                    {
                        IsMoveUpEnabled = false;
                        IsMoveDownEnabled = false;
                    }
                    else
                    {
                        // 第一项，不可向上移动
                        if (selectedItem.MenuIndex is 0)
                        {
                            IsMoveUpEnabled = false;
                            IsMoveDownEnabled = true;
                        }
                        // 最后一项，不可向下移动
                        else if (selectedItem.MenuIndex.Equals(shellMenuItemCollection.Count - 1))
                        {
                            IsMoveUpEnabled = true;
                            IsMoveDownEnabled = false;
                        }
                        // 不是首项也不是最后一项，可以向上移动，也可以向下移动
                        else
                        {
                            IsMoveUpEnabled = true;
                            IsMoveDownEnabled = true;
                        }
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
        /// 当应用显示主题发生修改时，枚举并递归修改带主题的菜单项
        /// </summary>
        private void EnumModifyShellMenuItemTheme(ShellMenuItemModel shellMenuItem)
        {
            // 修改遍历到的当前项
            if (shellMenuItem.ShouldUseIcon && !shellMenuItem.ShouldUseProgramIcon && shellMenuItem.ShouldUseThemeIcon)
            {
                shellMenuItem.MenuIcon.SetSource(emptyStream);

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
                            shellMenuItem.MenuIcon.SetSource(memoryStream.AsRandomAccessStream());
                            memoryStream.Dispose();
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, string.Format("Get light theme icon {0} failed", shellMenuItem.LightThemeIconPath), e);
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
                            shellMenuItem.MenuIcon.SetSource(memoryStream.AsRandomAccessStream());
                            memoryStream.Dispose();
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, string.Format("Get dark theme icon {0} failed", shellMenuItem.DarkThemeIconPath), e);
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
                            MenuProgramPath = shellMenuItemCollection[index - 1].MenuProgramPathText,
                            MenuParameter = shellMenuItemCollection[index - 1].MenuParameter,
                            IsAlwaysRunAsAdministrator = shellMenuItemCollection[index - 1].IsAlwaysRunAsAdministrator,
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
                            MenuProgramPath = shellMenuItemCollection[index].MenuProgramPathText,
                            MenuParameter = shellMenuItemCollection[index].MenuParameter,
                            IsAlwaysRunAsAdministrator = shellMenuItemCollection[index].IsAlwaysRunAsAdministrator,
                            FolderBackground = shellMenuItemCollection[index].FolderBackground,
                            FolderDesktop = shellMenuItemCollection[index].FolderDesktop,
                            FolderDirectory = shellMenuItemCollection[index].FolderDirectory,
                            FolderDrive = shellMenuItemCollection[index].FolderDrive,
                            MenuFileMatchRule = shellMenuItemCollection[index].MenuFileMatchRule,
                            MenuFileMatchFormatText = shellMenuItemCollection[index].MenuFileMatchFormatText
                        };

                        isChanger = true;
                        ShellMenuService.SaveShellMenuItem(shellMenuItemCollection[index - 1].MenuKey, swappedMenuItem);
                        ShellMenuService.SaveShellMenuItem(shellMenuItemCollection[index].MenuKey, selectedMenuItem);
                        autoResetEvent.Set();
                    });

                    autoResetEvent.WaitOne();
                    autoResetEvent.Dispose();
                    autoResetEvent = null;

                    // 修改已保存的数据
                    (shellMenuItemCollection[index], shellMenuItemCollection[index - 1]) = (shellMenuItemCollection[index - 1], shellMenuItemCollection[index]);

                    // 上移后是第一项
                    if ((index + 2).Equals(shellMenuItemCollection.Count))
                    {
                        IsMoveUpEnabled = false;
                        IsMoveDownEnabled = true;
                    }
                    break;
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
            for (int index = shellMenuItemCollection.Count - 1; index >= 0; index--)
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
                            MenuProgramPath = shellMenuItemCollection[index + 1].MenuProgramPathText,
                            MenuParameter = shellMenuItemCollection[index + 1].MenuParameter,
                            IsAlwaysRunAsAdministrator = shellMenuItemCollection[index + 1].IsAlwaysRunAsAdministrator,
                            FolderBackground = shellMenuItemCollection[index + 1].FolderBackground,
                            FolderDesktop = shellMenuItemCollection[index + 1].FolderDesktop,
                            FolderDirectory = shellMenuItemCollection[index + 1].FolderDirectory,
                            FolderDrive = shellMenuItemCollection[index + 1].FolderDrive,
                            MenuFileMatchRule = shellMenuItemCollection[index + 1].MenuFileMatchRule,
                            MenuFileMatchFormatText = shellMenuItemCollection[index + 1].MenuFileMatchFormatText
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
                            MenuProgramPath = shellMenuItemCollection[index].MenuProgramPathText,
                            MenuParameter = shellMenuItemCollection[index].MenuParameter,
                            IsAlwaysRunAsAdministrator = shellMenuItemCollection[index].IsAlwaysRunAsAdministrator,
                            FolderBackground = shellMenuItemCollection[index].FolderBackground,
                            FolderDesktop = shellMenuItemCollection[index].FolderDesktop,
                            FolderDirectory = shellMenuItemCollection[index].FolderDirectory,
                            FolderDrive = shellMenuItemCollection[index].FolderDrive,
                            MenuFileMatchRule = shellMenuItemCollection[index].MenuFileMatchRule,
                            MenuFileMatchFormatText = shellMenuItemCollection[index].MenuFileMatchFormatText,
                        };

                        isChanger = true;
                        ShellMenuService.SaveShellMenuItem(shellMenuItemCollection[index + 1].MenuKey, swappedMenuItem);
                        ShellMenuService.SaveShellMenuItem(shellMenuItemCollection[index].MenuKey, selectedMenuItem);
                        autoResetEvent.Set();
                    });

                    autoResetEvent.WaitOne();
                    autoResetEvent.Dispose();
                    autoResetEvent = null;

                    // 修改已保存的数据
                    (shellMenuItemCollection[index], shellMenuItemCollection[index + 1]) = (shellMenuItemCollection[index + 1], shellMenuItemCollection[index]);

                    // 上移后是最后一项
                    if ((index + 2).Equals(shellMenuItemCollection.Count))
                    {
                        IsMoveUpEnabled = true;
                        IsMoveDownEnabled = false;
                    }
                    break;
                }
            }

            // 递归遍历子项
            foreach (ShellMenuItemModel shellMenuItem in shellMenuItemCollection)
            {
                EnumMoveDownShellMenuItem(selectedItem, shellMenuItem.SubMenuItemCollection);
            }
        }

        #endregion 第四部分：递归遍历
    }
}
