using System.ComponentModel;
using Windows.UI.Xaml.Controls;

namespace WindowsTools.Views.Pages
{
    /// <summary>
    /// 数字签名页面
    /// </summary>
    public sealed partial class FileCertificatePage : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public FileCertificatePage()
        {
            InitializeComponent();
        }
    }
}
