using FileRenamer.Models;
using FileRenamer.Services.Controls.Settings.Appearance;
using FileRenamer.Services.Root;
using FileRenamer.ViewModels.Base;
using FileRenamer.WindowsAPI.PInvoke.DwmApi;
using FileRenamer.WindowsAPI.PInvoke.User32;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FileRenamer.ViewModels.Dialogs
{
    /// <summary>
    /// 应用改名示例视图模型
    /// </summary>
    public class NameChangeViewModel : ViewModelBase
    {
        private int _currentIndex = 0;

        public int CurrentIndex
        {
            get { return _currentIndex; }

            set
            {
                _currentIndex = value;
                OnPropertyChanged();
            }
        }

        public IntPtr WindowHandle { get; set; } = IntPtr.Zero;

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

        public List<OldAndNewNameModel> NameChangeList { get; } = new List<OldAndNewNameModel>()
        {
            new OldAndNewNameModel(){ OriginalFileName = string.Empty, NewFileName = string.Empty },
            new OldAndNewNameModel(){ OriginalFileName = string.Empty, NewFileName = string.Empty },
            new OldAndNewNameModel(){ OriginalFileName = string.Empty, NewFileName = string.Empty },
            new OldAndNewNameModel(){ OriginalFileName = string.Empty, NewFileName = string.Empty },
        };

        public List<string> NameChangeRuleList { get; } = new List<string>()
        {
            ResourceService.GetLocalized("Dialog/NameChangeRule1"),
            ResourceService.GetLocalized("Dialog/NameChangeRule2"),
            ResourceService.GetLocalized("Dialog/NameChangeRule3"),
            ResourceService.GetLocalized("Dialog/NameChangeRule4"),
        };

        public Dictionary<int, List<OldAndNewNameModel>> NameChangeDict = new Dictionary<int, List<OldAndNewNameModel>>()
        {
            { 0, new List<OldAndNewNameModel>()
                {
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("Dialog/NameChangeOriginalName1"), NewFileName = ResourceService.GetLocalized("Dialog/NameChangeList1ChangedName1") },
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("Dialog/NameChangeOriginalName2"), NewFileName = ResourceService.GetLocalized("Dialog/NameChangeList1ChangedName2") },
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("Dialog/NameChangeOriginalName3"), NewFileName = ResourceService.GetLocalized("Dialog/NameChangeList1ChangedName3") },
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("Dialog/NameChangeOriginalName4"), NewFileName = ResourceService.GetLocalized("Dialog/NameChangeList1ChangedName4") },
                }
            },
            { 1, new List<OldAndNewNameModel>()
                {
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("Dialog/NameChangeOriginalName1"), NewFileName = ResourceService.GetLocalized("Dialog/NameChangeList2ChangedName1") },
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("Dialog/NameChangeOriginalName2"), NewFileName = ResourceService.GetLocalized("Dialog/NameChangeList2ChangedName2") },
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("Dialog/NameChangeOriginalName3"), NewFileName = ResourceService.GetLocalized("Dialog/NameChangeList2ChangedName3") },
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("Dialog/NameChangeOriginalName4"), NewFileName = ResourceService.GetLocalized("Dialog/NameChangeList2ChangedName4") },
                }
            },
            { 2, new List<OldAndNewNameModel>()
                {
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("Dialog/NameChangeOriginalName1"), NewFileName = ResourceService.GetLocalized("Dialog/NameChangeList3ChangedName1") },
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("Dialog/NameChangeOriginalName2"), NewFileName = ResourceService.GetLocalized("Dialog/NameChangeList3ChangedName2") },
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("Dialog/NameChangeOriginalName3"), NewFileName = ResourceService.GetLocalized("Dialog/NameChangeList3ChangedName3") },
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("Dialog/NameChangeOriginalName4"), NewFileName = ResourceService.GetLocalized("Dialog/NameChangeList3ChangedName4") },
                }
            },
            { 3, new List<OldAndNewNameModel>()
                {
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("Dialog/NameChangeOriginalName1"), NewFileName = ResourceService.GetLocalized("Dialog/NameChangeList4ChangedName1") },
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("Dialog/NameChangeOriginalName2"), NewFileName = ResourceService.GetLocalized("Dialog/NameChangeList4ChangedName2") },
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("Dialog/NameChangeOriginalName3"), NewFileName = ResourceService.GetLocalized("Dialog/NameChangeList4ChangedName3") },
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("Dialog/NameChangeOriginalName4"), NewFileName = ResourceService.GetLocalized("Dialog/NameChangeList4ChangedName4") },
                }
            },
        };

        public NameChangeViewModel()
        {
            CurrentIndex = 0;

            for (int index = 0; index < NameChangeList.Count; index++)
            {
                NameChangeList[index].OriginalFileName = NameChangeDict[CurrentIndex][index].OriginalFileName;
                NameChangeList[index].NewFileName = NameChangeDict[CurrentIndex][index].NewFileName;
            }
        }

        /// <summary>
        /// 窗口关闭
        /// </summary>
        public void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            User32Library.SendMessage(WindowHandle, WindowMessage.WM_CLOSE, 0, IntPtr.Zero);
        }

        /// <summary>
        /// 向前导航
        /// </summary>
        public void OnForwardNavigateClicked(object sender, RoutedEventArgs args)
        {
            CurrentIndex = CurrentIndex == 0 ? 3 : CurrentIndex - 1;

            for (int index = 0; index < NameChangeList.Count; index++)
            {
                NameChangeList[index].OriginalFileName = NameChangeDict[CurrentIndex][index].OriginalFileName;
                NameChangeList[index].NewFileName = NameChangeDict[CurrentIndex][index].NewFileName;
            }
        }

        /// <summary>
        /// 窗口移动
        /// </summary>
        public async void OnMoveClicked(object sender, RoutedEventArgs args)
        {
            MenuFlyoutItem menuItem = sender as MenuFlyoutItem;
            if (menuItem.Tag is not null)
            {
                ((Flyout)menuItem.Tag).Hide();
                await Task.Delay(10);
                User32Library.SendMessage(WindowHandle, WindowMessage.WM_SYSCOMMAND, 0xF010, IntPtr.Zero);
            }
        }

        /// <summary>
        /// 向后导航
        /// </summary>
        public void OnNextNavigateClicked(object sender, RoutedEventArgs args)
        {
            CurrentIndex = CurrentIndex == 3 ? 0 : CurrentIndex + 1;

            for (int index = 0; index < NameChangeList.Count; index++)
            {
                NameChangeList[index].OriginalFileName = NameChangeDict[CurrentIndex][index].OriginalFileName;
                NameChangeList[index].NewFileName = NameChangeDict[CurrentIndex][index].NewFileName;
            }
        }

        /// <summary>
        /// 设置当前窗口的主题色
        /// </summary>
        public void SetAppTheme()
        {
            WindowTheme = (ElementTheme)Enum.Parse(typeof(ElementTheme), ThemeService.AppTheme.InternalName);
            if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[0].InternalName)
            {
                if (Application.Current.RequestedTheme is ApplicationTheme.Light)
                {
                    int useLightMode = 0;
                    DwmApiLibrary.DwmSetWindowAttribute(WindowHandle, DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, ref useLightMode, Marshal.SizeOf(typeof(int)));
                }
                else
                {
                    int useDarkMode = 1;
                    DwmApiLibrary.DwmSetWindowAttribute(WindowHandle, DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, ref useDarkMode, Marshal.SizeOf(typeof(int)));
                }
            }
            if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[1].InternalName)
            {
                int useLightMode = 0;
                DwmApiLibrary.DwmSetWindowAttribute(WindowHandle, DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, ref useLightMode, Marshal.SizeOf(typeof(int)));
            }
            else if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[2].InternalName)
            {
                int useDarkMode = 1;
                DwmApiLibrary.DwmSetWindowAttribute(WindowHandle, DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, ref useDarkMode, Marshal.SizeOf(typeof(int)));
            }
        }
    }
}
