using FileRenamer.Views.CustomControls.DialogsAndFlyouts;
using System.Windows.Forms;
using Windows.UI.Xaml.Controls;

namespace FileRenamer.UI.Dialogs
{
    /// <summary>
    /// 应用重启对话框
    /// </summary>
    public sealed partial class RestartAppsDialog : ExtendedContentDialog
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
            Program.ApplicationRoot.CloseApp(true);
            Application.Restart();
        }
    }
}
