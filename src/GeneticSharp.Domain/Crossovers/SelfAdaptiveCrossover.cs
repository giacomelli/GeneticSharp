using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GeneticSharp;

namespace GeneticSharp
{
    [DisplayName("Self Adaptive Crossover")]
    public class SelfAdaptiveCrossover : CrossoverBase
    {
        public SelfAdaptiveCrossover() : base(2, 2) { }

        protected override IList<IChromosome> PerformCross(IList<IChromosome> parents)
        {
            var parent1 = parents[0] as SelfAdaptiveChromosome;
            var parent2 = parents[1] as SelfAdaptiveChromosome;

            if (parent1 == null || parent2 == null)
            {
                throw new ArgumentException("Both parents must be of type SelfAdaptiveChromosome.");
            }

            IList<IChromosome> ret = UniformCrossover(parent1, parent2);
            return ret;
        }

        private IList<IChromosome> UniformCrossover(SelfAdaptiveChromosome parent1, SelfAdaptiveChromosome parent2)
        {
            int length = parent1.Length;
            
            
            SelfAdaptiveChromosome offspring1 = (SelfAdaptiveChromosome)parent1.Clone();
            SelfAdaptiveChromosome offspring2 = (SelfAdaptiveChromosome)parent2.Clone();

            for (int i = 0; i < length; i++)
            {
                if (RandomizationProvider.Current.GetDouble() < 0.5)
                {
                    var v = offspring1.GetMutationProbability(i);
                    offspring1.SetMutationProbability(i,offspring2.GetMutationProbability(i));
                    offspring2.SetMutationProbability(i, v);
                }
                    

                if (RandomizationProvider.Current.GetDouble() < 0.5)
                {
                    var g = offspring1.GetGene(i);
                    offspring1.ReplaceGene(i, offspring2.GetGene(i));
                    offspring2.ReplaceGene(i, g);
                }
                    
            }

            return new List<IChromosome>() { offspring1, offspring2 };
        }
    }
}