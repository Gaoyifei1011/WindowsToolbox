using FileRenamer.Models;
using FileRenamer.ViewModels.Base;
using System.Collections.ObjectModel;

namespace FileRenamer.ViewModels.Pages
{
    public sealed class FileNameViewModel : ViewModelBase
    {
        public ObservableCollection<FileNameModel> FileNameDataList { get; } = new ObservableCollection<FileNameModel>();

        public FileNameViewModel()
        {
            for (int i = 0; i < 500; i++)
            {
                FileNameDataList.Add(new FileNameModel { NewFileName = "NewFileName" + i.ToString(), OriginalFileName = "OriginalFileName" + i.ToString() });
            }
        }
    }
}
