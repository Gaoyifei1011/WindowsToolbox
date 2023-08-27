using Mile.Xaml;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace FileRenamer
{
    public partial class App : Windows.UI.Xaml.Application, IDisposable
    {
        private bool isDisposed;

        public App()
        {
            this.ThreadInitialize();
            InitializeComponent();
            UnhandledException += OnUnhandledException;
        }

        /// <summary>
        /// 处理应用程序未知异常处理
        /// </summary>
        private void OnUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs args)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("HelpLink:" + args.Exception.HelpLink);
            stringBuilder.AppendLine("HResult:" + args.Exception.HResult);
            stringBuilder.AppendLine("Message:" + args.Exception.Message);
            stringBuilder.AppendLine("Source:" + args.Exception.Source);
            stringBuilder.AppendLine("StackTrace:" + args.Exception.StackTrace);

            File.AppendAllText(Path.Combine(Program.ErrorFileFolderPath, "ErrorInformation.log"), stringBuilder.ToString());

            DialogResult Result = MessageBox.Show(
                Strings.Resources.Title + Environment.NewLine +
                Strings.Resources.Content1 + Environment.NewLine +
                Strings.Resources.Content2,
                Strings.Resources.AppDisplayName,
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Error
                );

            // 打开资源管理器存放异常信息的文件目录
            if (Result is DialogResult.OK)
            {
                Process.Start("explorer.exe", Program.ErrorFileFolderPath);
            }
        }

        /// <summary>
        /// 关闭应用并释放所有资源
        /// </summary>
        public void CloseApp()
        {
            Program.MainWindow.Close();
            this.ThreadUninitialize();
            Environment.Exit(0);
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
                    CloseApp();
                }

                isDisposed = true;
            }
        }
    }
}
