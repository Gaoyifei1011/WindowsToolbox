using FileRenamer.Views.CustomControls.Notifications;

namespace FileRenamer.UI.Notifications
{
    /// <summary>
    /// 操作失败时应用内通知
    /// </summary>
    public sealed partial class NoOperationNotification : InAppNotification
    {
        public NoOperationNotification()
        {
            InitializeComponent();
        }
    }
}
