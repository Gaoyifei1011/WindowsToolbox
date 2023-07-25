using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FileRenamer.Models
{
    /// <summary>
    /// 文件属性模型
    /// </summary>
    public class OldAndNewPropertiesModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        private string _fileName;

        public string FileName
        {
            get { return _fileName; }

            set
            {
                _fileName = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 文件路径
        /// </summary>
        private string _filePath;

        public string FilePath
        {
            get { return _filePath; }

            set
            {
                _filePath = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 文件属性
        /// </summary>
        private string _fileProperties;

        public string FileProperties
        {
            get { return _fileProperties; }

            set
            {
                _fileProperties = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
