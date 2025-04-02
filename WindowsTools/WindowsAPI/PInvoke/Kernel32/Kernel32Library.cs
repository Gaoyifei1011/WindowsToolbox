using System;
using System.Runtime.InteropServices;
using System.Text;

// 抑制 CA1401 警告
#pragma warning disable CA1401

namespace WindowsTools.WindowsAPI.PInvoke.Kernel32
{
    /// <summary>
    /// Kernel32.dll 函数库
    /// </summary>
    public static class Kernel32Library
    {
        private const string Kernel32 = "kernel32.dll";

        public const long APPMODEL_ERROR_NO_PACKAGE = 15700L;

        /// <summary>
        /// 为指定文件创建或打开命名或未命名的文件映射对象。若要为物理内存指定 NUMA 节点，请参阅 CreateFileMappingNuma。
        /// </summary>
        /// <param name="hFile">
        /// 要从中创建文件映射对象的文件的句柄。
        /// 必须使用与 flProtect 参数指定的保护标志兼容的访问权限打开该文件。 这不是必需的，但建议打开要映射的文件进行独占访问。 有关详细信息，请参阅 文件安全和访问权限。
        /// 如果 INVALID_HANDLE_VALUEhFile，调用过程还必须在 dwMaximumSizeHigh 中指定文件映射对象的大小，并 dwMaximumSizeLow 参数。 在此方案中，CreateFileMapping 创建由系统分页文件而不是文件系统中的文件提供支持的指定大小的文件映射对象。</param>
        /// <param name="lpFileMappingAttributes">
        /// 指向 SECURITY_ATTRIBUTES 结构的指针，该结构确定返回的句柄是否可以由子进程继承。 SECURITY_ATTRIBUTES 结构的 lpSecurityDescriptor 成员为新的文件映射对象指定安全描述符。
        /// 如果 lpAttributesNULL，则无法继承句柄，并且文件映射对象获取默认的安全描述符。 文件映射对象的默认安全描述符中的访问控制列表（ACL）来自创建者的主要或模拟令牌。 有关详细信息，请参阅 文件映射安全和访问权限。</param>
        /// <param name="flProtect">指定文件映射对象的页面保护。 对象的所有映射视图都必须与此保护兼容。</param>
        /// <param name="dwMaximumSizeHigh">文件映射对象的最大大小的高阶 DWORD。</param>
        /// <param name="dwMaximumSizeLow">文件映射对象最大大小的低序 DWORD。
        /// 如果此参数和 dwMaximumSizeHigh 为 0（零），则文件映射对象的最大大小等于 hFile 标识的文件的当前大小。
        /// 尝试映射长度为 0（零）的文件失败，错误代码为 ERROR_FILE_INVALID。 应用程序应测试长度为 0（零）的文件，并拒绝这些文件。</param>
        /// <param name="lpName">
        /// 文件映射对象的名称。
        /// 如果此参数与现有映射对象的名称匹配，函数将请求访问具有 flProtect 保护的对象。
        /// 如果此参数 NULL，则创建文件映射对象时不带名称。
        /// 如果 lpName 与现有事件、信号灯、互斥体、可等待计时器或作业对象的名称匹配，则函数将失败，GetLastError 函数返回 ERROR_INVALID_HANDLE。 之所以发生这种情况，是因为这些对象共享相同的命名空间。
        /// 该名称可以具有“全局”或“本地”前缀，以在全局或会话命名空间中显式创建对象。 名称的其余部分可以包含除反斜杠字符（\）以外的任何字符。 从会话零以外的会话创建全局命名空间中的文件映射对象需要 SeCreateGlobalPrivilege 特权。 有关详细信息，请参阅 内核对象命名空间。
        /// 使用终端服务会话实现快速用户切换。 第一个登录的用户使用会话 0 （零），下一个登录的用户使用会话 1（一），依此等。 内核对象名称必须遵循终端服务概述的准则，以便应用程序能够支持多个用户。
        /// </param>
        /// <returns>如果函数成功，则返回值是新创建的文件映射对象的句柄。
        /// 如果在函数调用之前存在该对象，该函数将返回现有对象的句柄（其当前大小，而不是指定大小），GetLastError 返回 ERROR_ALREADY_EXISTS。
        /// 如果函数失败，则返回值 NULL。</returns>
        [DllImport(Kernel32, CharSet = CharSet.Unicode, EntryPoint = "CreateFileMappingW", PreserveSig = true, SetLastError = false)]
        public static extern IntPtr CreateFileMapping(IntPtr hFile, IntPtr lpFileMappingAttributes, PAGE_PROTECTION flProtect, uint dwMaximumSizeHigh, uint dwMaximumSizeLow, string lpName);

        /// <summary>
        /// 获取当前进程的 应用程序用户模型 ID 。
        /// </summary>
        /// <param name="applicationUserModelIdLength">输入时， applicationUserModelId 缓冲区的大小（以宽字符为单位）。 成功时，使用的缓冲区大小，包括 null 终止符。</param>
        /// <param name="applicationUserModelId">指向接收应用程序用户模型 ID 的缓冲区的指针。</param>
        /// <returns>如果该函数成功，则返回 ERROR_SUCCESS。 否则，该函数将返回错误代码。</returns>
        [DllImport(Kernel32, CharSet = CharSet.Unicode, EntryPoint = "GetCurrentApplicationUserModelId", PreserveSig = true, SetLastError = false)]
        public static extern int GetCurrentApplicationUserModelId(ref uint applicationUserModelIdLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder applicationUserModelId);

        /// <summary>
        /// 获取调用进程的包系列名称。
        /// </summary>
        /// <param name="packageFamilyNameLength">输入时， packageFamilyName 缓冲区的大小（以字符为单位），包括 null 终止符。 输出时，返回的包系列名称的大小（以字符为单位），包括 null 终止符。</param>
        /// <param name="packageFamilyName">包系列名称。</param>
        /// <returns>如果函数成功，则返回 ERROR_SUCCESS。 否则，函数将返回错误代码。</returns>
        [DllImport(Kernel32, CharSet = CharSet.Unicode, EntryPoint = "GetCurrentPackageFamilyName", PreserveSig = true, SetLastError = false)]
        public static extern int GetCurrentPackageFamilyName(ref int packageFamilyNameLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder packageFamilyName);

        /// <summary>
        /// 获取调用进程的包全名。
        /// </summary>
        /// <param name="packageFullNameLength">输入时， packageFullName 缓冲区的大小（以字符为单位）。 输出时，返回包全名的大小（以字符为单位），包括 null 终止符。</param>
        /// <param name="packageFullName">包全名。</param>
        /// <returns>如果函数成功，则返回 ERROR_SUCCESS。 否则，函数将返回错误代码。</returns>
        [DllImport(Kernel32, CharSet = CharSet.Unicode, EntryPoint = "GetCurrentPackageFullName", PreserveSig = true, SetLastError = false)]
        public static extern int GetCurrentPackageFullName(ref int packageFullNameLength, StringBuilder packageFullName);

        /// <summary>
        /// 检索当前进程的伪句柄。
        /// </summary>
        /// <returns>返回值是当前进程的伪句柄。</returns>
        [DllImport(Kernel32, CharSet = CharSet.Unicode, EntryPoint = "GetCurrentProcess", PreserveSig = true, SetLastError = false)]
        public static extern IntPtr GetCurrentProcess();

        /// <summary>
        /// 检索调用线程的最后错误代码值。 最后一个错误代码按线程进行维护。 多个线程不会覆盖彼此的最后一个错误代码。
        /// </summary>
        /// <returns>
        /// 返回值是调用线程的最后错误代码。
        /// 设置最后错误代码的每个函数的文档的返回值部分记录了函数设置最后错误代码的条件。 设置线程最后错误代码的大多数函数在失败时设置它。 但是，某些函数还会在成功时设置最后一个错误代码。 如果未记录函数以设置最后一个错误代码，则此函数返回的值只是要设置的最新最后一个错误代码;某些函数在成功时将最后一个错误代码设置为 0，而其他函数则不这样做。
        /// </returns>
        [DllImport(Kernel32, CharSet = CharSet.Unicode, EntryPoint = "GetLastError", PreserveSig = true, SetLastError = false)]
        public static extern int GetLastError();

        /// <summary>
        /// 获取指定包的路径。
        /// </summary>
        /// <param name="packageFullName">包的全名。</param>
        /// <param name="pathLength">
        /// 指向包含包路径字符串中 WCHAR) 字符数 (的变量的指针，其中包含 null 终止符。
        /// 首先，将 NULL 传递给 路径 以获取字符数。 使用此数字为 路径分配内存空间。 然后传递此内存空间的地址以填充 路径。
        /// </param>
        /// <param name="path">指向接收包路径字符串（包括 null 终止符）的内存空间的指针。</param>
        /// <returns>如果函数成功，则返回 ERROR_SUCCESS。 否则，函数将返回错误代码。</returns>
        [DllImport(Kernel32, CharSet = CharSet.Unicode, EntryPoint = "GetPackagePathByFullName", PreserveSig = true, SetLastError = false)]
        public static extern int GetPackagePathByFullName([MarshalAs(UnmanagedType.LPWStr)] string packageFullName, ref int pathLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder path);

        /// <summary>
        /// 检索系统的电源状态。 状态指示系统是使用交流还是直流电源运行，电池当前是否正在充电，剩余的电池使用时间，以及节电模式是打开还是关闭。
        /// </summary>
        /// <param name="systemPowerStatus">指向接收状态信息的 SYSTEM_POWER_STATUS 结构的指针。</param>
        [DllImport(Kernel32, CharSet = CharSet.Unicode, EntryPoint = "GetSystemPowerStatus", PreserveSig = true, SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetSystemPowerStatus(out SYSTEM_POWER_STATUS lpSystemPowerStatus);

        /// <summary>
        /// 将文件映射的视图映射到调用进程的地址空间。若要为视图指定建议的基址，请使用 MapViewOfFileEx 函数。 但是，不建议使用这种做法。
        /// </summary>
        /// <param name="hFileMappingObject">文件映射对象的句柄。 CreateFileMapping 和 OpenFileMapping 函数返回此句柄。</param>
        /// <param name="dwDesiredAccess">对文件映射对象的访问类型，该对象确定页面的页面保护。 此参数可以是下列值之一，也可以是多个值的按位 OR 组合（如果适用）。</param>
        /// <param name="dwFileOffsetHigh">视图开始的文件偏移量 DWORD 高阶。</param>
        /// <param name="dwFileOffsetLow">要开始视图的文件偏移量 低序 DWORD。 高偏移和低偏移的组合必须在文件映射中指定偏移量。 它们还必须匹配系统的虚拟内存分配粒度。 也就是说，偏移量必须是 VirtualAlloc 分配粒度的倍数。 若要获取系统的 VirtualAlloc 内存分配粒度，请使用 GetSystemInfo 函数，该函数填充 SYSTEM_INFO 结构的成员。</param>
        /// <param name="dwNumberOfBytesToMap">要映射到视图的文件映射的字节数。 所有字节必须位于 CreateFileMapping指定的最大大小内。 如果此参数为 0（零），则映射从指定的偏移量扩展到文件映射的末尾。</param>
        /// <returns>如果函数成功，则返回值为映射视图的起始地址。如果函数失败，则返回值 NULL。 </returns>
        [DllImport(Kernel32, CharSet = CharSet.Unicode, EntryPoint = "MapViewOfFile", PreserveSig = true, SetLastError = false)]
        public static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, FileMapAccess dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, int dwNumberOfBytesToMap);

        /// <summary>
        /// 使应用程序能够通知系统它正在使用中，从而防止系统在应用程序运行时进入睡眠状态或关闭显示器。
        /// </summary>
        /// <param name="esFlags">线程的执行要求。</param>
        /// <returns>如果函数成功，则返回值为上一个线程执行状态。如果函数失败，则返回值为 NULL。</returns>
        [DllImport(Kernel32, CharSet = CharSet.Unicode, EntryPoint = "SetThreadExecutionState", PreserveSig = true, SetLastError = false)]
        public static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);
    }
}
