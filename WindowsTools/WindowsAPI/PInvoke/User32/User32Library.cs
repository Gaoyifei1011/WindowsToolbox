using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WindowsTools.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// User32.dll 函数库
    /// </summary>
    public static class User32Library
    {
        private const string User32 = "user32.dll";

        /// <summary>
        /// 将挂钩信息传递给当前挂钩链中的下一个挂钩过程。 挂钩过程可以在处理挂钩信息之前或之后调用此函数。
        /// </summary>
        /// <param name="idHook">传递给当前挂钩过程的类型。</param>
        /// <param name="nCode">传递给当前挂钩过程的挂钩代码。 下一个挂钩过程使用此代码来确定如何处理挂钩信息。</param>
        /// <param name="wParam">传递给当前挂钩过程的 wParam 值。 此参数的含义取决于与当前挂钩链关联的挂钩类型。</param>
        /// <param name="lParam">传递给当前挂钩过程的 lParam 值。 此参数的含义取决于与当前挂钩链关联的挂钩类型。</param>
        /// <returns>此值由链中的下一个挂钩过程返回。 当前挂钩过程还必须返回此值。 返回值的含义取决于挂钩类型。 有关详细信息，请参阅各个挂钩过程的说明。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "CallNextHookEx", SetLastError = false), PreserveSig]
        public static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, UIntPtr wParam, IntPtr lParam);

        /// <summary>
        /// 修改指定窗口的用户界面特权隔离 (UIPI) 消息筛选器。
        /// </summary>
        /// <param name="hWnd">要修改其 UIPI 消息筛选器的窗口的句柄。</param>
        /// <param name="message">消息筛选器允许通过或阻止的消息。</param>
        /// <param name="action">要执行的操作，可以执行以下值</param>
        /// <param name="pChangeFilterStruct">指向 CHANGEFILTERSTRUCT 结构的可选指针。</param>
        /// <returns>如果函数成功，则返回 TRUE;否则，它将返回 FALSE。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "ChangeWindowMessageFilterEx", SetLastError = false), PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ChangeWindowMessageFilterEx(IntPtr hWnd, WindowMessage message, ChangeFilterAction action, in CHANGEFILTERSTRUCT pChangeFilterStruct);

        /// <summary>
        /// 检索一个窗口的句柄，该窗口的类名和窗口名称与指定的字符串匹配。 该函数搜索子窗口，从指定子窗口后面的子窗口开始。 此函数不执行区分大小写的搜索。
        /// </summary>
        /// <param name="hWndParent">要搜索其子窗口的父窗口的句柄。如果 hwndParent 为 NULL，则该函数使用桌面窗口作为父窗口。 函数在桌面的子窗口之间搜索。 如果 hwndParent 为HWND_MESSAGE，则函数将搜索所有 仅消息窗口。</param>
        /// <param name="hWndChildAfter">子窗口的句柄。 搜索从 Z 顺序中的下一个子窗口开始。 子窗口必须是 hwndParent 的直接子窗口，而不仅仅是子窗口。 如果 hwndChildAfter 为 NULL，则搜索从 hwndParent 的第一个子窗口开始。请注意，如果 hwndParent 和 hwndChildAfter 均为 NULL，则该函数将搜索所有顶级窗口和仅消息窗口。</param>
        /// <param name="lpszClass">类名或上一次对 RegisterClass 或 RegisterClassEx 函数的调用创建的类名或类原子。 原子必须置于 lpszClass 的低序单词中;高阶单词必须为零。如果 lpszClass 是字符串，则指定窗口类名。 类名可以是注册到 RegisterClass 或 RegisterClassEx 的任何名称，也可以是预定义的控件类名称，也可以是 MAKEINTATOM(0x8000)。 在此后一种情况下，0x8000是菜单类的原子。 </param>
        /// <param name="lpszWindow">窗口名称 (窗口的标题) 。 如果此参数为 NULL，则所有窗口名称都匹配。</param>
        /// <returns>如果函数成功，则返回值是具有指定类和窗口名称的窗口的句柄。如果函数失败，则返回值为 NULL。 要获得更多的错误信息，请调用 GetLastError。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "FindWindowExW", SetLastError = false), PreserveSig]
        public static extern IntPtr FindWindowEx(IntPtr hWndParent, IntPtr hWndChildAfter, [MarshalAs(UnmanagedType.LPWStr)] string lpszClass, [MarshalAs(UnmanagedType.LPWStr)] string lpszWindow);

        /// <summary>
        /// 锁定工作站的显示器。 锁定工作站可防止未经授权的使用。
        /// </summary>
        /// <returns>如果该函数成功，则返回值为非零值。 由于函数以异步方式执行，因此非零返回值指示操作已启动。 它并不指示工作站是否已成功锁定。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "LockWorkStation", SetLastError = false), PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool LockWorkStation();

        /// <summary>
        /// 合成键击。 系统可以使用这种合成的击键来生成 WM_KEYUP 或 WM_KEYDOWN 消息。 键盘驱动程序的中断处理程序调用 keybd_event 函数。
        /// </summary>
        /// <param name="bVk">虚拟密钥代码。 代码必须是 1 到 254 范围内的值。</param>
        /// <param name="bScan">密钥的硬件扫描代码。</param>
        /// <param name="dwFlags">控制函数操作的各个方面。 此参数可使用以下一个或多个值。</param>
        /// <param name="dwExtraInfo">与键笔划关联的附加值。</param>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "keybd_event", SetLastError = false), PreserveSig]
        public static extern void keybd_event(Keys bVk, byte bScan, KEYEVENTFLAGS dwFlags, UIntPtr dwExtraInfo);

        /// <summary>
        /// 创建从指定文件中提取的图标的句柄数组。
        /// </summary>
        /// <param name="lpszFile">要从中提取图标的文件的路径和名称。</param>
        /// <param name="nIconIndex">要提取的第一个图标的从零开始的索引。 例如，如果此值为零，则函数提取指定文件中的第一个图标。</param>
        /// <param name="cxIcon">所需的水平图标大小。</param>
        /// <param name="cyIcon">所需的垂直图标大小。 </param>
        /// <param name="phicon">指向返回的图标句柄数组的指针。</param>
        /// <param name="piconid">指向图标返回的资源标识符的指针，该图标最适合当前显示设备。 如果标识符不可用于此格式，则返回的标识符0xFFFFFFFF。 如果无法以其他方式获取标识符，则返回的标识符为 0。</param>
        /// <param name="nIcons">要从文件中提取的图标数。 仅当从 .exe 和 .dll 文件中提取时，此参数才有效。</param>
        /// <param name="flags">指定控制此函数的标志。 这些标志是 LoadImage 函数使用的LR_* 标志。</param>
        /// <returns>
        /// 如果 phicon 参数为 NULL 并且此函数成功，则返回值是文件中的图标数。 如果函数失败，则返回值为 0。如果 phicon 参数不为 NULL 且函数成功，则返回值是提取的图标数。 否则，如果未找到该文件，则返回值0xFFFFFFFF。
        /// </returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "PrivateExtractIconsW", SetLastError = false), PreserveSig]
        public static extern int PrivateExtractIcons([MarshalAs(UnmanagedType.LPWStr)] string lpszFile, int nIconIndex, int cxIcon, int cyIcon, IntPtr[] phicon, int[] piconid, int nIcons, int flags);

        /// <summary>
        /// 注册应用程序以接收特定电源设置事件的电源设置通知。
        /// </summary>
        /// <param name="hRecipient">指示电源设置通知的发送位置的句柄。 对于交互式应用程序， Flags 参数应为零， hRecipient 参数应为窗口句柄。 对于服务，Flags 参数应为 1，hRecipient 参数应为从 RegisterServiceCtrlHandlerEx 返回的SERVICE_STATUS_HANDLE。</param>
        /// <param name="PowerSettingGuid">要为其发送通知的电源设置的 GUID 。</param>
        /// <param name="Flags">
        /// DEVICE_NOTIFY_WINDOW_HANDLE：使用 wParam 参数为 PBT_POWERSETTINGCHANGE 的WM_POWERBROADCAST消息发送通知。
        /// DEVICE_NOTIFY_SERVICE_HANDLE：通知发送到 HandlerEx 回调函数，其中 dwControl 参数 为 SERVICE_CONTROL_POWEREVENT ， dwEventType为 PBT_POWERSETTINGCHANGE。
        /// </param>
        /// <returns>返回用于取消注册电源通知的通知句柄。 如果函数失败，则返回值为 NULL。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "RegisterPowerSettingNotification", SetLastError = false), PreserveSig]
        public static extern IntPtr RegisterPowerSettingNotification(IntPtr hRecipient, [MarshalAs(UnmanagedType.LPStruct)] ref Guid PowerSettingGuid, uint Flags);

        /// <summary>
        /// 将指定的消息发送到窗口或窗口。SendMessage 函数调用指定窗口的窗口过程，在窗口过程处理消息之前不会返回。
        /// </summary>
        /// <param name="hWnd">
        /// 窗口过程的句柄将接收消息。 如果此参数 HWND_BROADCAST ( (HWND) 0xffff) ，则会将消息发送到系统中的所有顶级窗口，
        /// 包括已禁用或不可见的未所有者窗口、重叠窗口和弹出窗口;但消息不会发送到子窗口。消息发送受 UIPI 的约束。
        /// 进程的线程只能将消息发送到较低或等于完整性级别的线程的消息队列。
        /// </param>
        /// <param name="wMsg">要发送的消息。</param>
        /// <param name="wParam">其他的消息特定信息。</param>
        /// <param name="lParam">其他的消息特定信息。</param>
        /// <returns>返回值指定消息处理的结果;这取决于发送的消息。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "SendMessageW", SetLastError = false), PreserveSig]
        public static extern IntPtr SendMessage(IntPtr hWnd, WindowMessage wMsg, UIntPtr wParam, IntPtr lParam);

        /// <summary>
        /// 更改子窗口、弹出窗口或顶级窗口的大小、位置和 Z 顺序。 这些窗口根据屏幕上的外观进行排序。 最上面的窗口接收最高排名，是 Z 顺序中的第一个窗口。
        /// </summary>
        /// <param name="hWnd">更改子窗口、弹出窗口或顶级窗口的大小、位置和 Z 顺序。 这些窗口根据屏幕上的外观进行排序。 最上面的窗口接收最高排名，是 Z 顺序中的第一个窗口。</param>
        /// <param name="hWndInsertAfter">在 Z 顺序中定位窗口之前窗口的句柄。 </param>
        /// <param name="X">在 Z 顺序中定位窗口之前窗口的句柄。 </param>
        /// <param name="Y">窗口顶部的新位置，以客户端坐标表示。</param>
        /// <param name="cx">窗口的新宽度（以像素为单位）。</param>
        /// <param name="cy">窗口的新高度（以像素为单位）。</param>
        /// <param name="uFlags">窗口大小调整和定位标志。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。 </returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "SetWindowPos", SetLastError = false), PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

        /// <summary>
        /// 将应用程序定义的挂钩过程安装到挂钩链中。 你将安装挂钩过程来监视系统的某些类型的事件。 这些事件与特定线程或与调用线程位于同一桌面中的所有线程相关联。
        /// </summary>
        /// <param name="idHook">要安装的挂钩过程的类型。</param>
        /// <param name="lpfn">指向挂钩过程的指针。 如果 dwThreadId 参数为零或指定由其他进程创建的线程的标识符， 则 lpfn 参数必须指向 DLL 中的挂钩过程。 否则， lpfn 可以指向与当前进程关联的代码中的挂钩过程。</param>
        /// <param name="hMod">DLL 的句柄，其中包含 lpfn 参数指向的挂钩过程。 如果 dwThreadId 参数指定当前进程创建的线程，并且挂钩过程位于与当前进程关联的代码中，则必须将 hMod 参数设置为 NULL。</param>
        /// <param name="dwThreadId">要与挂钩过程关联的线程的标识符。 对于桌面应用，如果此参数为零，则挂钩过程与调用线程在同一桌面中运行的所有现有线程相关联。 对于 Windows 应用商店应用，请参阅“备注”部分。</param>
        /// <returns>如果函数成功，则返回值是挂钩过程的句柄。如果函数失败，则返回值为 NULL。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "SetWindowsHookExW", SetLastError = false), PreserveSig]
        public static extern IntPtr SetWindowsHookEx(HOOKTYPE idHook, HOOKPROC lpfn, IntPtr hMod, int dwThreadId);

        /// <summary>
        /// 删除 SetWindowsHookEx 函数安装在挂钩链中的挂钩过程。
        /// </summary>
        /// <param name="idHook">要移除的挂钩的句柄。 此参数是由先前调用 SetWindowsHookEx 获取的挂钩句柄。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "UnhookWindowsHookEx", SetLastError = false), PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr idHook);

        /// <summary>
        /// 取消注册电源设置通知。
        /// </summary>
        /// <param name="handle">从 RegisterPowerSettingNotification 函数返回的句柄。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "UnregisterPowerSettingNotification", SetLastError = false), PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnregisterPowerSettingNotification(IntPtr handle);
    }
}
