using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Randomizations;
using NUnit.Framework;
using NSubstitute;

namespace GeneticSharp.Domain.UnitTests.Crossovers
{
    [TestFixture]
    [Category("Crossovers")]
    public class UniformCrossoverTest
    {
        [TearDown]
        public void Cleanup()
        {
            RandomizationProvider.Current = new BasicRandomization();
        }

        [Test]
        public void Cross_ParentsWithTwoGenesProbabilityDiffPercents_DiffChildren()
        {
            var chromosome1 = Substitute.For<ChromosomeBase>(4);
            chromosome1.ReplaceGenes(0, new Gene[]
            {
                new Gene(1),
                new Gene(2),
                new Gene(3),
                new Gene(4),
            });
            chromosome1.CreateNew().Returns(Substitute.For<ChromosomeBase>(4));

            var chromosome2 = Substitute.For<ChromosomeBase>(4);
            chromosome2.ReplaceGenes(0, new Gene[]
            {
                new Gene(5),
                new Gene(6),
                new Gene(7),
                new Gene(8)
            });
            chromosome2.CreateNew().Returns(Substitute.For<ChromosomeBase>(4));
            var parents = new List<IChromosome>() { chromosome1, chromosome2 };

            var rnd = Substitute.For<IRandomization>();
            rnd.GetDouble().Returns(0, 0.49, 0.5, 1);

            RandomizationProvider.Current = rnd;

            // 50%
            var target = new UniformCrossover(0.5f);
      
            var actual = target.Cross(parents);
            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(4, actual[0].Length);
            Assert.AreEqual(4, actual[1].Length);

            Assert.AreEqual(1, actual[0].GetGene(0).Value);
            Assert.AreEqual(2, actual[0].GetGene(1).Value);
            Assert.AreEqual(7, actual[0].GetGene(2).Value);
            Assert.AreEqual(8, actual[0].GetGene(3).Value);

            Assert.AreEqual(5, actual[1].GetGene(0).Value);
            Assert.AreEqual(6, actual[1].GetGene(1).Value);
            Assert.AreEqual(3, actual[1].GetGene(2).Value);
            Assert.AreEqual(4, actual[1].GetGene(3).Value);


            // 70%
            rnd = Substitute.For<IRandomization>();
            rnd.GetDouble().Returns(0, 0.49, 0.5, 1);

            RandomizationProvider.Current = rnd;
     
            target = new UniformCrossover(0.7f);
            actual = target.Cross(parents);
            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(4, actual[0].Length);
            Assert.AreEqual(4, actual[1].Length);

            Assert.AreEqual(1, actual[0].GetGene(0).Value);
            Assert.AreEqual(2, actual[0].GetGene(1).Value);
            Assert.AreEqual(3, actual[0].GetGene(2).Value);
            Assert.AreEqual(8, actual[0].GetGene(3).Value);

            Assert.AreEqual(5, actual[1].GetGene(0).Value);
            Assert.AreEqual(6, actual[1].GetGene(1).Value);
            Assert.AreEqual(7, actual[1].GetGene(2).Value);
            Assert.AreEqual(4, actual[1].GetGene(3).Value);
        }
    }
}