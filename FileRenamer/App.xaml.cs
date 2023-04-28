using FileRenamer.Extensions.DataType.Enums;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;

namespace FileRenamer
{
    public sealed partial class App : Application
    {
        private WindowsXamlManager WindowsXamlManager { get; set; }

        public App()
        {
            WindowsXamlManager = WindowsXamlManager.InitializeForCurrentThread();
            InitializeComponent();
        }

        /// <summary>
        /// 关闭应用并释放所有资源
        /// </summary>
        public void CloseApp()
        {
            WindowsXamlManager?.Dispose();
            WindowsXamlManager = null;
            Environment.Exit(Convert.ToInt32(AppExitCode.Successfully));
        }
    }
}
