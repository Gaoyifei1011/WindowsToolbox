using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WindowsTools.Views.Pages
{
    /// <summary>
    /// 系统信息页面
    /// </summary>
    public sealed partial class SystemInfoPage : Page
    {
        public SystemInfoPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 打开计算机管理
        /// </summary>
        private void OnComputerManagementClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                Process.Start("CompMgmtLauncher.exe");
            });
        }

        /// <summary>
        /// 打开 Microsoft 管理控制台
        /// </summary>
        private void OnManagementConsoleClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                Process.Start("mmc.exe");
            });
        }

        /// <summary>
        /// 打开注册表编辑器
        /// </summary>
        private void OnRegistryEditorClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                Process.Start("Regedit.exe");
            });
        }
    }
}
