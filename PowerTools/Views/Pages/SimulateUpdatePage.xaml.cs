using PowerTools.Extensions.DataType.Enums;
using PowerTools.Services.Root;
using PowerTools.Views.Windows;
using System;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using Windows.UI.Xaml.Controls;

// 抑制 CA1822 警告
#pragma warning disable CA1822

namespace PowerTools.Views.Pages
{
    /// <summary>
    /// 模拟更新页面
    /// </summary>
    public sealed partial class SimulateUpdatePage : Page, INotifyPropertyChanged
    {
        private readonly string Windows10UpdateText1String = ResourceService.SimulateUpdateResource.GetString("Windows10UpdateText1");
        private readonly string Windows11UpdateText1String = ResourceService.SimulateUpdateResource.GetString("Windows11UpdateText1");
        private readonly SynchronizationContext synchronizationContext = SynchronizationContext.Current;
        private System.Timers.Timer simulateUpdateTimer = new();

        private readonly int simulateTotalTime = 0;
        private int simulatePassedTime = 0;

        private UpdateKind UpdateKind { get; }

        private string _windows11UpdateText;

        public string Windows11UpdateText
        {
            get { return _windows11UpdateText; }

            set
            {
                if (!Equals(_windows11UpdateText, value))
                {
                    _windows11UpdateText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Windows11UpdateText)));
                }
            }
        }

        private string _windows10UpdateText;

        public string Windows10UpdateText
        {
            get { return _windows10UpdateText; }

            set
            {
                if (!Equals(_windows10UpdateText, value))
                {
                    _windows10UpdateText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Windows10UpdateText)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public SimulateUpdatePage(UpdateKind updateKind, TimeSpan duration)
        {
            InitializeComponent();

            UpdateKind = updateKind;
            simulateTotalTime = duration.TotalSeconds is not 0 ? Convert.ToInt32(duration.TotalSeconds) : 1;
            string percentage = ((double)simulatePassedTime / simulateTotalTime).ToString("0%");
            Windows11UpdateText = string.Format(Windows11UpdateText1String, percentage);
            Windows10UpdateText = string.Format(Windows10UpdateText1String, percentage);
            simulateUpdateTimer.Interval = 1000;
            simulateUpdateTimer.Elapsed += OnElapsed;
            simulateUpdateTimer.Start();
        }

        #region 第一部分：自定义事件

        /// <summary>
        /// 当指定的计时器间隔已过去而且计时器处于启用状态时发生的事件
        /// </summary>
        private void OnElapsed(object sender, ElapsedEventArgs args)
        {
            simulatePassedTime++;

            try
            {
                // 到达约定时间，自动停止
                if (simulatePassedTime > simulateTotalTime)
                {
                    synchronizationContext.Post(_ =>
                    {
                        LoafWindow.Current.StopLoaf();
                    }, null);

                    return;
                }

                synchronizationContext.Post(_ =>
                {
                    string percentage = ((double)simulatePassedTime / simulateTotalTime).ToString("0%");
                    Windows11UpdateText = string.Format(ResourceService.SimulateUpdateResource.GetString("Windows11UpdateText1"), percentage);
                    Windows10UpdateText = string.Format(ResourceService.SimulateUpdateResource.GetString("Windows10UpdateText1"), percentage);
                }, null);
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, "Simulate update timer update failed", e);
            }
        }

        #endregion 第一部分：自定义事件

        /// <summary>
        /// 获取控件的文字转向
        /// </summary>
        private global::Windows.UI.Xaml.FlowDirection GetControlDirection(RightToLeft rightToLeft)
        {
            return rightToLeft is RightToLeft.Yes ? global::Windows.UI.Xaml.FlowDirection.RightToLeft : global::Windows.UI.Xaml.FlowDirection.LeftToRight;
        }

        /// <summary>
        /// 停止模拟自动更新
        /// </summary>
        public void StopSimulateUpdate()
        {
            if (simulateUpdateTimer is not null)
            {
                simulateUpdateTimer.Stop();
                simulateUpdateTimer.Dispose();
                simulatePassedTime = 0;
                simulateUpdateTimer = null;
            }
        }
    }
}
