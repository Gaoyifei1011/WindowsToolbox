using FileRenamer.Contracts;
using FileRenamer.Extensions.Command;
using FileRenamer.ViewModels.Base;
using System;
using Windows.System;

namespace FileRenamer.ViewModels.Controls.Settings
{
    public sealed class ThemeViewModel : ViewModelBase
    {
        // 打开系统主题设置
        public IRelayCommand SettingsColorCommand => new RelayCommand(async () =>
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:colors"));
        });
    }
}
