using System;
using System.Linq;
using NUnit.Framework;

namespace GeneticSharp.Domain.UnitTests.Mutations
{
    [TestFixture]
    public class SelfAdaptiveMutationTest
    {
        [Test]
        public void Constructor_ValidParameters_InitializesCorrectly()
        {
            double tau = 0.1;
            double minMutationRate = 0.05;
            double maxMutationRate = 0.9;

            var mutation = new SelfAdaptiveMutation(tau, minMutationRate, maxMutationRate);
        }

      

        [Test]
        public void PerformMutate_ValidChromosome_MutatesCorrectly()
        {
            int length = 100;
            var chromosome = new SelfAdaptiveChromosome(length, 0,20);
            var mutation = new SelfAdaptiveMutation();
            var random = new Random();

          

            mutation.Mutate(chromosome, 1.0f);

            // Check if mutation probabilities are within the expected range
            for (int i = 0; i < length; i++)
            {
                Assert.That(chromosome.GetMutationProbability(i), Is.InRange(mutation.MinMutationRate, mutation.MaxMutationRate));
            }

            // Check if genes have been mutated
            var genes = chromosome.GetGenes();
            for (int i = 0; i < length; i++)
            {
                Assert.That((double)genes[i].Value, Is.InRange(chromosome._minValue, chromosome._maxValue));
            }
        }

       
    }
}
