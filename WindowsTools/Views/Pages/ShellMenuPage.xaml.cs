using System;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using WindowsTools.Helpers.Controls.Extensions;
using WindowsTools.Services.Root;
using WindowsTools.Services.Shell;
using WindowsTools.Strings;
using WindowsTools.UI.TeachingTips;
using WindowsTools.Views.Windows;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace WindowsTools.Views.Pages
{
    /// <summary>
    /// 自定义扩展菜单页面
    /// </summary>
    public sealed partial class ShellMenuPage : Page, INotifyPropertyChanged
    {
        private BitmapImage _rootMenuImage;

        public BitmapImage RootMenuImage
        {
            get { return _rootMenuImage; }

            set
            {
                if (!Equals(_rootMenuImage, value))
                {
                    _rootMenuImage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RootMenuImage)));
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RootMenuText)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ShellMenuPage()
        {
            InitializeComponent();
            try
            {
                RootMenuImage = new() { UriSource = new Uri(ShellMenuService.RootMenuIconPath) };
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, "Set root menu icon failed", e);
            }

            RootMenuText = ShellMenuService.RootMenuText;
        }

        #region 第一部分：自定义扩展菜单页面——挂载的事件

        /// <summary>
        /// 修改根菜单图标
        /// </summary>
        private void OnModifyClicked(object sender, RoutedEventArgs args)
        {
            OpenFileDialog dialog = new()
            {
                Multiselect = false,
                Filter = ShellMenu.FilterCondition,
                Title = ShellMenu.SelectIcon
            };
            if (dialog.ShowDialog() is DialogResult.OK && !string.IsNullOrEmpty(dialog.FileName))
            {
                try
                {
                    Task.Run(() =>
                    {
                        string rootMenuFilePath = Path.Combine(ShellMenuService.ShellMenuConfigDirectory.FullName, "RootMenu.png");
                        File.Copy(dialog.FileName, rootMenuFilePath, true);
                        ShellMenuService.SetRootMenuIcon(false, rootMenuFilePath);

                        MainWindow.Current.BeginInvoke(() =>
                        {
                            RootMenuImage = new() { UriSource = new Uri(rootMenuFilePath) };
                        });
                    });
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Set root menu icon failed", e);
                }
            }
        }

        /// <summary>
        /// 还原默认
        /// </summary>
        private void OnReturnDefaultClicked(object sender, RoutedEventArgs args)
        {
            global::Windows.UI.Xaml.Controls.Button button = sender as global::Windows.UI.Xaml.Controls.Button;

            if (button is not null)
            {
                string tag = Convert.ToString(button.Tag);

                Task.Run(() =>
                {
                    if (tag.Equals("Icon"))
                    {
                        ShellMenuService.SetRootMenuIcon(true, string.Empty);

                        try
                        {
                            string rootMenuFilePath = Path.Combine(ShellMenuService.ShellMenuConfigDirectory.FullName, "RootMenu.png");

                            if (File.Exists(rootMenuFilePath))
                            {
                                File.Delete(rootMenuFilePath);
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Warning, "Delete RootMenu.png failed", e);
                        }

                        MainWindow.Current.BeginInvoke(() =>
                        {
                            RootMenuImage = new() { UriSource = new Uri(ShellMenuService.RootMenuIconPath) };
                        });
                    }
                    else if (tag.Equals("Text"))
                    {
                        ShellMenuService.SetRootMenuText(true, string.Empty);

                        MainWindow.Current.BeginInvoke(() =>
                        {
                            RootMenuText = ShellMenuService.RootMenuText;
                        });
                    }
                });
            }
        }

        /// <summary>
        /// 根菜单文字设置框文本内容发生变化时的事件
        /// </summary>
        private void OnRootMenuTextChanged(object sender, TextChangedEventArgs args)
        {
            RootMenuText = (sender as global::Windows.UI.Xaml.Controls.TextBox).Text;
        }

        /// <summary>
        /// 应用修改的根菜单名称
        /// </summary>
        private void OnApplyClicked(object sender, RoutedEventArgs args)
        {
            if (string.IsNullOrEmpty(RootMenuText))
            {
                RootMenuText = ShellMenuService.RootMenuText;
                TeachingTipHelper.Show(new TextEmptyTip());
                return;
            }

            Task.Run(() =>
            {
                ShellMenuService.SetRootMenuText(false, RootMenuText);
            });
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
