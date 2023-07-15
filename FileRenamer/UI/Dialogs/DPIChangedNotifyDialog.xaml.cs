using FileRenamer.Views.CustomControls.DialogsAndFlyouts;
using System.Windows.Forms;
using Windows.UI.Xaml.Controls;

namespace FileRenamer.UI.Dialogs
{
    /// <summary>
    /// 屏幕缩放通知对话框
    /// </summary>
    public sealed partial class DPIChangedNotifyDialog : ExtendedContentDialog
    {
        public DPIChangedNotifyDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 重启应用
        /// </summary>
        public void OnRestartClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            RestartApps();
        }

        /// <summary>
        /// 重启应用，并关闭其他进程
        /// </summary>
        private void RestartApps()
        {
            Program.ApplicationRoot.CloseApp();
            Application.Restart();
        }
    }
}
