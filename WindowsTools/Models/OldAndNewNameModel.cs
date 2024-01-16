using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WindowsTools.Models
{
    /// <summary>
    /// 文件名称模型
    /// </summary>
    public class OldAndNewNameModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 文件的初始名称
        /// </summary>
        private string _originalFileName;

        public string OriginalFileName
        {
            get { return _originalFileName; }

            set
            {
                _originalFileName = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 文件的初始路径
        /// </summary>
        private string _originalFilePath;

        public string OriginalFilePath
        {
            get { return _originalFilePath; }

            set
            {
                _originalFilePath = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 文件新名称
        /// </summary>
        private string _newFileName;

        public string NewFileName
        {
            get { return _newFileName; }

            set
            {
                _newFileName = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 文件新名称
        /// </summary>
        private string _newFilePath;

        public string NewFilePath
        {
            get { return _newFilePath; }

            set
            {
                _newFilePath = value;
                OnPropertyChanged();
            }
        }

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
