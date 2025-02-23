using System;
using System.Runtime.InteropServices;

// 抑制 CA1401 警告
#pragma warning disable CA1401

namespace WindowsTools.WindowsAPI.PInvoke.Dismapi
{
    /// <summary>
    /// Dismapi.dll 函数库
    /// </summary>
    public static class DismapiLibrary
    {
        private const string Dismapi = "dismapi.dll";
        public const string DISM_ONLINE_IMAGE = "DISM_{53BFAE52-B167-4E2F-A258-0A37B57FF845}";

        /// <summary>
        /// 列出映像中的驱动程序。
        /// </summary>
        /// <param name="Session">一个有效的 DismSession。 DismSession 必须关联一个映像。 可以使用 DismOpenSession 将会话与映像关联。</param>
        /// <param name="AllDrivers">指定要检索的驱动程序的布尔值。
        /// TRUE：检索所有驱动程序。
        /// FALSE：仅检索开箱即用驱动程序。 即装即用驱动程序是最初未包含在 Windows 映像中的驱动程序。
        /// </param>
        /// <param name="DriverPackage">指向 DismDriverPackage 结构数组地址的指针。</param>
        /// <param name="Count">返回的 DismDriverPackage 结构的数量。</param>
        /// <returns>成功时返回 S_OK 。</returns>
        [DllImport(Dismapi, CharSet = CharSet.Unicode, EntryPoint = "DismGetDrivers", PreserveSig = true, SetLastError = false)]
        public static extern int DismGetDrivers(IntPtr Session, [MarshalAs(UnmanagedType.Bool)] bool AllDrivers, out IntPtr DriverPackage, out uint Count);

        /// <summary>
        /// 初始化 DISM API。 在调用任何其他 DISM API 函数之前，每个进程都必须调用一次 DismInitialize。
        /// </summary>
        /// <param name="logLevel">DismLogLevel 枚举值。</param>
        /// <param name="logFilePath">可选。 日志文件的相对或绝对路径。 生成的所有消息都将记录到此路径。 如果为 NULL，则将使用默认日志路径 %windir%\Logs\DISM\dism.log。</param>
        /// <param name="scratchDirectory">可选。 临时目录的相对或绝对路径。 DISM API 将使用此目录进行内部操作。 如果为 NULL，则将使用默认临时目录 \Windows\%Temp%。</param>
        /// <returns>如果成功，则返回 S_OK。如果进程已调用 DismInitialize，但未对 DismShutdown 进行相应调用，则返回 DISMAPI_E_DISMAPI_ALREADY_INITIALIZED。如果进程未提升，则返回 HRESULT_FROM_WIN32(ERROR_ELEVATION_REQUIRED)。</returns>
        [DllImport(Dismapi, CharSet = CharSet.Unicode, EntryPoint = "DismInitialize", PreserveSig = true, SetLastError = false)]
        public static extern int DismInitialize(DismLogLevel LogLevel, [MarshalAs(UnmanagedType.LPWStr)] string LogFilePath, [MarshalAs(UnmanagedType.LPWStr)] string ScratchDirectory);

        /// <summary>
        /// 将脱机或联机 Windows 映像与 DISMSession 关联。
        /// </summary>
        /// <param name="ImagePath">
        /// 将 ImagePath 设置为以下值之一：
        /// 脱机 Windows 映像的根目录的绝对路径或相对路径。
        /// 已装载 Windows 映像的根目录的绝对路径或相对路径。 可以使用外部工具或 DismMountImage 在调用 DismOpenSession 之前装载映像。
        /// DISM_ONLINE_IMAGE，用于将会话与联机 Windows 安装进行关联。
        /// </param>
        /// <param name="WindowsDirectory">
        /// 可选。 Windows 目录的相对路径或绝对路径。 路径相对于装入点。
        /// 如果 WindowsDirectory 的值为NULL，则使用“Windows”的默认值。
        /// 当 ImagePath 参数设置为 DISM_ONLINE_IMAGE，WindowsDirectory 参数无法使用。
        /// </param>
        /// <param name="SystemDrive">
        /// 可选。 包含启动管理器的系统驱动器的号。 如果 SystemDrive 为 NULL，则使用包含装入点的驱动器的默认值
        /// 当 ImagePath 参数设置为DISM_ONLINE_IMAGE，SystemDrive 参数无法使用。
        /// </param>
        /// <param name="Session">一个有效的 DismSession。 DismSession 必须关联一个映像。 可以使用 DismOpenSession 将会话与映像关联。</param>
        /// <returns>如果成功，则返回 S_OK。
        /// 如果 DismSession 已有与之关联的图像，则返回 HRESULT_FROM_WIN32(ERROR_ALREADY_EXISTS)。
        /// 如果发生其他错误，则返回映射到 HRESULT 的 Win32 错误代码。
        /// </returns>
        [DllImport(Dismapi, CharSet = CharSet.Unicode, EntryPoint = "DismOpenSession", PreserveSig = true, SetLastError = false)]
        public static extern int DismOpenSession([MarshalAs(UnmanagedType.LPWStr)] string ImagePath, [MarshalAs(UnmanagedType.LPWStr)] string WindowsDirectory, [MarshalAs(UnmanagedType.LPWStr)] string SystemDrive, out IntPtr Session);

        /// <summary>
        /// 关闭 DISM API。 每个进程必须调用一次 DismShutdown。 调用 DismShutdown 后，其他 DISM API 函数调用将失败。
        /// </summary>
        /// <returns>
        /// 如果成功，则返回 S_OK。
        /// 如果未调用 DismInitialize，则返回 DISMAPI_E_DISMAPI_NOT_INITIALIZED。
        /// 如果未关闭任何打开的 DismSession，则返回 DISMAPI_E_OPEN_SESSION_HANDLES。
        /// </returns>
        [DllImport(Dismapi, CharSet = CharSet.Unicode, EntryPoint = "DismShutdown", PreserveSig = true, SetLastError = false)]
        public static extern int DismShutdown();
    }
}
