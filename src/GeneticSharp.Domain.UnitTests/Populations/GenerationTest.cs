using System;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;
using NUnit.Framework;
using NSubstitute;

namespace GeneticSharp.Domain.UnitTests.Populations
{
    [TestFixture()]
    [Category("Populations")]
    public class GenerationTest
    {
        [Test()]
        public void Constructor_ZeroOrNegativeNumber_Exception()
        {
            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                new Generation(-1, null);
            }, "Generation number -1 is invalid. Generation number should be positive and start in 1.");

            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                new Generation(0, null);
            }, "Generation number 0 is invalid. Generation number should be positive and start in 1.");
        }

        [Test()]
        public void Constructor_InvalidChromosomesQuantity_Exception()
        {
            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                new Generation(1, null);
            }, "A generation should have at least 2 chromosomes.");

            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                new Generation(1, new List<IChromosome>());
            }, "A generation should have at least 2 chromosomes.");

            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                new Generation(1, new List<IChromosome>() { Substitute.For<IChromosome>() });
            }, "A generation should have at least 2 chromosomes.");
        }

        [Test()]
        public void Constructor_OkArguments_Instanced()
        {
            var target = new Generation(1, new List<IChromosome>() {
                Substitute.For<IChromosome>(),
                Substitute.For<IChromosome>()
            });

            Assert.AreEqual(1, target.Number);
            Assert.AreEqual(2, target.Chromosomes.Count);
        }

        [Test]
        public void End_AnyChromosomeWithoutFitness_Exception()
        {
            var target = new Generation(1, new List<IChromosome>() {
                new ChromosomeStub() { Fitness = 0.2 },
                new ChromosomeStub() { Fitness = null},
                new ChromosomeStub() { Fitness = 0.1 }
            });

            Assert.Catch<InvalidOperationException>(() =>
            {
                target.End(2);
            }, "There is unknown problem in current generation, because a chromosome has no fitness value.");
        }

        [Test]
        public void End_ChromosomeNumberGreaterThan_Take()
        {
            var target = new Generation(1, new List<IChromosome>() {
                new ChromosomeStub() { Fitness = 0.2 },
                new ChromosomeStub() { Fitness = 0.3 },
                new ChromosomeStub() { Fitness = 0.1 }
            });

            target.End(2);
            Assert.AreEqual(2, target.Chromosomes.Count);
            Assert.AreEqual(0.3, target.Chromosomes[0].Fitness);
            Assert.AreEqual(0.2, target.Chromosomes[1].Fitness);
        }
    }
}

