﻿
using System;
using System.ComponentModel;

namespace GeneticSharp
{
    [DisplayName("Self Adaptive Mutation")]
    public class SelfAdaptiveMutation : MutationBase
    {
        private readonly double tau;
        private readonly double sigmaMutationProbability;

        public SelfAdaptiveMutation(double tau = 0.1, double sigmaMutationProbability = 0.05)
        {
            this.tau = tau;
            this.sigmaMutationProbability = sigmaMutationProbability;
        }

        protected override void PerformMutate(IChromosome chromosome, float probability)
        {
            var adaptiveChromosome = chromosome as SelfAdaptiveChromosome;
            if (adaptiveChromosome == null)
            {
                throw new ArgumentException("The chromosome must be of type SelfAdaptiveChromosome.", nameof(chromosome));
            }

            var random = RandomizationProvider.Current;
            adaptiveChromosome.MutateOperators();

            for (int i = 0; i < adaptiveChromosome.GenesValues.Length; i++)
            {
                if (random.GetDouble() < sigmaMutationProbability)
                {
                    double normal = NextGaussian(0, 1);
                    adaptiveChromosome.MutationProbabilities[i] *= Math.Exp(tau * normal);
                    adaptiveChromosome.MutationProbabilities[i] = Math.Min(1.0, Math.Max(0, adaptiveChromosome.MutationProbabilities[i]));
                }

                if (random.GetDouble() >= adaptiveChromosome.MutationProbabilities[i])
                    continue;
                
                double mutatedValue = 0;
                switch (adaptiveChromosome.MutationType)
                {
                    case MutationType.Simple:
                        mutatedValue = (double)adaptiveChromosome.GenesValues[i].Value + random.GetDouble(-1, 1);
                        break;
                    case MutationType.WithImprovement:
                        mutatedValue = (double)adaptiveChromosome.GenesValues[i].Value + random.GetDouble(-0.5, 0.5);
                        break;
                    case MutationType.Strong:
                        mutatedValue = (double)adaptiveChromosome.GenesValues[i].Value + random.GetDouble(-2, 2);
                        break;
                }
                mutatedValue = (double)adaptiveChromosome.GenesValues[i].Value + mutatedValue;

                if (mutatedValue > adaptiveChromosome._maxValue)
                    mutatedValue = adaptiveChromosome._maxValue;
                else if (mutatedValue < adaptiveChromosome._minValue)
                    mutatedValue = adaptiveChromosome._minValue;

                adaptiveChromosome.GenesValues[i] = new Gene(mutatedValue);
                adaptiveChromosome.ReplaceGene(i, new Gene(mutatedValue));
            }
        }

        private double NextGaussian(double mean, double stdDev)
        {
            var random = RandomizationProvider.Current;
            double u1 = random.GetDouble();
            double u2 = random.GetDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return mean + stdDev * randStdNormal;
        }
    }
}
