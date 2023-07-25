using FileRenamer.Views.CustomControls.Notifications;
using Windows.UI.Xaml;

namespace FileRenamer.UI.Notifications
{
    /// <summary>
    /// 语言设置修改成功时应用内通知
    /// </summary>
    public sealed partial class LanguageChangeNotification : InAppNotification
    {
        public LanguageChangeNotification(FrameworkElement element) : base(element)
        {
            InitializeComponent();
        }
    }
}
