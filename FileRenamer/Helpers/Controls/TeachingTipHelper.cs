using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FileRenamer.Helpers.Controls.Extensions
{
    /// <summary>
    /// 扩展后的教学提示辅助类，使用教学提示来显示应用内通知
    /// </summary>
    public static class TeachingTipHelper
    {
        /// <summary>
        /// 使用教学提示显示应用内通知
        /// </summary>
        public static void Show(TeachingTip teachingTip, int duration = 2000)
        {
            Program.MainWindow.BeginInvoke(async () =>
            {
                teachingTip.Name = "TeachingTip" + Guid.NewGuid().ToString();
                (Program.MainWindow.MainPage.Content as Grid).Children.Add(teachingTip);
                teachingTip.IsOpen = true;
                teachingTip.Closed += (sender, args) =>
                {
                    try
                    {
                        foreach (UIElement item in (Program.MainWindow.MainPage.Content as Grid).Children)
                        {
                            if ((item as FrameworkElement).Name == teachingTip.Name)
                            {
                                (Program.MainWindow.MainPage.Content as Grid).Children.Remove(item);
                                break;
                            }
                        }
                    }
                    catch { }
                };
                await Task.Delay(duration);
                teachingTip.IsOpen = false;
            });
        }
    }
}
