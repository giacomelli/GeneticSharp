using System;
using System.Diagnostics;
using GeneticSharp.Domain.Populations;

namespace GeneticSharp.Extensions.UnitTests.Tsp
{
    [DebuggerDisplay("Fitness:{Fitness}, Distance:{Distance}, TimeEvolving:{TimeEvolving}, Population:{Population}")]
    public class TspEvolutionResult
    {

        public double? Fitness => Population.BestChromosome.Fitness;

        public IPopulation Population { get; set; }

        public TimeSpan TimeEvolving { get; set; }

        public double Distance { get; set; }
    }
}