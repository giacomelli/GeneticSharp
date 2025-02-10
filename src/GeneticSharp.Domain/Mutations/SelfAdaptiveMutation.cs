using System;
using System.ComponentModel;
using System.Linq;
using GeneticSharp;

namespace GeneticSharp
{
    [DisplayName("Self Adaptive Mutation")]
    public class SelfAdaptiveMutation : MutationBase
    {
        public readonly double Tau = 0.1;
        public readonly double MinMutationRate = 0.05;
        public readonly double MaxMutationRate = 0.9;


        public SelfAdaptiveMutation()
        {
        }

        public SelfAdaptiveMutation(double tau = 0.1, double minMutationRate = 0.05, double maxMutationRate = 0.9)
        {
            this.Tau = tau;
            this.MinMutationRate = minMutationRate;
            this.MaxMutationRate = maxMutationRate;
        }

        protected override void PerformMutate(IChromosome chromosome, float probability)
        {
            if (chromosome is not SelfAdaptiveChromosome adaptiveChromosome)
            {
                throw new ArgumentException("The chromosome must be of type SelfAdaptiveChromosome.", nameof(chromosome));
            }

            var random = RandomizationProvider.Current;

            for (int i = 0; i < adaptiveChromosome.Length; i++)
            {
                if (random.GetDouble() < adaptiveChromosome.GetMutationProbability(i))
                {
                    double normal = NextGaussian(0, 1);
                    double p = adaptiveChromosome.GetMutationProbability(i) * Math.Exp(Tau * normal);
                    p = Math.Clamp(p, MinMutationRate, MaxMutationRate);
                    adaptiveChromosome.SetMutationProbability(i,p);
                }

                if (random.GetDouble() < adaptiveChromosome.GetMutationProbability(i))
                {
                    var g = chromosome.GenerateGene(i);
                    adaptiveChromosome.ReplaceGene(i, g);
                }
            }
        }

        private double NextGaussian(double mean, double stdDev)
        {
            var random = RandomizationProvider.Current;
            double u1 = random.GetDouble();
            double u2 = random.GetDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
            return mean + stdDev * randStdNormal;
        }
    }
}