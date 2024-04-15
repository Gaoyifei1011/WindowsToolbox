using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace WindowsTools.WindowsAPI.ComTypes
{
    /// <summary>
    /// 访问任务栏/开始菜单固定的应用程序
    /// </summary>
    [ComImport, Guid("0DD79AE2-D156-45D4-9EEB-3B549769E940"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPinnedList3
    {
        /// <summary>
        /// 枚举固定的应用程序
        /// </summary>
        /// <param name="ppv">枚举器</param>
        /// <returns>操作结果</returns>
        [PreserveSig]
        int EnumObjects([Out] out IntPtr ppv);

        /// <summary>
        /// 获取有关应用程序的可固定信息
        /// </summary>
        /// <param name="dataObject">要获取其可链接信息的数据对象</param>
        /// <param name="pinnableFlag">可固定的标志</param>
        /// <param name="ppsiApplication">应用程序 shell 项</param>
        /// <param name="ppsiDestination">目标 shell 项</param>
        /// <param name="ppszAppID">App id</param>
        /// <param name="pfLaunchable">标记固定项是否可启动</param>
        /// <returns>如果项目不是可固定的，返回 S_FALSE</returns>
        [PreserveSig]
        int GetPinnableInfo(IDataObject dataObject, int pinnableFlag,
             [Out] out IntPtr ppsiApplication, [Out] out IntPtr ppsiDestination,
             [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder ppszAppID, [Out] uint pfLaunchable);

        /// <summary>
        /// 数据对象是否可固定的标志
        /// </summary>
        /// <param name="dataObject">要测试的数据对象</param>
        /// <param name="pinnableFlag">可固定的标志</param>
        /// <returns>如果项目不是可固定的，返回 S_FALSE</returns>
        [PreserveSig]
        int IsPinnable(IDataObject dataObject, int pinnableFlag);

        /// <summary>
        /// 在固定列表中找到 pidl 并解析 link / target。
        /// </summary>
        /// <param name="HWND">窗口句柄</param>
        /// <param name="flags">可固定的标志</param>
        /// <param name="pidl">要寻找的 pidl</param>
        /// <param name="ppidlResolved">已解析pidl(目标)</param>
        /// <returns>如果解析失败，返回错误。</returns>
        [PreserveSig]
        int Resolve(IntPtr HWND, uint flags, PIDLIST_ABSOLUTE pidl, [Out] out PIDLIST_ABSOLUTE ppidlResolved);

        /// <summary>
        /// 修改固定项的 pidl(目标)
        /// </summary>
        /// <param name="pidlFrom">最初固定的 pidl</param>
        /// <param name="pidlTo">需要固定的 pidl</param>
        /// <returns>操作结果</returns>
        [PreserveSig]
        int LegacyModify(PIDLIST_ABSOLUTE pidlFrom, PIDLIST_ABSOLUTE pidlTo);

        /// <summary>
        /// 返回固定列表中更改的次数
        /// </summary>
        /// <param name="count">更改次数</param>
        /// <returns>操作结果</returns>
        [PreserveSig]
        int GetChangeCount([Out] out int count);

        /// <summary>
        /// 如果pidl被固定则返回S_OK，如果pidl未固定则返回错误
        /// </summary>
        /// <param name="pidl">要测试的 pidl</param>
        /// <returns>如果pidl被固定则返回S_OK，如果pidl未固定则返回错误</returns>
        [PreserveSig]
        int IsPinned(PIDLIST_ABSOLUTE pidl);

        /// <summary>
        /// 查找 AppID 与 pidl 匹配的第一个固定项，并在 ppidlPinned 中返回其 pidl
        /// </summary>
        /// <param name="pidl">应用的 pidl</param>
        /// <param name="ppidlPinned">固定项的 pidl</param>
        /// <returns>操作结果</returns>
        [PreserveSig]
        int GetPinnedItem(PIDLIST_ABSOLUTE pidl, [Out] out PIDLIST_ABSOLUTE ppidlPinned);

        /// <summary>
        /// 返回固定项的AppID。
        /// </summary>
        /// <param name="pidl">Pidl必须由固定列表提供，否则找不到该AppID</param>
        /// <param name="ppszAppID">应用ID</param>
        /// <returns>操作结果</returns>
        [PreserveSig]
        int GetAppIDForPinnedItem(PIDLIST_ABSOLUTE pidl, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder ppszAppID);

        /// <summary>
        /// 更新固定项目以响应更改通知。特别是要确保缓存的AppID是正确的。
        /// </summary>
        /// <param name="pidlFrom">最初的pidl</param>
        /// <param name="pidlTo">目标pidl</param>
        /// <returns>操作结果</returns>
        [PreserveSig]
        int ItemChangeNotify(PIDLIST_ABSOLUTE pidlFrom, PIDLIST_ABSOLUTE pidlTo);

        /// <summary>
        /// 通过解析每个链接并在解析失败时将其删除，验证列表中的所有项未从机器中删除
        /// </summary>
        /// <returns>操作结果</returns>
        [PreserveSig]
        int UpdateForRemovedItemsAsNecessary();

        /// <summary>
        /// 固定一个 shell 链接
        /// </summary>
        /// <param name="us">未知</param>
        /// <param name="shellLink">要链接的固定项</param>
        /// <returns></returns>
        [PreserveSig]
        int PinShellLink(ushort us, IShellLink shellLink);

        /// <summary>
        /// 获取appId的固定项
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="pidl">固定项的 pidl</param>
        /// <returns>操作结果</returns>
        [PreserveSig]
        int GetPinnedItemForAppID(ushort appId, [Out] out PIDLIST_ABSOLUTE pidl);

        /// <summary>
        /// 修改固定项的 pidl(目标)
        /// </summary>
        /// <param name="pidlFrom">最初的pidl</param>
        /// <param name="pidlTo">目标pidl</param>
        /// <param name="caller">跟踪信息</param>
        /// <returns>操作结果</returns>
        [PreserveSig]
        int Modify(PIDLIST_ABSOLUTE pidlFrom, PIDLIST_ABSOLUTE pidlTo, PLMC caller);
    }
}
