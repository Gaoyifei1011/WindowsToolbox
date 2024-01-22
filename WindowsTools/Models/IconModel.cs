using System.IO;
using Windows.UI.Xaml.Media;

namespace WindowsTools.Models
{
    public class IconModel
    {
        public string DisplayIndex { get; set; }

        public MemoryStream IconMemoryStream { get; set; }

        public ImageSource IconImage { get; set; }
    }
}
