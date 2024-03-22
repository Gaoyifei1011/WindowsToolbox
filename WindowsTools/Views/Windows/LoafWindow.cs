using Mile.Xaml;
using System;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Windows.Forms;
using WindowsTools.Extensions.DataType.Enums;
using WindowsTools.Views.Pages;
using WindowsTools.WindowsAPI.PInvoke.DwmApi;

namespace WindowsTools.Views.Windows
{
    /// <summary>
    /// 摸鱼窗口
    /// </summary>
    public class LoafWindow : Form
    {
        private IContainer components = new Container();
        private WindowsXamlHost windowsXamlHost = new WindowsXamlHost();

        public static LoafWindow Current { get; private set; }

        public LoafWindow(UpdatingKind updatingKind, TimeSpan duration, bool blockAllKeys, bool lockScreenAutomaticly)
        {
            AllowDrop = false;
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = System.Drawing.Color.Black;
            Current = this;
            Controls.Add(windowsXamlHost);
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            ShowInTaskbar = false;
            windowsXamlHost.AutoSize = true;
            windowsXamlHost.Dock = DockStyle.Fill;
            windowsXamlHost.Child = new SimulateUpdatePage(updatingKind, duration, blockAllKeys, lockScreenAutomaticly);
        }

        protected override void OnKeyDown(KeyEventArgs args)
        {
            base.OnKeyDown(args);

            if (args.KeyCode is Keys.Escape)
            {
                (windowsXamlHost.Child as SimulateUpdatePage).StopSimulateUpdate();
            }
        }

        /// <summary>
        /// 处置由主窗体占用的资源
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing && components is not null)
            {
                components.Dispose();
            }
        }

        /// <summary>
        /// 窗体程序加载时初始化应用程序设置
        /// </summary>
        protected override void OnLoad(EventArgs args)
        {
            base.OnLoad(args);

            Margins margin = new Margins();
            DwmApiLibrary.DwmExtendFrameIntoClientArea(Handle, ref margin);
            Invalidate();
        }
    }
}
