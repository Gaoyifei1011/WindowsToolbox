using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WindowsTools.Extensions.DataType.Enums;
using WindowsTools.Helpers.Controls;
using WindowsTools.Helpers.Root;
using WindowsTools.Services.Root;
using WindowsTools.UI.TeachingTips;
using WindowsTools.WindowsAPI.PInvoke.KernelAppCore;

// 抑制 CA1806 警告
#pragma warning disable CA1806

namespace WindowsTools.UI.Dialogs
{
    /// <summary>
    /// 应用信息对话框
    /// </summary>
    public sealed partial class AppInformationDialog : ContentDialog
    {
        private ObservableCollection<DictionaryEntry> AppInformationCollection { get; } = [];

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

                KernelAppCoreLibrary.GetCurrentPackageInfo2(PACKAGE_FLAGS.PACKAGE_PROPERTY_STATIC, PackagePathType.PackagePathType_Install, ref bufferLength, null, out uint count);

                if (count > 0)
                {
                    List<PACKAGE_INFO> packageInfoList = [];
                    byte[] buffer = new byte[bufferLength];
                    KernelAppCoreLibrary.GetCurrentPackageInfo2(PACKAGE_FLAGS.PACKAGE_PROPERTY_STATIC, PackagePathType.PackagePathType_Install, ref bufferLength, buffer, out count);

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
                            dependencyInformationList.Add(new KeyValuePair<string, Version>(ResourceService.DialogResource.GetString("WinUI2Version"), new Version(winUI2File.ProductMajorPart, winUI2File.ProductMinorPart, winUI2File.ProductBuildPart, winUI2File.ProductPrivatePart)));
                        }
                    }

                    // Windows UI 版本信息
                    FileVersionInfo windowsUIFile = FileVersionInfo.GetVersionInfo(Path.Combine(Environment.SystemDirectory, "Windows.UI.Xaml.dll"));
                    dependencyInformationList.Add(new KeyValuePair<string, Version>(ResourceService.DialogResource.GetString("WindowsUIVersion"), new Version(windowsUIFile.ProductMajorPart, windowsUIFile.ProductMinorPart, windowsUIFile.ProductBuildPart, windowsUIFile.ProductPrivatePart)));

                    // .NET 版本信息
                    dependencyInformationList.Add(new KeyValuePair<string, Version>(ResourceService.DialogResource.GetString("DoNetVersion"), new Version(RuntimeInformation.FrameworkDescription.Remove(0, 15))));
                }
            });

            foreach (KeyValuePair<string, Version> dependencyInformation in dependencyInformationList)
            {
                AppInformationCollection.Add(new DictionaryEntry(dependencyInformation.Key, dependencyInformation.Value));
            }
        }

        /// <summary>
        /// 复制应用信息
        /// </summary>
        private async void OnCopyAppInformationClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            args.Cancel = true;
            StringBuilder stringBuilder = new();

            await Task.Run(() =>
            {
                StringBuilder stringBuilder = new();
                foreach (DictionaryEntry appInformationItem in AppInformationCollection)
                {
                    stringBuilder.Append(appInformationItem.Key);
                    stringBuilder.Append(appInformationItem.Value);
                    stringBuilder.Append(Environment.NewLine);
                }
            });

            bool copyResult = CopyPasteHelper.CopyToClipboard(stringBuilder.ToString());
            sender.Hide();
            await TeachingTipHelper.ShowAsync(new DataCopyTip(DataCopyKind.AppInformation, copyResult));
        }
    }
}
