using WindowsTools.Extensions.DataType.Enums;

namespace WindowsTools.Extensions.PriExtract
{
    public sealed class DistinctQualifierInfo
    {
        public QualifierType QualifierType { get; set; }

        public uint OperandValueOffset { get; set; }
    }
}
