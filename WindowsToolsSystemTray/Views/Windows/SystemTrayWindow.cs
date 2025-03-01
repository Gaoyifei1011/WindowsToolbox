using System.ComponentModel;
using System.Windows.Forms;

namespace WindowsToolsSystemTray.Views.Windows
{
    /// <summary>
    /// 托盘程序辅助窗口
    /// </summary>
    public class SystemTrayWindow : Form
    {
        private readonly Container components = new();

        public static SystemTrayWindow Current { get; private set; }

        public SystemTrayWindow()
        {
            Current = this;
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
    }
}
