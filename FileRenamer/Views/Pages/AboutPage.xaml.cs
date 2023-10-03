using FileRenamer.Helpers.Controls;
using FileRenamer.Helpers.Root;
using FileRenamer.Models;
using FileRenamer.Strings;
using FileRenamer.UI.Dialogs.About;
using System.Collections.Generic;
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
        private readonly int MajorVersion = InfoHelper.AppVersion.Major;

        private readonly int MinorVersion = InfoHelper.AppVersion.Minor;

        private readonly int BuildVersion = InfoHelper.AppVersion.Build;

        private readonly int RevisionVersion = InfoHelper.AppVersion.Revision;

        public string AppVersion => string.Format(About.AppVersion, MajorVersion, MinorVersion, BuildVersion, RevisionVersion);

        //项目引用信息
        public List<KeyValuePairModel> ReferenceDict = new List<KeyValuePairModel>()
        {
            new KeyValuePairModel() { Key = "Mile.Xaml" , Value = "https://github.com/ProjectMile/Mile.Xaml" },
            new KeyValuePairModel() { Key = "Microsoft.Windows.CppWinRT" , Value = "https://github.com/Microsoft/cppwinrt" },
            new KeyValuePairModel() { Key = "Newtonsoft.Json" , Value = "https://www.newtonsoft.com/json" },
        };

        public List<KeyValuePairModel> ThanksDict = new List<KeyValuePairModel>()
        {
            new KeyValuePairModel() { Key ="AndromedaMelody", Value="https://github.com/AndromedaMelody"},
            new KeyValuePairModel() { Key = "MouriNaruto" , Value = "https://github.com/MouriNaruto" }
        };

        public AboutPage()
        {
            InitializeComponent();

            if (RuntimeHelper.IsMSIX)
            {
                ReferenceDict.Insert(2, new KeyValuePairModel() { Key = "Microsoft.WindowsAppSDK", Value = "https://github.com/microsoft/windowsappsdk" });
            }
        }

        /// <summary>
        /// 应用信息
        /// </summary>
        public async void OnAppInformationClicked(object sender, RoutedEventArgs args)
        {
            await ContentDialogHelper.ShowAsync(new AppInformationDialog(), this);
        }

        /// <summary>
        /// 检查更新
        /// </summary>
        public void OnCheckUpdateClicked(object sender, RoutedEventArgs args)
        {
            Process.Start("explorer.exe", "https://github.com/Gaoyifei1011/FileRenamer/releases");
        }

        /// <summary>
        /// 项目主页
        /// </summary>
        public void OnProjectDescriptionClicked(object sender, RoutedEventArgs args)
        {
            Process.Start("explorer.exe", "https://github.com/Gaoyifei1011/FileRenamer");
        }

        /// <summary>
        /// 发送反馈
        /// </summary>
        public void OnSendFeedbackClicked(object sender, RoutedEventArgs args)
        {
            Process.Start("explorer.exe", "https://github.com/Gaoyifei1011/FileRenamer/issues");
        }

        /// <summary>
        /// 系统信息
        /// </summary>
        public void OnSystemInformationClicked(object sender, RoutedEventArgs args)
        {
            Process.Start("explorer.exe", "ms-settings:about");
        }
    }
}
