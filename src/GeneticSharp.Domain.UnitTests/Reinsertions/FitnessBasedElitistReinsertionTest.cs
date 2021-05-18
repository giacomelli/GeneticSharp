using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Reinsertions;
using NSubstitute;
using NUnit.Framework;

namespace GeneticSharp.Domain.UnitTests.Reinsertions
{
    [TestFixture]
    [Category("Reinsertions")]
    public class FitnessBasedElitistReinsertionTest
    {
        [Test]
        public void SelectChromosomes_offspringSizeLowerThanMinSize_Selectoffspring()
        {
            var target = new FitnessBasedElitistReinsertion();

            var population = new Population(6, 8, Substitute.For<ChromosomeBase>(2));
            var offspring = new List<IChromosome>
            {
                Substitute.For<ChromosomeBase> (2),
                Substitute.For<ChromosomeBase> (2),
                Substitute.For<ChromosomeBase> (3),
                Substitute.For<ChromosomeBase> (4)
            };

            var parents = new List<IChromosome>
            {
                Substitute.For<ChromosomeBase> (5),
                Substitute.For<ChromosomeBase> (6),
                Substitute.For<ChromosomeBase> (7),
                Substitute.For<ChromosomeBase> (8)
            };

            
            offspring[0].Fitness = 0;
            offspring[1].Fitness = 0.1;
            offspring[2].Fitness = 0.4;
            offspring[3].Fitness = 0.5;
            parents[0].Fitness = 0.2;
            parents[1].Fitness = 0.3;
            parents[2].Fitness = 0.6;
            parents[3].Fitness = 0.7;


            var selected = target.SelectChromosomes(population, offspring, parents);
            Assert.AreEqual(6, selected.Count);
            Assert.AreEqual(8, selected[0].Length);
            Assert.AreEqual(7, selected[1].Length);
            Assert.AreEqual(4, selected[2].Length);
            Assert.AreEqual(3, selected[3].Length);
            Assert.AreEqual(6, selected[4].Length);
            Assert.AreEqual(5, selected[5].Length);
        }
    }
}