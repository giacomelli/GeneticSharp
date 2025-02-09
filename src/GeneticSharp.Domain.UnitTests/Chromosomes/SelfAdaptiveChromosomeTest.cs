using System;
using System.Linq;
using NUnit.Framework;

namespace GeneticSharp.Domain.UnitTests.Chromosomes
{
    [TestFixture]
    public class SelfAdaptiveChromosomeTest
    {
        [Test]
        public void Constructor_ValidParameters_InitializesCorrectly()
        {
            int length = 10;
            double minValue = 0.0;
            double maxValue = 1.0;
            double initMutationProvVal = 0.05;

            var chromosome = new SelfAdaptiveChromosome(length, minValue, maxValue, initMutationProvVal);

            Assert.AreEqual(length, chromosome.Length);
        }

        [Test]
        public void Clone_ValidChromosome_ClonesCorrectly()
        {
            int length = 10;
            var chromosome = new SelfAdaptiveChromosome(length);

            var clone = chromosome.Clone() as SelfAdaptiveChromosome;

            Assert.IsNotNull(clone);
            Assert.AreEqual(chromosome.Length, clone.Length);
        }

        [Test]
        public void CreateNew_ValidChromosome_CreatesNewInstance()
        {
            int length = 10;
            var chromosome = new SelfAdaptiveChromosome(length);

            var newChromosome = chromosome.CreateNew() as SelfAdaptiveChromosome;

            Assert.IsNotNull(newChromosome);
            Assert.AreEqual(length, newChromosome.Length);
        }

        [Test]
        public void GenerateGene_ValidIndex_GeneratesGeneWithinRange()
        {
            int length = 10;
            double minValue = 0.0;
            double maxValue = 1.0;
            var chromosome = new SelfAdaptiveChromosome(length, minValue, maxValue);

            for (int i = 0; i < length; i++)
            {
                var gene = chromosome.GenerateGene(i);
                Assert.IsTrue((double)gene.Value >= minValue && (double)gene.Value <= maxValue);
            }
        }
    }
}
