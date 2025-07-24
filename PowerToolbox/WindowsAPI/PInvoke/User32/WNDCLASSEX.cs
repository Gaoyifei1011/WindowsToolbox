using System;
using System.Runtime.InteropServices;

namespace PowerToolbox.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 包含窗口类信息。 它与 RegisterClassEx 和 GetClassInfoEx 函数一起使用。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct WNDCLASSEX
    {
        /// <summary>
        /// 此结构的大小（以字节为单位）。 将此成员设置为 sizeof(WNDCLASSEX)。 在调用 GetClassInfoEx 函数之前，请务必设置此成员。
        /// </summary>
        public int cbSize;

        /// <summary>
        /// 类样式。 此成员可以是类样式的任意组合。
        /// </summary>
        public WNDCLASS_STYLES style;

        /// <summary>
        /// 指向窗口过程的指针。 必须使用 CallWindowProc 函数来调用窗口过程。 有关详细信息，请参阅 WindowProc。
        /// </summary>
        public WNDPROC lpfnWndProc;

        /// <summary>
        /// 要按照窗口类结构分配的额外字节数。 系统将字节初始化为零。
        /// </summary>
        public int cbClsExtra;

        /// <summary>
        /// 在窗口实例之后分配的额外字节数。 系统将字节初始化为零。 如果应用程序使用 WNDCLASSEX 注册通过使用资源文件中的 CLASS 指令创建的对话框，则必须将此成员设置为 DLGWINDOWEXTRA。
        /// </summary>
        public int cbWndExtra;

        /// <summary>
        /// 包含类的窗口过程的实例的句柄。
        /// </summary>
        public nint hInstance;

        /// <summary>
        /// 类图标的句柄。 此成员必须是图标资源的句柄。 如果此成员 NULL，则系统提供默认图标。
        /// </summary>
        public nint hIcon;

        /// <summary>
        /// 类游标的句柄。 此成员必须是游标资源的句柄。 如果此成员 NULL，则每当鼠标移动到应用程序的窗口中时，应用程序都必须显式设置光标形状。
        /// </summary>
        public nint hCursor;

        /// <summary>
        /// 类背景画笔的句柄。 此成员可以是用于绘制背景的画笔的句柄，也可以是颜色值。 颜色值必须是以下标准系统颜色之一（值 1 必须添加到所选颜色中）。
        /// </summary>
        public nint hbrBackground;

        /// <summary>
        /// 指向以 null 结尾的字符串的指针，该字符串指定类菜单的资源名称，因为名称显示在资源文件中。 如果使用整数来标识菜单，请使用 MAKEINTRESOURCE 宏。 如果此成员 NULL，则属于此类的窗口没有默认菜单。
        /// </summary>
        public string lpszMenuName;

        /// <summary>
        /// 指向以 null 结尾的字符串或原子的指针。 如果此参数是 atom，则它必须是上一次调用 RegisterClass 或 RegisterClassEx 函数创建的类 atom。 原子必须位于 lpszClassName的低序单词中;高序单词必须为零。
        /// 如果 lpszClassName 是字符串，则指定窗口类名。 类名称可以是注册到 RegisterClass 或 RegisterClassEx的任何名称，也可以是预定义的控件类名称。
        /// lpszClassName 的最大长度为 256。 如果 lpszClassName 大于最大长度，则 RegisterClassEx 函数将失败。
        /// </summary>
        public string lpszClassName;

        /// <summary>
        /// 与窗口类关联的小图标的句柄。 如果此成员 NULL，系统将搜索由 hIcon 成员指定的图标资源，以获取要用作小图标的相应大小的图标。
        /// </summary>
        public nint hIconSm;
    }
}
