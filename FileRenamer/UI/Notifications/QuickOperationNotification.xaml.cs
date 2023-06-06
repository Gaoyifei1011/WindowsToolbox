using FileRenamer.Extensions.DataType.Enums;
using FileRenamer.Views.CustomControls.Notifications;
using System.Runtime.InteropServices;

namespace FileRenamer.UI.Notifications
{
    /// <summary>
    /// 快捷操作应用内通知视图
    /// </summary>
    public sealed partial class QuickOperationNotification : InAppNotification
    {
        public QuickOperationNotification(QuickOperationType operationType, [Optional, DefaultParameterValue(false)] bool isPinnedSuccessfully)
        {
            XamlRoot = Program.MainWindow.MainPage.XamlRoot;
            InitializeComponent();
            ViewModel.Initialize(operationType, isPinnedSuccessfully);
        }

        public bool ControlLoad(QuickOperationType operationType, bool isPinnedSuccessfully, string controlName)
        {
            if (controlName is "DesktopShortcutSuccess" && operationType is QuickOperationType.DesktopShortcut && isPinnedSuccessfully)
            {
                return true;
            }
            else if (controlName is "DesktopShortcutFailed" && operationType is QuickOperationType.DesktopShortcut && !isPinnedSuccessfully)
            {
                return true;
            }
            else if (controlName is "StartScreenSuccess" && operationType is QuickOperationType.StartScreen && isPinnedSuccessfully)
            {
                return true;
            }
            else if (controlName is "StartScreenFailed" && operationType is QuickOperationType.StartScreen && !isPinnedSuccessfully)
            {
                return true;
            }
            else if (controlName is "TaskbarSuccess" && operationType is QuickOperationType.Taskbar && isPinnedSuccessfully)
            {
                return true;
            }
            else if (controlName is "TaskbarFailed" && operationType is QuickOperationType.Taskbar && !isPinnedSuccessfully)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
