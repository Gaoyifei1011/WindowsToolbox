using FileRenamer.Views.Pages;
using FileRenamer.WindowsAPI.PInvoke.User32;
using Mile.Xaml;
using System.Collections.Generic;
using System.Windows.Forms;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace FileRenamer
{
    public partial class MileWindow : Form
    {
        private WindowsXamlHost MileXamlHost = new WindowsXamlHost();

        public MileWindow()
        {
            InitializeComponent();
            Controls.Add(MileXamlHost);
            MileXamlHost.AutoSize = true;
            MileXamlHost.Dock = DockStyle.Fill;
            MileXamlHost.Child = new MainPage();
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                // 窗口移动时的事件
                case (int)WindowMessage.WM_MOVE:
                    {
                        IReadOnlyList<Popup> PopupRoot = VisualTreeHelper.GetOpenPopupsForXamlRoot(MainPage.Current.XamlRoot);
                        foreach (Popup popup in PopupRoot)
                        {
                            if (popup.Child as MenuFlyoutPresenter != null)
                            {
                                popup.IsOpen = false;
                            }
                        }
                        break;
                    }
            }

            base.WndProc(ref m);
        }
    }
}
