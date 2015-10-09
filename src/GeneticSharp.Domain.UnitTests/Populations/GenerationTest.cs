using System;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;
using NUnit.Framework;
using Rhino.Mocks;
using TestSharp;

namespace GeneticSharp.Domain.UnitTests.Populations
{
    [TestFixture()]
    [Category("Populations")]
    public class GenerationTest
    {
        [Test()]
        public void Constructor_ZeroOrNegativeNumber_Exception()
        {
            ExceptionAssert.IsThrowing(new ArgumentOutOfRangeException("number", "Generation number -1 is invalid. Generation number should be positive and start in 1."), () =>
            {
                new Generation(-1, null);
            });

            ExceptionAssert.IsThrowing(new ArgumentOutOfRangeException("number", "Generation number 0 is invalid. Generation number should be positive and start in 1."), () =>
            {
                new Generation(0, null);
            });
        }

        [Test()]
        public void Constructor_InvalidChromosomesQuantity_Exception()
        {
            ExceptionAssert.IsThrowing(new ArgumentOutOfRangeException("chromosomes", "A generation should have at least 2 chromosomes."), () =>
            {
                new Generation(1, null);
            });

            ExceptionAssert.IsThrowing(new ArgumentOutOfRangeException("chromosomes", "A generation should have at least 2 chromosomes."), () =>
            {
                new Generation(1, new List<IChromosome>());
            });

            ExceptionAssert.IsThrowing(new ArgumentOutOfRangeException("chromosomes", "A generation should have at least 2 chromosomes."), () =>
            {
                new Generation(1, new List<IChromosome>() { MockRepository.GenerateMock<IChromosome>() });
            });
        }

        [Test()]
        public void Constructor_OkArguments_Instanced()
        {
            var target = new Generation(1, new List<IChromosome>() {
                MockRepository.GenerateMock<IChromosome>(),
                MockRepository.GenerateMock<IChromosome>()
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

            ExceptionAssert.IsThrowing(new InvalidOperationException("There is unknown problem in current generation, because a chromosome has no fitness value."), () =>
            {
                target.End(2);
            });
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

