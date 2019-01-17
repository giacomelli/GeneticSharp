using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Randomizations;
using NUnit.Framework;
using NSubstitute;
using System;

namespace GeneticSharp.Domain.UnitTests.Crossovers
{
    [TestFixture]
    [Category("Crossovers")]
    public class AlternatingPositionCrossoverTest
    {
        [TearDown]
        public void Cleanup()
        {
            RandomizationProvider.Current = new BasicRandomization();
        }

        [Test]
        public void Cross_ParentWithNoOrderedGenes_Exception()
        {
            var target = new AlternatingPositionCrossover();

            var chromosome1 = Substitute.For<ChromosomeBase>(8);
            chromosome1.ReplaceGenes(0, new Gene[] {
                new Gene(8),
                new Gene(2),
                new Gene(3),
                new Gene(4),
                new Gene(6),
                new Gene(5),
                new Gene(7),
                new Gene(1)
            });

            // 3 7 5 1 6 8 2 4
            var chromosome2 = Substitute.For<ChromosomeBase>(8);
            chromosome2.ReplaceGenes(0, new Gene[] {
                new Gene(1),
                new Gene(2),
                new Gene(3),
                new Gene(4),
                new Gene(5),
                new Gene(5),
                new Gene(6),
                new Gene(7)
            });

            Assert.Catch<CrossoverException>(() =>
            {
                target.Cross(new List<IChromosome>() { chromosome1, chromosome2 });
            }, "The Alternating-position (AP) can be only used with ordered chromosomes. The specified chromosome has repeated genes.");
        }

        [Test]
        public void Cross_DocumentationSample_Child()
        {
            var target = new AlternatingPositionCrossover();

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
                new Gene(8)
            });

            var child = Substitute.For<ChromosomeBase>(8);
            chromosome1.CreateNew().Returns(child);

            // 3 7 5 1 6 8 2 4
            var chromosome2 = Substitute.For<ChromosomeBase>(8);
            chromosome2.ReplaceGenes(0, new Gene[] {
                new Gene(3),
                new Gene(7),
                new Gene(5),
                new Gene(1),
                new Gene(6),
                new Gene(8),
                new Gene(2),
                new Gene(4)
            });

            var actual = target.Cross(new List<IChromosome>() { chromosome1, chromosome2 });

            Assert.AreEqual(1, actual.Count);
            var actualChild = actual[0];

            Assert.AreEqual(8, actualChild.Length);

            // 1 3 2 7 5 4 6 8
            Assert.AreEqual(1, actualChild.GetGene(0).Value);
            Assert.AreEqual(3, actualChild.GetGene(1).Value);
            Assert.AreEqual(2, actualChild.GetGene(2).Value);
            Assert.AreEqual(7, actualChild.GetGene(3).Value);
            Assert.AreEqual(5, actualChild.GetGene(4).Value);
            Assert.AreEqual(4, actualChild.GetGene(5).Value);
            Assert.AreEqual(6, actualChild.GetGene(6).Value);
            Assert.AreEqual(8, actualChild.GetGene(7).Value);
        }

        [Test]
        public void Cross_DocumentationSampleExchangingParents_Child()
        {
            var target = new AlternatingPositionCrossover();

            // 3 7 5 1 6 8 2 4
            var chromosome1 = Substitute.For<ChromosomeBase>(8);
            chromosome1.ReplaceGenes(0, new Gene[] {
                new Gene(3),
                new Gene(7),
                new Gene(5),
                new Gene(1),
                new Gene(6),
                new Gene(8),
                new Gene(2),
                new Gene(4)
            });
            var child = Substitute.For<ChromosomeBase>(8);
            chromosome1.CreateNew().Returns(child);

            // 1 2 3 4 5 6 7 8
            var chromosome2 = Substitute.For<ChromosomeBase>(8);
            chromosome2.ReplaceGenes(0, new Gene[] {
                new Gene(1),
                new Gene(2),
                new Gene(3),
                new Gene(4),
                new Gene(5),
                new Gene(6),
                new Gene(7),
                new Gene(8)
            });

            var actual = target.Cross(new List<IChromosome>() { chromosome1, chromosome2 });

            Assert.AreEqual(1, actual.Count);
            var actualChild = actual[0];

            Assert.AreEqual(8, actualChild.Length);

            // 3 1 7 2 5 4 6 8)
            Assert.AreEqual(3, actualChild.GetGene(0).Value);
            Assert.AreEqual(1, actualChild.GetGene(1).Value);
            Assert.AreEqual(7, actualChild.GetGene(2).Value);
            Assert.AreEqual(2, actualChild.GetGene(3).Value);
            Assert.AreEqual(5, actualChild.GetGene(4).Value);
            Assert.AreEqual(4, actualChild.GetGene(5).Value);
            Assert.AreEqual(6, actualChild.GetGene(6).Value);
            Assert.AreEqual(8, actualChild.GetGene(7).Value);
        }
    }
}