using FileRenamer.Contracts;
using FileRenamer.Extensions.Command;
using FileRenamer.Models.Window;
using FileRenamer.Services.Root;
using FileRenamer.Services.Window;
using FileRenamer.ViewModels.Base;
using FileRenamer.Views.Pages;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace FileRenamer.ViewModels.Pages
{
    public sealed class MainViewModel : ViewModelBase
    {
        private ElementTheme _windowTheme;

        public ElementTheme WindowTheme
        {
            get { return _windowTheme; }

            set
            {
                _windowTheme = value;
                OnPropertyChanged();
            }
        }

        private bool _isBackEnabled;

        public bool IsBackEnabled
        {
            get { return _isBackEnabled; }

            set
            {
                _isBackEnabled = value;
                OnPropertyChanged();
            }
        }

        private NavigationViewItem _selectedItem;

        public NavigationViewItem SelectedItem
        {
            get { return _selectedItem; }

            set
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }

        private Dictionary<string, Type> PageDict { get; } = new Dictionary<string, Type>()
        {
            {"FileName",typeof(FileNamePage) },
            {"ExtensionName",typeof(ExtensionNamePage) },
            {"UpperAndLowerCase",typeof(UpperAndLowerCasePage) },
            {"FileProperties",typeof(FilePropertiesPage) },
            {"About",typeof(AboutPage) },
            {"Settings",typeof(SettingsPage) }
        };

        public List<string> TagList { get; } = new List<string>()
        {
            "FileName",
            "ExtensionName",
            "UpperAndLowerCase",
            "FileProperties",
            "About",
            "Settings"
        };

        // 当菜单中的项收到交互（如单击或点击）时发生
        public IRelayCommand NavigationItemCommand => new RelayCommand<object>((invokedItemTag) =>
        {
            if (invokedItemTag is not null)
            {
                NavigationModel navigationViewItem = NavigationService.NavigationItemList.Find(item => item.NavigationTag == Convert.ToString(invokedItemTag));
                if (SelectedItem != navigationViewItem.NavigationItem)
                {
                    NavigationService.NavigateTo(navigationViewItem.NavigationPage);
                }
            }
        });

        /// <summary>
        /// 当后退按钮收到交互（如单击或点击）时发生
        /// </summary>
        public void OnNavigationViewBackRequested(object sender, NavigationViewBackRequestedEventArgs args)
        {
            NavigationService.NavigationFrom();
        }

        public void OnNavigationViewLoaded(object sender, RoutedEventArgs args)
        {
            if (sender is not NavigationView navigationView)
            {
                return;
            }

            foreach (object item in navigationView.MenuItems)
            {
                NavigationViewItem navigationViewItem = item as NavigationViewItem;
                if (navigationViewItem is not null)
                {
                    string Tag = Convert.ToString(navigationViewItem.Tag);

                    NavigationService.NavigationItemList.Add(new NavigationModel()
                    {
                        NavigationTag = Tag,
                        NavigationItem = navigationViewItem,
                        NavigationPage = PageDict[Tag],
                    });
                }
            }

            SelectedItem = NavigationService.NavigationItemList[0].NavigationItem;
            NavigationService.NavigateTo(typeof(FileNamePage));
            IsBackEnabled = NavigationService.CanGoBack();
        }

        /// <summary>
        /// 导航完成后发生
        /// </summary>
        public void OnFrameNavigated(object sender, NavigationEventArgs args)
        {
            Type CurrentPageType = NavigationService.GetCurrentPageType();
            SelectedItem = NavigationService.NavigationItemList.Find(item => item.NavigationPage == CurrentPageType).NavigationItem;
            IsBackEnabled = NavigationService.CanGoBack();
        }

        /// <summary>
        /// 导航失败时发生
        /// </summary>
        public void OnFrameNavgationFailed(object sender, NavigationFailedEventArgs args)
        {
            throw new ApplicationException(string.Format(ResourceService.GetLocalized("Window/NavigationFailed"), args.SourcePageType.FullName));
        }
    }
}
