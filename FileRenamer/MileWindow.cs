using FileRenamer.Views.Pages;
using Mile.Xaml;
using System.Windows.Forms;

namespace FileRenamer
{
    public partial class MileWindow : Form
    {
        WindowsXamlHost MileXamlHost = new WindowsXamlHost();

        public MileWindow()
        {
            InitializeComponent();
            Controls.Add(MileXamlHost);
            MileXamlHost.AutoSize = true;
            MileXamlHost.Dock = DockStyle.Fill;
            MileXamlHost.Child = new MainPage();
        }
    }
}
