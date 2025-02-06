using System;
using System.Collections.Generic;
using GeneticSharp;

namespace GeneticSharp
{
    public class SelfAdaptiveCrossover : CrossoverBase
    {
        private static readonly int CrossoverOperatorsCount = 3;

        public SelfAdaptiveCrossover() : base(2, 2) { }

        protected override IList<IChromosome> PerformCross(IList<IChromosome> parents)
        {
            var parent1 = parents[0] as SelfAdaptiveChromosome;
            var parent2 = parents[1] as SelfAdaptiveChromosome;

            if (parent1 == null || parent2 == null)
            {
                throw new ArgumentException("Both parents must be of type SelfAdaptiveChromosome.");
            }

            var random = RandomizationProvider.Current;
            CrossoverType crossoverType = parent1.CrossoverType; // Se usa el operador del primer padre

            switch (crossoverType)
            {
                case  CrossoverType.OnePoin:
                    return OnePointCrossover(parent1, parent2);
                case  CrossoverType.TwoPoints:
                    return TwoPointCrossover(parent1, parent2);
                case  CrossoverType.Uniform:
                    return UniformCrossover(parent1, parent2);
                default:
                    return OnePointCrossover(parent1, parent2);
            }
        }

        private IList<IChromosome> OnePointCrossover(SelfAdaptiveChromosome parent1, SelfAdaptiveChromosome parent2)
        {
            int length = parent1.Length;
            int crossoverPoint = RandomizationProvider.Current.GetInt(1, length - 1);

            var offspring1 = parent1.CreateNew() as SelfAdaptiveChromosome;
            var offspring2 = parent2.CreateNew() as SelfAdaptiveChromosome;

            for (int i = 0; i < length; i++)
            {
                if (i < crossoverPoint)
                {
                    offspring1.ReplaceGene(i, parent1.GetGene(i));
                    offspring2.ReplaceGene(i, parent2.GetGene(i));
                }
                else
                {
                    offspring1.ReplaceGene(i, parent2.GetGene(i));
                    offspring2.ReplaceGene(i, parent1.GetGene(i));
                }
            }
            return new List<IChromosome> { offspring1, offspring2 };
        }

        private IList<IChromosome> TwoPointCrossover(SelfAdaptiveChromosome parent1, SelfAdaptiveChromosome parent2)
        {
            int length = parent1.Length;
            int point1 = RandomizationProvider.Current.GetInt(1, length / 2);
            int point2 = RandomizationProvider.Current.GetInt(length / 2, length - 1);

            var offspring1 = parent1.CreateNew() as SelfAdaptiveChromosome;
            var offspring2 = parent2.CreateNew() as SelfAdaptiveChromosome;

            for (int i = 0; i < length; i++)
            {
                if (i < point1 || i > point2)
                {
                    offspring1.ReplaceGene(i, parent1.GetGene(i));
                    offspring2.ReplaceGene(i, parent2.GetGene(i));
                }
                else
                {
                    offspring1.ReplaceGene(i, parent2.GetGene(i));
                    offspring2.ReplaceGene(i, parent1.GetGene(i));
                }
            }
            return new List<IChromosome> { offspring1, offspring2 };
        }

        private IList<IChromosome> UniformCrossover(SelfAdaptiveChromosome parent1, SelfAdaptiveChromosome parent2)
        {
            int length = parent1.Length;
            var offspring1 = parent1.CreateNew() as SelfAdaptiveChromosome;
            var offspring2 = parent2.CreateNew() as SelfAdaptiveChromosome;

            for (int i = 0; i < length; i++)
            {
                if (RandomizationProvider.Current.GetDouble() < 0.5)
                {
                    offspring1.ReplaceGene(i, parent1.GetGene(i));
                    offspring2.ReplaceGene(i, parent2.GetGene(i));
                }
                else
                {
                    offspring1.ReplaceGene(i, parent2.GetGene(i));
                    offspring2.ReplaceGene(i, parent1.GetGene(i));
                }
            }
            return new List<IChromosome> { offspring1, offspring2 };
        }
    }
}