using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using PowerToolbox.WindowsAPI.PInvoke.Dnsapi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// Hosts 文件编辑器页面
    /// </summary>
    public sealed partial class HostsPage : Page, INotifyPropertyChanged
    {
        private readonly string hostsPath = Path.Combine(Environment.SystemDirectory, "drivers\\etc\\hosts");
        private readonly string HostsEmptyDescriptionString = ResourceService.HostsResource.GetString("HostsEmptyDescription");
        private readonly string HostsEmptyWithConditionDescriptionString = ResourceService.HostsResource.GetString("HostsEmptyWithConditionDescription");
        private readonly SynchronizationContext synchronizationContext = SynchronizationContext.Current;
        private FileSystemWatcher fileSystemWatcher;

        private HostsResultKind _hostsResultKind;

        public HostsResultKind HostsResultKind
        {
            get { return _hostsResultKind; }

            set
            {
                if (!Equals(_hostsResultKind, value))
                {
                    _hostsResultKind = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HostsResultKind)));
                }
            }
        }

        private string _hostsFailedContent;

        public string HostsFailedContent
        {
            get { return _hostsFailedContent; }

            set
            {
                if (!Equals(_hostsFailedContent, value))
                {
                    _hostsFailedContent = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HostsFailedContent)));
                }
            }
        }

        private string _searchText = string.Empty;

        public string SearchText
        {
            get { return _searchText; }

            set
            {
                if (!Equals(_searchText, value))
                {
                    _searchText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchText)));
                }
            }
        }

        private List<HostsModel> HostsList { get; } = [];

        private ObservableCollection<HostsModel> HostsCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public HostsPage()
        {
            InitializeComponent();
        }

        #region 第一部分：重写父类事件

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);

            Task.Run(() =>
            {
                if (fileSystemWatcher is null)
                {
                    fileSystemWatcher = new()
                    {
                        Path = Path.GetDirectoryName(hostsPath),
                        Filter = Path.GetFileName(hostsPath),
                        NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size
                    };

                    if (fileSystemWatcher is not null)
                    {
                        fileSystemWatcher.Changed += OnWatcherChanged;
                        fileSystemWatcher.Created += OnWatcherCreated;
                        fileSystemWatcher.Deleted += OnWatcherDeleted;
                        fileSystemWatcher.Error += OnWatcherError;
                        fileSystemWatcher.Renamed += OnWatcherRenamed;
                    }
                }
            });
        }

        /// <summary>
        /// 离开该页面触发的事件
        /// </summary>
        protected override void OnNavigatedFrom(NavigationEventArgs args)
        {
            base.OnNavigatedFrom(args);

            Task.Run(() =>
            {
                if (fileSystemWatcher is not null)
                {
                    try
                    {
                        fileSystemWatcher.Changed -= OnWatcherChanged;
                        fileSystemWatcher.Created -= OnWatcherCreated;
                        fileSystemWatcher.Deleted -= OnWatcherDeleted;
                        fileSystemWatcher.Error -= OnWatcherError;
                        fileSystemWatcher.Renamed -= OnWatcherRenamed;

                        fileSystemWatcher.Dispose();
                        fileSystemWatcher = null;
                    }
                    catch (Exception e)
                    {
                        fileSystemWatcher = null;
                        LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(HostsPage), nameof(OnNavigatedFrom), 1, e);
                    }
                }
            });
        }

        #endregion 第一部分：重写父类事件

        #region 第三部分：网络回环管理页面——挂载的事件

        /// <summary>
        /// 查询搜索内容
        /// </summary>
        private void OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!string.IsNullOrEmpty(SearchText) && HostsList.Count > 0)
            {
                HostsResultKind = HostsResultKind.Loading;
                HostsCollection.Clear();
                foreach (HostsModel hostsItem in HostsList)
                {
                    if (!string.IsNullOrEmpty(hostsItem.AddRess) && hostsItem.AddRess.Contains(SearchText))
                    {
                        hostsItem.IsSelected = false;
                        HostsList.Add(hostsItem);
                        continue;
                    }

                    if (!string.IsNullOrEmpty(hostsItem.Hosts) && hostsItem.Hosts.Contains(SearchText))
                    {
                        hostsItem.IsSelected = false;
                        HostsList.Add(hostsItem);
                        continue;
                    }

                    if (!string.IsNullOrEmpty(hostsItem.Annotation) && hostsItem.Annotation.Contains(SearchText))
                    {
                        hostsItem.IsSelected = false;
                        HostsList.Add(hostsItem);
                        continue;
                    }
                }

                HostsResultKind = HostsCollection.Count is 0 ? HostsResultKind.Failed : HostsResultKind.Successfully;
                HostsFailedContent = HostsCollection.Count is 0 ? HostsEmptyWithConditionDescriptionString : string.Empty;
            }
        }

        /// <summary>
        /// 搜索应用名称内容发生变化事件
        /// </summary>
        private void OnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            SearchText = sender.Text;
            if (string.IsNullOrEmpty(SearchText) && HostsList.Count > 0)
            {
                HostsResultKind = HostsResultKind.Loading;
                HostsCollection.Clear();
                foreach (HostsModel hostsItem in HostsList)
                {
                    hostsItem.IsSelected = false;
                    HostsCollection.Add(hostsItem);
                }

                HostsResultKind = HostsCollection.Count is 0 ? HostsResultKind.Failed : HostsResultKind.Successfully;
                HostsFailedContent = HostsCollection.Count is 0 ? HostsEmptyWithConditionDescriptionString : string.Empty;
            }
        }

        /// <summary>
        /// 全选
        /// </summary>
        private void OnSelectAllClicked(object sender, RoutedEventArgs args)
        {
            foreach (HostsModel hostsItem in HostsCollection)
            {
                hostsItem.IsSelected = true;
            }
        }

        /// <summary>
        /// 全部不选
        /// </summary>
        private void OnSelectNoneClicked(object sender, RoutedEventArgs args)
        {
            foreach (HostsModel hostsItem in HostsCollection)
            {
                hostsItem.IsSelected = false;
            }
        }

        /// <summary>
        /// 添加条目
        /// </summary>
        private void OnAddNewHostClicked(object sender, RoutedEventArgs args)
        {
            // TODO：使用 Flyout 作为对话框
        }

        /// <summary>
        /// 打开 Hosts 文件
        /// </summary>
        private void OnOpenFileClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    if (File.Exists(hostsPath))
                    {
                        Process.Start(hostsPath);
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(HostsPage), nameof(OnOpenFileClicked), 1, e);
                }
            });
        }

        /// <summary>
        /// 刷新
        /// </summary>
        private async void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            await GetHostsAsync();
        }

        /// <summary>
        /// 更新 DNS 缓存
        /// </summary>
        private void OnUpdateDNSCacheClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                DnsapiLibrary.DnsFlushResolverCache();
            });
        }

        #endregion 第三部分：网络回环管理页面——挂载的事件

        #region 第四部分：网络回环管理页面——自定义事件

        /// <summary>
        /// 当更改指定 Path 中的文件和目录时发生的事件
        /// </summary>
        private void OnWatcherChanged(object sender, FileSystemEventArgs args)
        {
            synchronizationContext.Post(async (_) =>
            {
                await GetHostsAsync();
            }, null);
        }

        /// <summary>
        /// 当在指定 Path 中创建文件和目录时发生的事件
        /// </summary>
        private void OnWatcherCreated(object sender, FileSystemEventArgs args)
        {
            synchronizationContext.Post(async (_) =>
            {
                await GetHostsAsync();
            }, null);
        }

        /// <summary>
        /// 删除指定 Path 中的文件或目录时发生的事件
        /// </summary>
        private void OnWatcherDeleted(object sender, FileSystemEventArgs args)
        {
            synchronizationContext.Post(async (_) =>
            {
                await GetHostsAsync();
            }, null);
        }

        /// <summary>
        /// 当 FileSystemWatcher 的实例无法继续监视更改或内部缓冲区溢出时发生的事件
        /// </summary>
        private void OnWatcherError(object sender, ErrorEventArgs args)
        {
            synchronizationContext.Post(async (_) =>
            {
                await GetHostsAsync();
            }, null);
        }

        /// <summary>
        /// 重命名指定 Path 中的文件或目录时发生的事件
        /// </summary>
        private void OnWatcherRenamed(object sender, RenamedEventArgs args)
        {
            synchronizationContext.Post(async (_) =>
            {
                await GetHostsAsync();
            }, null);
        }

        #endregion 第四部分：网络回环管理页面——自定义事件

        /// <summary>
        /// 获取 Hosts 文件内容
        /// </summary>
        private async Task GetHostsAsync()
        {
            HostsResultKind = HostsResultKind.Loading;
            HostsList.Clear();
            HostsCollection.Clear();

            List<HostsModel> hostsList = await Task.Run(() =>
            {
                List<HostsModel> hostsList = [];

                return hostsList;
            });

            HostsList.AddRange(hostsList);

            if (HostsList.Count is 0)
            {
                HostsResultKind = HostsResultKind.Failed;
                HostsFailedContent = HostsEmptyDescriptionString;
            }
            else
            {
                foreach (HostsModel hostsItem in HostsList)
                {
                }
            }
        }

        /// <summary>
        /// 获取是否正在加载中
        /// </summary>

        private bool GetIsLoading(HostsResultKind hostsResultKind)
        {
            return hostsResultKind is not HostsResultKind.Loading;
        }
    }
}
