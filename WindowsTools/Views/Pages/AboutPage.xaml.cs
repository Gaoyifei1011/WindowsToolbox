using IWshRuntimeLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WindowsTools.Extensions.DataType.Enums;
using WindowsTools.Helpers.Controls;
using WindowsTools.Helpers.Controls.Extensions;
using WindowsTools.Helpers.Root;
using WindowsTools.Services.Root;
using WindowsTools.Strings;
using WindowsTools.UI.Dialogs;
using WindowsTools.UI.Dialogs.About;
using WindowsTools.UI.TeachingTips;
using WindowsTools.Views.Windows;
using WindowsTools.WindowsAPI.ComTypes;
using WindowsTools.WindowsAPI.PInvoke.Kernel32;

namespace WindowsTools.Views.Pages
{
    /// <summary>
    /// 关于页面
    /// </summary>
    public sealed partial class AboutPage : Page, INotifyPropertyChanged
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

        private static Guid taskbarPinCLSID = new Guid("90AA3A4E-1CBA-4233-B8BB-535773D48449");
        private static Guid ishellLinkCLSID = new Guid("00021401-0000-0000-C000-000000000046");

        //项目引用信息
        private Hashtable ReferenceDict { get; } = new Hashtable()
        {
            { "Mile.Xaml","https://github.com/ProjectMile/Mile.Xaml" },
            { "Microsoft.UI.Xaml","https://github.com/microsoft/microsoft-ui-xaml" },
            { "Microsoft.WindowsAppSDK","https://github.com/microsoft/windowsappsdk" },
            { "ZXing.Net","https://github.com/micjahn/ZXing.Net" },
        };

        //项目感谢者信息
        private Hashtable ThanksDict { get; } = new Hashtable()
        {
            { "AndromedaMelody","https://github.com/AndromedaMelody" },
            { "MicaApps","https://github.com/MicaApps" },
            { "MouriNaruto" , "https://github.com/MouriNaruto" }
        };

        public event PropertyChangedEventHandler PropertyChanged;

        public AboutPage()
        {
            InitializeComponent();
        }

        #region 第一部分：关于页面——挂载的事件

        /// <summary>
        /// 固定应用到桌面
        /// </summary>
        private void OnPinToDesktopClicked(object sender, RoutedEventArgs args)
        {
            bool isCreatedSuccessfully = false;

            Task.Run(() =>
            {
                try
                {
                    WshShell shell = new WshShell();
                    WshShortcut appShortcut = (WshShortcut)shell.CreateShortcut(string.Format(@"{0}\{1}.lnk", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), About.AppName));
                    uint aumidLength = 260;
                    StringBuilder aumidBuilder = new StringBuilder((int)aumidLength);
                    Kernel32Library.GetCurrentApplicationUserModelId(ref aumidLength, aumidBuilder);
                    appShortcut.TargetPath = string.Format(@"shell:AppsFolder\{0}", aumidBuilder.ToString());
                    appShortcut.Save();
                    isCreatedSuccessfully = true;
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Create desktop shortcut failed.", e);
                }
                finally
                {
                    MainWindow.Current.BeginInvoke(() =>
                    {
                        TeachingTipHelper.Show(new QuickOperationTip(QuickOperationKind.Desktop, isCreatedSuccessfully));
                    });
                }
            });
        }

        /// <summary>
        /// 将应用固定到“开始”屏幕
        /// </summary>
        private async void OnPinToStartScreenClicked(object sender, RoutedEventArgs args)
        {
            bool isPinnedSuccessfully = false;

            try
            {
                IReadOnlyList<AppListEntry> appEntries = await Package.Current.GetAppListEntriesAsync();

                AppListEntry defaultEntry = appEntries[0];

                if (defaultEntry is not null)
                {
                    StartScreenManager startScreenManager = StartScreenManager.GetDefault();

                    bool containsEntry = await startScreenManager.ContainsAppListEntryAsync(defaultEntry);

                    if (!containsEntry)
                    {
                        await startScreenManager.RequestAddAppListEntryAsync(defaultEntry);
                    }

                    isPinnedSuccessfully = true;
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, "Pin app to startscreen failed.", e);
            }
            finally
            {
                MainWindow.Current.BeginInvoke(() =>
                {
                    TeachingTipHelper.Show(new QuickOperationTip(QuickOperationKind.StartScreen, isPinnedSuccessfully));
                });
            }
        }

        /// <summary>
        /// 将应用固定到任务栏
        /// </summary>
        private void OnPinToTaskbarClicked(object sender, RoutedEventArgs args)
        {
            bool isPinnedSuccessfully = false;

            try
            {
                IShellLink appLink = (IShellLink)Activator.CreateInstance(Type.GetTypeFromCLSID(ishellLinkCLSID));
                uint aumidLength = 260;
                StringBuilder aumidBuilder = new StringBuilder((int)aumidLength);
                Kernel32Library.GetCurrentApplicationUserModelId(ref aumidLength, aumidBuilder);
                appLink.SetPath(string.Format(@"shell:AppsFolder\{0}", aumidBuilder.ToString()));
                appLink.GetIDList(out IntPtr pidl);

                IPinnedList3 pinnedList = (IPinnedList3)Activator.CreateInstance(Type.GetTypeFromCLSID(taskbarPinCLSID));

                if (pinnedList is not null)
                {
                    PIDLIST_ABSOLUTE pidlFrom = new PIDLIST_ABSOLUTE();
                    pidlFrom.Ptr = IntPtr.Zero;
                    PIDLIST_ABSOLUTE pidlTo = new PIDLIST_ABSOLUTE();
                    pidlTo.Ptr = pidl;
                    isPinnedSuccessfully = pinnedList.Modify(pidlFrom, pidlTo, PLMC.PLMC_EXPLORER) is 0;
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, "Pin app to taskbar failed.", e);
            }
            finally
            {
                TeachingTipHelper.Show(new QuickOperationTip(QuickOperationKind.Taskbar, isPinnedSuccessfully));
            }
        }

        /// <summary>
        /// 查看更新日志
        /// </summary>
        private void OnShowReleaseNotesClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                Process.Start("https://github.com/Gaoyifei1011/WindowsTools/releases");
            });
        }

        /// <summary>
        /// 查看许可证
        /// </summary>
        private async void OnShowLicenseClicked(object sender, RoutedEventArgs args)
        {
            await ContentDialogHelper.ShowAsync(new LicenseDialog(), this);
        }

        /// <summary>
        /// 帮助翻译应用
        /// </summary>
        private void OnHelpTranslateClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                Process.Start("https://github.com/Gaoyifei1011/WindowsTools/releases");
            });
        }

        /// <summary>
        /// 项目主页
        /// </summary>
        private void OnProjectDescriptionClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                Process.Start("https://github.com/Gaoyifei1011/WindowsTools");
            });
        }

        /// <summary>
        /// 发送反馈
        /// </summary>
        private void OnSendFeedbackClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                Process.Start("https://github.com/Gaoyifei1011/WindowsTools/issues");
            });
        }

        /// <summary>
        /// 检查更新
        /// </summary>
        private void OnCheckUpdateClicked(object sender, RoutedEventArgs args)
        {
            if (!IsChecking)
            {
                IsChecking = true;

                Task.Run(async () =>
                {
                    try
                    {
                        HttpClient httpClient = new HttpClient();
                        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.71 Safari/537.36");
                        httpClient.Timeout = new TimeSpan(0, 0, 30);
                        HttpResponseMessage responseMessage = await httpClient.GetAsync(new Uri("https://api.github.com/repos/Gaoyifei1011/WindowsToolbox/releases/latest"));

                        // 请求成功
                        if (responseMessage.IsSuccessStatusCode)
                        {
                            StringBuilder responseBuilder = new StringBuilder();

                            responseBuilder.Append("Status Code:");
                            responseBuilder.AppendLine(responseMessage.StatusCode.ToString());
                            responseBuilder.Append("Headers:");
                            responseBuilder.AppendLine(responseMessage.Headers is null ? "" : responseMessage.Headers.ToString().Replace('\r', ' ').Replace('\n', ' '));
                            responseBuilder.Append("ResponseMessage:");
                            responseBuilder.AppendLine(responseMessage.RequestMessage is null ? "" : responseMessage.RequestMessage.ToString().Replace('\r', ' ').Replace('\n', ' '));

                            string responseString = await responseMessage.Content.ReadAsStringAsync();
                            httpClient.Dispose();
                            responseMessage.Dispose();

                            Regex tagRegex = new Regex(@"""tag_name"":[\s]*""v(\d.\d.\d{3}.\d)""");

                            MatchCollection tagCollection = tagRegex.Matches(responseString);

                            if (tagCollection.Count > 0)
                            {
                                GroupCollection tagGroups = tagCollection[0].Groups;

                                if (tagGroups.Count > 0)
                                {
                                    Version tagVersion = new Version(tagGroups[1].Value);

                                    if (tagVersion is not null)
                                    {
                                        bool isNewest = InfoHelper.AppVersion >= tagVersion;

                                        MainWindow.Current.BeginInvoke(() =>
                                        {
                                            TeachingTipHelper.Show(new CheckUpdateTip(isNewest));
                                        });
                                    }
                                }
                            }
                        }
                        else
                        {
                            httpClient.Dispose();
                            responseMessage.Dispose();
                        }
                    }
                    // 捕捉因为网络失去链接获取信息时引发的异常
                    catch (COMException e)
                    {
                        LogService.WriteLog(EventLevel.Informational, "Check update request failed", e);
                    }

                    // 捕捉因访问超时引发的异常
                    catch (TaskCanceledException e)
                    {
                        LogService.WriteLog(EventLevel.Informational, "Check update request timeout", e);
                    }

                    // 其他异常
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Warning, "Check update request unknown exception", e);
                    }
                    finally
                    {
                        MainWindow.Current.BeginInvoke(() =>
                        {
                            IsChecking = false;
                        });
                    }
                });
            }
        }

        /// <summary>
        /// 系统信息
        /// </summary>
        private void OnSystemInformationClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                Process.Start("ms-settings:about");
            });
        }

        /// <summary>
        /// 应用信息
        /// </summary>
        private async void OnAppInformationClicked(object sender, RoutedEventArgs args)
        {
            await ContentDialogHelper.ShowAsync(new AppInformationDialog(), this);
        }

        /// <summary>
        /// 应用设置
        /// </summary>
        private void OnAppSettingsClicked(object sender, RoutedEventArgs args)
        {
            if (RuntimeHelper.IsElevated)
            {
                Task.Run(() =>
                {
                    Process.Start("ms-settings:appsfeatures-app");
                });
            }
        }

        /// <summary>
        /// 疑难解答
        /// </summary>
        private void OnTroubleShootClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                Process.Start("ms-settings:troubleshoot");
            });
        }

        #endregion 第一部分：关于页面——挂载的事件
    }
}
