using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using WindowsTools.Extensions.DataType.Enums;
using WindowsTools.Helpers.Controls;
using WindowsTools.Models;
using WindowsTools.Services.Root;
using WindowsTools.UI.TeachingTips;
using WindowsTools.Views.Windows;
using WindowsTools.WindowsAPI.PInvoke.FirewallAPI;
using WindowsTools.WindowsAPI.PInvoke.Kernel32;
using WindowsTools.WindowsAPI.PInvoke.Shlwapi;

// 抑制 CA18062，CA1822，IDE0060 警告
#pragma warning disable CA1806,CA1822,IDE0060

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
        private readonly List<ContextMenuModel> contextMenuList = [];

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

        private string _searchAppNameText = string.Empty;

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

        private bool _isContextMenuEmpty = false;

        public bool IsContextMenuEmpty
        {
            get { return _isContextMenuEmpty; }

            set
            {
                if (!Equals(_isContextMenuEmpty, value))
                {
                    _isContextMenuEmpty = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsContextMenuEmpty)));
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

        /// <summary>
        /// 打开应用包路径
        /// </summary>
        private void OnOpenPackagePathExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string path)
            {
                Task.Run(() =>
                {
                    try
                    {
                        Process.Start(path);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, "Open package path failed", e);
                    }
                });
            }
        }

        /// <summary>
        /// 点击复选框时使保存按钮处于可选状态
        /// </summary>
        private async void OnCheckBoxClickExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is ContextMenuItemModel contextMenuItem)
            {
                Tuple<BlockedClsidType, bool> result = await Task.Run(() =>
                {
                    RegistryKey registryKey = null;
                    BlockedClsidType blockedClsidType = BlockedClsidType.Unknown;
                    if (contextMenuItem.BlockedClsidType is BlockedClsidType.LocalMachine)
                    {
                        try
                        {
                            registryKey = Registry.LocalMachine.CreateSubKey(blockedKey, true);
                            blockedClsidType = BlockedClsidType.LocalMachine;
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, string.Format("Open block key {0} of value in local machine failed", contextMenuItem.Clsid.ToString()), e);
                        }
                    }
                    else
                    {
                        try
                        {
                            registryKey = Registry.CurrentUser.CreateSubKey(blockedKey, true);
                            blockedClsidType = BlockedClsidType.CurrentUser;
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, string.Format("Open block key {0} of value in current user failed", contextMenuItem.Clsid.ToString()), e);
                        }
                    }

                    if (registryKey is not null)
                    {
                        try
                        {
                            string name = contextMenuItem.Clsid.ToString("B").ToUpperInvariant();
                            if (contextMenuItem.IsEnabled)
                            {
                                object oldValue = registryKey.GetValue(name);
                                if (oldValue is null)
                                {
                                    registryKey.SetValue(name, "Blocked by WindowsTools");
                                    registryKey.Close();
                                    registryKey.Dispose();
                                }
                                return Tuple.Create(blockedClsidType, true);
                            }
                            else
                            {
                                registryKey.DeleteValue(name);
                                registryKey.Close();
                                registryKey.Dispose();
                                return Tuple.Create(blockedClsidType, true);
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, "Update block result failed", e);
                            registryKey.Close();
                            registryKey.Dispose();
                            return Tuple.Create(blockedClsidType, false); ;
                        }
                    }
                    else
                    {
                        registryKey.Close();
                        registryKey.Dispose();
                        return Tuple.Create(blockedClsidType, true);
                    }
                });

                if (result.Item2)
                {
                    foreach (ContextMenuModel item in ContextMenuCollection)
                    {
                        bool isModified = false;
                        foreach (ContextMenuItemModel subItem in item.ContextMenuItemCollection)
                        {
                            if (subItem.Clsid.Equals(contextMenuItem.Clsid))
                            {
                                if (result.Item2)
                                {
                                    subItem.BlockedClsidType = IsEnabled ? BlockedClsidType.Unknown : result.Item1;
                                }

                                subItem.IsEnabled = !subItem.IsEnabled;
                                if (!result.Item2)
                                {
                                    subItem.IsEnabled = !subItem.IsEnabled;
                                }

                                isModified = true;
                                break;
                            }
                        }

                        if (isModified)
                        {
                            break;
                        }
                    }
                }

                await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.ContextMenuUpdate, result.Item2));
            }
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
                contextMenuList.Clear();
                ContextMenuCollection.Clear();

                await Task.Run(async () =>
                {
                    await Task.Delay(500);
                    contextMenuList.AddRange(GetContextMenuList());
                });

                foreach (ContextMenuModel contextMenuItem in contextMenuList)
                {
                    try
                    {
                        BitmapImage bitmapImage = new()
                        {
                            UriSource = contextMenuItem.PackageIconUri
                        };
                        contextMenuItem.PackageIcon = bitmapImage;
                    }
                    catch (Exception)
                    {
                        contextMenuItem.PackageIcon = new BitmapImage();
                    }

                    if (string.IsNullOrEmpty(SearchAppNameText))
                    {
                        ContextMenuCollection.Add(contextMenuItem);
                        continue;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(contextMenuItem.PackageDisplayName) && contextMenuItem.PackageDisplayName.Contains(SearchAppNameText))
                        {
                            ContextMenuCollection.Add(contextMenuItem);
                            continue;
                        }

                        if (!string.IsNullOrEmpty(contextMenuItem.PackageFullName) && contextMenuItem.PackageFullName.Contains(SearchAppNameText))
                        {
                            ContextMenuCollection.Add(contextMenuItem);
                            continue;
                        }
                    }
                }

                IsContextMenuEmpty = contextMenuList.Count is 0;
                IsSearchEmpty = ContextMenuCollection.Count is 0;
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
        /// 打开设置
        /// </summary>
        private void OnOpenSettingsClicked(object sender, RoutedEventArgs args)
        {
            (MainWindow.Current.Content as MainPage).NavigateTo(typeof(SettingsPage));
        }

        /// <summary>
        /// 查询搜索内容
        /// </summary>
        private void OnSearchAppNameQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!string.IsNullOrEmpty(SearchAppNameText))
            {
                ContextMenuCollection.Clear();
                foreach (ContextMenuModel contextMenuItem in contextMenuList)
                {
                    if (!string.IsNullOrEmpty(contextMenuItem.PackageDisplayName) && contextMenuItem.PackageDisplayName.Contains(SearchAppNameText))
                    {
                        ContextMenuCollection.Add(contextMenuItem);
                        continue;
                    }

                    if (!string.IsNullOrEmpty(contextMenuItem.PackageFullName) && contextMenuItem.PackageFullName.Contains(SearchAppNameText))
                    {
                        ContextMenuCollection.Add(contextMenuItem);
                        continue;
                    }
                }

                IsSearchEmpty = ContextMenuCollection.Count is 0;
            }
        }

        /// <summary>
        /// 搜索应用名称内容发生变化事件
        /// </summary>
        private void OnSerachAppNameTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            SearchAppNameText = sender.Text;

            if (string.IsNullOrEmpty(SearchAppNameText))
            {
                ContextMenuCollection.Clear();
                foreach (ContextMenuModel contextMenuItem in contextMenuList)
                {
                    ContextMenuCollection.Add(contextMenuItem);
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
            contextMenuList.Clear();
            ContextMenuCollection.Clear();

            await Task.Run(async () =>
            {
                await Task.Delay(500);
                contextMenuList.AddRange(GetContextMenuList());
            });

            foreach (ContextMenuModel contextMenuItem in contextMenuList)
            {
                try
                {
                    BitmapImage bitmapImage = new()
                    {
                        UriSource = contextMenuItem.PackageIconUri
                    };
                    contextMenuItem.PackageIcon = bitmapImage;
                }
                catch (Exception)
                {
                    contextMenuItem.PackageIcon = new BitmapImage();
                }

                if (string.IsNullOrEmpty(SearchAppNameText))
                {
                    ContextMenuCollection.Add(contextMenuItem);
                    continue;
                }
                else
                {
                    if (!string.IsNullOrEmpty(contextMenuItem.PackageDisplayName) && contextMenuItem.PackageDisplayName.Contains(SearchAppNameText))
                    {
                        ContextMenuCollection.Add(contextMenuItem);
                        continue;
                    }

                    if (!string.IsNullOrEmpty(contextMenuItem.PackageFullName) && contextMenuItem.PackageFullName.Contains(SearchAppNameText))
                    {
                        ContextMenuCollection.Add(contextMenuItem);
                        continue;
                    }
                }
            }

            IsContextMenuEmpty = contextMenuList.Count is 0;
            IsSearchEmpty = ContextMenuCollection.Count is 0;
            IsLoadCompleted = true;
        }

        /// <summary>
        /// 恢复默认
        /// </summary>
        private async void OnRestoreDefaultClicked(object sender, RoutedEventArgs args)
        {
            await Task.Run(() =>
            {
                List<ContextMenuModel> contextMenuList = [.. ContextMenuCollection];

                try
                {
                    RegistryKey blockKey = Registry.LocalMachine.OpenSubKey(blockedKey, true);
                    string[] blcokedClsidArray = blockKey.GetValueNames();

                    foreach (string blockedClsid in blcokedClsidArray)
                    {
                        if (blockKey.GetValue(blockedClsid).ToString() is "Blocked by WindowsTools")
                        {
                            blockKey.DeleteValue(blockedClsid);
                        }
                    }

                    blockKey.Close();
                    blockKey.Dispose();
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Return context menu value to default in local machine failed", e);
                }

                try
                {
                    RegistryKey blockKey = Registry.CurrentUser.OpenSubKey(blockedKey, true);
                    string[] blcokedClsidArray = blockKey.GetValueNames();

                    foreach (string blockedClsid in blcokedClsidArray)
                    {
                        if (blockKey.GetValue(blockedClsid).ToString() is "Blocked by WindowsTools")
                        {
                            blockKey.DeleteValue(blockedClsid);
                        }
                    }

                    blockKey.Close();
                    blockKey.Dispose();
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Return context menu value to default in current user failed", e);
                }
            });

            IsLoadCompleted = false;
            SearchAppNameText = string.Empty;
            IsSearchEmpty = false;
            contextMenuList.Clear();
            ContextMenuCollection.Clear();

            await Task.Run(async () =>
            {
                await Task.Delay(500);
                contextMenuList.AddRange(GetContextMenuList());
            });

            foreach (ContextMenuModel contextMenuItem in contextMenuList)
            {
                try
                {
                    BitmapImage bitmapImage = new()
                    {
                        UriSource = contextMenuItem.PackageIconUri
                    };
                    contextMenuItem.PackageIcon = bitmapImage;
                }
                catch (Exception)
                {
                    contextMenuItem.PackageIcon = new BitmapImage();
                }

                if (string.IsNullOrEmpty(SearchAppNameText))
                {
                    ContextMenuCollection.Add(contextMenuItem);
                    continue;
                }
                else
                {
                    if (!string.IsNullOrEmpty(contextMenuItem.PackageDisplayName) && contextMenuItem.PackageDisplayName.Contains(SearchAppNameText))
                    {
                        ContextMenuCollection.Add(contextMenuItem);
                        continue;
                    }

                    if (!string.IsNullOrEmpty(contextMenuItem.PackageFullName) && contextMenuItem.PackageFullName.Contains(SearchAppNameText))
                    {
                        ContextMenuCollection.Add(contextMenuItem);
                        continue;
                    }
                }
            }

            IsContextMenuEmpty = contextMenuList.Count is 0;
            IsSearchEmpty = ContextMenuCollection.Count is 0;
            IsLoadCompleted = true;
        }

        #endregion 第一部分：右键菜单管理页面——挂载的事件

        /// <summary>
        /// 获取右键菜单列表信息
        /// </summary>
        private List<ContextMenuModel> GetContextMenuList()
        {
            List<ContextMenuModel> queryedContextMenuList = [];
            List<KeyValuePair<Guid, string>> blockedList = GetBlockedClsidList();
            List<INET_FIREWALL_APP_CONTAINER> inetLoopbackList = GetAppContainerList();

            try
            {
                RegistryKey packageListKey = Registry.LocalMachine.OpenSubKey(packageComPackageKey, false);

                if (packageListKey is not null)
                {
                    string[] packageFullNameArray = packageListKey.GetSubKeyNames();
                    int length = 0;
                    string currentPackageFullName = string.Empty;

                    if (Kernel32Library.GetCurrentPackageFullName(ref length, null) is 122)
                    {
                        StringBuilder packageFullNameBuilder = new(length + 1);
                        Kernel32Library.GetCurrentPackageFullName(ref length, packageFullNameBuilder);
                        currentPackageFullName = packageFullNameBuilder.ToString();
                    }

                    foreach (string packageFullName in packageFullNameArray)
                    {
                        if (packageFullName.Equals(currentPackageFullName))
                        {
                            continue;
                        }

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
                                            int index = blockedList.FindIndex(item => item.Key.Equals(clsid));

                                            contextMenuItemList.Add(new ContextMenuItemModel()
                                            {
                                                BlockedClsidType = index >= 0 && index < blockedList.Count ? Enum.TryParse(blockedList[index].Value, out BlockedClsidType blockedClsidType) ? blockedClsidType : BlockedClsidType.Unknown : BlockedClsidType.Unknown,
                                                Clsid = clsid,
                                                DllPath = dllPath,
                                                IsEnabled = index is -1,
                                                ThreadingMode = threading switch
                                                {
                                                    0 => ApartmentState.STA,
                                                    1 => ApartmentState.MTA,
                                                    _ => ApartmentState.Unknown,
                                                }
                                            });
                                        }
                                    }

                                    clsidKey.Close();
                                    clsidKey.Dispose();
                                }
                            }

                            length = 0;
                            string packagePath = string.Empty;

                            if (Kernel32Library.GetPackagePathByFullName(packageFullName, ref length, null) is 122)
                            {
                                StringBuilder packagePathBuilder = new(length + 1);
                                int result = Kernel32Library.GetPackagePathByFullName(packageFullName, ref length, packagePathBuilder);
                                packagePath = packagePathBuilder.ToString();
                            }

                            Tuple<string, string, List<Guid>> appInfo = GetAppInfo(packagePath);

                            if (appInfo.Item3.Count > 0)
                            {
                                StringBuilder displayNameBuilder = new(1024);
                                foreach (INET_FIREWALL_APP_CONTAINER inetContainerItem in inetLoopbackList)
                                {
                                    if (inetContainerItem.displayName.Contains(packageFullName))
                                    {
                                        ShlwapiLibrary.SHLoadIndirectString(inetContainerItem.displayName, displayNameBuilder, displayNameBuilder.Capacity, IntPtr.Zero);
                                    }
                                }

                                ContextMenuModel contextMenuItem = new()
                                {
                                    PackageDisplayName = string.IsNullOrEmpty(displayNameBuilder.ToString()) ? appInfo.Item1 : displayNameBuilder.ToString(),
                                    PackageFullName = packageFullName,
                                    PackageIconUri = Uri.TryCreate(appInfo.Item2, UriKind.Absolute, out Uri uri) ? uri : null,
                                    PackagePath = packagePath,
                                    ContextMenuItemCollection = [.. contextMenuItemList]
                                };

                                queryedContextMenuList.Add(contextMenuItem);
                            }

                            classKey.Close();
                            classKey.Dispose();
                        }
                    }

                    packageListKey.Close();
                    packageListKey.Dispose();
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, "Get com package clsidList info failed", e);
            }

            return queryedContextMenuList;
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

                    blockKey.Close();
                    blockKey.Dispose();
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, "Get com package blocked clsid clsidList in local machine registry failed", e);
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

                    blockKey.Close();
                    blockKey.Dispose();
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, "Get com package blocked clsid clsidList in current user registry failed", e);
            }

            return blockClsidList;
        }

        /// <summary>
        /// 检索打包应用信息
        /// </summary>
        /// <returns>
        /// 参数 1：返回应用的显示名称
        /// 参数 2：返回应用的显示图片位置
        /// 参数 3：返回应用的菜单 clsid 项列表
        /// </returns>
        private static Tuple<string, string, List<Guid>> GetAppInfo(string packageInstalledLocation)
        {
            try
            {
                if (string.IsNullOrEmpty(packageInstalledLocation))
                {
                    return Tuple.Create(string.Empty, string.Empty, new List<Guid>());
                }

                string manifestFilePath = Path.Combine(packageInstalledLocation, "AppxManifest.xml");

                if (File.Exists(manifestFilePath))
                {
                    string contents = File.ReadAllText(manifestFilePath);
                    XmlDocument xmlDocument = new();
                    xmlDocument.LoadXml(contents);

                    XmlNamespaceManager xmlNamespaceManager = new(xmlDocument.NameTable);
                    xmlNamespaceManager.AddNamespace("default", "http://schemas.microsoft.com/appx/manifest/foundation/windows10");
                    xmlNamespaceManager.AddNamespace("desktop4", "http://schemas.microsoft.com/appx/manifest/desktop/windows10/4");
                    xmlNamespaceManager.AddNamespace("desktop5", "http://schemas.microsoft.com/appx/manifest/desktop/windows10/5");
                    xmlNamespaceManager.AddNamespace("uap", "http://schemas.microsoft.com/appx/manifest/uap/windows10");

                    XmlNodeList desktop4VerbNodesList = xmlDocument.SelectNodes("//desktop4:FileExplorerContextMenus//desktop4:Verb", xmlNamespaceManager);
                    XmlNodeList desktop5VerbNodesList = xmlDocument.SelectNodes("//desktop4:FileExplorerContextMenus//desktop5:Verb", xmlNamespaceManager);

                    // 获取右键菜单项 CLSID 列表
                    List<Guid> clsidList = [];
                    if ((desktop4VerbNodesList?.Count ?? 0) + (desktop5VerbNodesList?.Count ?? 0) > 0)
                    {
                        if (desktop4VerbNodesList is not null)
                        {
                            for (int i = 0; i < desktop4VerbNodesList.Count; i++)
                            {
                                string clsid = desktop4VerbNodesList[i]?.Attributes?["Clsid"]?.Value;
                                if (Guid.TryParse(clsid, out Guid guid))
                                {
                                    clsidList.Add(guid);
                                }
                            }
                        }

                        if (desktop5VerbNodesList is not null)
                        {
                            for (int i = 0; i < desktop5VerbNodesList.Count; i++)
                            {
                                string clsid = desktop5VerbNodesList[i]?.Attributes?["Clsid"]?.Value;
                                if (Guid.TryParse(clsid, out Guid guid))
                                {
                                    clsidList.Add(guid);
                                }
                            }
                        }

                        clsidList = clsidList.Distinct().ToList();
                    }

                    // 获取应用的显示图片
                    XmlNode logoNode = xmlDocument.SelectSingleNode("//default:Properties/default:Logo", xmlNamespaceManager);
                    string logo = logoNode?.InnerText ?? string.Empty;
                    string logoFullPath = Path.Combine(packageInstalledLocation, logo);

                    if (!File.Exists(logoFullPath))
                    {
                        string logoDirectory = Path.GetDirectoryName(logoFullPath);
                        logoFullPath = string.Empty;
                        string logoKey = Path.GetFileNameWithoutExtension(logo);
                        string extension = Path.GetExtension(logo);
                        if (Directory.Exists(logoDirectory))
                        {
                            string[] files = Directory.GetFiles(logoDirectory, $"{logoKey}*{extension}");
                            logoFullPath = files?.FirstOrDefault(c => !c.Contains("contrast"));
                            if (string.IsNullOrEmpty(logoFullPath))
                            {
                                logoFullPath = files?.FirstOrDefault() ?? string.Empty;
                            }
                        }
                    }

                    // 获取应用的显示名称
                    string displayName = string.Empty;
                    XmlNodeList appNodesList = xmlDocument.SelectNodes("//uap:VisualElements", xmlNamespaceManager);

                    if (appNodesList is not null && appNodesList.Count > 0)
                    {
                        foreach (XmlNode appNode in appNodesList.OfType<XmlNode>().OrderBy(c => c.Attributes?["AppListEntry"]?.Value == "none" ? 1 : 0))
                        {
                            displayName = appNode.Attributes?["DisplayName"]?.Value ?? string.Empty;
                        }
                    }

                    return Tuple.Create(displayName, logoFullPath, clsidList);
                }

                return Tuple.Create(string.Empty, string.Empty, new List<Guid>());
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, string.Format("Get specified app info failed,location in {0}", packageInstalledLocation), e);
                return Tuple.Create(string.Empty, string.Empty, new List<Guid>());
            }
        }

        /// <summary>
        /// 获取设备所有应用容器数据列表
        /// </summary>
        private List<INET_FIREWALL_APP_CONTAINER> GetAppContainerList()
        {
            IntPtr arrayValue = IntPtr.Zero;
            uint size = 0;
            List<INET_FIREWALL_APP_CONTAINER> inetContainerList = [];

            GCHandle handle_pdwCntPublicACs = GCHandle.Alloc(size, GCHandleType.Pinned);
            GCHandle handle_ppACs = GCHandle.Alloc(arrayValue, GCHandleType.Pinned);
            FirewallAPILibrary.NetworkIsolationEnumAppContainers(NETISO_FLAG.NETISO_FLAG_MAX, out size, out arrayValue);

            IntPtr pACs = arrayValue;

            int structSize = Marshal.SizeOf<INET_FIREWALL_APP_CONTAINER>();

            for (int index = 0; index < size; index++)
            {
                INET_FIREWALL_APP_CONTAINER container = Marshal.PtrToStructure<INET_FIREWALL_APP_CONTAINER>(arrayValue);

                inetContainerList.Add(container);
                arrayValue = new IntPtr((long)arrayValue + structSize);
            }

            handle_pdwCntPublicACs.Free();
            handle_ppACs.Free();
            FirewallAPILibrary.NetworkIsolationFreeAppContainers(pACs);
            return inetContainerList;
        }
    }
}
