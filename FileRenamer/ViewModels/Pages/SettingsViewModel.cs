using FileRenamer.UI.Dialogs.Settings;
using FileRenamer.ViewModels.Base;
using System.Collections.Generic;
using Windows.UI.Xaml;

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

        /// <summary>
        /// 打开重启应用确认的窗口对话框
        /// </summary>
        public async void OnRestartAppsClicked(object sender, RoutedEventArgs args)
        {
            await new RestartAppsDialog().ShowAsync();
        }
    }
}
