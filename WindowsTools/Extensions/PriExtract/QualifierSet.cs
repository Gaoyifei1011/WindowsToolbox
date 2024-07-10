using System.Collections.Generic;

namespace WindowsTools.Extensions.PriExtract
{
    public class QualifierSet
    {
        public ushort Index { get; set; }

        public IReadOnlyList<Qualifier> QualifiersList { get; set; }
    }
}
