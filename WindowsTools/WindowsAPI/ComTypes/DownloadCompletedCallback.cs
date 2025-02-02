using System;
using WUApiLib;

namespace WindowsTools.WindowsAPI.ComTypes
{
    /// <summary>
    /// 提供异步下载完成时使用的回调。 此接口由调用 IUpdateDownloader：：BeginDownload 方法的程序员实现。
    /// </summary>
    public class DownloadCompletedCallback : IDownloadCompletedCallback
    {
        public IDownloadJob DownloadJob { get; private set; }

        public IDownloadCompletedCallbackArgs CallbackArgs { get; private set; }

        public event EventHandler DownloadCompleted;

        /// <summary>
        /// 处理通过调用 IUpdateInstaller.BeginInstall 或 IUpdateInstaller.BeginUninstall 启动的异步安装或卸载完成的通知。
        /// </summary>
        public void Invoke(IDownloadJob downloadJob, IDownloadCompletedCallbackArgs callbackArgs)
        {
            DownloadJob = downloadJob;
            CallbackArgs = callbackArgs;
            DownloadCompleted?.Invoke(this, EventArgs.Empty);
        }
    }
}
