using System;

namespace GeneticSharp.Domain.Metaheuristics
{
    /// <summary>
    /// Defines one or several operators available to Metaheuristics
    /// </summary>
    [Flags]
    public enum MetaHeuristicsScope
    {
        None = 0,
        Selection = 1,
        Crossover = 2,
        Mutation = 4,
        Reinsertion = 8,
        All = Selection | Crossover | Mutation | Reinsertion
    }
}