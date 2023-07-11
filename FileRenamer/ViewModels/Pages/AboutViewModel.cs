﻿using FileRenamer.Helpers.Root;
using FileRenamer.Models.Base;
using FileRenamer.UI.Dialogs.About;
using FileRenamer.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.System;
using Windows.UI.Xaml;

namespace FileRenamer.ViewModels.Pages
{
    /// <summary>
    /// 关于页面视图模型
    /// </summary>
    public sealed class AboutViewModel : ViewModelBase
    {
        private readonly int MajorVersion = InfoHelper.AppVersion.Major;

        private readonly int MinorVersion = InfoHelper.AppVersion.Minor;

        private readonly int BuildVersion = InfoHelper.AppVersion.Build;

        private readonly int RevisionVersion = InfoHelper.AppVersion.Revision;

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
        public List<KeyValuePairModel> ReferenceDict = new List<KeyValuePairModel>()
        {
            new KeyValuePairModel() {Key = "Mile.Xaml" , Value = "https://github.com/ProjectMile/Mile.Xaml" }
        };

        public List<KeyValuePairModel> ThanksDict = new List<KeyValuePairModel>()
        {
            new KeyValuePairModel() { Key ="AndromedaMelody", Value="https://github.com/AndromedaMelody"},
            new KeyValuePairModel() { Key = "MouriNaruto" , Value = "https://github.com/MouriNaruto" }
        };

        /// <summary>
        /// 应用信息
        /// </summary>
        public async void OnAppInformationClicked(object sender, RoutedEventArgs args)
        {
            await new AppInformationDialog().ShowAsync();
        }

        /// <summary>
        /// 应用设置
        /// </summary>
        public async void OnAppSettingsClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:appsfeatures-app"));
        }

        /// <summary>
        /// 检查更新
        /// </summary>
        public void OnCheckUpdateClicked(object sender, RoutedEventArgs args)
        {
            Process.Start("explorer.exe", "https://github.com/Gaoyifei1011/FileRenamer/releases");
        }

        /// <summary>
        /// 开发者个人信息
        /// </summary>
        public void OnDeveloperDescriptionClicked(object sender, RoutedEventArgs args)
        {
            Process.Start("explorer.exe", "https://github.com/Gaoyifei1011");
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
