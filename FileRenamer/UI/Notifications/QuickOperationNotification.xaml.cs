using FileRenamer.Extensions.DataType.Enums;
using FileRenamer.Views.CustomControls.Notifications;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.UI.Xaml;

namespace FileRenamer.UI.Notifications
{
    /// <summary>
    /// 快捷操作应用内通知
    /// </summary>
    public sealed partial class QuickOperationNotification : InAppNotification, INotifyPropertyChanged
    {
        private QuickOperationKind _operationType;

        public QuickOperationKind OperationType
        {
            get { return _operationType; }

            set
            {
                _operationType = value;
                OnPropertyChanged();
            }
        }

        private bool _isPinnedSuccessfully = false;

        public bool IsPinnedSuccessfully
        {
            get { return _isPinnedSuccessfully; }

            set
            {
                _isPinnedSuccessfully = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public QuickOperationNotification(FrameworkElement element, QuickOperationKind operationType, [Optional, DefaultParameterValue(false)] bool isPinnedSuccessfully) : base(element)
        {
            InitializeComponent();
            OperationType = operationType;
            IsPinnedSuccessfully = isPinnedSuccessfully;
        }

        private bool ControlLoad(QuickOperationKind operationKind, bool isPinnedSuccessfully, string controlName)
        {
            if (controlName is "DesktopShortcutSuccess" && operationKind is QuickOperationKind.DesktopShortcut && isPinnedSuccessfully)
            {
                return true;
            }
            else if (controlName is "DesktopShortcutFailed" && operationKind is QuickOperationKind.DesktopShortcut && !isPinnedSuccessfully)
            {
                return true;
            }
            else if (controlName is "StartScreenSuccess" && operationKind is QuickOperationKind.StartScreen && isPinnedSuccessfully)
            {
                return true;
            }
            else if (controlName is "StartScreenFailed" && operationKind is QuickOperationKind.StartScreen && !isPinnedSuccessfully)
            {
                return true;
            }
            else if (controlName is "TaskbarSuccess" && operationKind is QuickOperationKind.Taskbar && isPinnedSuccessfully)
            {
                return true;
            }
            else if (controlName is "TaskbarFailed" && operationKind is QuickOperationKind.Taskbar && !isPinnedSuccessfully)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
