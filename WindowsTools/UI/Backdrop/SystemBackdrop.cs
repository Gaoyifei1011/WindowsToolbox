using Microsoft.Win32;
using System;
using Windows.UI;
using Windows.UI.Composition.Desktop;
using Windows.UI.Xaml;

namespace WindowsTools.UI.Backdrop
{
    /// <summary>
    /// 抽象类：系统背景色
    /// </summary>
    public abstract class SystemBackdrop(DesktopWindowTarget target) : IDisposable
    {
        public abstract float LightTintOpacity { get; set; }

        public abstract float LightLuminosityOpacity { get; set; }

        public abstract float DarkTintOpacity { get; set; }

        public abstract float DarkLuminosityOpacity { get; set; }

        public abstract Color LightTintColor { get; set; }

        public abstract Color LightFallbackColor { get; set; }

        public abstract Color DarkTintColor { get; set; }

        public abstract Color DarkFallbackColor { get; set; }

        public abstract ElementTheme RequestedTheme { get; set; }

        public abstract bool IsInputActive { get; set; }

        public abstract bool IsSupported { get; }

        public DesktopWindowTarget DesktopWindowTarget { get; } = target;

        public abstract void InitializeBackdrop();

        public abstract void ResetProperties();

        public abstract void Dispose();

        /// <summary>
        /// 检查是否启用系统透明度效果设置
        /// </summary>
        protected bool IsAdvancedEffectsEnabled()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            if (key is not null)
            {
                object value = key.GetValue("EnableTransparency");
                if (value is not null && Convert.ToInt32(value) is 1)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
