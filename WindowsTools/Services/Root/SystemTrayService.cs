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

        public static event MouseEventHandler MouseClick;

        public static event MouseEventHandler MouseDoubleClick;

        /// <summary>
        /// 初始化系统托盘
        /// </summary>
        public static void InitializeSystemTray(string content, string iconPath)
        {
            if (notifyIcon is null)
            {
                notifyIcon = new NotifyIcon();
                notifyIcon.Text = content;
                notifyIcon.Icon = Icon.ExtractAssociatedIcon(iconPath);
                notifyIcon.Visible = true;
                notifyIcon.MouseClick += OnMouseClick;
                notifyIcon.MouseDoubleClick += OnMouseDoubleClick;
            }
        }

        /// <summary>
        /// 关闭托盘菜单
        /// </summary>
        public static void CloseSystemTray()
        {
            if (notifyIcon is not null)
            {
                notifyIcon.Visible = false;
                notifyIcon.MouseClick -= OnMouseClick;
                notifyIcon.Dispose();
                notifyIcon = null;
            }
        }

        /// <summary>
        /// 处理托盘菜单鼠标点击事件
        /// </summary>
        private static void OnMouseClick(object sender, MouseEventArgs args)
        {
            MouseClick?.Invoke(sender, args);
        }

        /// <summary>
        /// 处理托盘菜单鼠标双击事件
        /// </summary>
        private static void OnMouseDoubleClick(object sender, MouseEventArgs args)
        {
            MouseDoubleClick?.Invoke(sender, args);
        }
    }
}
