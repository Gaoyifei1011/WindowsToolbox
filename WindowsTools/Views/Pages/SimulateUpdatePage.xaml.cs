using System;
using System.ComponentModel;
using System.Timers;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using WindowsTools.Extensions.DataType.Enums;
using WindowsTools.Strings;
using WindowsTools.Views.Windows;
using WindowsTools.WindowsAPI.PInvoke.User32;

namespace WindowsTools.Views.Pages
{
    /// <summary>
    /// 模拟更新页面
    /// </summary>
    public sealed partial class SimulateUpdatePage : Page, INotifyPropertyChanged
    {
        private Timer simulateUpdateTimer = new Timer();

        private int simulatePassedTime = 0;
        private int simulateTotalTime = 0;

        private UpdatingKind UpdatingKind { get; }

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

        public SimulateUpdatePage(UpdatingKind updatingKind, TimeSpan duration, bool blockAllKeys, bool lockScreenAutomaticly)
        {
            InitializeComponent();

            UpdatingKind = updatingKind;
            simulateTotalTime = Convert.ToInt32(duration.TotalSeconds);
            simulateUpdateTimer.Interval = 1;
            simulateUpdateTimer.Elapsed += OnElapsed;
            simulateUpdateTimer.Start();
            User32Library.ShowCursor(false);
        }

        protected override void OnKeyDown(KeyRoutedEventArgs args)
        {
            base.OnKeyDown(args);

            if (args.Key is VirtualKey.Escape)
            {
                StopSimulateUpdate();
            }
        }

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
                    LoafWindow.Current.BeginInvoke(() =>
                    {
                        StopSimulateUpdate();
                    });

                    return;
                }

                LoafWindow.Current.BeginInvoke(() =>
                {
                    Windows11UpdateText = string.Format(SimulateUpdate.Windows11UpdateText1, Convert.ToInt32(simulatePassedTime / simulateTotalTime));
                    Windows10UpdateText = string.Format(SimulateUpdate.Windows10UpdateText1, Convert.ToInt32(simulatePassedTime / simulateTotalTime));
                });
            }
            catch (Exception)
            {
            }
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
                simulateUpdateTimer = null;
                User32Library.ShowCursor(true);
                LoafWindow.Current.Close();
            }
        }
    }
}
