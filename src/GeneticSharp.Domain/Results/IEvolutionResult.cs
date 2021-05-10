using System;
using GeneticSharp.Domain.Populations;

namespace GeneticSharp.Domain.Results
{
    /// <summary>
    /// The IEvolutionResult interface represent the result of a genetic algorithm evolution. It aggregates the evolution time and the resulting population.
    /// </summary>
    public interface IEvolutionResult
    {
        double Fitness { get; }
        IPopulation Population { get; }
        TimeSpan TimeEvolving { get;  }

        int GenerationsNumber { get; }

    }
}