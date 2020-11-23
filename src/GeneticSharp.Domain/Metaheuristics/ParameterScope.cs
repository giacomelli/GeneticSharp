using System;

namespace GeneticSharp.Domain.Metaheuristics
{
    [Flags()]
    public enum ParameterScope
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