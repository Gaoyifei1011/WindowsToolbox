using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WindowsTools.Views.Pages;
using WindowsTools.Views.Windows;

namespace WindowsTools.Helpers.Controls
{
    /// <summary>
    /// 扩展后的教学提示控件辅助类
    /// </summary>
    public static class TeachingTipHelper
    {
        /// <summary>
        /// 使用教学提示显示应用内通知
        /// </summary>
        public static async Task ShowAsync(TeachingTip teachingTip, int duration = 2000)
        {
            teachingTip.Name = "TeachingTip" + Guid.NewGuid().ToString();

            ((MainWindow.Current.Content as MainPage).Content as Grid).Children.Add(teachingTip);

            teachingTip.IsOpen = true;
            teachingTip.Closed += (sender, args) =>
            {
                try
                {
                    foreach (UIElement item in ((MainWindow.Current.Content as MainPage).Content as Grid).Children)
                    {
                        if ((item as FrameworkElement).Name == teachingTip.Name)
                        {
                            ((MainWindow.Current.Content as MainPage).Content as Grid).Children.Remove(item);
                            break;
                        }
                    }
                }
                catch { }
            };
            await Task.Delay(duration);
            teachingTip.IsOpen = false;
        }
    }
}
