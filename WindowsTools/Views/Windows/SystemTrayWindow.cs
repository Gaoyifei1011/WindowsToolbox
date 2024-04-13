using Mile.Xaml;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Windows.UI.Xaml.Controls.Primitives;
using WindowsTools.Helpers.Root;
using WindowsTools.Views.Pages;

namespace WindowsTools.Views.Windows
{
    /// <summary>
    /// 托盘图标窗口
    /// </summary>
    public class SystemTrayWindow : Form
    {
        private IContainer components = new Container();
        private WindowsXamlHost windowsXamlHost = new WindowsXamlHost();

        public static SystemTrayWindow Current { get; private set; }

        public SystemTrayWindow()
        {
            AllowDrop = false;
            AutoScaleMode = AutoScaleMode.Font;
            Current = this;
            Controls.Add(windowsXamlHost);
            ShowInTaskbar = false;
            windowsXamlHost.AutoSize = true;
            windowsXamlHost.Dock = DockStyle.Fill;
            windowsXamlHost.Child = new SystemTrayPage();
            Text = "托盘窗口";
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams createParams = base.CreateParams;
                createParams.Style = unchecked((int)(0x80000000 | 0x00800000L | 0x00080000L));
                createParams.ExStyle = unchecked(0x00000080 | 0x00000020 | 0x00080000 | 0x00200000 | 0x08000000);
                return createParams;
            }
        }

        #region 第一部分：窗口类内置需要重载的事件

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

        /// <summary>
        /// 显示托盘图标右键菜单
        /// </summary>
        public void ShowSystemTrayMenu()
        {
            Point clientPoint = PointToClient(MousePosition);

            FlyoutShowOptions options = new FlyoutShowOptions();
            options.Placement = FlyoutPlacementMode.BottomEdgeAlignedLeft;
            options.ShowMode = FlyoutShowMode.Standard;
            options.Position = InfoHelper.SystemVersion.Build >= 22000 ?
                new global::Windows.Foundation.Point(clientPoint.X / ((double)DeviceDpi / 96), clientPoint.Y / ((double)DeviceDpi / 96)) :
                new global::Windows.Foundation.Point(clientPoint.X, clientPoint.Y);
            (windowsXamlHost.Child as SystemTrayPage).SystemTrayFlyout.ShowAt(null, options);
        }
    }
}
