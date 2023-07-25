using FileRenamer.Services.Root;
using FileRenamer.Views.CustomControls.Notifications;
using Windows.UI.Xaml;

namespace FileRenamer.UI.Notifications
{
    /// <summary>
    /// 操作完成后应用内通知
    /// </summary>
    public sealed partial class OperationResultNotification : InAppNotification
    {
        public string OperationResult { get; }

        public OperationResultNotification(FrameworkElement element, int successItems, int failedItems) : base(element)
        {
            InitializeComponent();
            OperationResult = failedItems is 0 ?
                string.Format(ResourceService.GetLocalized("Notification/OperationResultSuccessfully"), successItems) :
                string.Format(ResourceService.GetLocalized("Notification/OperationResultFailed"), successItems, failedItems);
        }
    }
}
