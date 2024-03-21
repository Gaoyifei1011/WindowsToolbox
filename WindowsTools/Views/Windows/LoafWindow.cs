using System.Windows.Forms;

namespace WindowsTools.Views.Windows
{
    /// <summary>
    /// 摸鱼窗口
    /// </summary>
    public class LoafWindow : Form
    {
        public LoafWindow()
        {
            AllowDrop = false;
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = System.Drawing.Color.Black;
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            ShowInTaskbar = false;
        }

        protected override void OnKeyDown(KeyEventArgs args)
        {
            base.OnKeyDown(args);

            if (args.KeyCode == Keys.Escape)
            {
                Close();
            }
        }
    }
}
