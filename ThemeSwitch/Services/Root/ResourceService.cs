using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Resources;
using Windows.UI.Xaml;

namespace ThemeSwitch.Services.Root
{
    /// <summary>
    /// 应用资源服务
    /// </summary>
    public static class ResourceService
    {
        private static Assembly CurrentAssembly { get; } = Assembly.LoadFrom(Path.Combine(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), "PowerTools.exe"));

        public static List<string> ThemeList { get; } = [];

        public static ResourceManager ThemeSwitchResource { get; } = new("PowerTools.Strings.ThemeSwitch", CurrentAssembly);
    }
}
