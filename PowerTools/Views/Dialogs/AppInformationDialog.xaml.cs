using PowerTools.Extensions.DataType.Enums;
using PowerTools.Helpers.Root;
using PowerTools.Services.Root;
using PowerTools.Views.TeachingTips;
using PowerTools.Views.Windows;
using PowerTools.WindowsAPI.PInvoke.KernelAppCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// 抑制 CA1806，IDE0060 警告
#pragma warning disable CA1806,IDE0060

namespace PowerTools.Views.Dialogs
{
    /// <summary>
    /// 应用信息对话框
    /// </summary>
    public sealed partial class AppInformationDialog : ContentDialog, INotifyPropertyChanged
    {
        private readonly string WinUI2VersionString = ResourceService.DialogResource.GetString("WinUI2Version");
        private readonly string WindowsUIVersionString = ResourceService.DialogResource.GetString("WindowsUIVersion");
        private readonly string DoNetVersionString = ResourceService.DialogResource.GetString("DoNetVersion");

        private bool _isLoadCompleted = false;

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

        private ObservableCollection<DictionaryEntry> AppInformationCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public AppInformationDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 应用信息初始化触发的事件
        /// </summary>
        private async void OnLoaded(object sender, RoutedEventArgs args)
        {
            List<KeyValuePair<string, Version>> dependencyInformationList = [];
            await Task.Run(() =>
            {
                uint bufferLength = 0;

                KernelAppCoreLibrary.GetCurrentPackageInfo(PACKAGE_FLAGS.PACKAGE_PROPERTY_STATIC, ref bufferLength, null, out uint count);

                if (count > 0)
                {
                    List<PACKAGE_INFO> packageInfoList = [];
                    byte[] buffer = new byte[bufferLength];
                    KernelAppCoreLibrary.GetCurrentPackageInfo(PACKAGE_FLAGS.PACKAGE_PROPERTY_STATIC, ref bufferLength, buffer, out count);

                    for (int index = 0; index < count; index++)
                    {
                        int packageInfoSize = Marshal.SizeOf<PACKAGE_INFO>();
                        IntPtr packageInfoPtr = Marshal.AllocHGlobal(packageInfoSize);
                        Marshal.Copy(buffer, index * packageInfoSize, packageInfoPtr, packageInfoSize);
                        PACKAGE_INFO packageInfo = Marshal.PtrToStructure<PACKAGE_INFO>(packageInfoPtr);
                        packageInfoList.Add(packageInfo);
                        Marshal.FreeHGlobal(packageInfoPtr);
                    }

                    foreach (PACKAGE_INFO packageInfo in packageInfoList)
                    {
                        // WinUI 2 版本信息
                        if (packageInfo.packageFullName.Contains("Microsoft.UI.Xaml"))
                        {
                            FileVersionInfo winUI2File = FileVersionInfo.GetVersionInfo(Path.Combine(packageInfo.path, "Microsoft.UI.Xaml.dll"));
                            dependencyInformationList.Add(new KeyValuePair<string, Version>(WinUI2VersionString, new Version(winUI2File.ProductMajorPart, winUI2File.ProductMinorPart, winUI2File.ProductBuildPart, winUI2File.ProductPrivatePart)));
                        }
                    }

                    // Windows UI 版本信息
                    FileVersionInfo windowsUIFile = FileVersionInfo.GetVersionInfo(Path.Combine(Environment.SystemDirectory, "Windows.UI.Xaml.dll"));
                    dependencyInformationList.Add(new KeyValuePair<string, Version>(WindowsUIVersionString, new Version(windowsUIFile.ProductMajorPart, windowsUIFile.ProductMinorPart, windowsUIFile.ProductBuildPart, windowsUIFile.ProductPrivatePart)));

                    // .NET 版本信息
                    dependencyInformationList.Add(new KeyValuePair<string, Version>(DoNetVersionString, new Version(RuntimeInformation.FrameworkDescription.Remove(0, 15))));
                }
            });

            foreach (KeyValuePair<string, Version> dependencyInformation in dependencyInformationList)
            {
                AppInformationCollection.Add(new DictionaryEntry(dependencyInformation.Key, dependencyInformation.Value));
            }

            IsLoadCompleted = true;
        }

        /// <summary>
        /// 复制应用信息
        /// </summary>
        private async void OnCopyAppInformationClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            bool copyResult = false;
            ContentDialogButtonClickDeferral contentDialogButtonClickDeferral = args.GetDeferral();

            try
            {
                StringBuilder stringBuilder = await Task.Run(() =>
                {
                    StringBuilder stringBuilder = new();
                    foreach (DictionaryEntry appInformationItem in AppInformationCollection)
                    {
                        stringBuilder.Append(appInformationItem.Key);
                        stringBuilder.Append(appInformationItem.Value);
                        stringBuilder.Append(Environment.NewLine);
                    }

                    return stringBuilder;
                });

                copyResult = CopyPasteHelper.CopyToClipboard(Convert.ToString(stringBuilder));
            }
            catch (Exception)
            {
                return;
            }
            finally
            {
                contentDialogButtonClickDeferral.Complete();
            }

            await MainWindow.Current.ShowNotificationAsync(new DataCopyTip(DataCopyKind.AppInformation, copyResult));
        }
    }
}
