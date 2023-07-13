using FileRenamer.Services.Root;
using FileRenamer.Views.CustomControls.Notifications;

namespace FileRenamer.UI.Notifications
{
    /// <summary>
    /// 操作完成后应用内通知视图
    /// </summary>
    public sealed partial class OperationResultNotification : InAppNotification
    {
        public string OperationResult { get; }

        public OperationResultNotification(int successItems, int failedItems)
        {
            InitializeComponent();
            OperationResult = failedItems is 0 ?
                string.Format(ResourceService.GetLocalized("Notification/OperationResultSuccessfully"), successItems) :
                string.Format(ResourceService.GetLocalized("Notification/OperationResultFailed"), successItems, failedItems);
        }
    }
}
