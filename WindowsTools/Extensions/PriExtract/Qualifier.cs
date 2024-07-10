using WindowsTools.Extensions.DataType.Enums;

namespace WindowsTools.Extensions.PriExtract
{
    public class Qualifier
    {
        public ushort Index { get; set; }

        public QualifierType Type { get; set; }

        public ushort Priority { get; set; }

        public float FallbackScore { get; set; }

        public string Value { get; set; }
    }
}
