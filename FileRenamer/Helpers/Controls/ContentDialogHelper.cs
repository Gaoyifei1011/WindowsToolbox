using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FileRenamer.Helpers.Controls
{
    /// <summary>
    /// 扩展后的内容对话框辅助类，只允许在同一时间段内打开一个内容对话框
    /// </summary>
    public static class ContentDialogHelper
    {
        private static bool IsDialogOpening { get; set; } = false;

        /// <summary>
        /// 显示对话框
        /// </summary>
        public static async Task<ContentDialogResult> ShowAsync(ContentDialog dialog, XamlRoot xamlRoot, ElementTheme theme = ElementTheme.Default)
        {
            ContentDialogResult dialogResult = ContentDialogResult.None;
            if (!IsDialogOpening && dialog is not null && xamlRoot is not null)
            {
                IsDialogOpening = true;
                dialog.XamlRoot = xamlRoot;
                dialog.RequestedTheme = theme;
                dialogResult = await dialog.ShowAsync();
                IsDialogOpening = false;
            }
            return dialogResult;
        }
    }
}
