namespace WindowsTools.Extensions.PriExtract
{
    public class HierarchicalSchemaVersion
    {
        public ushort MajorVersion { get; set; }

        public ushort MinorVersion { get; set; }

        public uint Checksum { get; set; }

        public uint NumScopes { get; set; }

        public uint NumItems { get; set; }
    }
}
