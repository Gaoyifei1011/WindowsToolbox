using System.Collections.Generic;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WindowsTools.Extensions.DataType.Enums;
using WindowsTools.Services.Root;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace WindowsTools.Views.Pages
{
    /// <summary>
    /// 修改主题页面
    /// </summary>
    public sealed partial class ChangeThemePage : Page, INotifyPropertyChanged
    {
        private KeyValuePair<ElementTheme, string> _selectedSystemThemeStyle;

        public KeyValuePair<ElementTheme, string> SelectedSystemThemeStyle
        {
            get { return _selectedSystemThemeStyle; }

            set
            {
                if (!Equals(_selectedSystemThemeStyle, value))
                {
                    _selectedSystemThemeStyle = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedSystemThemeStyle)));
                }
            }
        }

        private KeyValuePair<ElementTheme, string> _selectedAppThemeStyle;

        public KeyValuePair<ElementTheme, string> SelectedAppThemeStyle
        {
            get { return _selectedAppThemeStyle; }

            set
            {
                if (!Equals(_selectedAppThemeStyle, value))
                {
                    _selectedAppThemeStyle = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedAppThemeStyle)));
                }
            }
        }

        private bool _showThemeColorInStartAndTaskbar = false;

        public bool ShowThemeColorInStartAndTaskbar
        {
            get { return _showThemeColorInStartAndTaskbar; }

            set
            {
                if (!Equals(_showThemeColorInStartAndTaskbar, value))
                {
                    _showThemeColorInStartAndTaskbar = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowThemeColorInStartAndTaskbar)));
                }
            }
        }

        private bool _isShowThemeColorInStartAndTaskbarEnabled = false;

        public bool IsShowThemeColorInStartAndTaskbarEnabled
        {
            get { return _isShowThemeColorInStartAndTaskbarEnabled; }

            set
            {
                if (!Equals(_isShowThemeColorInStartAndTaskbarEnabled, value))
                {
                    _isShowThemeColorInStartAndTaskbarEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsShowThemeColorInStartAndTaskbarEnabled)));
                }
            }
        }

        private List<KeyValuePair<ElementTheme, string>> SystemThemeStyleList { get; } =
        [
            new KeyValuePair<ElementTheme,string>(ElementTheme.Light,ResourceService.ChangeThemeResource.GetString("Light")),
            new KeyValuePair<ElementTheme,string>(ElementTheme.Dark,ResourceService.ChangeThemeResource.GetString("Dark")),
        ];

        private List<KeyValuePair<ElementTheme, string>> AppThemeStyleList { get; } =
        [
            new KeyValuePair<ElementTheme,string>(ElementTheme.Light,ResourceService.ChangeThemeResource.GetString("Light")),
            new KeyValuePair<ElementTheme,string>(ElementTheme.Dark,ResourceService.ChangeThemeResource.GetString("Dark")),
        ];

        public event PropertyChangedEventHandler PropertyChanged;

        public ChangeThemePage()
        {
            InitializeComponent();
            SelectedSystemThemeStyle = SystemThemeStyleList[0];
            SelectedAppThemeStyle = AppThemeStyleList[0];
        }

        #region 第一部分：修改主题页面——挂载的事件

        /// <summary>
        /// 刷新主题样式设置值
        /// </summary>
        private void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
        }

        /// <summary>
        /// 修改系统主题样式
        /// </summary>
        private void OnSystemThemeStyleClicked(object sender, RoutedEventArgs args)
        {
        }

        /// <summary>
        /// 修改应用主题样式
        /// </summary>
        private void OnAppThemeStyleClicked(object sender, RoutedEventArgs args)
        {
        }

        /// <summary>
        /// 在“开始菜单”和任务栏中显示主题色
        /// </summary>
        private void OnShowThemeColorInStartAndTaskbarToggled(object sender, RoutedEventArgs args)
        {
        }

        #endregion 第一部分：修改主题页面——挂载的事件
    }
}
