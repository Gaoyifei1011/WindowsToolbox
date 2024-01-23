using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WindowsTools.Models
{
    public class EmbeddedDataModel : INotifyPropertyChanged
    {
        private bool _isChecked;

        public bool IsChecked
        {
            get { return _isChecked; }

            set
            {
                _isChecked = value;
                OnPropertyChanged();
            }
        }

        public int EmbeddedDataIndex { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
