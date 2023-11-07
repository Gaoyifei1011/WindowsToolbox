using FileRenamer.Helpers.Controls;
using FileRenamer.Helpers.Root;
using FileRenamer.Strings;
using FileRenamer.UI.Dialogs.About;
using System.Collections;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FileRenamer.Views.Pages
{
    /// <summary>
    /// 关于页面
    /// </summary>
    public sealed partial class AboutPage : Page
    {
        private string AppVersion = string.Format(About.AppVersion, InfoHelper.AppVersion.ToString());

        //项目引用信息
        private Hashtable ReferenceDict = new Hashtable()
        {
            { "Mile.Xaml","https://github.com/ProjectMile/Mile.Xaml" },
            { "Microsoft.UI.Xaml","https://github.com/microsoft/microsoft-ui-xaml" },
            { "Microsoft.WindowsAppSDK","https://github.com/microsoft/windowsappsdk" },
            { "Microsoft.Windows.CppWinRT","https://github.com/Microsoft/cppwinrt" },
        };

        //项目感谢者信息
        private Hashtable ThanksDict = new Hashtable()
        {
            { "AndromedaMelody","https://github.com/AndromedaMelody" },
            { "MouriNaruto" , "https://github.com/MouriNaruto" }
        };

        public AboutPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 应用信息
        /// </summary>
        private async void OnAppInformationClicked(object sender, RoutedEventArgs args)
        {
            await ContentDialogHelper.ShowAsync(new AppInformationDialog(), this);
        }

        /// <summary>
        /// 检查更新
        /// </summary>
        private void OnCheckUpdateClicked(object sender, RoutedEventArgs args)
        {
            Process.Start("explorer.exe", "https://github.com/Gaoyifei1011/FileRenamer/releases");
        }

        /// <summary>
        /// 项目主页
        /// </summary>
        private void OnProjectDescriptionClicked(object sender, RoutedEventArgs args)
        {
            Process.Start("explorer.exe", "https://github.com/Gaoyifei1011/FileRenamer");
        }

        /// <summary>
        /// 发送反馈
        /// </summary>
        private void OnSendFeedbackClicked(object sender, RoutedEventArgs args)
        {
            Process.Start("explorer.exe", "https://github.com/Gaoyifei1011/FileRenamer/issues");
        }

        /// <summary>
        /// 系统信息
        /// </summary>
        private void OnSystemInformationClicked(object sender, RoutedEventArgs args)
        {
            Process.Start("explorer.exe", "ms-settings:about");
        }
    }
}
