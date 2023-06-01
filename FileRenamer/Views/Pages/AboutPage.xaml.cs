using FileRenamer.Services.Root;
using Windows.UI.Xaml.Controls;

namespace FileRenamer.Views.Pages
{
    /// <summary>
    /// 关于页面
    /// </summary>
    public sealed partial class AboutPage : Page
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 本地化应用版本信息
        /// </summary>
        private string LocalizeAppVersion(string appVersion)
        {
            return string.Format(ResourceService.GetLocalized("About/AppVersion"), appVersion);
        }
    }
}
