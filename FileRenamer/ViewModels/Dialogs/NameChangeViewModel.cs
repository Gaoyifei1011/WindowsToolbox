using FileRenamer.Models;
using FileRenamer.Services.Controls.Settings.Appearance;
using FileRenamer.Services.Root;
using FileRenamer.ViewModels.Base;
using FileRenamer.WindowsAPI.PInvoke.DwmApi;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Windows.UI.Xaml;

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
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("Dialog/NameChangeOriginalFileName1"), NewFileName = ResourceService.GetLocalized("Dialog/NameChangeList1NewFileName1") },
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("Dialog/NameChangeOriginalFileName2"), NewFileName = ResourceService.GetLocalized("Dialog/NameChangeList1NewFileName2") },
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("Dialog/NameChangeOriginalFileName3"), NewFileName = ResourceService.GetLocalized("Dialog/NameChangeList1NewFileName3") },
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("Dialog/NameChangeOriginalFileName4"), NewFileName = ResourceService.GetLocalized("Dialog/NameChangeList1NewFileName4") },
                }
            },
            { 1, new List<OldAndNewNameModel>()
                {
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("Dialog/NameChangeOriginalFileName1"), NewFileName = ResourceService.GetLocalized("Dialog/NameChangeList2NewFileName1") },
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("Dialog/NameChangeOriginalFileName2"), NewFileName = ResourceService.GetLocalized("Dialog/NameChangeList2NewFileName2") },
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("Dialog/NameChangeOriginalFileName3"), NewFileName = ResourceService.GetLocalized("Dialog/NameChangeList2NewFileName3") },
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("Dialog/NameChangeOriginalFileName4"), NewFileName = ResourceService.GetLocalized("Dialog/NameChangeList2NewFileName4") },
                }
            },
            { 2, new List<OldAndNewNameModel>()
                {
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("Dialog/NameChangeOriginalFileName1"), NewFileName = ResourceService.GetLocalized("Dialog/NameChangeList3NewFileName1") },
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("Dialog/NameChangeOriginalFileName2"), NewFileName = ResourceService.GetLocalized("Dialog/NameChangeList3NewFileName2") },
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("Dialog/NameChangeOriginalFileName3"), NewFileName = ResourceService.GetLocalized("Dialog/NameChangeList3NewFileName3") },
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("Dialog/NameChangeOriginalFileName4"), NewFileName = ResourceService.GetLocalized("Dialog/NameChangeList3NewFileName4") },
                }
            },
            { 3, new List<OldAndNewNameModel>()
                {
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("Dialog/NameChangeOriginalFileName1"), NewFileName = ResourceService.GetLocalized("Dialog/NameChangeList4NewFileName1") },
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("Dialog/NameChangeOriginalFileName2"), NewFileName = ResourceService.GetLocalized("Dialog/NameChangeList4NewFileName2") },
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("Dialog/NameChangeOriginalFileName3"), NewFileName = ResourceService.GetLocalized("Dialog/NameChangeList4NewFileName3") },
                    new OldAndNewNameModel(){ OriginalFileName = ResourceService.GetLocalized("Dialog/NameChangeOriginalFileName4"), NewFileName = ResourceService.GetLocalized("Dialog/NameChangeList4NewFileName4") },
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
