using FileRenamer.Models.Controls.About;
using System.Collections.Generic;

namespace FileRenamer.ViewModels.Controls.About
{
    public sealed class ThanksViewModel
    {
        public List<ThanksKeyValuePairModel> ThanksDict = new List<ThanksKeyValuePairModel>()
        {
            new ThanksKeyValuePairModel(){Key = "MouriNaruto" ,Value = "https://github.com/MouriNaruto" },
        };
    }
}
