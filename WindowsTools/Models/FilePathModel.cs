using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WindowsTools.Models
{
    public class FilePathModel : INotifyPropertyChanged
    {
        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }

            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public uint FilePathIndex { get; set; }

        public string Key { get; set; }

        public string AbsolutePath { get; set; }

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
