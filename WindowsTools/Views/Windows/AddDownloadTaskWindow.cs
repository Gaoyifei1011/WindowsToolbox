using Mile.Xaml;
using System.ComponentModel;
using System.Windows.Forms;
using Windows.UI.Xaml;
using WindowsTools.Views.Pages;

namespace WindowsTools.Views.Windows
{
    /// <summary>
    /// 添加下载任务窗口
    /// </summary>
    public class AddDownloadTaskWindow : Form
    {
        private readonly IContainer components = new Container();
        public WindowsXamlHost windowsXamlHost = new();

        public UIElement Content { get; set; } = new AddDownloadTaskPage();

        public static AddDownloadTaskWindow Current { get; private set; }

        public AddDownloadTaskWindow()
        {
            AllowDrop = false;
            AutoScaleMode = AutoScaleMode.Font;
            Current = this;
            Controls.Add(windowsXamlHost);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Height = 410 * DeviceDpi / 96;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            Width = 600 * DeviceDpi / 96;
            windowsXamlHost.AutoSize = true;
            windowsXamlHost.Dock = DockStyle.Fill;
            windowsXamlHost.Child = Content;
        }

        #region 第一部分：窗口类内置需要重载的事件

        /// <summary>
        /// 关闭窗体后发生的事件
        /// </summary>
        protected override void OnFormClosed(FormClosedEventArgs args)
        {
            base.OnFormClosed(args);
            Current = null;
        }

        /// <summary>
        /// 按 Esc 键时，关闭窗口
        /// </summary>
        protected override void OnKeyDown(KeyEventArgs args)
        {
            base.OnKeyDown(args);

            if (args.KeyCode is Keys.Escape)
            {
                Close();
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

        #endregion 第一部分：窗口类内置需要重载的事件
    }
}
