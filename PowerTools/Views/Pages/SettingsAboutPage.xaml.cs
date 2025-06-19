using PowerTools.Extensions.DataType.Enums;
using PowerTools.Helpers.Root;
using PowerTools.Services.Root;
using PowerTools.Views.Dialogs;
using PowerTools.Views.TeachingTips;
using PowerTools.Views.Windows;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// 抑制 CA1806，CA1822，IDE0060 警告
#pragma warning disable CA1806,CA1822,IDE0060

namespace PowerTools.Views.Pages
{
    /// <summary>
    /// 关于页面
    /// </summary>
    public sealed partial class SettingsAboutPage : Page, INotifyPropertyChanged
    {
        private bool _isChecking;

        public bool IsChecking
        {
            get { return _isChecking; }

            set
            {
                if (!Equals(_isChecking, value))
                {
                    _isChecking = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(IsChecking)));
                }
            }
        }

        //项目引用信息
        private ListDictionary ReferenceList { get; } = new()
        {
            { "Microsoft.UI.Xaml", new Uri("https://github.com/microsoft/microsoft-ui-xaml") },
            { "Microsoft.Windows.SDK.BuildTools", new Uri("https://aka.ms/WinSDKProjectURL") },
            { "Microsoft.Windows.SDK.Contracts", new Uri("https://aka.ms/WinSDKProjectURL") },
            { "System.Private.Uri", new Uri("https://dot.net") }
        };

        //项目感谢者信息
        private ListDictionary ThanksList { get; } = new()
        {
            { "AndromedaMelody", new Uri("https://github.com/AndromedaMelody") },
            { "Blinue", new Uri("https://github.com/Blinue") },
            { "cnbluefire", new Uri("https://github.com/cnbluefire") },
            { "MicaApps", new Uri("https://github.com/MicaApps") },
            { "MouriNaruto" , new Uri("https://github.com/MouriNaruto") }
        };

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsAboutPage()
        {
            InitializeComponent();
        }

        #region 第一部分：关于页面——挂载的事件

        /// <summary>
        /// 查看更新日志
        /// </summary>
        private void OnShowReleaseNotesClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("https://github.com/Gaoyifei1011/WindowsToolbox/releases");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Open show release notes url failed", e);
                }
            });
        }

        /// <summary>
        /// 应用信息
        /// </summary>
        private async void OnAppInformationClicked(object sender, RoutedEventArgs args)
        {
            await MainWindow.Current.ShowDialogAsync(new AppInformationDialog());
        }

        /// <summary>
        /// 系统信息
        /// </summary>
        private void OnSystemInformationClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("ms-settings:about");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Open system information failed", e);
                }
            });
        }

        /// <summary>
        /// 查看许可证
        /// </summary>
        private async void OnShowLicenseClicked(object sender, RoutedEventArgs args)
        {
            await MainWindow.Current.ShowDialogAsync(new LicenseDialog());
        }

        /// <summary>
        /// 帮助翻译应用
        /// </summary>
        private void OnHelpTranslateClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("https://github.com/Gaoyifei1011/WindowsToolbox/releases");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Open help translate url failed", e);
                }
            });
        }

        /// <summary>
        /// 项目主页
        /// </summary>
        private void OnProjectDescriptionClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("https://github.com/Gaoyifei1011/WindowsToolbox/releases");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Open project description url failed", e);
                }
            });
        }

        /// <summary>
        /// 发送反馈
        /// </summary>
        private void OnSendFeedbackClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("https://github.com/Gaoyifei1011/WindowsToolbox/releases");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Open send feedback url failed", e);
                }
            });
        }

        /// <summary>
        /// 检查更新
        /// </summary>
        private async void OnCheckUpdateClicked(object sender, RoutedEventArgs args)
        {
            if (!IsChecking)
            {
                IsChecking = true;

                bool? isNewest = await Task.Run<bool?>(async () =>
                {
                    bool isNewest = false;

                    try
                    {
                        HttpClient httpClient = new();
                        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.71 Safari/537.36");
                        httpClient.Timeout = new TimeSpan(0, 0, 30);
                        HttpResponseMessage responseMessage = await httpClient.GetAsync(new Uri("https://api.github.com/repos/Gaoyifei1011/WindowsToolbox/releases/latest"));

                        // 请求成功
                        if (responseMessage.IsSuccessStatusCode)
                        {
                            string responseString = await responseMessage.Content.ReadAsStringAsync();
                            httpClient.Dispose();
                            responseMessage.Dispose();
                            Regex tagRegex = new(@"""tag_name"":[\s]*""v(\d.\d.\d{3,}.\d)""", RegexOptions.IgnoreCase | RegexOptions.Compiled);
                            MatchCollection tagCollection = tagRegex.Matches(responseString);

                            if (tagCollection.Count > 0)
                            {
                                GroupCollection tagGroups = tagCollection[0].Groups;

                                if (tagGroups.Count > 0 && new Version(tagGroups[1].Value) is Version tagVersion)
                                {
                                    isNewest = InfoHelper.AppVersion >= tagVersion;
                                }
                            }
                        }
                        else
                        {
                            httpClient.Dispose();
                            responseMessage.Dispose();
                        }

                        return isNewest;
                    }
                    // 捕捉因为网络失去链接获取信息时引发的异常
                    catch (COMException e)
                    {
                        LogService.WriteLog(EventLevel.Informational, "Check update request failed", e);
                        return null;
                    }

                    // 捕捉因访问超时引发的异常
                    catch (TaskCanceledException e)
                    {
                        LogService.WriteLog(EventLevel.Informational, "Check update request timeout", e);
                        return null;
                    }

                    // 其他异常
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Warning, "Check update request unknown exception", e);
                        return null;
                    }
                });

                IsChecking = false;
                await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.CheckUpdate, isNewest.Value));
            }
        }

        #endregion 第一部分：关于页面——挂载的事件
    }
}
