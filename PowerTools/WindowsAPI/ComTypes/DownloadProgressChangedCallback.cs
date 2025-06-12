using System;
using WUApiLib;

namespace PowerTools.WindowsAPI.ComTypes
{
    /// <summary>
    /// 处理指示异步下载操作进度更改的通知。 此接口由调用 IUpdateDownloader.BeginDownload 方法的程序员实现。
    /// </summary>
    public class DownloadProgressChangedCallback : IDownloadProgressChangedCallback
    {
        public IDownloadJob DownloadJob { get; private set; }

        public IDownloadProgressChangedCallbackArgs CallbackArgs { get; private set; }

        public EventHandler DownloadProgressChanged;

        /// <summary>
        /// 处理通过调用 IUpdateDownloader.BeginDownload 方法启动的异步下载进度更改通知。
        /// </summary>
        public void Invoke(IDownloadJob downloadJob, IDownloadProgressChangedCallbackArgs callbackArgs)
        {
            DownloadJob = downloadJob;
            CallbackArgs = callbackArgs;
            DownloadProgressChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
