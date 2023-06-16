using FileRenamer.Services.Controls.Settings.Appearance;
using FileRenamer.ViewModels.Base;
using FileRenamer.WindowsAPI.PInvoke.DwmApi;
using System;
using System.Runtime.InteropServices;
using Windows.UI.Xaml;

namespace FileRenamer.ViewModels.Controls
{
    /// <summary>
    /// 应用改名示例视图模型
    /// </summary>
    public class NameChangeViewModel : ViewModelBase
    {
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
