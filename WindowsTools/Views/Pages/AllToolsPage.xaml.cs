using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using WindowsTools.Models;
using WindowsTools.Strings;
using WindowsTools.Views.Windows;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace WindowsTools.Views.Pages
{
    /// <summary>
    /// 所有工具页面
    /// </summary>
    public sealed partial class AllToolsPage : Page
    {
        private List<ControlItemModel> RelaxToolsList { get; } =
        [
            new ControlItemModel()
            {
                Title = AllTools.Loaf,
                Description = AllTools.LoafDescription,
                ImagePath = "ms-appx:///Assets/ControlIcon/Loaf.png",
                Tag = "Loaf"
            }
        ];

        // 文件工具列表
        private List<ControlItemModel> FileToolsList { get; } =
        [
            new ControlItemModel()
            {
                Title = AllTools.FileName,
                Description = AllTools.FileNameDescription,
                ImagePath = "ms-appx:///Assets/ControlIcon/FileName.png",
                Tag = "FileName"
            },
            new ControlItemModel()
            {
                Title = AllTools.ExtensionName,
                Description = AllTools.ExtensionNameDescription,
                ImagePath = "ms-appx:///Assets/ControlIcon/ExtensionName.png",
                Tag = "ExtensionName"
            },
            new ControlItemModel()
            {
                Title = AllTools.UpperAndLowerCase,
                Description = AllTools.UpperAndLowerCaseDescription,
                ImagePath = "ms-appx:///Assets/ControlIcon/UpperAndLowerCase.png",
                Tag = "UpperAndLowerCase"
            },
            new ControlItemModel()
            {
                Title = AllTools.FileProperties,
                Description = AllTools.FilePropertiesDescription,
                ImagePath = "ms-appx:///Assets/ControlIcon/FileProperties.png",
                Tag = "FileProperties"
            },
            new ControlItemModel()
            {
                Title = AllTools.FileCertificate,
                Description = AllTools.FileCertificateDescription,
                ImagePath = "ms-appx:///Assets/ControlIcon/FileCertificate.png",
                Tag = "FileCertificate"
            },
            new ControlItemModel()
            {
                Title = AllTools.FileUnlock,
                Description = AllTools.FileUnlockDescription,
                ImagePath = "ms-appx:///Assets/ControlIcon/FileUnlock.png",
                Tag = "FileUnlock"
            }
        ];

        // 资源工具列表
        private List<ControlItemModel> ResourceToolsList { get; } =
        [
            new ControlItemModel()
            {
                Title = AllTools.DownloadManager,
                Description = AllTools.DownloadManagerDescription,
                ImagePath = "ms-appx:///Assets/ControlIcon/DownloadManager.png",
                Tag = "DownloadManager"
            },
            new ControlItemModel()
            {
                Title = AllTools.CodeScanner,
                Description = AllTools.CodeScannerDescription,
                ImagePath = "ms-appx:///Assets/ControlIcon/CodeScanner.png",
                Tag = "CodeScanner"
            },
            new ControlItemModel()
            {
                Title = AllTools.IconExtract,
                Description = AllTools.IconExtractDescription,
                ImagePath = "ms-appx:///Assets/ControlIcon/IconExtract.png",
                Tag = "IconExtract"
            },
            new ControlItemModel()
            {
                Title = AllTools.PriExtract,
                Description = AllTools.PriExtractDescription,
                ImagePath = "ms-appx:///Assets/ControlIcon/PriExtract.png",
                Tag = "PriExtract"
            }
        ];

        // 个性化工具列表
        private List<ControlItemModel> PersonalizeToolsList { get; } =
        [
            new ControlItemModel()
            {
                Title = AllTools.ShellMenu,
                Description = AllTools.ShellMenuDescription,
                ImagePath = "ms-appx:///Assets/ControlIcon/ShellMenu.png",
                Tag = "ShellMenu"
            }
        ];

        // 系统工具列表
        private List<ControlItemModel> SystemToolsList { get; } =
        [
            //new ControlItemModel()
            //{
            //    Title = AllTools.SystemInfo,
            //    Description = AllTools.SystemInfoDescription,
            //    ImagePath = "ms-appx:///Assets/ControlIcon/SystemInfo.png",
            //    Tag = "SystemInfo"
            //},
            new ControlItemModel()
            {
                Title = AllTools.LoopbackManager,
                Description = AllTools.LoopbackManagerDescription,
                ImagePath = "ms-appx:///Assets/ControlIcon/LoopbackManager.png",
                Tag = "LoopbackManager"
            },
            //new ControlItemModel()
            //{
            //    Title = AllTools.DriverManager,
            //    Description = AllTools.DriverManagerDescription,
            //    ImagePath = "ms-appx:///Assets/ControlIcon/DriverManager.png",
            //    Tag = "DriverManager"
            //},
            //new ControlItemModel()
            //{
            //    Title = AllTools.UpdateManager,
            //    Description = AllTools.UpdateManagerDescription,
            //    ImagePath = "ms-appx:///Assets/ControlIcon/UpdateManager.png",
            //    Tag = "UpdateManager"
            //},
            new ControlItemModel()
            {
                Title = AllTools.WinSAT,
                Description = AllTools.WinSATDescription,
                ImagePath = "ms-appx:///Assets/ControlIcon/WinSAT.png",
                Tag = "WinSAT"
            }
        ];

        public AllToolsPage()
        {
            InitializeComponent();
        }

        #region 第一部分：所有工具页面——挂载的事件

        /// <summary>
        /// 点击条目时进入条目对应的页面
        /// </summary>
        private void OnItemClicked(object sender, ItemClickEventArgs args)
        {
            ControlItemModel controlItem = args.ClickedItem as ControlItemModel;

            if (controlItem is not null)
            {
                NavigationModel navigationItem = (MainWindow.Current.Content as MainPage).NavigationItemList.Find(item => item.NavigationTag.Equals(controlItem.Tag, StringComparison.OrdinalIgnoreCase));

                if (navigationItem is not null)
                {
                    if (navigationItem.NavigationPage == typeof(ShellMenuPage))
                    {
                        (MainWindow.Current.Content as MainPage).NavigateTo(navigationItem.NavigationPage, "ShellMenu");
                    }
                    else
                    {
                        (MainWindow.Current.Content as MainPage).NavigateTo(navigationItem.NavigationPage);
                    }
                }
            }
        }

        #endregion 第一部分：所有工具页面——挂载的事件
    }
}
