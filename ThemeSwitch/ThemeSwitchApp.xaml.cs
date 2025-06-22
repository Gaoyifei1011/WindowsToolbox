using System;
using System.Diagnostics.Tracing;
using ThemeSwitch.Services.Root;
using ThemeSwitch.Views.Windows;
using ThemeSwitch.WindowsAPI.ComTypes;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Hosting;

namespace ThemeSwitch
{
    /// <summary>
    /// 主题切换
    /// </summary>
    public partial class ThemeSwitchApp : Application, IDisposable
    {
        private bool isDisposed;
        private WindowsXamlManager windowXamlManager;

        public ThemeSwitchApp()
        {
            windowXamlManager = WindowsXamlManager.InitializeForCurrentThread();
            (Window.Current as object as IXamlSourceTransparency).SetIsBackgroundTransparent(true);
            InitializeComponent();
            LoadComponent(this, new Uri("ms-appx:///ThemeSwitchApp.xaml"), ComponentResourceLocation.Application);
            UnhandledException += OnUnhandledException;
        }

        /// <summary>
        /// 处理应用程序未知异常处理
        /// </summary>
        private void OnUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs args)
        {
            LogService.WriteLog(EventLevel.Warning, nameof(ThemeSwitch), nameof(ThemeSwitchApp), nameof(OnUnhandledException), 1, args.Exception);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ThemeSwitchApp()
        {
            Dispose(false);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                isDisposed = true;
                if (disposing)
                {
                    if (ThemeSwitchTrayWindow.Current is not null && !ThemeSwitchTrayWindow.Current.IsDisposed)
                    {
                        ThemeSwitchTrayWindow.Current?.Close();
                    }

                    SystemTrayService.CloseSystemTray();
                    windowXamlManager.Dispose();
                    LogService.CloseLog();
                    windowXamlManager = null;
                    System.Windows.Forms.Application.Exit();
                }
            }
        }
    }
}
