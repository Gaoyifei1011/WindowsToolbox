using System;
using System.Collections.Generic;

namespace WindowsTools.Extensions.PriExtract
{
    public sealed class CandidateSet
    {
        public (int SchemaSectionIndex, int ResourceMapItemIndex) ResourceMapSectionAndIndex { get; set; }

        public ushort DecisionIndex { get; set; }

        public IReadOnlyList<Candidate> CandidatesList { get; set; }
    }
}
