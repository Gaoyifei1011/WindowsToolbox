using FileRenamer.Models.Base;

namespace FileRenamer.Models
{
    /// <summary>
    /// 文件名称模型
    /// </summary>
    public class OldAndNewNameModel : ModelBase
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
    }
}
