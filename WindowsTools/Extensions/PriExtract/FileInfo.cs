﻿namespace WindowsTools.Extensions.PriExtract
{
    public class FileInfo
    {
        public ushort ParentFolder { get; set; }

        public ushort FullPathLength { get; set; }

        public ushort FileNameLength { get; set; }

        public uint FileNameOffset { get; set; }
    }
}
