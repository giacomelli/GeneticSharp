using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Randomizations;
using NUnit.Framework;
using NSubstitute;

namespace GeneticSharp.Domain.UnitTests.Crossovers
{
    [TestFixture]
    [Category("Crossovers")]
    public class OrderBasedCrossoverTest
    {
        [TearDown]
        public void Cleanup()
        {
            RandomizationProvider.Current = new BasicRandomization();
        }

        [Test]
        public void Cross_ParentWithNoOrderedGenes_Exception()
        {
            var target = new OrderBasedCrossover();

            var chromosome1 = Substitute.For<ChromosomeBase>(10);
            chromosome1.ReplaceGenes(0, new Gene[] {
                new Gene(8),
                new Gene(4),
                new Gene(7),
                new Gene(3),
                new Gene(6),
                new Gene(2),
                new Gene(5),
                new Gene(1),
                new Gene(9),
                new Gene(0)
            });
            chromosome1.CreateNew().Returns(Substitute.For<ChromosomeBase>(10));

            var chromosome2 = Substitute.For<ChromosomeBase>(10);
            chromosome2.ReplaceGenes(0, new Gene[]
            {
                new Gene(0),
                new Gene(1),
                new Gene(2),
                new Gene(3),
                new Gene(5),
                new Gene(5),
                new Gene(6),
                new Gene(7),
                new Gene(8),
                new Gene(9),
            });
            chromosome2.CreateNew().Returns(Substitute.For<ChromosomeBase>(10));

            Assert.Catch<CrossoverException>(() =>
            {
                target.Cross(new List<IChromosome>() { chromosome1, chromosome2 });
            }, "The Order-based Crossover (OX2) can be only used with ordered chromosomes. The specified chromosome has repeated genes.");
        }

        [Test]
        public void Cross_ParentsWith8Genes_Cross()
        {
            var target = new OrderBasedCrossover();

			// 1 2 3 4 5 6 7 8
            var chromosome1 = Substitute.For<ChromosomeBase>(8);
            chromosome1.ReplaceGenes(0, new Gene[] {
                new Gene(1),
                new Gene(2),
                new Gene(3),
                new Gene(4),
                new Gene(5),
                new Gene(6),
                new Gene(7),
                new Gene(8)            });
            chromosome1.CreateNew().Returns(Substitute.For<ChromosomeBase>(8));

			// 2 4 6 8 7 5 3 1
            var chromosome2 = Substitute.For<ChromosomeBase>(8);
            chromosome2.ReplaceGenes(0, new Gene[]
            {
                new Gene(2),
                new Gene(4),
                new Gene(6),
                new Gene(8),
                new Gene(7),
                new Gene(5),
                new Gene(3),
                new Gene(1)
            });
            chromosome2.CreateNew().Returns(Substitute.For<ChromosomeBase>(8));

			// Child one: 1 2 3 4 6 5 7 8
			// Child two: 2 4 3 8 7 5 6 1
            var rnd = Substitute.For<IRandomization>();
			rnd.GetInt(1, 7).Returns(3);
            rnd.GetUniqueInts(3, 0, 8).Returns(new int[] { 1, 2, 5 });
            RandomizationProvider.Current = rnd;

            var actual = target.Cross(new List<IChromosome>() { chromosome1, chromosome2 });

            Assert.AreEqual(2, actual.Count);
			var childOne = actual [0];
			var childTwo = actual [1];
            Assert.AreEqual(8, childOne.Length);
            Assert.AreEqual(8, childTwo.Length);

            Assert.AreEqual(8, childOne.GetGenes().Distinct().Count());
            Assert.AreEqual(8, childTwo.GetGenes().Distinct().Count());

            Assert.AreEqual(1, childOne.GetGene(0).Value);
            Assert.AreEqual(2, childOne.GetGene(1).Value);
            Assert.AreEqual(3, childOne.GetGene(2).Value);
            Assert.AreEqual(4, childOne.GetGene(3).Value);
            Assert.AreEqual(6, childOne.GetGene(4).Value);
            Assert.AreEqual(5, childOne.GetGene(5).Value);
            Assert.AreEqual(7, childOne.GetGene(6).Value);
            Assert.AreEqual(8, childOne.GetGene(7).Value);

            Assert.AreEqual(2, childTwo.GetGene(0).Value);
            Assert.AreEqual(4, childTwo.GetGene(1).Value);
            Assert.AreEqual(3, childTwo.GetGene(2).Value);
            Assert.AreEqual(8, childTwo.GetGene(3).Value);
            Assert.AreEqual(7, childTwo.GetGene(4).Value);
            Assert.AreEqual(5, childTwo.GetGene(5).Value);
            Assert.AreEqual(6, childTwo.GetGene(6).Value);
            Assert.AreEqual(1, childTwo.GetGene(7).Value);
        }
    }
}