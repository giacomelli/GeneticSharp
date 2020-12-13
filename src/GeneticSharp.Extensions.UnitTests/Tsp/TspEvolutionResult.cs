using System;
using System.Diagnostics;
using System.Linq;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Extensions.Tsp;

namespace GeneticSharp.Extensions.UnitTests.Tsp
{

    public interface ITspEvolutionResult: IEvolutionResult
    {
        double Distance { get; }
    }


    [DebuggerDisplay("Fitness:{Fitness}, Distance:{Distance}, TimeEvolving:{TimeEvolving}, Population:{Population}")]
    public class TspEvolutionResult: EvolutionResult, ITspEvolutionResult
    {
        public double Distance => ((TspChromosome) this.Population.BestChromosome).Distance;
    }

    [DebuggerDisplay("Fitness:{Fitness}, Distance:{Distance}, TimeEvolving:{TimeEvolving}, Population:{Population}")]
    public class TspMeanEvolutionResult : MeanEvolutionResult, ITspEvolutionResult
    {
        public double Distance => Results.Count > 0 ? GetScopedResults().Sum(r => ((ITspEvolutionResult) r).Distance) / GetScopedResults().Count() : double.MaxValue;
    }


}