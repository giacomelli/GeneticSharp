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
            switch (parent1.CrossoverType)
            {
                case CrossoverType.Uniform:
                    ret = UniformCrossover(parent1, parent2);
                    break;
                case CrossoverType.TwoPoints:
                    ret = TwoPointCrossover(parent1, parent2);
                    break;
                case CrossoverType.OnePoint:
                    ret = OnePointCrossover(parent1, parent2);
                    break;
                default:
                    ret = UniformCrossover(parent1, parent2);
                    break;
            }

            foreach (var e in ret)
                ((SelfAdaptiveChromosome)e).CrossoverType = RandomizationProvider.Current.GetDouble() > 0.5 ? parent1.CrossoverType : parent2.CrossoverType;

            return ret;
        }

        private IList<IChromosome> OnePointCrossover(SelfAdaptiveChromosome parent1, SelfAdaptiveChromosome parent2)
        {
            int length = parent1.Length;
            int crossoverPoint = RandomizationProvider.Current.GetInt(0, length);

            SelfAdaptiveChromosome offspring1 = (SelfAdaptiveChromosome)parent1.Clone();
            SelfAdaptiveChromosome offspring2 = (SelfAdaptiveChromosome)parent2.Clone();

            for (int i = 0; i < length; i++)
            {
                if (i >= crossoverPoint)
                {
                    var v = offspring1.GetMutationProbability(i);
                    offspring1.SetMutationProbability(i, parent2.GetMutationProbability(i));
                    offspring2.SetMutationProbability(i, parent1.GetMutationProbability(i));

                    var g = offspring1.GetGene(i);
                    offspring1.ReplaceGene(i, parent2.GetGene(i));
                    offspring2.ReplaceGene(i, parent1.GetGene(i));
                }
            }

            return new List<IChromosome>() { offspring1, offspring2 };
        }

        private IList<IChromosome> TwoPointCrossover(SelfAdaptiveChromosome parent1, SelfAdaptiveChromosome parent2)
        {
            int length = parent1.Length;
            int crossoverPoint1 = RandomizationProvider.Current.GetInt(0, length);
            int crossoverPoint2 = RandomizationProvider.Current.GetInt(crossoverPoint1, length);

            SelfAdaptiveChromosome offspring1 = (SelfAdaptiveChromosome)parent1.Clone();
            SelfAdaptiveChromosome offspring2 = (SelfAdaptiveChromosome)parent2.Clone();

            for (int i = 0; i < length; i++)
            {
                if (i >= crossoverPoint1 && i <= crossoverPoint2)
                {
                    var v = offspring1.GetMutationProbability(i);
                    offspring1.SetMutationProbability(i, parent2.GetMutationProbability(i));
                    offspring2.SetMutationProbability(i, parent1.GetMutationProbability(i));

                    var g = offspring1.GetGene(i);
                    offspring1.ReplaceGene(i, parent2.GetGene(i));
                    offspring2.ReplaceGene(i, parent1.GetGene(i));
                }
            }

            return new List<IChromosome>() { offspring1, offspring2 };
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