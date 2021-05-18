using System;

namespace GeneticSharp.Domain.Crossovers.Geometric
{
    [Flags]
    public enum GeneSelectionMode
    {
        AllIndexed = 0,
        SingleFirstAllowed = 1,
        RandomOrder = 2,
        
    }
}