using FileRenamer.Strings;
using FileRenamer.Views.CustomControls.Notifications;
using Windows.UI.Xaml;

namespace FileRenamer.UI.Notifications
{
    /// <summary>
    /// 操作完成后应用内通知
    /// </summary>
    public sealed partial class OperationResultNotification : InAppNotification
    {
        public OperationResultNotification(FrameworkElement element, int successItems, int failedItems) : base(element)
        {
            InitializeComponent();
            Content = failedItems is 0 ?
                string.Format(Notification.OperationResultSuccessfully, successItems) :
                string.Format(Notification.OperationResultFailed, successItems, failedItems);
        }
    }
}
