using System.Diagnostics;
using Windows.UI.Xaml.Controls;

namespace FileRenamer.UI.Dialogs
{
    /// <summary>
    /// 应用重启对话框
    /// </summary>
    public sealed partial class RestartAppsDialog : ContentDialog
    {
        public RestartAppsDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 重启应用，并关闭其他进程
        /// </summary>
        public void OnRestartAppsClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Process.Start(Process.GetCurrentProcess().MainModule.FileName, "Restart");
            Program.ApplicationRoot.Dispose();
        }
    }
}
