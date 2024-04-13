using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WindowsTools.Views.Windows;

namespace WindowsTools.Views.Pages
{
    /// <summary>
    /// 系统托盘图标页面
    /// </summary>
    public sealed partial class SystemTrayPage : Page
    {
        public SystemTrayPage()
        {
            InitializeComponent();
        }

        #region 第一部分：系统托盘图标页面——挂载的事件

        /// <summary>
        /// 显示/隐藏窗口
        /// </summary>

        private void OnShowOrHideWindowClicked(object sender, RoutedEventArgs args)
        {
            if (MainWindow.Current.Visible)
            {
                MainWindow.Current.Hide();
            }
            else
            {
                MainWindow.Current.Show();
            }
        }

        /// <summary>
        /// 退出应用
        /// </summary>
        private void OnExitClicked(object sender, RoutedEventArgs args)
        {
            (Application.Current as App).Dispose();
        }

        #endregion 第一部分：系统托盘图标页面——挂载的事件
    }
}
