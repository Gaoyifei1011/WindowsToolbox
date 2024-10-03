using Microsoft.Win32;
using System;
using Windows.UI;
using Windows.UI.Composition.Desktop;
using Windows.UI.Xaml;

// 抑制 CA1822 警告
#pragma warning disable CA1822

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

        public DesktopWindowTarget DesktopWindowTarget { get; } = target;

        public abstract void InitializeBackdrop();

        public abstract void ResetProperties();

        public abstract void Dispose();

        /// <summary>
        /// 检查是否启用系统透明度效果设置
        /// </summary>
        protected bool IsAdvancedEffectsEnabled()
        {
            return Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize") is RegistryKey key && key.GetValue("EnableTransparency") is object value && Convert.ToInt32(value) is 1;
        }
    }
}
