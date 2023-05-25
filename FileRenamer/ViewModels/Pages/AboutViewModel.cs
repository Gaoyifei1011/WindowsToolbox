using FileRenamer.Services.Root;
using FileRenamer.ViewModels.Base;
using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.System;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;

namespace FileRenamer.ViewModels.Pages
{
    /// <summary>
    /// 关于页面视图模型
    /// </summary>
    public sealed class AboutViewModel : ViewModelBase
    {
        private int _selectedIndex = 0;

        public int SelectedIndex
        {
            get { return _selectedIndex; }

            set
            {
                _selectedIndex = value;
                OnPropertyChanged();
            }
        }

        public List<string> TagList = new List<string>()
        {
            "Introduction",
            "Reference",
            "UseInstruction",
            "Precaution",
            "SettingsHelp",
            "Thanks",
        };

        /// <summary>
        /// 创建应用的桌面快捷方式
        /// </summary>
        public async void OnCreateDesktopShortcutClicked(object sender, RoutedEventArgs args)
        {
            //bool IsCreatedSuccessfully = false;

            IWshShell shell = new WshShell();
            WshShortcut AppShortcut = (WshShortcut)shell.CreateShortcut(string.Format(@"{0}\{1}.lnk", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), ResourceService.GetLocalized("Resources/AppDisplayName")));
            IReadOnlyList<AppListEntry> AppEntries = await Package.Current.GetAppListEntriesAsync();
            AppListEntry DefaultEntry = AppEntries[0];
            AppShortcut.TargetPath = string.Format(@"shell:AppsFolder\{0}", DefaultEntry.AppUserModelId);
            AppShortcut.Save();
            //new QuickOperationNotification(QuickOperationType.DesktopShortcut, IsCreatedSuccessfully).Show();
        }

        /// <summary>
        /// 将应用固定到“开始”屏幕
        /// </summary>
        public async void OnPinToStartScreenClicked(object sender, RoutedEventArgs args)
        {
            //bool IsPinnedSuccessfully = false;

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
                //IsPinnedSuccessfully = true;
            }
            catch (Exception)
            {
                //new QuickOperationNotification(QuickOperationType.StartScreen, IsPinnedSuccessfully).Show();
            }
        }

        // 将应用固定到任务栏
        //public void OnPinToTaskbarClicked(object sender,RoutedEventArgs args) { }

        /// <summary>
        /// 查看许可证
        /// </summary>
        public void OnShowLicenseClicked(object sender, RoutedEventArgs args)
        {
            //await new LicenseDialog().ShowAsync();
        }

        /// <summary>
        /// 查看更新日志
        /// </summary>
        public async void OnShowReleaseNotesClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/Gaoyifei1011/FileRenamer/releases"));
        }
    }
}
