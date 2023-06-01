using FileRenamer.Extensions.DataType.Enums;
using FileRenamer.Helpers.Root;
using FileRenamer.Services.Root;
using FileRenamer.WindowsAPI.PInvoke.Comctl32;
using System;
using System.Text;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;

namespace FileRenamer
{
    public sealed partial class App : Application
    {
        private WindowsXamlManager WindowsXamlManager { get; set; }

        public App()
        {
            WindowsXamlManager = WindowsXamlManager.InitializeForCurrentThread();
            InitializeComponent();            
            UnhandledException += OnUnhandledException;
        }

        /// <summary>
        /// 处理应用程序未知异常处理
        /// </summary>
        private void OnUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs args)
        {
            Comctl32Library.TaskDialog(
                Program.MainWindow.Handle,
                IntPtr.Zero,
                ResourceService.GetLocalized("Resources/AppDisplayName"),
                ResourceService.GetLocalized("MessageInfo/Title"),
                ResourceService.GetLocalized("MessageInfo/Content1") + Environment.NewLine + ResourceService.GetLocalized("MessageInfo/Content2"),
                TASKDIALOG_COMMON_BUTTON_FLAGS.TDCBF_OK_BUTTON | TASKDIALOG_COMMON_BUTTON_FLAGS.TDCBF_CANCEL_BUTTON,
                TASKDIALOGICON.TD_SHIELD_ERROR_RED_BAR,
                out TaskDialogResult Result
                );

            // 复制异常信息到剪贴板
            if (Result == TaskDialogResult.IDOK)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("HelpLink:" + args.Exception.HelpLink);
                stringBuilder.AppendLine("HResult:" + args.Exception.HResult);
                stringBuilder.AppendLine("Message:" + args.Exception.Message);
                stringBuilder.AppendLine("Source:" + args.Exception.Source);
                stringBuilder.AppendLine("StackTrace:" + args.Exception.StackTrace);

                CopyPasteHelper.CopyToClipBoard(stringBuilder.ToString());
            }

            return;
        }

        /// <summary>
        /// 关闭应用并释放所有资源
        /// </summary>
        public void CloseApp()
        {
            WindowsXamlManager?.Dispose();
            WindowsXamlManager = null;
            Environment.Exit(Convert.ToInt32(AppExitCode.Successfully));
        }
    }
}
