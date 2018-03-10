using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Domain.Reinsertions;
using NUnit.Framework;
using NSubstitute;

namespace GeneticSharp.Domain.UnitTests.Reinsertions
{
    [TestFixture()]
    [Category("Reinsertions")]
    public class UniformReinsertionTest
    {
        [TearDown]
        public void Cleanup()
        {
            RandomizationProvider.Current = new BasicRandomization();
        }

        [Test()]
        public void SelectChromosomes_OffspringSizeEqualsZero_Exception()
        {
            var target = new UniformReinsertion();
            var population = new Population(6, 8, Substitute.For<ChromosomeBase>(2));
            var offspring = new List<IChromosome>();

            var parents = new List<IChromosome>() {
                Substitute.For<ChromosomeBase> (5),
                Substitute.For<ChromosomeBase> (6),
                Substitute.For<ChromosomeBase> (7),
                Substitute.For<ChromosomeBase> (8)
            };

            Assert.Catch<ReinsertionException>(() =>
            {
                target.SelectChromosomes(population, offspring, parents);
            }, "The minimum size of the offspring is 1.");
        }

        [Test()]
        public void SelectChromosomes_offspringSizeLowerThanMinSize_Selectoffspring()
        {
            var target = new UniformReinsertion();

            var population = new Population(6, 8, Substitute.For<ChromosomeBase>(2));
            var offspring = new List<IChromosome>() {
                Substitute.For<ChromosomeBase> (2),
                Substitute.For<ChromosomeBase> (2),
                Substitute.For<ChromosomeBase> (3),
                Substitute.For<ChromosomeBase> (4)
            };

            var parents = new List<IChromosome>() {
                Substitute.For<ChromosomeBase> (5),
                Substitute.For<ChromosomeBase> (6),
                Substitute.For<ChromosomeBase> (7),
                Substitute.For<ChromosomeBase> (8)
            };

            var rnd = Substitute.For<IRandomization>();
            rnd.GetInt(0, 4).Returns(1);
            rnd.GetInt(0, 5).Returns(3);
            RandomizationProvider.Current = rnd;

            var selected = target.SelectChromosomes(population, offspring, parents);
            Assert.AreEqual(6, selected.Count);
            Assert.AreEqual(2, selected[0].Length);
            Assert.AreEqual(2, selected[1].Length);
            Assert.AreEqual(3, selected[2].Length);
            Assert.AreEqual(4, selected[3].Length);
            Assert.AreEqual(2, selected[4].Length);
            Assert.AreEqual(4, selected[5].Length);
        }
    }
}

