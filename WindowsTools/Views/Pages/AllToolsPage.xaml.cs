using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using WindowsTools.Models;
using WindowsTools.Services.Root;
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
                Title = ResourceService.AllToolsResource.GetString("Loaf"),
                Description = ResourceService.AllToolsResource.GetString("LoafDescription"),
                ImagePath = "ms-appx:///Assets/ControlIcon/Loaf.png",
                Tag = "Loaf"
            }
        ];

        // 文件工具列表
        private List<ControlItemModel> FileToolsList { get; } =
        [
            new ControlItemModel()
            {
                Title = ResourceService.AllToolsResource.GetString("FileName"),
                Description = ResourceService.AllToolsResource.GetString("FileNameDescription"),
                ImagePath = "ms-appx:///Assets/ControlIcon/FileName.png",
                Tag = "FileName"
            },
            new ControlItemModel()
            {
                Title = ResourceService.AllToolsResource.GetString("ExtensionName"),
                Description = ResourceService.AllToolsResource.GetString("ExtensionNameDescription"),
                ImagePath = "ms-appx:///Assets/ControlIcon/ExtensionName.png",
                Tag = "ExtensionName"
            },
            new ControlItemModel()
            {
                Title = ResourceService.AllToolsResource.GetString("UpperAndLowerCase"),
                Description = ResourceService.AllToolsResource.GetString("UpperAndLowerCaseDescription"),
                ImagePath = "ms-appx:///Assets/ControlIcon/UpperAndLowerCase.png",
                Tag = "UpperAndLowerCase"
            },
            new ControlItemModel()
            {
                Title = ResourceService.AllToolsResource.GetString("FileProperties"),
                Description = ResourceService.AllToolsResource.GetString("FilePropertiesDescription"),
                ImagePath = "ms-appx:///Assets/ControlIcon/FileProperties.png",
                Tag = "FileProperties"
            },
            new ControlItemModel()
            {
                Title = ResourceService.AllToolsResource.GetString("FileCertificate"),
                Description = ResourceService.AllToolsResource.GetString("FileCertificateDescription"),
                ImagePath = "ms-appx:///Assets/ControlIcon/FileCertificate.png",
                Tag = "FileCertificate"
            },
            new ControlItemModel()
            {
                Title = ResourceService.AllToolsResource.GetString("FileUnlock"),
                Description = ResourceService.AllToolsResource.GetString("FileUnlockDescription"),
                ImagePath = "ms-appx:///Assets/ControlIcon/FileUnlock.png",
                Tag = "FileUnlock"
            }
        ];

        // 资源工具列表
        private List<ControlItemModel> ResourceToolsList { get; } =
        [
            new ControlItemModel()
            {
                Title = ResourceService.AllToolsResource.GetString("DownloadManager"),
                Description = ResourceService.AllToolsResource.GetString("DownloadManagerDescription"),
                ImagePath = "ms-appx:///Assets/ControlIcon/DownloadManager.png",
                Tag = "DownloadManager"
            },
            new ControlItemModel()
            {
                Title = ResourceService.AllToolsResource.GetString("IconExtract"),
                Description = ResourceService.AllToolsResource.GetString("IconExtractDescription"),
                ImagePath = "ms-appx:///Assets/ControlIcon/IconExtract.png",
                Tag = "IconExtract"
            },
            new ControlItemModel()
            {
                Title = ResourceService.AllToolsResource.GetString("PriExtract"),
                Description = ResourceService.AllToolsResource.GetString("PriExtractDescription"),
                ImagePath = "ms-appx:///Assets/ControlIcon/PriExtract.png",
                Tag = "PriExtract"
            }
        ];

        // 个性化工具列表
        private List<ControlItemModel> PersonalizeToolsList { get; } =
        [
            new ControlItemModel()
            {
                Title = ResourceService.AllToolsResource.GetString("ShellMenu"),
                Description = ResourceService.AllToolsResource.GetString("ShellMenuDescription"),
                ImagePath = "ms-appx:///Assets/ControlIcon/ShellMenu.png",
                Tag = "ShellMenu"
            }
        ];

        // 系统工具列表
        private List<ControlItemModel> SystemToolsList { get; } =
        [
            //new ControlItemModel()
            //{
            //    Title = ResourceService.AllToolsResource.GetString("SystemInfo"),
            //    Description = ResourceService.AllToolsResource.GetString("SystemInfoDescription"),
            //    ImagePath = "ms-appx:///Assets/ControlIcon/SystemInfo.png",
            //    Tag = "SystemInfo"
            //},
            new ControlItemModel()
            {
                Title = ResourceService.AllToolsResource.GetString("LoopbackManager"),
                Description = ResourceService.AllToolsResource.GetString("LoopbackManagerDescription"),
                ImagePath = "ms-appx:///Assets/ControlIcon/LoopbackManager.png",
                Tag = "LoopbackManager"
            },
            //new ControlItemModel()
            //{
            //    Title = ResourceService.AllToolsResource.GetString("DriverManager"),
            //    Description = ResourceService.AllToolsResource.GetString("DriverManagerDescription"),
            //    ImagePath = "ms-appx:///Assets/ControlIcon/DriverManager.png",
            //    Tag = "DriverManager"
            //},
            //new ControlItemModel()
            //{
            //    Title = ResourceService.AllToolsResource.GetString("UpdateManager"),
            //    Description = ResourceService.AllToolsResource.GetString("UpdateManagerDescription"),
            //    ImagePath = "ms-appx:///Assets/ControlIcon/UpdateManager.png",
            //    Tag = "UpdateManager"
            //},
            new ControlItemModel()
            {
                Title = ResourceService.AllToolsResource.GetString("WinSAT"),
                Description = ResourceService.AllToolsResource.GetString("WinSATDescription"),
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
            if (args.ClickedItem is ControlItemModel controlItem && (MainWindow.Current.Content as MainPage).NavigationItemList.Find(item => item.NavigationTag.Equals(controlItem.Tag, StringComparison.OrdinalIgnoreCase)) is NavigationModel navigationItem)
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

        #endregion 第一部分：所有工具页面——挂载的事件
    }
}
