using System;
using System.Diagnostics.Tracing;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Hosting;
using WindowsTools.Services.Controls.Download;
using WindowsTools.Services.Root;
using WindowsTools.Views.Windows;
using WindowsTools.WindowsAPI.ComTypes;

namespace WindowsTools
{
    /// <summary>
    /// Windows 工具箱应用程序
    /// </summary>
    public partial class XamlIslandsApp : Application, IDisposable
    {
        private bool isDisposed;
        private WindowsXamlManager windowXamlManager;

        public XamlIslandsApp()
        {
            windowXamlManager = WindowsXamlManager.InitializeForCurrentThread();
            (Window.Current as object as IXamlSourceTransparency).SetIsBackgroundTransparent(true);
            InitializeComponent();
            LoadComponent(this, new Uri("ms-appx:///XamlIslandsApp.xaml"), ComponentResourceLocation.Application);
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

        ~XamlIslandsApp()
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
                    windowXamlManager.Dispose();
                    windowXamlManager = null;
                    DownloadSchedulerService.TerminateDownload();
                    DownloadSchedulerService.CloseDownloadScheduler();
                    System.Windows.Forms.Application.Exit();
                }
            }
        }
    }
}
