using FileRenamer.Contracts;
using FileRenamer.Extensions.Command;
using FileRenamer.UI.Dialogs.Settings;
using FileRenamer.ViewModels.Base;
using System.Collections.Generic;

namespace FileRenamer.ViewModels.Pages
{
    /// <summary>
    /// 设置页面数据模型
    /// </summary>
    public sealed class SettingsViewModel : ViewModelBase
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
            "Appearance",
            "General",
            "Advanced"
        };

        // 打开重启应用确认的窗口对话框
        public IRelayCommand RestartCommand = new RelayCommand(async () =>
        {
            await new RestartAppsDialog().ShowAsync();
        });
    }
}
