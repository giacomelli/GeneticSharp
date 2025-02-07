using System;
using System.ComponentModel;
using GeneticSharp;

namespace GeneticSharp
{
    [DisplayName("Self Adaptive Mutation")]
    public class SelfAdaptiveMutation : MutationBase
    {
        private readonly double tau;
        private readonly double minMutationRate;
        private readonly double maxMutationRate;


        public SelfAdaptiveMutation(double tau = 0.1, double minMutationRate = 0.05, double maxMutationRate = 0.9)
        {
            this.tau = tau;
            this.minMutationRate = minMutationRate;
            this.maxMutationRate = maxMutationRate;
        }

        protected override void PerformMutate(IChromosome chromosome, float probability)
        {
            if (chromosome is not SelfAdaptiveChromosome adaptiveChromosome)
            {
                throw new ArgumentException("The chromosome must be of type SelfAdaptiveChromosome.", nameof(chromosome));
            }

            var random = RandomizationProvider.Current;

            for (int i = 0; i < adaptiveChromosome.GenesValues.Length; i++)
            {
                if (random.GetDouble() < adaptiveChromosome.MutationProbabilities[i])
                {
                    // Mutación auto-adaptativa de la tasa de mutación
                    double normal = NextGaussian(0, 1);
                    adaptiveChromosome.MutationProbabilities[i] *= Math.Exp(tau * normal);

                    // Restringir la tasa de mutación dentro de límites
                    adaptiveChromosome.MutationProbabilities[i] = Math.Clamp(adaptiveChromosome.MutationProbabilities[i], minMutationRate, maxMutationRate);
                }

                // Aplicar mutación al gen con la tasa adaptada
                if (random.GetDouble() < adaptiveChromosome.MutationProbabilities[i])
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