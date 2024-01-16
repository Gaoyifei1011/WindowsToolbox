using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WindowsTools.Helpers.Controls
{
    /// <summary>
    /// 扩展后的内容对话框辅助类，只允许在同一时间段内打开一个内容对话框
    /// </summary>
    public static class ContentDialogHelper
    {
        private static bool IsDialogOpening = false;

        /// <summary>
        /// 显示对话框
        /// </summary>
        public static async Task<ContentDialogResult> ShowAsync(ContentDialog dialog, FrameworkElement element)
        {
            ContentDialogResult dialogResult = ContentDialogResult.None;
            if (!IsDialogOpening && dialog is not null && element is not null)
            {
                IsDialogOpening = true;
                dialog.XamlRoot = element.XamlRoot;
                dialog.RequestedTheme = element.ActualTheme;
                element.ActualThemeChanged += (sender, args) =>
                {
                    dialog.RequestedTheme = element.ActualTheme;
                };
                dialogResult = await dialog.ShowAsync();
                IsDialogOpening = false;
            }
            return dialogResult;
        }
    }
}
