using System;

namespace PowerTools.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 下面是窗口类样式。
    /// </summary>
    [Flags]
    public enum WNDCLASS_STYLES : uint
    {
        /// <summary>
        /// 如果移动或大小调整更改了工作区的高度，则重新绘制整个窗口。
        /// </summary>
        CS_VREDRAW = 1,

        /// <summary>
        /// 如果移动或大小调整更改了工作区的宽度，则重绘整个窗口。
        /// </summary>
        CS_HREDRAW = 2,

        /// <summary>
        /// 当用户在光标位于属于 类的窗口中时双击鼠标时，将双击消息发送到窗口过程。
        /// </summary>
        CS_DBLCLKS = 8,

        /// <summary>
        /// 为类中的每个窗口分配唯一的设备上下文。
        /// </summary>
        CS_OWNDC = 32,

        /// <summary>
        /// 分配一个设备上下文，以便类中的所有窗口共享。 由于窗口类特定于进程，因此应用程序的多个线程可以创建同一类的窗口。 线程还可以尝试同时使用设备上下文。 发生这种情况时，系统仅允许一个线程成功完成其绘制操作。
        /// </summary>
        CS_CLASSDC = 64,

        /// <summary>
        /// 将子窗口的剪裁矩形设置为父窗口的剪裁矩形，以便子窗口可以在父窗口上绘制。 具有 CS_PARENTDC 样式位的窗口从系统的设备上下文缓存接收常规设备上下文。 它不会为子级提供父级的设备上下文或设备上下文设置。 指定 CS_PARENTDC 可增强应用程序的性能。
        /// </summary>
        CS_PARENTDC = 128,

        /// <summary>
        /// 在窗口菜单上禁用 “关闭 ”。
        /// </summary>
        CS_NOCLOSE = 512,

        /// <summary>
        /// 保存此类窗口遮盖的屏幕图像部分作为位图。 删除窗口时，系统会使用保存的位图还原屏幕图像，包括被遮盖的其他窗口。 因此，如果位图使用的内存尚未丢弃，并且其他屏幕操作未使存储的图像失效，则系统不会将 WM_PAINT 消息发送到被遮盖的窗口。
        /// 此样式适用于小型窗口 (例如菜单或对话框) ，这些菜单或对话框在发生其他屏幕活动之前会短暂显示，然后删除。 此样式会增加显示窗口所需的时间，因为系统必须先分配内存来存储位图。
        /// </summary>
        CS_SAVEBITS = 2048,

        /// <summary>
        /// 将窗口的工作区与 x 方向) 的字节边界 (对齐。 此样式会影响窗口的宽度及其在显示器上的水平位置。
        /// </summary>
        CS_BYTEALIGNCLIENT = 4096,

        /// <summary>
        /// 使窗口在字节边界 (沿 x 方向) 对齐。 此样式会影响窗口的宽度及其在显示器上的水平位置。
        /// </summary>
        CS_BYTEALIGNWINDOW = 8192,

        /// <summary>
        /// 指示窗口类是应用程序全局类。 有关详细信息，请参阅 关于窗口类的“应用程序全局类”部分。
        /// </summary>
        CS_GLOBALCLASS = 16384,

        CS_IME = 65536,

        /// <summary>
        /// 在窗口上启用投影效果。 通过 SPI_SETDROPSHADOW打开和关闭效果。 通常，对于小型、生存期较短的窗口（如菜单）启用此功能，以强调其与其他窗口的 Z 顺序关系。 从具有此样式的类创建的 Windows 必须是顶级窗口;它们可能不是子窗口。
        /// </summary>
        CS_DROPSHADOW = 131072,
    }
}
