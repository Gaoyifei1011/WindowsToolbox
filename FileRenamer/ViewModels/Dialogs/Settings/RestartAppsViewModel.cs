using FileRenamer.Contracts;
using FileRenamer.Extensions.Command;
using System.Windows.Forms;
using Windows.UI.Xaml.Controls;

namespace FileRenamer.ViewModels.Dialogs.Settings
{
    /// <summary>
    /// 应用重启对话框视图模型
    /// </summary>
    public sealed class RestartAppsViewModel
    {
        // 重启应用
        public IRelayCommand RestartAppsCommand => new RelayCommand<ContentDialog>((dialog) =>
        {
            dialog.Hide();
            Program.AppMutex.ReleaseMutex();
            Application.Restart();
        });

        // 取消重启应用
        public IRelayCommand CloseDialogCommand => new RelayCommand<ContentDialog>((dialog) =>
        {
            dialog.Hide();
        });
    }
}
