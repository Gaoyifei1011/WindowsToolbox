using FileRenamer.Models.Controls.About;
using System.Collections.Generic;

namespace FileRenamer.ViewModels.Controls.About
{
    public sealed class ReferenceViewModel
    {
        //项目引用信息
        public List<ReferenceKeyValuePairModel> ReferenceDict = new List<ReferenceKeyValuePairModel>()
        {
            new ReferenceKeyValuePairModel() {Key = "Microsoft.Windows.SDK.BuildTools" ,Value = "https://aka.ms/WinSDKProjectURL" },
            new ReferenceKeyValuePairModel() {Key = "Mile.Xaml" ,Value = "https://github.com/ProjectMile/Mile.Xaml" },
        };
    }
}
