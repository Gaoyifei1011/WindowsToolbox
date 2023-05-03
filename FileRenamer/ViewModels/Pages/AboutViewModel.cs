using FileRenamer.Contracts;
using FileRenamer.Extensions.Command;
using FileRenamer.Services.Root;
using FileRenamer.ViewModels.Base;
using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.System;
using Windows.UI.StartScreen;

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

        // 创建应用的桌面快捷方式
        public IRelayCommand CreateDesktopShortcutCommand => new RelayCommand(async () =>
        {
            //bool IsCreatedSuccessfully = false;

            IWshShell shell = new WshShell();
            WshShortcut AppShortcut = (WshShortcut)shell.CreateShortcut(string.Format(@"{0}\{1}.lnk", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), ResourceService.GetLocalized("Resources/AppDisplayName")));
            IReadOnlyList<AppListEntry> AppEntries = await Package.Current.GetAppListEntriesAsync();
            AppListEntry DefaultEntry = AppEntries[0];
            AppShortcut.TargetPath = string.Format(@"shell:AppsFolder\{0}", DefaultEntry.AppUserModelId);
            AppShortcut.Save();
            //new QuickOperationNotification(QuickOperationType.DesktopShortcut, IsCreatedSuccessfully).Show();
        });

        // 将应用固定到“开始”屏幕
        public IRelayCommand PinToStartScreenCommand => new RelayCommand(async () =>
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
        });

        // 将应用固定到任务栏
        public IRelayCommand PinToTaskbarCommand => new RelayCommand(() => { });

        // 查看更新日志
        public IRelayCommand ShowReleaseNotesCommand => new RelayCommand(async () =>
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/Gaoyifei1011/FileRenamer/releases"));
        });

        // 查看许可证
        public IRelayCommand ShowLicenseCommand => new RelayCommand(() =>
        {
            //await new LicenseDialog().ShowAsync();
        });
    }
}
