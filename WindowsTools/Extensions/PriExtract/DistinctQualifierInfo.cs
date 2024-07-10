using WindowsTools.Extensions.DataType.Enums;

namespace WindowsTools.Extensions.PriExtract
{
    public struct DistinctQualifierInfo
    {
        public QualifierType QualifierType { get; set; }

        public uint OperandValueOffset { get; set; }
    }
}
