﻿using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WindowsTools.Views.Windows;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

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
            Task.Run(() =>
            {
                Process.Start(Process.GetCurrentProcess().MainModule.FileName, "Restart");
                MainWindow.Current.BeginInvoke(() =>
                {
                    (Application.Current as App).Dispose();
                });
            });
        }
    }
}
