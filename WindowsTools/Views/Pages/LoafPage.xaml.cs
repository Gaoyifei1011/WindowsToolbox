using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WindowsTools.Strings;

namespace WindowsTools.Views.Pages
{
    /// <summary>
    /// 摸鱼页面
    /// </summary>
    public sealed partial class LoafPage : Page, INotifyPropertyChanged
    {
        private bool _blockAllKeys = false;

        public bool BlockAllKeys
        {
            get { return _blockAllKeys; }

            set
            {
                _blockAllKeys = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BlockAllKeys)));
            }
        }

        private DictionaryEntry _selectedUpdateStyle;

        public DictionaryEntry SelectedUpdateStyle
        {
            get { return _selectedUpdateStyle; }

            set
            {
                _selectedUpdateStyle = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedUpdateStyle)));
            }
        }

        private List<DictionaryEntry> UpdateStyleList { get; } = new List<DictionaryEntry>()
        {
            new DictionaryEntry(Loaf.Windows11Style,"Windows11Style"),
            new DictionaryEntry(Loaf.Windows10Style,"Windows10Style"),
        };

        public event PropertyChangedEventHandler PropertyChanged;

        public LoafPage()
        {
            InitializeComponent();
            SelectedUpdateStyle = UpdateStyleList[0];
        }

        /// <summary>
        /// 开始摸鱼
        /// </summary>
        private void OnStartLoafClicked(object sender, RoutedEventArgs args)
        {
        }

        /// <summary>
        /// 选择模拟更新的界面风格
        /// </summary>
        private void OnUpdateStyleClicked(object sender, RoutedEventArgs args)
        {
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            if (item.Tag is not null)
            {
                SelectedUpdateStyle = UpdateStyleList[Convert.ToInt32(item.Tag)];
            }
        }

        /// <summary>
        /// 是否在模拟更新的时候屏蔽所有键盘按键
        /// </summary>
        private void OnBlockAllKeysToggled(object sender, RoutedEventArgs args)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch is not null)
            {
                BlockAllKeys = toggleSwitch.IsOn;
            }
        }
    }
}
