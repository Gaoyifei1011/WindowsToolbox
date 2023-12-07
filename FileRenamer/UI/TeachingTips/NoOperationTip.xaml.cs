using FileRenamer.Strings;
using Microsoft.UI.Xaml.Controls;

namespace FileRenamer.UI.TeachingTips
{
    /// <summary>
    /// 操作失败时应用内通知
    /// </summary>
    public sealed partial class NoOperationTip : TeachingTip
    {
        public NoOperationTip()
        {
            InitializeComponent();
            Content = Notification.NoOperation;
        }
    }
}
