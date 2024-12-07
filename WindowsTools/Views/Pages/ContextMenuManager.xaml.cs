using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WindowsTools.Models;
using WindowsTools.Services.Root;

namespace WindowsTools.Views.Pages
{
    /// <summary>
    /// 右键菜单管理页面
    /// </summary>
    public sealed partial class ContextMenuManagerPage : Page, INotifyPropertyChanged
    {
        private bool isInitialized;
        private const string packageComPackageKey = @"SOFTWARE\Classes\PackagedCom\Package";
        private const string blockedKey = @"Software\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked";

        private bool _isLoadCompleted;

        public bool IsLoadCompleted
        {
            get { return _isLoadCompleted; }

            set
            {
                if (!Equals(_isLoadCompleted, value))
                {
                    _isLoadCompleted = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoadCompleted)));
                }
            }
        }

        private string _searchAppNameText;

        public string SearchAppNameText
        {
            get { return _searchAppNameText; }

            set
            {
                if (!Equals(_searchAppNameText, value))
                {
                    _searchAppNameText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchAppNameText)));
                }
            }
        }

        private bool _isSearchEmpty = false;

        public bool IsSearchEmpty
        {
            get { return _isSearchEmpty; }

            set
            {
                if (!Equals(_isSearchEmpty, value))
                {
                    _isSearchEmpty = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSearchEmpty)));
                }
            }
        }

        private ObservableCollection<ContextMenuModel> ContextMenuCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public ContextMenuManagerPage()
        {
            InitializeComponent();
        }

        #region 第一部分：右键菜单管理页面——挂载的事件

        /// <summary>
        /// 右键菜单管理页面初始化触发的事件
        /// </summary>
        private async void OnLoaded(object sender, RoutedEventArgs args)
        {
            if (!isInitialized)
            {
                isInitialized = true;
                await Task.Delay(500);
                IsLoadCompleted = true;
            }
        }

        /// <summary>
        /// 了解自定义右键菜单
        /// </summary>
        private void OnLearnCustomRightClickMenuClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("https://blogs.windows.com/windowsdeveloper/2021/07/19/extending-the-context-menu-and-share-dialog-in-windows-11");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Open url failed", e);
                }
            });
        }

        /// <summary>
        /// 查询搜索内容
        /// </summary>
        private void OnSearchAppNameQuerySubmitted(object sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!string.IsNullOrEmpty(SearchAppNameText))
            {
                IsSearchEmpty = ContextMenuCollection.All(item => item.IsVisible is Visibility.Collapsed);
            }
        }

        /// <summary>
        /// 搜索应用名称内容发生变化事件
        /// </summary>
        private void OnSerachAppNameTextChanged(object sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            SearchAppNameText = (sender as AutoSuggestBox).Text;

            if (string.IsNullOrEmpty(SearchAppNameText))
            {
                foreach (ContextMenuModel contextMenu in ContextMenuCollection)
                {
                    contextMenu.IsVisible = Visibility.Visible;
                }

                IsSearchEmpty = false;
            }
        }

        /// <summary>
        /// 刷新
        /// </summary>
        private async void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            IsLoadCompleted = false;
            await Task.Delay(500);
            IsLoadCompleted = true;
        }

        #endregion 第一部分：右键菜单管理页面——挂载的事件

        /// <summary>
        /// 获取右键菜单列表信息
        /// </summary>
        private List<ContextMenuModel> GetContextMenuList()
        {
            List<ContextMenuModel> contextMenuList = [];

            try
            {
                RegistryKey packageListKey = Registry.LocalMachine.OpenSubKey(packageComPackageKey, false);

                if (packageListKey is not null)
                {
                    string[] packageFullNameArray = packageListKey.GetSubKeyNames();
                    foreach (string packageFullName in packageFullNameArray)
                    {
                        RegistryKey classKey = packageListKey.OpenSubKey(string.Join(@"\", packageFullName, "Class"));

                        if (classKey is not null)
                        {
                            string[] clsidArray = classKey.GetSubKeyNames();
                            List<ContextMenuItemModel> contextMenuItemList = [];

                            foreach (string clsidString in clsidArray)
                            {
                                if (Guid.TryParse(clsidString, out Guid clsid))
                                {
                                    RegistryKey clsidKey = classKey.OpenSubKey(clsidString, false);

                                    if (clsidKey is not null)
                                    {
                                        string dllPath = Convert.ToString(clsidKey.GetValue("DllPath", string.Empty));

                                        if (!string.IsNullOrEmpty(dllPath) && dllPath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                                        {
                                            int serverId = Convert.ToInt32(clsidKey.GetValue("ServerId", 0));
                                            int threading = Convert.ToInt32(clsidKey.GetValue("Threading", 0));

                                            contextMenuItemList.Add(new ContextMenuItemModel()
                                            {
                                                Clsid = clsid,
                                                DllPath = dllPath,
                                                ThreadingMode = threading switch
                                                {
                                                    0 => ApartmentState.STA,
                                                    1 => ApartmentState.MTA,
                                                    _ => ApartmentState.Unknown,
                                                }
                                            });
                                        }
                                    }

                                    classKey.Close();
                                    classKey.Dispose();
                                }
                            }

                            contextMenuList.Add(new()
                            {
                                PackageFullName = packageFullName,
                                ContextMenuItemCollection = new(contextMenuItemList)
                            });
                        }

                        classKey.Close();
                        classKey.Dispose();
                    }
                }

                packageListKey.Close();
                packageListKey.Dispose();
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, "Get com package list info failed", e);
            }

            return contextMenuList;
        }

        /// <summary>
        /// 获取已禁用菜单 CLSID 列表
        /// </summary>
        private List<KeyValuePair<Guid, string>> GetBlockedClsidList()
        {
            List<KeyValuePair<Guid, string>> blockClsidList = [];

            try
            {
                RegistryKey blockKey = Registry.LocalMachine.OpenSubKey(blockedKey, false);

                if (blockKey is not null)
                {
                    string[] blcokedClsidArray = blockKey.GetValueNames();

                    foreach (string blockedClsid in blcokedClsidArray)
                    {
                        if (Guid.TryParse(blockedClsid, out Guid clsid))
                        {
                            blockClsidList.Add(new KeyValuePair<Guid, string>(clsid, Registry.LocalMachine.ToString()));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, "Get com package blocked clsid list in local machine registry failed", e);
            }

            try
            {
                RegistryKey blockKey = Registry.CurrentUser.OpenSubKey(blockedKey, false);

                if (blockKey is not null)
                {
                    string[] blcokedClsidArray = blockKey.GetValueNames();

                    foreach (string blockedClsid in blcokedClsidArray)
                    {
                        if (Guid.TryParse(blockedClsid, out Guid clsid))
                        {
                            blockClsidList.Add(new KeyValuePair<Guid, string>(clsid, Registry.CurrentUser.ToString()));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, "Get com package blocked clsid list in current user registry failed", e);
            }

            return blockClsidList;
        }
    }
}
