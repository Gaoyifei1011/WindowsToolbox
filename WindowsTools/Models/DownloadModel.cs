using System;
using System.ComponentModel;
using Windows.UI.Xaml;
using WindowsTools.WindowsAPI.ComTypes;

namespace WindowsTools.Models
{
    public class DownloadModel : INotifyPropertyChanged
    {
        private Visibility _isVisible;

        public Visibility IsVisible
        {
            get { return _isVisible; }

            set
            {
                if (!Equals(_isVisible, value))
                {
                    _isVisible = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsVisible)));
                }
            }
        }

        private bool _isNotOperated;

        public bool IsNotOperated
        {
            get { return _isNotOperated; }

            set
            {
                if (!Equals(_isNotOperated, value))
                {
                    _isNotOperated = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsNotOperated)));
                }
            }
        }

        /// <summary>
        /// 任务在下载状态时，获取的GID码。该值唯一
        /// </summary>
        public string DownloadID { get; set; }

        /// <summary>
        /// 下载文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件下载链接
        /// </summary>
        public string FileLink { get; set; }

        /// <summary>
        /// 文件下载保存的路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 文件下载状态
        /// </summary>
        private DODownloadState _downloadState;

        public DODownloadState DownloadState
        {
            get { return _downloadState; }

            set
            {
                if (!Equals(_downloadState, value))
                {
                    _downloadState = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DownloadState)));
                }
            }
        }

        /// <summary>
        /// 下载文件的总大小
        /// </summary>
        private double _totalSize;

        public double TotalSize
        {
            get { return _totalSize; }

            set
            {
                if (!Equals(_totalSize, value))
                {
                    _totalSize = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalSize)));
                }
            }
        }

        /// <summary>
        /// 下载文件已完成的进度
        /// </summary>
        private double _finishedSize;

        public double FinishedSize
        {
            get { return _finishedSize; }

            set
            {
                if (!Equals(_finishedSize, value))
                {
                    _finishedSize = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FinishedSize)));
                }
            }
        }

        /// <summary>
        /// 文件下载速度
        /// </summary>
        private double _currentSpeed;

        public double CurrentSpeed
        {
            get { return _currentSpeed; }

            set
            {
                if (!Equals(_currentSpeed, value))
                {
                    _currentSpeed = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentSpeed)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 计算当前文件的下载进度
        /// </summary>
        public double DownloadProgress(double finishedSize, double totalSize)
        {
            return totalSize == default ? 0 : Math.Round(finishedSize / totalSize, 4) * 100;
        }

        /// <summary>
        /// 检查任务是否处于下载中
        /// </summary>
        public Visibility IsDownloading(DODownloadState downloadState)
        {
            return downloadState is DODownloadState.DODownloadState_Transferring || downloadState is DODownloadState.DODownloadState_Transferred ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 检查任务是否处于暂停中
        /// </summary>
        public Visibility IsPaused(DODownloadState downloadState)
        {
            return downloadState is DODownloadState.DODownloadState_Paused ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 检查任务是否下载完成
        /// </summary>
        public Visibility IsCompleted(DODownloadState downloadState)
        {
            return downloadState is DODownloadState.DODownloadState_Finalized ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
