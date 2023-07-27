using System;

namespace FileRenamer.Models
{
    public class OperationFailedModel
    {
        public string FileName { get; set; }

        public string FilePath { get; set; }

        public Exception Exception { get; set; }
    }
}
