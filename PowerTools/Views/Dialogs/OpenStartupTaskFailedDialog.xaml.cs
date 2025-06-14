using PowerTools.Services.Root;
using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

// 抑制 CA1822 警告
#pragma warning disable CA1822

namespace PowerTools.Views.Dialogs
{
    /// <summary>
    /// 打开自启任务失败提示
    /// </summary>
    public sealed partial class OpenStartupTaskFailedDialog : ContentDialog
    {
        public OpenStartupTaskFailedDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 打开任务管理器
        /// </summary>
        private void OnOpenTaskManagerClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("taskmgr.exe");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Open task manager failed", e);
                }
            });
        }

        /// <summary>
        /// 打开组策略
        /// </summary>

        private void OnOpenGroupPolicyClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("gpedit.msc");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Open group policy failed", e);
                }
            });
        }
    }
}
