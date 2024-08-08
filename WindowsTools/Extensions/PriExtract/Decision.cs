using System.Collections.Generic;

namespace WindowsTools.Extensions.PriExtract
{
    public sealed class Decision
    {
        public ushort Index { get; set; }

        public IReadOnlyList<QualifierSet> QualifierSetsList { get; set; }
    }
}
