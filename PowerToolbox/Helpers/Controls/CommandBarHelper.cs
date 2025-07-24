using PowerToolbox.Views.Windows;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace PowerToolbox.Helpers.Controls
{
    /// <summary>
    /// 命令栏辅助类
    /// </summary>
    public static class CommandBarHelper
    {
        /// <summary>
        /// 弹出控件打开时触发的事件
        /// </summary>
        public static async void OnOpening(object sender, object args)
        {
            if (sender is CommandBar commandBar)
            {
                if (MainWindow.Current.Content is not null && MainWindow.Current.Content.XamlRoot is not null)
                {
                    await Task.Delay(10);
                    // 窗口移动时，校对命令栏浮出菜单主题错误的问题
                    foreach (Popup popup in VisualTreeHelper.GetOpenPopupsForXamlRoot(MainWindow.Current.Content.XamlRoot))
                    {
                        if (popup.Child is Grid grid && grid.Name is "OverflowContentRoot")
                        {
                            popup.RequestedTheme = commandBar.ActualTheme;
                            grid.RequestedTheme = commandBar.ActualTheme;
                        }
                    }
                }
            }
        }
    }
}
