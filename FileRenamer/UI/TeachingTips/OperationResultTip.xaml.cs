using FileRenamer.Strings;
using Microsoft.UI.Xaml.Controls;

namespace FileRenamer.UI.TeachingTips
{
    /// <summary>
    /// 操作完成后应用内通知
    /// </summary>
    public sealed partial class OperationResultTip : TeachingTip
    {
        public OperationResultTip(int successItems, int failedItems)
        {
            InitializeComponent();
            Content = failedItems is 0 ?
                string.Format(Notification.OperationResultSuccessfully, successItems) :
                string.Format(Notification.OperationResultFailed, successItems, failedItems);
        }
    }
}
