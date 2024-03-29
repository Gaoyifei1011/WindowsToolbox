﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WindowsTools.Helpers.Root;
using WindowsTools.Models;
using WindowsTools.Services.Controls.Settings;
using WindowsTools.Services.Root;
using WindowsTools.Views.Windows;
using WindowsTools.WindowsAPI.PInvoke.User32;

namespace WindowsTools.Views.Pages
{
    /// <summary>
    /// 应用主窗口页面
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        private ElementTheme _windowTheme;

        public ElementTheme WindowTheme
        {
            get { return _windowTheme; }

            set
            {
                if (!Equals(_windowTheme, value))
                {
                    _windowTheme = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WindowTheme)));
                }
            }
        }

        private bool _isBackEnabled;

        public bool IsBackEnabled
        {
            get { return _isBackEnabled; }

            set
            {
                if (!Equals(_isBackEnabled, value))
                {
                    _isBackEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsBackEnabled)));
                }
            }
        }

        private bool _isWindowMaximized;

        public bool IsWindowMaximized
        {
            get { return _isWindowMaximized; }

            set
            {
                if (!Equals(_isWindowMaximized, value))
                {
                    _isWindowMaximized = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsWindowMaximized)));
                }
            }
        }

        private Microsoft.UI.Xaml.Controls.NavigationViewItem _selectedItem;

        public Microsoft.UI.Xaml.Controls.NavigationViewItem SelectedItem
        {
            get { return _selectedItem; }

            set
            {
                if (!Equals(_selectedItem, value))
                {
                    _selectedItem = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedItem)));
                }
            }
        }

        private List<KeyValuePair<string, Type>> PageList { get; } = new List<KeyValuePair<string, Type>>()
        {
            new KeyValuePair<string, Type>("AllTools",typeof(AllToolsPage)),
            new KeyValuePair<string, Type>("Relaxation", null),
            new KeyValuePair<string, Type>("Loaf", typeof(LoafPage)),
            new KeyValuePair<string, Type>("File",null),
            new KeyValuePair<string, Type>("FileName",typeof(FileNamePage)),
            new KeyValuePair<string, Type>("ExtensionName", typeof(ExtensionNamePage)),
            new KeyValuePair<string, Type>("UpperAndLowerCase", typeof(UpperAndLowerCasePage)),
            new KeyValuePair<string, Type>("FileProperties", typeof(FilePropertiesPage)),
            new KeyValuePair<string, Type>("FileCertificate",typeof(FileCertificatePage)),
            new KeyValuePair<string, Type>("Resource",null),
            new KeyValuePair<string, Type>("IconExtract",typeof(IconExtractPage)),
            new KeyValuePair<string, Type>("PriExtract",typeof(PriExtractPage)),
            new KeyValuePair<string, Type>("Personalize",null),
            new KeyValuePair<string, Type>("ShellMenu",typeof(ShellMenuPage)),
            new KeyValuePair<string, Type>("System",null),
            new KeyValuePair<string, Type>("SystemInfo",typeof(SystemInfoPage)),
            new KeyValuePair<string, Type>("DriverManager",typeof(DriverManagerPage)),
            new KeyValuePair<string, Type>("UpdateManager",typeof(UpdateManagerPage)),
            new KeyValuePair<string, Type>("WinSAT",typeof(WinSATPage)),
            new KeyValuePair<string, Type>("About", typeof(AboutPage)),
            new KeyValuePair<string, Type>("Settings",typeof(SettingsPage)),
        };

        public List<NavigationModel> NavigationItemList { get; } = new List<NavigationModel>();

        public event PropertyChangedEventHandler PropertyChanged;

        public MainPage()
        {
            InitializeComponent();
        }

        #region 第一部分：重写父类事件

        /// <summary>
        /// 按下 Alt + Space 键时，导航控件返回到上一页
        /// </summary>
        protected override void OnKeyDown(KeyRoutedEventArgs args)
        {
            if (args.Key is VirtualKey.Back && args.KeyStatus.IsMenuKeyDown)
            {
                NavigationFrom();
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：窗口内容挂载的事件

        /// <summary>
        /// 应用主题发生变化时修改应用的背景色
        /// </summary>
        private void OnActualThemeChanged(FrameworkElement sender, object args)
        {
            if (BackdropService.AppBackdrop.Value.Equals(BackdropService.BackdropList[0].Value))
            {
                if (sender.ActualTheme is ElementTheme.Light)
                {
                    Background = new SolidColorBrush(Color.FromArgb(255, 240, 243, 249));
                }
                else
                {
                    Background = new SolidColorBrush(Color.FromArgb(255, 20, 20, 20));
                }
            }

            MainWindow.Current.SetTitleBarColor(sender.ActualTheme);
        }

        #endregion 第二部分：窗口内容挂载的事件

        #region 第三部分：窗口右键菜单事件

        /// <summary>
        /// 窗口还原
        /// </summary>
        private void OnRestoreClicked(object sender, RoutedEventArgs args)
        {
            MainWindow.Current.WindowState = FormWindowState.Normal;
        }

        /// <summary>
        /// 窗口移动
        /// </summary>
        private async void OnMoveClicked(object sender, RoutedEventArgs args)
        {
            MenuFlyoutItem menuItem = sender as MenuFlyoutItem;
            if (menuItem.Tag is not null)
            {
                ((MenuFlyout)menuItem.Tag).Hide();
                await Task.Delay(10);
                User32Library.SendMessage(MainWindow.Current.Handle, WindowMessage.WM_SYSCOMMAND, 0xF010, IntPtr.Zero);
            }
        }

        /// <summary>
        /// 窗口大小
        /// </summary>
        private void OnSizeClicked(object sender, RoutedEventArgs args)
        {
            MenuFlyoutItem menuItem = sender as MenuFlyoutItem;
            if (menuItem.Tag is not null)
            {
                ((MenuFlyout)menuItem.Tag).Hide();
                User32Library.SendMessage(MainWindow.Current.Handle, WindowMessage.WM_SYSCOMMAND, 0xF000, IntPtr.Zero);
            }
        }

        /// <summary>
        /// 窗口最小化
        /// </summary>
        private void OnMinimizeClicked(object sender, RoutedEventArgs args)
        {
            MainWindow.Current.WindowState = FormWindowState.Minimized;
        }

        /// <summary>
        /// 窗口最大化
        /// </summary>
        private void OnMaximizeClicked(object sender, RoutedEventArgs args)
        {
            MainWindow.Current.WindowState = FormWindowState.Maximized;
        }

        /// <summary>
        /// 窗口关闭
        /// </summary>
        private void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            MainWindow.Current.Close();
        }

        #endregion 第三部分：窗口右键菜单事件

        #region 第四部分：导航控件及其内容挂载的事件

        /// <summary>
        /// 导航控件加载完成后初始化内容，初始化导航控件属性和应用的背景色
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            Microsoft.UI.Xaml.Controls.NavigationView navigationView = sender as Microsoft.UI.Xaml.Controls.NavigationView;
            if (navigationView is not null)
            {
                foreach (object item in navigationView.MenuItems)
                {
                    Microsoft.UI.Xaml.Controls.NavigationViewItem navigationViewItem = item as Microsoft.UI.Xaml.Controls.NavigationViewItem;
                    if (navigationViewItem is not null)
                    {
                        int TagIndex = Convert.ToInt32(navigationViewItem.Tag);

                        NavigationItemList.Add(new NavigationModel()
                        {
                            NavigationTag = PageList[TagIndex].Key,
                            NavigationItem = navigationViewItem,
                            NavigationPage = PageList[TagIndex].Value,
                            ParentTag = null
                        });

                        if (navigationViewItem.MenuItems.Count > 0)
                        {
                            foreach (object subItem in navigationViewItem.MenuItems)
                            {
                                Microsoft.UI.Xaml.Controls.NavigationViewItem subNavigationViewItem = subItem as Microsoft.UI.Xaml.Controls.NavigationViewItem;
                                if (subNavigationViewItem is not null)
                                {
                                    int subTagIndex = Convert.ToInt32(subNavigationViewItem.Tag);

                                    NavigationItemList.Add(new NavigationModel()
                                    {
                                        NavigationTag = PageList[subTagIndex].Key,
                                        NavigationItem = subNavigationViewItem,
                                        NavigationPage = PageList[subTagIndex].Value,
                                        ParentTag = PageList[TagIndex].Key
                                    });
                                }
                            }
                        }
                    }
                }

                foreach (object item in navigationView.FooterMenuItems)
                {
                    Microsoft.UI.Xaml.Controls.NavigationViewItem navigationViewItem = item as Microsoft.UI.Xaml.Controls.NavigationViewItem;
                    if (navigationViewItem is not null)
                    {
                        int TagIndex = Convert.ToInt32(navigationViewItem.Tag);

                        NavigationItemList.Add(new NavigationModel()
                        {
                            NavigationTag = PageList[TagIndex].Key,
                            NavigationItem = navigationViewItem,
                            NavigationPage = PageList[TagIndex].Value,
                        });
                    }
                }

                SelectedItem = NavigationItemList[0].NavigationItem;
                NavigateTo(typeof(AllToolsPage));
                IsBackEnabled = CanGoBack();
            }
        }

        /// <summary>
        /// 当后退按钮收到交互（如单击或点击）时发生
        /// </summary>
        private void OnBackRequested(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs args)
        {
            NavigationFrom();
        }

        /// <summary>
        /// 当菜单中的项收到交互（如单击或点击）时发生
        /// </summary>
        private void OnItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            Microsoft.UI.Xaml.Controls.NavigationViewItemBase navigationViewItem = args.InvokedItemContainer;
            if (navigationViewItem.Tag is not null)
            {
                NavigationModel navigationItem = NavigationItemList.Find(item => item.NavigationTag.Equals(PageList[Convert.ToInt32(navigationViewItem.Tag)].Key, StringComparison.OrdinalIgnoreCase));

                if (navigationItem.NavigationPage is not null && SelectedItem != navigationItem.NavigationItem)
                {
                    NavigateTo(navigationItem.NavigationPage);
                }
            }
        }

        /// <summary>
        /// 当树中的节点开始展开时发生时的事件
        /// </summary>
        private void OnExpanding(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemExpandingEventArgs args)
        {
            if (sender.SelectedItem != SelectedItem && SelectedItem.Tag is not "0")
            {
                sender.SelectedItem = SelectedItem;
            }
        }

        /// <summary>
        /// 导航完成后发生
        /// </summary>
        private void OnNavigated(object sender, NavigationEventArgs args)
        {
            try
            {
                Type currentPageType = GetCurrentPageType();
                foreach (NavigationModel navigationItem in NavigationItemList)
                {
                    if (navigationItem.NavigationPage is not null && navigationItem.NavigationPage.Equals(currentPageType))
                    {
                        SelectedItem = navigationItem.NavigationItem;
                        IsBackEnabled = CanGoBack();
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLogEntryType.Error, string.Format(Strings.Window.NavigationFailed, args.SourcePageType.FullName), e);
            }
        }

        /// <summary>
        /// 导航失败时发生
        /// </summary>
        private void OnNavgationFailed(object sender, NavigationFailedEventArgs args)
        {
            throw new ApplicationException(string.Format(Strings.Window.NavigationFailed, args.SourcePageType.FullName));
        }

        #endregion 第四部分：导航控件及其内容挂载的事件

        #region 第五部分：窗口导航方法

        /// <summary>
        /// 页面向前导航
        /// </summary>
        public void NavigateTo(Type navigationPageType, object parameter = null)
        {
            try
            {
                NavigationModel navigationItem = NavigationItemList.Find(item => item.NavigationPage == navigationPageType);

                if (navigationItem is not null)
                {
                    // 如果点击的是子项，而父项没有展开，则自动展开父项中所有的子项
                    if (navigationItem.ParentTag is not null)
                    {
                        // 查找父项
                        NavigationModel parentNavigationItem = NavigationItemList.Find(item => item.NavigationTag.Equals(navigationItem.ParentTag, StringComparison.OrdinalIgnoreCase));

                        // 展开父项
                        if (parentNavigationItem is not null)
                        {
                            MainNavigationView.Expand(parentNavigationItem.NavigationItem);
                        }
                    }

                    // 导航到该项目对应的页面
                    (MainNavigationView.Content as Frame).Navigate(navigationItem.NavigationPage, parameter);
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLogEntryType.Error, string.Format(Strings.Window.NavigationFailed, navigationPageType.FullName), e);
            }
        }

        /// <summary>
        /// 页面向后导航
        /// </summary>
        public void NavigationFrom()
        {
            if ((MainNavigationView.Content as Frame).CanGoBack)
            {
                // 在向后导航前，如果向后导航选中的是子项，而父项没有展开，则自动展开父项中所有的子项
                try
                {
                    NavigationModel navigationItem = NavigationItemList.Find(item => item.NavigationPage == (MainNavigationView.Content as Frame).BackStack.Last().SourcePageType);

                    if (navigationItem is not null)
                    {
                        // 如果点击的是子项，而父项没有展开，则自动展开父项中所有的子项
                        if (navigationItem.ParentTag is not null)
                        {
                            // 查找父项
                            NavigationModel parentNavigationItem = NavigationItemList.Find(item => item.NavigationTag.Equals(navigationItem.ParentTag, StringComparison.OrdinalIgnoreCase));

                            // 展开父项
                            if (parentNavigationItem is not null)
                            {
                                MainNavigationView.Expand(parentNavigationItem.NavigationItem);
                            }
                        }
                    }
                }
                catch (Exception) { }

                (MainNavigationView.Content as Frame).GoBack();
            }
        }

        /// <summary>
        /// 获取当前导航到的页
        /// </summary>
        public Type GetCurrentPageType()
        {
            return (MainNavigationView.Content as Frame).CurrentSourcePageType;
        }

        /// <summary>
        /// 检查当前页面是否能向后导航
        /// </summary>
        public bool CanGoBack()
        {
            return (MainNavigationView.Content as Frame).CanGoBack;
        }

        #endregion 第五部分：窗口导航方法

        /// <summary>
        /// 将提权模式下拖放获得到的文件列表发送到各个页面
        /// </summary>
        public void SendReceivedFilesList(List<string> filesList)
        {
            Type currentPageType = GetCurrentPageType();
            if (currentPageType.Equals(typeof(FileNamePage)))
            {
                FileNamePage page = (MainNavigationView.Content as Frame).Content as FileNamePage;

                Task.Run(() =>
                {
                    List<OldAndNewNameModel> fileNameList = new List<OldAndNewNameModel>();

                    foreach (string file in filesList)
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        if ((fileInfo.Attributes & FileAttributes.Hidden) is FileAttributes.Hidden)
                        {
                            continue;
                        }

                        fileNameList.Add(new OldAndNewNameModel()
                        {
                            OriginalFileName = Path.GetFileName(file),
                            OriginalFilePath = file,
                        });
                    }

                    page.AddToFileNamePage(fileNameList);
                });
            }
            else if (currentPageType.Equals(typeof(ExtensionNamePage)))
            {
                ExtensionNamePage page = (MainNavigationView.Content as Frame).Content as ExtensionNamePage;

                Task.Run(() =>
                {
                    List<OldAndNewNameModel> extensionNameList = new List<OldAndNewNameModel>();

                    foreach (string file in filesList)
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        if ((fileInfo.Attributes & FileAttributes.Hidden) is FileAttributes.Hidden)
                        {
                            continue;
                        }

                        if (!IOHelper.IsDir(fileInfo.FullName))
                        {
                            extensionNameList.Add(new OldAndNewNameModel()
                            {
                                OriginalFileName = fileInfo.Name,
                                OriginalFilePath = fileInfo.FullName
                            });
                        }
                    }

                    page.AddToExtensionNamePage(extensionNameList);
                });
            }
            else if (currentPageType.Equals(typeof(UpperAndLowerCasePage)))
            {
                UpperAndLowerCasePage page = (MainNavigationView.Content as Frame).Content as UpperAndLowerCasePage;

                Task.Run(() =>
                {
                    List<OldAndNewNameModel> upperAndLowerCaseList = new List<OldAndNewNameModel>();

                    foreach (string file in filesList)
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        if ((fileInfo.Attributes & FileAttributes.Hidden) is FileAttributes.Hidden)
                        {
                            continue;
                        }

                        upperAndLowerCaseList.Add(new OldAndNewNameModel()
                        {
                            OriginalFileName = Path.GetFileName(file),
                            OriginalFilePath = file,
                        });
                    }

                    page.AddtoUpperAndLowerCasePage(upperAndLowerCaseList);
                });
            }
            else if (currentPageType.Equals(typeof(FilePropertiesPage)))
            {
                FilePropertiesPage page = (MainNavigationView.Content as Frame).Content as FilePropertiesPage;

                Task.Run(() =>
                {
                    List<OldAndNewPropertiesModel> filePropertiesList = new List<OldAndNewPropertiesModel>();

                    foreach (string file in filesList)
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        if ((fileInfo.Attributes & FileAttributes.Hidden) is FileAttributes.Hidden)
                        {
                            continue;
                        }

                        filePropertiesList.Add(new OldAndNewPropertiesModel()
                        {
                            FileName = Path.GetFileName(file),
                            FilePath = file,
                        });
                    }

                    page.AddToFilePropertiesPage(filePropertiesList);
                });
            }
            else if (currentPageType.Equals(typeof(FileCertificatePage)))
            {
                FileCertificatePage page = (MainNavigationView.Content as Frame).Content as FileCertificatePage;

                Task.Run(() =>
                {
                    List<CertificateResultModel> fileCertificateList = new List<CertificateResultModel>();

                    foreach (string file in filesList)
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        if ((fileInfo.Attributes & FileAttributes.Hidden) is FileAttributes.Hidden)
                        {
                            continue;
                        }

                        if (!IOHelper.IsDir(fileInfo.FullName))
                        {
                            fileCertificateList.Add(new CertificateResultModel()
                            {
                                FileName = fileInfo.Name,
                                FilePath = fileInfo.FullName
                            });
                        }
                    }

                    page.AddToFileCertificatePage(fileCertificateList);
                });
            }
            else if (currentPageType.Equals(typeof(IconExtractPage)))
            {
                IconExtractPage page = (MainNavigationView.Content as Frame).Content as IconExtractPage;
                if (filesList.Count is 1 && (Path.GetExtension(filesList[0]).Equals(".exe") || Path.GetExtension(filesList[0]).Equals(".dll")))
                {
                    page.ParseIconFile(filesList[0]);
                }
            }
            else if (currentPageType.Equals(typeof(PriExtractPage)))
            {
                PriExtractPage page = (MainNavigationView.Content as Frame).Content as PriExtractPage;
                if (filesList.Count is 1 && Path.GetExtension(filesList[0]).Equals(".pri"))
                {
                    page.ParseResourceFile(filesList[0]);
                }
            }
        }
    }
}
