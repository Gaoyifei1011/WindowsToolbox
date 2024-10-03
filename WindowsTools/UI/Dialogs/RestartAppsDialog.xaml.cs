using System.Windows.Forms;
using Windows.UI.Xaml.Controls;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace WindowsTools.UI.Dialogs
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
        private void OnRestartAppsClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Application.Restart();
        }
    }
}
