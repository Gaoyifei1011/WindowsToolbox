using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Text;
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
    public sealed partial class AboutPage : Page
    {
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
            bool isCreatedSuccessfully = false;

            Task.Run(() =>
            {
                try
                {
                    IWshRuntimeLibrary.IWshShell shell = new IWshRuntimeLibrary.WshShell();
                    IWshRuntimeLibrary.WshShortcut appShortcut = (IWshRuntimeLibrary.WshShortcut)shell.CreateShortcut(string.Format(@"{0}\{1}.lnk", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), About.AppName));
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
