using System;
using System.Windows.Forms;

namespace WindowsToolsSystemTray
{
    public static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Application.Run(new Form1());
        }
    }
}
