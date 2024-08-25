using System;
using System.Collections.Generic;

namespace WindowsToolsShellExtension.Extensions.ShellMenu
{
    /// <summary>
    /// 菜单项设置选项
    /// </summary>
    public sealed class ShellMenuItem
    {
        /// <summary>
        /// 菜单键值
        /// </summary>
        public string MenuKey { get; set; }

        /// <summary>
        /// 菜单项 GUID 值
        /// </summary>
        public Guid MenuGuid { get; set; }

        /// <summary>
        /// 菜单项标题
        /// </summary>
        public string MenuTitleText { get; set; }

        /// <summary>
        /// 是否使用图标
        /// </summary>
        public bool ShouldUseIcon { get; set; }

        /// <summary>
        /// 是否使用应用程序图标
        /// </summary>
        public bool ShouldUseProgramIcon { get; set; }

        /// <summary>
        /// 是否使用主题图标
        /// </summary>
        public bool ShouldUseThemeIcon { get; set; }

        /// <summary>
        /// 默认的菜单项图标
        /// </summary>
        public string DefaultIconPath { get; set; }

        /// <summary>
        /// 浅色主题下的菜单项图标
        /// </summary>
        public string LightThemeIconPath { get; set; }

        /// <summary>
        /// 深色主题下的菜单项图标
        /// </summary>
        public string DarkThemeIconPath { get; set; }

        /// <summary>
        /// 菜单程序路径
        /// </summary>
        public string MenuProgramPath { get; set; }

        /// <summary>
        /// 菜单参数
        /// </summary>
        public string MenuParameter { get; set; }

        /// <summary>
        /// 是否启用文件夹背景菜单项
        /// </summary>
        public bool FolderBackground { get; set; }

        /// <summary>
        /// 是否启用文件夹桌面菜单项
        /// </summary>
        public bool FolderDesktop { get; set; }

        /// <summary>
        /// 是否启用文件夹目录菜单项
        /// </summary>
        public bool FolderDirectory { get; set; }

        /// <summary>
        /// 是否启用文件夹驱动器菜单项
        /// </summary>
        public bool FolderDrive { get; set; }

        /// <summary>
        /// 菜单项文件匹配规则
        /// </summary>
        public string MenuFileMatchRule { get; set; }

        /// <summary>
        /// 菜单项文件匹配格式
        /// </summary>
        public string MenuFileMatchFormatText { get; set; }

        /// <summary>
        /// 菜单项索引
        /// </summary>
        public int MenuIndex { get; set; }

        /// <summary>
        /// 子菜单项
        /// </summary>
        public List<ShellMenuItem> SubShellMenuItem { get; set; } = [];
    }
}
