using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using WindowsTools.Extensions.DataType.Enums;
using WindowsTools.Helpers.Controls.Extensions;
using WindowsTools.Models;
using WindowsTools.Services.Root;
using WindowsTools.Services.Shell;
using WindowsTools.Strings;
using WindowsTools.UI.TeachingTips;
using WindowsTools.Views.Windows;
using WindowsTools.WindowsAPI.PInvoke.Shell32;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace WindowsTools.Views.Pages
{
    /// <summary>
    /// 自定义扩展菜单页面
    /// </summary>
    public sealed partial class ShellMenuPage : Page, INotifyPropertyChanged
    {
        private Guid firstLevelMenuGuid = Guid.Empty;
        private Guid secondLevelMenuGuid = Guid.Empty;

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

        public ObservableCollection<DictionaryEntry> BreadCollection { get; } =
        [
            new DictionaryEntry(ShellMenu.Title, "Title")
        ];

        private ObservableCollection<ShellMenuItemModel> ShellMenuItemCollection { get; } = [];

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

            Icon icon = Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);

            MemoryStream memoryStream = new();
            icon.ToBitmap().Save(memoryStream, ImageFormat.Png);
            memoryStream.Seek(0, SeekOrigin.Begin);
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.SetSource(memoryStream.AsRandomAccessStream());
            ShellMenuItemCollection.Add(new ShellMenuItemModel()
            {
                Title = "Windows 工具箱",
                MenuIndex = 0,
                ProgramPath = System.Windows.Forms.Application.ExecutablePath,
                IconPath = System.Windows.Forms.Application.ExecutablePath,
                Icon = bitmapImage
            });
        }

        #region 第一部分：重写父类事件

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);

            string parameter = args.Parameter as string;

            if (!string.IsNullOrEmpty(parameter))
            {
                while (BreadCollection.Count > 1)
                {
                    BreadCollection.RemoveAt(1);
                }
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 编辑菜单项
        /// </summary>
        private void OnEditExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            int menuIndex = Convert.ToInt32(args.Parameter);

            if (BreadCollection.Count is 1)
            {
                BreadCollection.Add(new DictionaryEntry(ShellMenu.FirstLevelMenu, "FirstLevelMenu"));
            }
            else if (BreadCollection.Count is 2)
            {
                BreadCollection.Add(new DictionaryEntry(ShellMenu.SecondLevelMenu, "SecondLevelMenu"));
            }
        }

        /// <summary>
        /// 定位菜单项
        /// </summary>
        private void OnOpenProgramPathRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            string filePath = args.Parameter as string;

            if (!string.IsNullOrEmpty(filePath))
            {
                Task.Run(() =>
                {
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        if (File.Exists(filePath))
                        {
                            IntPtr pidlList = Shell32Library.ILCreateFromPath(filePath);
                            if (pidlList != IntPtr.Zero)
                            {
                                Shell32Library.SHOpenFolderAndSelectItems(pidlList, 0, IntPtr.Zero, 0);
                                Shell32Library.ILFree(pidlList);
                            }
                        }
                        else
                        {
                            string directoryPath = Path.GetDirectoryName(filePath);

                            if (Directory.Exists(directoryPath))
                            {
                                Process.Start(directoryPath);
                            }
                            else
                            {
                                Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                            }
                        }
                    }
                });
            }
        }

        #endregion 第二部分：XamlUICommand 命令调用时挂载的事件

        #region 第三部分：根菜单页面——挂载的事件

        /// <summary>
        /// 单击痕迹栏条目时发生的事件
        /// </summary>
        private void OnItemClicked(object sender, BreadcrumbBarItemClickedEventArgs args)
        {
            DictionaryEntry breadItem = (DictionaryEntry)args.Item;
            if (BreadCollection.Count is 3)
            {
                if (breadItem.Value.Equals(BreadCollection[0].Value))
                {
                    BreadCollection.RemoveAt(2);
                    BreadCollection.RemoveAt(1);
                }
                else if (breadItem.Value.Equals(BreadCollection[1].Value))
                {
                    BreadCollection.RemoveAt(1);
                }
            }
            else if (BreadCollection.Count is 2)
            {
                if (breadItem.Value.Equals(BreadCollection[0].Value))
                {
                    BreadCollection.RemoveAt(1);
                }
            }
        }

        /// <summary>
        /// 打开菜单设置
        /// </summary>
        private void OnMenuSettingsClicked(object sender, RoutedEventArgs args)
        {
            (MainWindow.Current.Content as MainPage).NavigateTo(typeof(SettingsPage));
        }

        /// <summary>
        /// 保存更改
        /// </summary>
        private void OnSaveClicked(object sender, RoutedEventArgs args)
        {
            // 保存一级菜单内容
            if (BreadCollection.Count is 2)
            {
            }
            // 保存二级菜单内容
            else if (BreadCollection.Count is 3)
            {
            }
        }

        /// <summary>
        /// 修改根菜单图标
        /// </summary>
        private void OnModifyClicked(object sender, RoutedEventArgs args)
        {
            OpenFileDialog dialog = new()
            {
                Multiselect = false,
                Filter = ShellMenu.IconFilterCondition,
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
                TeachingTipHelper.Show(new OperationResultTip(OperationKind.TextEmpty));
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
        /// 清空菜单项
        /// </summary>
        private void OnClearMenuClicked(object sender, RoutedEventArgs args)
        {
        }

        /// <summary>
        /// 刷新列表
        /// </summary>
        private void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
        }

        #endregion 第三部分：根菜单页面——挂载的事件
    }
}
