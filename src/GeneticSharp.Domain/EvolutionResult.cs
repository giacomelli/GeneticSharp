using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GeneticSharp.Domain.Populations;

namespace GeneticSharp.Domain
{
    [DebuggerDisplay("Fitness:{Fitness}, TimeEvolving:{TimeEvolving}, Population:{Population}")]
    public class EvolutionResult
    {

        public double? Fitness => Population.BestChromosome.Fitness;

        public IPopulation Population { get; set; }

        public TimeSpan TimeEvolving { get; set; }

       

    }

    [DebuggerDisplay("Fitness:{Fitness}, TimeEvolving:{TimeEvolving}, Population:{Population}")]
    public class MeanEvolutionResult
    {

        public List<EvolutionResult> Results { get; set; } = new List<EvolutionResult>();

        public double Fitness => Results.Count > 0 ? Results.Sum(r => r.Fitness.Value) / Results.Count : 0;

        public IPopulation Population => Results.Count > 0 ? Results[0].Population : null;

        public TimeSpan TimeEvolving => Results.Count > 0 ? TimeSpan.FromTicks(Results.Sum(r => r.TimeEvolving.Ticks) /  Results.Count) : TimeSpan.Zero;



    }


}