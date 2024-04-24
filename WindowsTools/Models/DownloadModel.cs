using System.ComponentModel;
using Windows.UI.Xaml;

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

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
