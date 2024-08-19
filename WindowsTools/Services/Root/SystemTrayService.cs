using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Windows.Forms;
using Windows.UI.Xaml;
using WindowsTools.Services.Controls.Settings;
using WindowsTools.WindowsAPI.PInvoke.Uxtheme;

namespace WindowsTools.Services.Root
{
    /// <summary>
    /// 系统托盘菜单服务
    /// </summary>
    public static class SystemTrayService
    {
        public static NotifyIcon notifyIcon;

        public static event EventHandler MenuItemClick;

        public static event MouseEventHandler MouseDoubleClick;

        /// <summary>
        /// 初始化系统托盘
        /// </summary>
        public static void InitializeSystemTray(string content, string iconPath, List<MenuItem> contextMenuItemList)
        {
            if (notifyIcon is null)
            {
                notifyIcon = new NotifyIcon
                {
                    Text = content,
                    Icon = Icon.ExtractAssociatedIcon(iconPath),
                    Visible = true,
                    ContextMenu = new ContextMenu()
                };
                notifyIcon.MouseDoubleClick += OnMouseDoubleClick;

                foreach (MenuItem menuItem in contextMenuItemList)
                {
                    menuItem.Click += OnItemClick;
                    notifyIcon.ContextMenu.MenuItems.Add(menuItem);
                }
            }
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
                    notifyIcon.MouseDoubleClick -= OnMouseDoubleClick;
                    foreach (MenuItem menuItem in notifyIcon.ContextMenu.MenuItems)
                    {
                        menuItem.Click -= OnItemClick;
                    }
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
            notifyIcon.ShowBalloonTip(30, title, content, toolTipIcon);
        }

        /// <summary>
        /// 设置系统托盘图标右键菜单主题色
        /// </summary>
        public static void SetMenuTheme()
        {
            if (ThemeService.AppTheme.Equals(ThemeService.ThemeList[0]))
            {
                if (Windows.UI.Xaml.Application.Current.RequestedTheme is ApplicationTheme.Light)
                {
                    UxthemeLibrary.SetPreferredAppMode(PreferredAppMode.ForceLight);
                    UxthemeLibrary.FlushMenuThemes();
                }
                else
                {
                    UxthemeLibrary.SetPreferredAppMode(PreferredAppMode.ForceDark);
                    UxthemeLibrary.FlushMenuThemes();
                }
            }
            else if (ThemeService.AppTheme.Equals(ThemeService.ThemeList[1]))
            {
                UxthemeLibrary.SetPreferredAppMode(PreferredAppMode.ForceLight);
                UxthemeLibrary.FlushMenuThemes();
            }
            else if (ThemeService.AppTheme.Equals(ThemeService.ThemeList[2]))
            {
                UxthemeLibrary.SetPreferredAppMode(PreferredAppMode.ForceDark);
                UxthemeLibrary.FlushMenuThemes();
            }
        }

        /// <summary>
        /// 处理托盘菜单鼠标点击事件
        /// </summary>
        private static void OnItemClick(object sender, EventArgs args)
        {
            MenuItemClick?.Invoke(sender, args);
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
