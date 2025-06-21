using PowerTools.Extensions.DataType.Enums;
using Windows.UI.Xaml;

namespace PowerTools.Helpers.Converters
{
    /// <summary>
    /// 值检查辅助类
    /// </summary>
    public static class ValueCheckConverterHelper
    {
        public static Visibility IsCurrentItem(object selectedItem, object item)
        {
            return Equals(selectedItem, item) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 检查下载任务状态
        /// </summary>
        public static Visibility CheckDownloadProgressState(DownloadProgressState downloadProgressState, DownloadProgressState comparedDownloadProgressState, bool needReverse)
        {
            return needReverse ? Equals(downloadProgressState, comparedDownloadProgressState) ? Visibility.Collapsed : Visibility.Visible : Equals(downloadProgressState, comparedDownloadProgressState) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 检查文件是否正在下载中
        /// </summary>
        public static Visibility CheckDownloadingState(DownloadProgressState downloadProgressState)
        {
            return downloadProgressState is DownloadProgressState.Queued || downloadProgressState is DownloadProgressState.Downloading ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
