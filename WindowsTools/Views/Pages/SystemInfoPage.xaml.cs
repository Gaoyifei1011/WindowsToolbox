using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WindowsTools.Services.Root;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

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
                try
                {
                    Process.Start("CompMgmtLauncher.exe");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Open computer management launcher failed", e);
                }
            });
        }

        /// <summary>
        /// 打开 Microsoft 管理控制台
        /// </summary>
        private void OnManagementConsoleClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("mmc.exe");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Open management console failed", e);
                }
            });
        }

        /// <summary>
        /// 打开注册表编辑器
        /// </summary>
        private void OnRegistryEditorClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("Regedit.exe");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Open registry editor failed", e);
                }
            });
        }
    }
}
