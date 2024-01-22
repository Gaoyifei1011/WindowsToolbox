using Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml;
using WindowsTools.Strings;

namespace WindowsTools.UI.TeachingTips
{
    /// <summary>
    /// 图标提取结果通知
    /// </summary>
    public sealed partial class IconExtractTip : TeachingTip
    {
        public IconExtractTip(bool extractResult)
        {
            InitializeComponent();
            InitializeContent(extractResult);
        }

        /// <summary>
        /// 初始化内容
        /// </summary>
        private void InitializeContent(bool extractResult)
        {
            if (extractResult)
            {
                IconExtractSuccess.Text = Notification.IconExtractSuccessfully;
                IconExtractSuccess.Visibility = Visibility.Visible;
                IconExtractFailed.Visibility = Visibility.Collapsed;
            }
            else
            {
                IconExtractSuccess.Text = Notification.IconExtractFailed;
                IconExtractSuccess.Visibility = Visibility.Collapsed;
                IconExtractFailed.Visibility = Visibility.Visible;
            }
        }
    }
}
