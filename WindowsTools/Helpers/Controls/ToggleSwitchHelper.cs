using System.Windows.Forms;

namespace WindowsTools.Helpers.Controls
{
    /// <summary>
    /// 扩展后的切换开关控件辅助类
    /// </summary>
    public static class ToggleSwitchHelper
    {
        /// <summary>
        /// 获取切换开关控件的文字转向
        /// </summary>
        public static Windows.UI.Xaml.FlowDirection GetFlowDirection(RightToLeft rightToLeft)
        {
            return rightToLeft is RightToLeft.Yes ? Windows.UI.Xaml.FlowDirection.LeftToRight : Windows.UI.Xaml.FlowDirection.RightToLeft;
        }
    }
}
