using FileRenamer.Extensions.DataType.Enums;
using FileRenamer.Strings;
using Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml;

namespace FileRenamer.UI.TeachingTips
{
    /// <summary>
    /// 快捷操作应用内通知
    /// </summary>
    public sealed partial class QuickOperationTip : TeachingTip
    {
        public QuickOperationTip(QuickOperationKind quickOperationKind, bool isPinnedSuccessfully = false)
        {
            InitializeComponent();
            InitializeContent(quickOperationKind, isPinnedSuccessfully);
        }

        /// <summary>
        /// 初始化内容
        /// </summary>
        private void InitializeContent(QuickOperationKind quickOperationKind, bool isPinnedSuccessfully)
        {
            if (quickOperationKind is QuickOperationKind.Desktop && isPinnedSuccessfully)
            {
                QuickOperationSuccess.Text = Notification.DesktopShortcutSuccessfully;
                QuickOperationSuccess.Visibility = Visibility.Visible;
                QuickOperationFailed.Visibility = Visibility.Collapsed;
            }
            else if (quickOperationKind is QuickOperationKind.Desktop && !isPinnedSuccessfully)
            {
                QuickOperationFailed.Text = Notification.DesktopShortFailed;
                QuickOperationSuccess.Visibility = Visibility.Collapsed;
                QuickOperationFailed.Visibility = Visibility.Visible;
            }
            else if (quickOperationKind is QuickOperationKind.StartScreen && isPinnedSuccessfully)
            {
                QuickOperationSuccess.Text = Notification.StartScreenSuccessfully;
                QuickOperationSuccess.Visibility = Visibility.Visible;
                QuickOperationFailed.Visibility = Visibility.Collapsed;
            }
            else if (quickOperationKind is QuickOperationKind.StartScreen && !isPinnedSuccessfully)
            {
                QuickOperationFailed.Text = Notification.StartScreenFailed;
                QuickOperationSuccess.Visibility = Visibility.Collapsed;
                QuickOperationFailed.Visibility = Visibility.Visible;
            }
            else if (quickOperationKind is QuickOperationKind.Taskbar && isPinnedSuccessfully)
            {
                QuickOperationSuccess.Text = Notification.TaskbarSuccessfully;
                QuickOperationSuccess.Visibility = Visibility.Visible;
                QuickOperationFailed.Visibility = Visibility.Collapsed;
            }
            else if (quickOperationKind is QuickOperationKind.Taskbar && !isPinnedSuccessfully)
            {
                QuickOperationFailed.Text = Notification.TaskbarFailed;
                QuickOperationSuccess.Visibility = Visibility.Collapsed;
                QuickOperationFailed.Visibility = Visibility.Visible;
            }
        }
    }
}
