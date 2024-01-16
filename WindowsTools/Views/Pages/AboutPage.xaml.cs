using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.UI.Shell;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WindowsTools.Extensions.DataType.Enums;
using WindowsTools.Helpers.Controls;
using WindowsTools.Helpers.Controls.Extensions;
using WindowsTools.Helpers.Root;
using WindowsTools.Strings;
using WindowsTools.UI.Dialogs;
using WindowsTools.UI.Dialogs.About;
using WindowsTools.UI.TeachingTips;
using WindowsTools.Views.Windows;

namespace WindowsTools.Views.Pages
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

        #region 第一部分：关于页面——挂载的事件

        /// <summary>
        /// 固定应用到桌面
        /// </summary>
        private void OnPinToDesktopClicked(object sender, RoutedEventArgs args)
        {
            bool IsCreatedSuccessfully = false;

            Task.Run(async () =>
            {
                try
                {
                    IWshRuntimeLibrary.IWshShell shell = new IWshRuntimeLibrary.WshShell();
                    IWshRuntimeLibrary.WshShortcut AppShortcut = (IWshRuntimeLibrary.WshShortcut)shell.CreateShortcut(string.Format(@"{0}\{1}.lnk", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), About.AppName));
                    IReadOnlyList<AppListEntry> AppEntries = await Package.Current.GetAppListEntriesAsync();
                    AppListEntry DefaultEntry = AppEntries[0];
                    AppShortcut.TargetPath = string.Format(@"shell:AppsFolder\{0}", DefaultEntry.AppUserModelId);
                    AppShortcut.Save();
                    IsCreatedSuccessfully = true;
                }
                catch (Exception) { }
                finally
                {
                    MainWindow.Current.BeginInvoke(() =>
                    {
                        TeachingTipHelper.Show(new QuickOperationTip(QuickOperationKind.Desktop, IsCreatedSuccessfully));
                    });
                }
            });
        }

        /// <summary>
        /// 将应用固定到“开始”屏幕
        /// </summary>
        private void OnPinToStartScreenClicked(object sender, RoutedEventArgs args)
        {
            bool IsPinnedSuccessfully = false;

            Task.Run(async () =>
            {
                try
                {
                    IReadOnlyList<AppListEntry> AppEntries = await Package.Current.GetAppListEntriesAsync();

                    AppListEntry DefaultEntry = AppEntries[0];

                    if (DefaultEntry is not null)
                    {
                        StartScreenManager startScreenManager = StartScreenManager.GetDefault();

                        bool containsEntry = await startScreenManager.ContainsAppListEntryAsync(DefaultEntry);

                        if (!containsEntry)
                        {
                            await startScreenManager.RequestAddAppListEntryAsync(DefaultEntry);
                        }
                    }
                    IsPinnedSuccessfully = true;
                }
                catch (Exception) { }
                finally
                {
                    MainWindow.Current.BeginInvoke(() =>
                    {
                        TeachingTipHelper.Show(new QuickOperationTip(QuickOperationKind.StartScreen, IsPinnedSuccessfully));
                    });
                }
            });
        }

        /// <summary>
        /// 将应用固定到任务栏
        /// </summary>
        private async void OnPinToTaskbarClicked(object sender, RoutedEventArgs args)
        {
            bool IsPinnedSuccessfully = false;

            try
            {
                string featureId = "com.microsoft.windows.taskbar.pin";
                string token = FeatureAccessHelper.GenerateTokenFromFeatureId(featureId);
                string attestation = FeatureAccessHelper.GenerateAttestation(featureId);
                LimitedAccessFeatureRequestResult accessResult = LimitedAccessFeatures.TryUnlockFeature(featureId, token, attestation);

                if (accessResult.Status is LimitedAccessFeatureStatus.Available)
                {
                    IsPinnedSuccessfully = await TaskbarManager.GetDefault().RequestPinCurrentAppAsync();
                }
            }
            catch (Exception) { }
            finally
            {
                TeachingTipHelper.Show(new QuickOperationTip(QuickOperationKind.Taskbar, IsPinnedSuccessfully));
            }
        }

        /// <summary>
        /// 查看许可证
        /// </summary>
        private async void OnShowLicenseClicked(object sender, RoutedEventArgs args)
        {
            await ContentDialogHelper.ShowAsync(new LicenseDialog(), this);
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
        /// 检查更新
        /// </summary>
        private void OnCheckUpdateClicked(object sender, RoutedEventArgs args)
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
        /// 系统信息
        /// </summary>
        private void OnSystemInformationClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                Process.Start("ms-settings:about");
            });
        }

        #endregion 第一部分：关于页面——挂载的事件
    }
}
