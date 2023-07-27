using FileRenamer.Services.Root;
using FileRenamer.Views.CustomControls.Notifications;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;

namespace FileRenamer.UI.Notifications
{
    /// <summary>
    /// 错误信息列表复制应用内通知
    /// </summary>
    public sealed partial class OperationFailedCopyNotification : InAppNotification, INotifyPropertyChanged
    {
        private int Count = 0;

        private bool _isMultiSelected = false;

        public bool IsMultiSelected
        {
            get { return _isMultiSelected; }

            set
            {
                _isMultiSelected = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public OperationFailedCopyNotification(FrameworkElement element, bool isMultiSelected = false, int count = 0) : base(element)
        {
            InitializeComponent();
            IsMultiSelected = isMultiSelected;
            Count = count;
        }

        public void CopySelectedSuccessLoaded(object sender, RoutedEventArgs args)
        {
            CopySelectedSuccess.Text = string.Format(ResourceService.GetLocalized("Notification/OperationFailedSelectedCopy"), Count);
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
