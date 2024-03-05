using Mile.Xaml.Interop;
using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;
using WindowsTools.Services.Root;
using WindowsTools.Views.Windows;

namespace WindowsTools
{
    /// <summary>
    /// Windows 工具箱应用程序
    /// </summary>
    public partial class App : Application, IDisposable
    {
        private bool isDisposed;

        public App()
        {
            WindowsXamlManager.InitializeForCurrentThread();
            Window.Current.GetInterop().TransparentBackground = true;
            InitializeComponent();
            UnhandledException += OnUnhandledException;
        }

        /// <summary>
        /// 处理应用程序未知异常处理
        /// </summary>
        private void OnUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs args)
        {
            LogService.WriteLog(EventLogEntryType.Warning, "Xaml islands UI Exception", args.Exception);
            Dispose();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~App()
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
                if (disposing)
                {
                    MainWindow.Current.Close();
                    System.Windows.Forms.Application.Exit();
                }

                isDisposed = true;
            }
        }
    }
}
