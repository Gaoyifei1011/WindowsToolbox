using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace WindowsTools.UI.Dialogs
{
    /// <summary>
    /// 应用重启对话框
    /// </summary>
    public sealed partial class RestartAppsDialog : ContentDialog
    {
        private readonly SynchronizationContext synchronizationContext = SynchronizationContext.Current;

        public RestartAppsDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 重启应用，并关闭其他进程
        /// </summary>
        private void OnRestartAppsClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Task.Run(() =>
            {
                Process.Start(Process.GetCurrentProcess().MainModule.FileName, "Restart");
                synchronizationContext.Post(_ =>
                {
                    (Application.Current as App).Dispose();
                }, null);
            });
        }
    }
}
