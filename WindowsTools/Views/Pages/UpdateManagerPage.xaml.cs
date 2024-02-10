using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using WindowsTools.Models;
using WindowsTools.Strings;
using WUApiLib;

namespace WindowsTools.Views.Pages
{
    /// <summary>
    /// Windows 更新管理页面
    /// </summary>
    public sealed partial class UpdateManagerPage : Page, INotifyPropertyChanged
    {
        private bool isInitialized;

        private bool _isChecking;

        public bool IsChecking
        {
            get { return _isChecking; }

            set
            {
                _isChecking = value;
                OnPropertyChanged();
            }
        }

        private bool _isExcludeDrivers;

        public bool IsExcludeDrivers
        {
            get { return _isExcludeDrivers; }

            set
            {
                _isExcludeDrivers = value;
                OnPropertyChanged();
            }
        }

        private DictionaryEntry _selectedPreviewChannel;

        public DictionaryEntry SelectedPreviewChannel
        {
            get { return _selectedPreviewChannel; }

            set
            {
                _selectedPreviewChannel = value;
                OnPropertyChanged();
            }
        }

        private UpdateSession updateSession = new UpdateSession();

        private List<DictionaryEntry> PreviewChannelList { get; } = new List<DictionaryEntry>()
        {
            new DictionaryEntry() { Key = UpdateManager.DonotEnter, Value = "DoNotEnter" },
            new DictionaryEntry() { Key = UpdateManager.ReleasePreview, Value = "ReleasePreview" },
            new DictionaryEntry() { Key = UpdateManager.Beta, Value = "Beta" },
            new DictionaryEntry() { Key = UpdateManager.Dev, Value = "Dev" },
            new DictionaryEntry() { Key = UpdateManager.Canary, Value = "Canary" },
        };

        private ObservableCollection<UpdateModel> AvailableUpdateCollection { get; } = new ObservableCollection<UpdateModel>();

        private ObservableCollection<UpdateModel> InstalledUpdateCollection { get; } = new ObservableCollection<UpdateModel>();

        private ObservableCollection<UpdateModel> HiddenUpdateCollection { get; } = new ObservableCollection<UpdateModel>();

        private ObservableCollection<UpdateModel> UpdateHistoryCollection { get; } = new ObservableCollection<UpdateModel>();

        public event PropertyChangedEventHandler PropertyChanged;

        public UpdateManagerPage()
        {
            InitializeComponent();
            SelectedPreviewChannel = PreviewChannelList[0];
        }

        #region 第一部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 可用更新：隐藏
        /// </summary>
        private void OnAvailableHideExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
        }

        /// <summary>
        /// 可用更新：安装
        /// </summary>
        private void OnAvailableInstallExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
        }

        /// <summary>
        /// 已安装更新：卸载
        /// </summary>
        private void OnInstalledUnInstallExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
        }

        /// <summary>
        /// 隐藏更新：显示
        /// </summary>
        private void OnHiddenShowExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
        }

        #endregion 第一部分：XamlUICommand 命令调用时挂载的事件

        #region 第二部分：Windows 更新管理页面——挂载的事件

        /// <summary>
        /// 第一次加载时初始化更新内容
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs args)
        {
        }

        /// <summary>
        /// 打开 Windows 更新
        /// </summary>
        private void OnWindowsUpdateClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                Process.Start("ms-settings:windowsupdate");
            });
        }

        /// <summary>
        /// 打开预览体验计划设置
        /// </summary>
        private void OnWIPSettingsClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                Process.Start("ms-settings:windowsinsider");
            });
        }

        /// <summary>
        /// 可用更新：全选
        /// </summary>
        private void OnAvailableSelectAllClicked(object sender, RoutedEventArgs args)
        {
            foreach (UpdateModel updateItem in AvailableUpdateCollection)
            {
                updateItem.IsSelected = true;
            }
        }

        /// <summary>
        /// 可用更新：全部不选
        /// </summary>
        private void OnAvailableSelectNoneClicked(object sender, RoutedEventArgs args)
        {
            foreach (UpdateModel updateItem in AvailableUpdateCollection)
            {
                updateItem.IsSelected = false;
            }
        }

        /// <summary>
        /// 可用更新：隐藏
        /// </summary>
        private void OnAvailableHideClicked(object sender, RoutedEventArgs args)
        {
            List<UpdateModel> hideList = AvailableUpdateCollection.Where(item => item.IsSelected is true).ToList();
            foreach (UpdateModel hideItem in hideList)
            {
            }
        }

        /// <summary>
        /// 可用更新：安装
        /// </summary>
        private void OnAvailableInstallClicked(object sender, RoutedEventArgs args)
        {
            List<UpdateModel> hideList = AvailableUpdateCollection.Where(item => item.IsSelected is true).ToList();
            foreach (UpdateModel hideItem in hideList)
            {
            }
        }

        /// <summary>
        /// 已安装更新：全选
        /// </summary>
        private void OnInstalledSelectAllClicked(object sender, RoutedEventArgs args)
        {
            foreach (UpdateModel updateItem in InstalledUpdateCollection)
            {
                updateItem.IsSelected = true;
            }
        }

        /// <summary>
        /// 已安装更新：全部不选
        /// </summary>
        private void OnInstalledSelectNoneClicked(object sender, RoutedEventArgs args)
        {
            foreach (UpdateModel updateItem in InstalledUpdateCollection)
            {
                updateItem.IsSelected = false;
            }
        }

        /// <summary>
        /// 已安装更新：隐藏
        /// </summary>
        private void OnInstalledUnInstallClicked(object sender, RoutedEventArgs args)
        {
            List<UpdateModel> unInstallList = InstalledUpdateCollection.Where(item => item.IsSelected is true).ToList();
            foreach (UpdateModel unInstallItem in unInstallList)
            {
            }
        }

        /// <summary>
        /// 隐藏更新：全选
        /// </summary>
        private void OnHiddenSelectAllClicked(object sender, RoutedEventArgs args)
        {
            foreach (UpdateModel updateItem in HiddenUpdateCollection)
            {
                updateItem.IsSelected = true;
            }
        }

        /// <summary>
        /// 隐藏更新：全部不选
        /// </summary>
        private void OnHiddenSelectNoneClicked(object sender, RoutedEventArgs args)
        {
            foreach (UpdateModel updateItem in HiddenUpdateCollection)
            {
                updateItem.IsSelected = false;
            }
        }

        /// <summary>
        /// 隐藏更新：显示
        /// </summary>
        private void OnHiddenShowClicked(object sender, RoutedEventArgs args)
        {
            List<UpdateModel> showList = HiddenUpdateCollection.Where(item => item.IsSelected is true).ToList();
            foreach (UpdateModel showItem in showList)
            {
            }
        }

        /// <summary>
        /// 检查更新
        /// </summary>
        private void OnCheckUpdateClicked(object sender, RoutedEventArgs args)
        {
        }

        /// <summary>
        /// Windows 更新不包括驱动程序
        /// </summary>
        private void OnExcludeDriversToggled(object sender, RoutedEventArgs args)
        {
        }

        /// <summary>
        /// 更改设备的预览计划频道
        /// </summary>
        private void OnPreviewChannelSelectClicked(object sender, RoutedEventArgs args)
        {
        }

        #endregion 第二部分：Windows 更新管理页面——挂载的事件

        /// <summary>
        /// 检查更新
        /// </summary>
        private void CheckUpdate()
        {
            IUpdateSearcher updateSearcher = updateSession.CreateUpdateSearcher();
            int count = updateSearcher.GetTotalHistoryCount();

            foreach (IUpdateHistoryEntry updateHistoryEntry in updateSearcher.QueryHistory(0, count))
            {
                Match match = Regex.Match(updateHistoryEntry.Title, @"\b(KB\d+)\b");
                UpdateModel updateItem = new UpdateModel()
                {
                    UpdateName = updateHistoryEntry.Title,
                    KBNumber = match.Success ? match.Groups[1].Value : UpdateManager.Unknown,
                    Date = updateHistoryEntry.Date.ToLocalTime(),
                    ResultCode = updateHistoryEntry.ResultCode,
                    HResult = updateHistoryEntry.HResult.ToString(),
                    TagUpdateOperation = updateHistoryEntry.Operation,
                    UpdateID = updateHistoryEntry.UpdateIdentity.UpdateID,
                    Description = updateHistoryEntry.Description,
                    SupportURL = updateHistoryEntry.SupportUrl,
                    ELDescription = ""
                };
            }
        }

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
