using System;

namespace GeneticSharp.Domain.Metaheuristics.Parameters
{
    [Flags]
    public enum ParamScope
    {
        None = 0,
        Constant = 1,
        Evolution = 2,
        Generation = 4,
        Stage = 8,
        MetaHeuristic = 16,
        Individual = 32
    }
}