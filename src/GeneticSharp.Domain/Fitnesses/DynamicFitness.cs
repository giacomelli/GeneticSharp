using System;
using System.Collections.Generic;

namespace GeneticSharp.Domain.Fitnesses
{
    public class DynamicFitness<TGene> : GenericFitness<TGene>
    {


        private Func<IEnumerable<TGene>, double> FitnessFunction { get; set; }



        protected override double EvaluateGenes(IEnumerable<TGene> genes)
        {
            return FitnessFunction(genes);
        }
    }
}