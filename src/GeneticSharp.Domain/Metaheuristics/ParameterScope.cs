using System;

namespace GeneticSharp.Domain.Metaheuristics
{
    [Flags()]
    public enum ParameterScope
    {
        Global = 0,
        Generation = 1,
        Stage = 2,
        MetaHeuristic = 4,
        Individual = 8
    }
}