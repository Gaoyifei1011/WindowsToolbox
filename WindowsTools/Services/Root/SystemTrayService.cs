using System;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsTools.Services.Root
{
    /// <summary>
    /// 系统托盘菜单服务
    /// </summary>
    public static class SystemTrayService
    {
        private static NotifyIcon notifyIcon;

        /// <summary>
        /// 初始化系统托盘
        /// </summary>
        public static void InitializeSystemTray(string content, string iconPath)
        {
            notifyIcon = new NotifyIcon
            {
                Text = content,
                Icon = Icon.ExtractAssociatedIcon(iconPath),
                Visible = false,
            };
        }

        /// <summary>
        /// 关闭托盘菜单
        /// </summary>
        public static void CloseSystemTray()
        {
            if (notifyIcon is not null)
            {
                try
                {
                    notifyIcon.Visible = false;
                    notifyIcon.Dispose();
                    notifyIcon = null;
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Warning, "Unregister system tray event failed", e);
                }
            }
        }

        /// <summary>
        /// 显示应用通知
        /// </summary>
        public static void ShowToolTip(string title, string content, ToolTipIcon toolTipIcon)
        {
            notifyIcon.Visible = true;
            notifyIcon.ShowBalloonTip(30, title, content, toolTipIcon);
            notifyIcon.Visible = false;
        }
    }
}
