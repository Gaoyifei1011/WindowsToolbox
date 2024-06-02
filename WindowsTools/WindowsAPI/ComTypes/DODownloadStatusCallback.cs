using System;
using System.Runtime.InteropServices;

namespace WindowsTools.WindowsAPI.ComTypes
{
    /// <summary>
    /// IDODownloadStatusCallback 接口的实现
    /// </summary>
    public class DODownloadStatusCallback : IDODownloadStatusCallback
    {
        public Guid DownloadID { get; set; } = Guid.Empty;

        /// <summary>
        /// 下载状态发生变化时触发的事件
        /// </summary>
        public event Action<DODownloadStatusCallback, IDODownload, DO_DOWNLOAD_STATUS> StatusChanged;

        public void OnStatusChange([MarshalAs(UnmanagedType.Interface)] IDODownload download, ref DO_DOWNLOAD_STATUS status)
        {
            StatusChanged?.Invoke(this, download, status);
        }
    }
}
