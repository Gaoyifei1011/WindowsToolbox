using System;
using System.Diagnostics.Tracing;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Hosting;
using ThemeSwitch.Services.Root;
using ThemeSwitch.Views.Windows;
using ThemeSwitch.WindowsAPI.ComTypes;

namespace ThemeSwitch
{
    /// <summary>
    /// 主题切换
    /// </summary>
    public partial class SystemTrayApp : Application, IDisposable
    {
        private bool isDisposed;
        private WindowsXamlManager windowXamlManager;

        public SystemTrayApp()
        {
            windowXamlManager = WindowsXamlManager.InitializeForCurrentThread();
            (Window.Current as object as IXamlSourceTransparency).SetIsBackgroundTransparent(true);
            InitializeComponent();
            LoadComponent(this, new Uri("ms-appx:///SystemTrayApp.xaml"), ComponentResourceLocation.Application);
            UnhandledException += OnUnhandledException;
        }

        /// <summary>
        /// 处理应用程序未知异常处理
        /// </summary>
        private void OnUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs args)
        {
            LogService.WriteLog(EventLevel.Warning, "Xaml islands UI Exception", args.Exception);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~SystemTrayApp()
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
                    if (SystemTrayWindow.Current is not null && !SystemTrayWindow.Current.IsDisposed)
                    {
                        SystemTrayWindow.Current?.Close();
                    }

                    SystemTrayService.CloseSystemTray();
                    windowXamlManager.Dispose();
                    windowXamlManager = null;
                    System.Windows.Forms.Application.Exit();
                }
            }
        }
    }
}
