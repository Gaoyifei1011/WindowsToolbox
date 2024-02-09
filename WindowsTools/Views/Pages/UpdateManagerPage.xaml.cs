using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
            IsChecking = true;
        }

        #region 第一部分：Windows 更新管理页面——挂载的事件

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

        #endregion 第一部分：Windows 更新管理页面——挂载的事件

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
