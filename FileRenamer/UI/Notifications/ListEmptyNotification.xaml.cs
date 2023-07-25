using FileRenamer.Views.CustomControls.Notifications;
using Windows.UI.Xaml;

namespace FileRenamer.UI.Notifications
{
    /// <summary>
    /// 操作失败时应用内通知
    /// </summary>
    public sealed partial class ListEmptyNotification : InAppNotification
    {
        public ListEmptyNotification(FrameworkElement element) : base(element)
        {
            InitializeComponent();
        }
    }
}
