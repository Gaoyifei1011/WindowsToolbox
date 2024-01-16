using Microsoft.UI.Xaml.Controls;
using System;

namespace WindowsTools.Models
{
    /// <summary>
    /// 页面导航数据模型
    /// </summary>
    public class NavigationModel
    {
        /// <summary>
        /// 页面导航标签
        /// </summary>
        public string NavigationTag { get; set; }

        /// <summary>
        /// 页面导航子标签中对应的父标签
        /// </summary>
        public string ParentTag { get; set; }

        /// <summary>
        /// 页面导航控件中项的容器
        /// </summary>
        public NavigationViewItem NavigationItem { get; set; }

        /// <summary>
        /// 页面导航类型
        /// </summary>
        public Type NavigationPage { get; set; }
    }
}
