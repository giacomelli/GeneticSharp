using System;
using System.Collections;
using System.Diagnostics;
using GeneticSharp.Domain.Populations;

namespace GeneticSharp.Domain.Results
{
    /// <summary>
    /// The EvolutionResult class represent the result of a genetic algorithm evolution. It aggregates the evolution time and the resulting population.
    /// </summary>
    [DebuggerDisplay("Fit:{Fitness}, Time:{TimeEvolvingDisplay}, GenNb: {GenerationsNumber}")]
    public class EvolutionResult : IEvolutionResult
    {

        public double Fitness => Population.BestChromosome.Fitness.Value;

        public IPopulation Population { get; set; }

        public string TimeEvolvingDisplay => TimeEvolving.ToString();

        public TimeSpan TimeEvolving { get; set; }
        public int GenerationsNumber => Population.GenerationsNumber;
    }
}