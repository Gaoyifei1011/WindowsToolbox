using Microsoft.UI.Xaml.Controls;
using WindowsTools.Extensions.DataType.Enums;
using WindowsTools.Strings;

namespace WindowsTools.UI.TeachingTips
{
    /// <summary>
    /// 操作完成后应用内通知
    /// </summary>
    public sealed partial class OperationResultTip : TeachingTip
    {
        public OperationResultTip(OperationKind operationKind, int successItems, int failedItems)
        {
            InitializeComponent();

            if (operationKind is OperationKind.File)
            {
                Content = failedItems is 0 ?
                    string.Format(Notification.FileResultSuccessfully, successItems) :
                    string.Format(Notification.FileResultFailed, successItems, failedItems);
            }
            else if (operationKind is OperationKind.IconExtract)
            {
                Content = failedItems is 0 ?
                    string.Format(Notification.IconExtractSuccessfully, successItems) :
                    string.Format(Notification.IconExtractFailed, successItems, failedItems);
            }
        }
    }
}
