using System.Collections.Generic;

namespace PowerToolbox.Extensions.PriExtract
{
    public sealed class QualifierSet
    {
        public ushort Index { get; set; }

        public IReadOnlyList<Qualifier> QualifiersList { get; set; }
    }
}
