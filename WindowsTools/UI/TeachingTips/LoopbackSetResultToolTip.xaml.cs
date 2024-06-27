using Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml;
using WindowsTools.Strings;

namespace WindowsTools.UI.TeachingTips
{
    /// <summary>
    /// 日志记录清除应用内通知
    /// </summary>
    public sealed partial class LoopbackSetResultToolTip : TeachingTip
    {
        public LoopbackSetResultToolTip(bool setResult)
        {
            InitializeComponent();
            InitializeContent(setResult);
        }

        /// <summary>
        /// 初始化内容
        /// </summary>
        private void InitializeContent(bool setResult)
        {
            if (setResult)
            {
                LoopbackSetSuccess.Text = Notification.LoopbackSetResultSuccessfully;
                LoopbackSetSuccess.Visibility = Visibility.Visible;
                LoopbackSetFailed.Visibility = Visibility.Collapsed;
            }
            else
            {
                LoopbackSetFailed.Text = Notification.LoopbackSetResultFailed;
                LoopbackSetSuccess.Visibility = Visibility.Collapsed;
                LoopbackSetFailed.Visibility = Visibility.Visible;
            }
        }
    }
}
