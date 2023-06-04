using FileRenamer.Helpers.Root;
using FileRenamer.Models.Controls.About;
using FileRenamer.ViewModels.Base;
using System;
using System.Collections.Generic;
using Windows.System;
using Windows.UI.Xaml;

namespace FileRenamer.ViewModels.Pages
{
    /// <summary>
    /// 关于页面视图模型
    /// </summary>
    public sealed class AboutViewModel : ViewModelBase
    {
        private readonly ushort MajorVersion = InfoHelper.GetAppVersion().MajorVersion;

        private readonly ushort MinorVersion = InfoHelper.GetAppVersion().MinorVersion;

        private readonly ushort BuildVersion = InfoHelper.GetAppVersion().BuildVersion;

        private readonly ushort RevisionVersion = InfoHelper.GetAppVersion().RevisionVersion;

        private string _appVersion;

        public string AppVersion
        {
            get { return _appVersion; }

            set
            {
                _appVersion = value;
                OnPropertyChanged();
            }
        }

        //项目引用信息
        public List<ReferenceKeyValuePairModel> ReferenceDict = new List<ReferenceKeyValuePairModel>()
        {
            new ReferenceKeyValuePairModel() {Key = "Mile.Xaml" , Value = "https://github.com/ProjectMile/Mile.Xaml" }
        };

        public List<ThanksKeyValuePairModel> ThanksDict = new List<ThanksKeyValuePairModel>()
        {
            new ThanksKeyValuePairModel() { Key ="AndromedaMelody", Value="https://github.com/AndromedaMelody"},
            new ThanksKeyValuePairModel() { Key = "MouriNaruto" , Value = "https://github.com/MouriNaruto" }
        };

        /// <summary>
        /// 检查更新
        /// </summary>
        public async void OnCheckUpdateClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/Gaoyifei1011/FileRenamer/releases"));
        }

        /// <summary>
        /// 开发者个人信息
        /// </summary>
        public async void OnDeveloperDescriptionClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/Gaoyifei1011"));
        }

        /// <summary>
        /// 初始化应用版本信息
        /// </summary>
        public void OnLoaded(object sender, RoutedEventArgs args)
        {
            AppVersion = string.Format("{0}.{1}.{2}.{3}", MajorVersion, MinorVersion, BuildVersion, RevisionVersion);
        }

        /// <summary>
        /// 项目主页
        /// </summary>
        public async void OnProjectDescriptionClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/Gaoyifei1011/FileRenamer"));
        }

        /// <summary>
        /// 发送反馈
        /// </summary>
        public async void OnSendFeedbackClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/Gaoyifei1011/FileRenamer/issues"));
        }
    }
}
