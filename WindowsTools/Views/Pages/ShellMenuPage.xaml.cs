using System;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace WindowsTools.Views.Pages
{
    /// <summary>
    /// 自定义扩展菜单页面
    /// </summary>
    public sealed partial class ShellMenuPage : Page, INotifyPropertyChanged
    {
        private BitmapImage _iconImage;

        public BitmapImage IconImage
        {
            get { return _iconImage; }

            set
            {
                if (!Equals(_iconImage, value))
                {
                    _iconImage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IconImage)));
                }
            }
        }

        private string _rootMenuText;

        public string RootMenuText
        {
            get { return _rootMenuText; }

            set
            {
                if (!Equals(_rootMenuText, value))
                {
                    _rootMenuText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IconImage)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ShellMenuPage()
        {
            InitializeComponent();
        }

        #region 第一部分：自定义扩展菜单页面——挂载的事件

        /// <summary>
        /// 修改根菜单图标
        /// </summary>
        private void OnModifyClicked(object sender, RoutedEventArgs args)
        {
        }

        /// <summary>
        /// 还原默认
        /// </summary>
        private void OnReturnDefaultClicked(object sender, RoutedEventArgs args)
        {
            Button button = sender as Button;

            if (button is not null)
            {
                string tag = Convert.ToString(button.Tag);

                if (tag.Equals("Icon"))
                {
                }
                else if (tag.Equals("Text"))
                {
                }
            }
        }

        /// <summary>
        /// 应用修改的根菜单名称
        /// </summary>
        private void OnApplyClicked(object sender, RoutedEventArgs args)
        {
        }

        /// <summary>
        /// 添加菜单
        /// </summary>
        private void OnAddMenuClicked(object sender, RoutedEventArgs args)
        {
        }

        /// <summary>
        /// 应用更改
        /// </summary>
        private void OnAddModifyClicked(object sender, RoutedEventArgs args)
        {
        }

        /// <summary>
        /// 刷新列表
        /// </summary>
        private void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
        }

        #endregion 第一部分：自定义扩展菜单页面——挂载的事件
    }
}
