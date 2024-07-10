using System.Collections.Generic;
using WindowsTools.Extensions.PriExtract;

namespace WindowsTools.Extensions.DataType.Enums
{
    public class Decision
    {
        public ushort Index { get; set; }

        public IReadOnlyList<QualifierSet> QualifierSetsList { get; set; }
    }
}
