using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using WindowsTools.Models;
using WindowsTools.Strings;
using WindowsTools.Views.Windows;

namespace WindowsTools.Views.Pages
{
    /// <summary>
    /// 所有工具页面
    /// </summary>
    public sealed partial class AllToolsPage : Page
    {
        // 文件重命名工具列表
        private List<ControlItemModel> FileToolsList { get; } = new List<ControlItemModel>()
        {
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
        };

        // 系统工具列表
        private List<ControlItemModel> SystemToolsList { get; } = new List<ControlItemModel>()
        {
            new ControlItemModel()
            {
                Title = AllTools.SystemInfo,
                Description = AllTools.SystemInfoDescription,
                ImagePath = "ms-appx:///Assets/ControlIcon/SystemInfo.png",
                Tag = "SystemInfo"
            },
            new ControlItemModel()
            {
                Title = AllTools.WinSAT,
                Description = AllTools.WinSATDescription,
                ImagePath = "ms-appx:///Assets/ControlIcon/WinSAT.png",
                Tag = "WinSAT"
            },
        };

        public AllToolsPage()
        {
            InitializeComponent();
        }

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
                    (MainWindow.Current.Content as MainPage).NavigateTo(navigationItem.NavigationPage);
                }
            }
        }
    }
}
