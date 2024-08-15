using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using Windows.UI.Xaml.Controls;

namespace WindowsTools.UI.Dialogs
{
    /// <summary>
    /// 许可证文字内容对话框
    /// </summary>
    public sealed partial class LicenseDialog : ContentDialog, INotifyPropertyChanged
    {
        private string _licenseText;

        public string LicenseText
        {
            get { return _licenseText; }

            set
            {
                if (!Equals(_licenseText, value))
                {
                    _licenseText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LicenseText)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public LicenseDialog()
        {
            InitializeComponent();
            LicenseText = Encoding.UTF8.GetString(Strings.Resources.LICENSE);
        }

        /// <summary>
        /// 获取控件的文字转向
        /// </summary>
        private Windows.UI.Xaml.FlowDirection GetControlDirection(RightToLeft rightToLeft)
        {
            return rightToLeft is RightToLeft.Yes ? Windows.UI.Xaml.FlowDirection.RightToLeft : Windows.UI.Xaml.FlowDirection.LeftToRight;
        }
    }
}
