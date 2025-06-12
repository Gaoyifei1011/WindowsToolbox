using System.Collections.Generic;

namespace PowerTools.Extensions.PriExtract
{
    public sealed class QualifierSet
    {
        public ushort Index { get; set; }

        public IReadOnlyList<Qualifier> QualifiersList { get; set; }
    }
}
