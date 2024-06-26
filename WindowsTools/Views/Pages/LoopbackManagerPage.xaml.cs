using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using WindowsTools.Models;
using WindowsTools.Services.Root;
using WindowsTools.Views.Windows;
using WindowsTools.WindowsAPI.PInvoke.FirewallAPI;
using WindowsTools.WindowsAPI.PInvoke.Shlwapi;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace WindowsTools.Views.Pages
{
    /// <summary>
    /// 网络回环管理页面
    /// </summary>
    public sealed partial class LoopbackManagerPage : Page, INotifyPropertyChanged
    {
        private IntPtr pACs;

        private bool _isProcessing;

        public bool IsProcessing
        {
            get { return _isProcessing; }

            set
            {
                if (!Equals(_isProcessing, value))
                {
                    _isProcessing = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsProcessing)));
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

        private bool _isSaved = false;

        public bool IsSaved
        {
            get { return _isSaved; }

            set
            {
                if (!Equals(_isSaved, value))
                {
                    _isSaved = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSaved)));
                }
            }
        }

        private ObservableCollection<LoopbackModel> LoopbackCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public LoopbackManagerPage()
        {
            InitializeComponent();
            Task.Run(GetLoopbackData);
        }

        #region 第一部分：

        /// <summary>
        /// 点击复选框时使保存按钮处于可选状态
        /// </summary>
        private void OnCheckExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (!IsSaved)
            {
                IsSaved = true;
            }
        }

        #endregion 第一部分：

        #region 第二部分：网络回环管理页面——挂载的事件

        /// <summary>
        /// 查询搜索内容
        /// </summary>
        private void OnSearchAppNameQuerySubmitted(object sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!string.IsNullOrEmpty(SearchAppNameText))
            {
                foreach (LoopbackModel loopbackItem in LoopbackCollection)
                {
                    if (!string.IsNullOrEmpty(loopbackItem.AppContainerName) && loopbackItem.AppContainerName.Contains(SearchAppNameText))
                    {
                        loopbackItem.IsVisible = Visibility.Visible;
                        continue;
                    }

                    if (!string.IsNullOrEmpty(loopbackItem.DisplayName) && loopbackItem.DisplayName.Contains(SearchAppNameText))
                    {
                        loopbackItem.IsVisible = Visibility.Visible;
                        continue;
                    }

                    if (!string.IsNullOrEmpty(loopbackItem.Description) && loopbackItem.Description.Contains(SearchAppNameText))
                    {
                        loopbackItem.IsVisible = Visibility.Visible;
                        continue;
                    }

                    if (!string.IsNullOrEmpty(loopbackItem.PackageFullName) && loopbackItem.PackageFullName.Contains(SearchAppNameText))
                    {
                        loopbackItem.IsVisible = Visibility.Visible;
                        continue;
                    }

                    if (!string.IsNullOrEmpty(loopbackItem.UserAccountSIDName) && loopbackItem.UserAccountSIDName.Contains(SearchAppNameText))
                    {
                        loopbackItem.IsVisible = Visibility.Visible;
                        continue;
                    }

                    if (!string.IsNullOrEmpty(loopbackItem.AppContainerSIDName) && loopbackItem.AppContainerSIDName.Contains(SearchAppNameText))
                    {
                        loopbackItem.IsVisible = Visibility.Visible;
                        continue;
                    }

                    loopbackItem.IsVisible = Visibility.Collapsed;
                }

                IsSearchEmpty = LoopbackCollection.All(item => item.IsVisible is Visibility.Collapsed);
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
                foreach (LoopbackModel loopbackItem in LoopbackCollection)
                {
                    loopbackItem.IsVisible = Visibility.Visible;
                }

                IsSearchEmpty = false;
            }
        }

        /// <summary>
        /// 全选
        /// </summary>
        private void OnSelectAllClicked(object sender, RoutedEventArgs args)
        {
            IsSaved = true;
            foreach (LoopbackModel loopbackItem in LoopbackCollection)
            {
                if (loopbackItem.IsVisible is Visibility.Visible)
                {
                    loopbackItem.IsChecked = true;
                }
            }
        }

        /// <summary>
        /// 全部不选
        /// </summary>
        private void OnSelectNoneClicked(object sender, RoutedEventArgs args)
        {
            IsSaved = true;
            foreach (LoopbackModel loopbackItem in LoopbackCollection)
            {
                if (loopbackItem.IsVisible is Visibility.Visible)
                {
                    loopbackItem.IsChecked = false;
                }
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        private void OnResetClicked(object sender, RoutedEventArgs args)
        {
            IsSaved = true;
            foreach (LoopbackModel loopbackItem in LoopbackCollection)
            {
                loopbackItem.IsChecked = loopbackItem.IsOldChecked;
            }
        }

        /// <summary>
        /// 刷新
        /// </summary>
        private void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            LoopbackCollection.Clear();
            Task.Run(GetLoopbackData);
        }

        /// <summary>
        /// 保存
        /// </summary>
        private void OnSaveClicked(object sender, RoutedEventArgs args)
        {
        }

        #endregion 第二部分：网络回环管理页面——挂载的事件

        /// <summary>
        /// 获取应用数据
        /// </summary>
        private void GetLoopbackData()
        {
            pACs = IntPtr.Zero;
            List<INET_FIREWALL_APP_CONTAINER> inetLoopbackList = GetLoopbackList();
            List<SID_AND_ATTRIBUTES> inetLoopbackEnabledList = GetLoopbackEnabledList();

            foreach (INET_FIREWALL_APP_CONTAINER inetContainerItem in inetLoopbackList)
            {
                try
                {
                    bool isEnabled = GetLoopbackEnabled(inetContainerItem.appContainerSid, inetLoopbackEnabledList);

                    StringBuilder displayNameBuilder = new();
                    ShlwapiLibrary.SHLoadIndirectString(inetContainerItem.displayName, displayNameBuilder, displayNameBuilder.Capacity, IntPtr.Zero);

                    StringBuilder descriptionBuilder = new();
                    ShlwapiLibrary.SHLoadIndirectString(inetContainerItem.description, descriptionBuilder, descriptionBuilder.Capacity, IntPtr.Zero);

                    INET_FIREWALL_AC_BINARIES inetBinaries = inetContainerItem.binaries;
                    string[] stringBinaries = null;
                    if (inetBinaries.count != 0 && inetBinaries.binaries != IntPtr.Zero)
                    {
                        stringBinaries = new string[inetBinaries.count];
                        long num = inetBinaries.binaries.ToInt64();
                        for (int i = 0; i < inetBinaries.count; i++)
                        {
                            stringBinaries[i] = Marshal.PtrToStringUni(Marshal.ReadIntPtr((IntPtr)num));
                            num += IntPtr.Size;
                        }
                    }

                    SecurityIdentifier appContainerSid = null;

                    try
                    {
                        if (inetContainerItem.appContainerSid != IntPtr.Zero)
                        {
                            appContainerSid = new SecurityIdentifier(inetContainerItem.appContainerSid);
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, "Parse app container sid failed", e);
                    }

                    NTAccount userAccountType = null;

                    try
                    {
                        if (inetContainerItem.userSid != IntPtr.Zero)
                        {
                            SecurityIdentifier userSid = new(inetContainerItem.userSid);
                            userAccountType = (NTAccount)userSid.Translate(typeof(NTAccount));
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, "Parse user sid failed", e);
                    }

                    LoopbackModel appContainerItem = new()
                    {
                        AppContainerName = inetContainerItem.appContainerName,
                        AppContainerSID = inetContainerItem.appContainerSid,
                        AppContainerSIDName = appContainerSid.ToString(),
                        DisplayName = displayNameBuilder.ToString(),
                        Description = descriptionBuilder.ToString(),
                        WorkingDirectory = inetContainerItem.workingDirectory,
                        PackageFullName = inetContainerItem.packageFullName,
                        StringBinaries = stringBinaries is not null ? stringBinaries : [],
                        UserAccountSIDName = userAccountType is not null ? userAccountType.ToString() : string.Empty,
                        IsChecked = isEnabled,
                        IsOldChecked = isEnabled,
                    };

                    MainWindow.Current.BeginInvoke(() =>
                    {
                        LoopbackCollection.Add(appContainerItem);
                    });
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }

        /// <summary>
        /// 获取网络回环列表数据
        /// </summary>
        private List<INET_FIREWALL_APP_CONTAINER> GetLoopbackList()
        {
            IntPtr arrayValue = IntPtr.Zero;
            uint size = 0;
            List<INET_FIREWALL_APP_CONTAINER> inetContainerList = [];

            GCHandle handle_pdwCntPublicACs = GCHandle.Alloc(size, GCHandleType.Pinned);
            GCHandle handle_ppACs = GCHandle.Alloc(arrayValue, GCHandleType.Pinned);
            FirewallAPILibrary.NetworkIsolationEnumAppContainers(NETISO_FLAG.NETISO_FLAG_MAX, out size, out arrayValue);

            pACs = arrayValue;

            int structSize = Marshal.SizeOf(typeof(INET_FIREWALL_APP_CONTAINER));

            for (int index = 0; index < size; index++)
            {
                INET_FIREWALL_APP_CONTAINER container = (INET_FIREWALL_APP_CONTAINER)Marshal.PtrToStructure(arrayValue, typeof(INET_FIREWALL_APP_CONTAINER));

                inetContainerList.Add(container);
                arrayValue = new IntPtr((long)arrayValue + structSize);
            }

            handle_pdwCntPublicACs.Free();
            handle_ppACs.Free();
            return inetContainerList;
        }

        /// <summary>
        /// 获取已开启网络回环的信息列表数据
        /// </summary>
        private List<SID_AND_ATTRIBUTES> GetLoopbackEnabledList()
        {
            IntPtr arrayValue = IntPtr.Zero;
            uint size = 0;
            List<SID_AND_ATTRIBUTES> inetContainerConfigList = [];

            GCHandle handle_pdwCntPublicACs = GCHandle.Alloc(size, GCHandleType.Pinned);
            GCHandle handle_ppACs = GCHandle.Alloc(arrayValue, GCHandleType.Pinned);
            FirewallAPILibrary.NetworkIsolationGetAppContainerConfig(out size, out arrayValue);

            int structSize = Marshal.SizeOf(typeof(SID_AND_ATTRIBUTES));
            for (int index = 0; index < size; index++)
            {
                SID_AND_ATTRIBUTES cur = (SID_AND_ATTRIBUTES)Marshal.PtrToStructure(arrayValue, typeof(SID_AND_ATTRIBUTES));
                inetContainerConfigList.Add(cur);
                arrayValue = new IntPtr((long)arrayValue + structSize);
            }

            handle_pdwCntPublicACs.Free();
            handle_ppACs.Free();

            return inetContainerConfigList;
        }

        /// <summary>
        /// 检查应用网络回环是否已开启
        /// </summary>
        private bool GetLoopbackEnabled(IntPtr appContainerSid, List<SID_AND_ATTRIBUTES> loopbackEnabledList)
        {
            foreach (SID_AND_ATTRIBUTES sidItem in loopbackEnabledList)
            {
                if (new SecurityIdentifier(sidItem.Sid).Equals(new SecurityIdentifier(appContainerSid)))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 设置网络回环状态
        /// </summary>
        private void SetLoopbackState(List<LoopbackModel> loopbackList)
        {
            if (loopbackList is not null)
            {
                SID_AND_ATTRIBUTES[] sidAndAttributesArray = new SID_AND_ATTRIBUTES[loopbackList.Count];
                int count = 0;

                foreach (LoopbackModel loopbackItem in loopbackList)
                {
                    if (loopbackItem.IsChecked)
                    {
                        sidAndAttributesArray[count].Attributes = 0;
                        sidAndAttributesArray[count].Sid = loopbackItem.AppContainerSID;
                        count++;
                    }
                }

                FirewallAPILibrary.NetworkIsolationSetAppContainerConfig(loopbackList.Count, sidAndAttributesArray);
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        private void FreeResources()
        {
            FirewallAPILibrary.NetworkIsolationFreeAppContainers(pACs);
        }
    }
}
