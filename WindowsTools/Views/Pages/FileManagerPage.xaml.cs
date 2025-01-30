using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using WindowsTools.Models;
using WindowsTools.Services.Root;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace WindowsTools.Views.Pages
{
    /// <summary>
    /// 文件管理页面
    /// </summary>
    public sealed partial class FileManagerPage : Page, INotifyPropertyChanged
    {
        private bool isLoaded;
        private readonly SlideNavigationTransitionInfo slideNavigationTransitionInfo = new() { Effect = SlideNavigationTransitionEffect.FromRight };

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

        private List<KeyValuePair<string, Type>> PageList { get; } =
        [
            new KeyValuePair<string, Type>("FileName",typeof(FileNamePage)),
            new KeyValuePair<string, Type>("ExtensionName", typeof(ExtensionNamePage)),
            new KeyValuePair<string, Type>("UpperAndLowerCase", typeof(UpperAndLowerCasePage)),
            new KeyValuePair<string, Type>("FileProperties", typeof(FilePropertiesPage)),
        ];

        public List<NavigationModel> NavigationItemList { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public FileManagerPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 导航控件加载完成后初始化内容，初始化导航控件属性和应用的背景色
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            if (!isLoaded)
            {
                isLoaded = true;
                if (sender is Microsoft.UI.Xaml.Controls.NavigationView navigationView)
                {
                    foreach (object item in navigationView.MenuItems)
                    {
                        if (item is Microsoft.UI.Xaml.Controls.NavigationViewItem navigationViewItem && navigationViewItem.Tag is not null)
                        {
                            int tagIndex = PageList.FindIndex(item => item.Key.Equals(navigationViewItem.Tag));

                            NavigationItemList.Add(new NavigationModel()
                            {
                                NavigationTag = PageList[tagIndex].Key,
                                NavigationItem = navigationViewItem,
                                NavigationPage = PageList[tagIndex].Value,
                            });
                        }
                    }
                }

                SelectedItem = NavigationItemList[0].NavigationItem;
                NavigateTo(typeof(FileNamePage));
            }
        }

        /// <summary>
        /// 当菜单中的项收到交互（如单击或点击）时发生
        /// </summary>
        private void OnItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            if (args.InvokedItemContainer is Microsoft.UI.Xaml.Controls.NavigationViewItemBase navigationViewItem && navigationViewItem.Tag is not null)
            {
                NavigationModel navigationItem = NavigationItemList.Find(item => item.NavigationTag.Equals(navigationViewItem.Tag.ToString(), StringComparison.OrdinalIgnoreCase));

                if (navigationItem.NavigationPage is not null && SelectedItem != navigationItem.NavigationItem)
                {
                    NavigateTo(navigationItem.NavigationPage);
                }
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
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, string.Format(ResourceService.WindowResource.GetString("NavigationFailed"), args.SourcePageType.FullName), e);
            }
        }

        /// <summary>
        /// 导航失败时发生
        /// </summary>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs args)
        {
            args.Handled = true;
            LogService.WriteLog(EventLevel.Warning, string.Format(ResourceService.WindowResource.GetString("NavigationFailed"), args.SourcePageType.FullName), args.Exception);
            (Application.Current as App).Dispose();
        }

        /// <summary>
        /// 页面向前导航
        /// </summary>
        private void NavigateTo(Type navigationPageType, object parameter = null)
        {
            try
            {
                if (NavigationItemList.Find(item => item.NavigationPage == navigationPageType) is NavigationModel navigationItem)
                {
                    // 导航到该项目对应的页面
                    (FileManagerNavigationView.Content as Frame).Navigate(navigationItem.NavigationPage, parameter, slideNavigationTransitionInfo);
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, string.Format(ResourceService.WindowResource.GetString("NavigationFailed"), navigationPageType.FullName), e);
            }
        }

        /// <summary>
        /// 获取当前导航到的页
        /// </summary>
        public Type GetCurrentPageType()
        {
            return (FileManagerNavigationView.Content as Frame).CurrentSourcePageType;
        }
    }
}
