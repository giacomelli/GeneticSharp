using System;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Domain.Selections;
using NUnit.Framework;
using Rhino.Mocks;
using TestSharp;

namespace GeneticSharp.Domain.UnitTests.Selections
{
    [TestFixture]
    [Category("Selections")]
    public class StochasticUniversalSamplingSelectionTest
    {
        [SetUp]
        public void Cleanup()
        {
            RandomizationProvider.Current = new BasicRandomization();
        }

        [Test()]
        public void SelectChromosomes_InvalidNumber_Exception()
        {
            var target = new StochasticUniversalSamplingSelection();

            ExceptionAssert.IsThrowing(new ArgumentOutOfRangeException("number", "The number of selected chromosomes should be at least 2."), () =>
            {
                target.SelectChromosomes(-1, null);
            });

            ExceptionAssert.IsThrowing(new ArgumentOutOfRangeException("number", "The number of selected chromosomes should be at least 2."), () =>
            {
                target.SelectChromosomes(0, null);
            });

            ExceptionAssert.IsThrowing(new ArgumentOutOfRangeException("number", "The number of selected chromosomes should be at least 2."), () =>
            {
                target.SelectChromosomes(1, null);
            });
        }

        [Test()]
        public void SelectChromosomes_NullGeneration_Exception()
        {
            var target = new StochasticUniversalSamplingSelection();

            ExceptionAssert.IsThrowing(new ArgumentNullException("generation"), () =>
            {
                target.SelectChromosomes(2, null);
            });
        }

        [Test()]
        public void SelectChromosomes_Generation_ChromosomesSelected()
        {

            var target = new StochasticUniversalSamplingSelection();
            var c1 = new ChromosomeStub();
            c1.Fitness = 0.1;

            var c2 = new ChromosomeStub();
            c2.Fitness = 0.5;

            var c3 = new ChromosomeStub();
            c3.Fitness = 0;

            var c4 = new ChromosomeStub();
            c4.Fitness = 0.7;

            var generation = new Generation(1, new List<IChromosome>() {
                c1, c2, c3, c4
            });

            // Fitness sum: 0.1 + 0.5 + 0 + 0.7 = 1.3
            // c1:  8% = 0.08
            // c2: 38% = 0.46
            // c3:  0% = 0.46
            // c4: 54% = 1.00
            var rnd = MockRepository.GenerateMock<IRandomization>();
            rnd.Expect(r => r.GetDouble()).Return(0.3);
            RandomizationProvider.Current = rnd;

            // Step size 1/2 = 0.5
            var actual = target.SelectChromosomes(2, generation);
            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(c2.Fitness, actual[0].Fitness); // 0.3
            Assert.AreEqual(c4.Fitness, actual[1].Fitness); // 0.8

            // Step size 1/3 = 0.33
            actual = target.SelectChromosomes(3, generation);
            Assert.AreEqual(3, actual.Count);
            Assert.AreEqual(c2.Fitness, actual[0].Fitness); // 0.3
            Assert.AreEqual(c4.Fitness, actual[1].Fitness); // 0.63
            Assert.AreEqual(c4.Fitness, actual[2].Fitness); // 0.96

            // Step size 1/4 = 0.25
            actual = target.SelectChromosomes(4, generation);
            Assert.AreEqual(4, actual.Count);
            Assert.AreEqual(c2.Fitness, actual[0].Fitness); // 0.3
            Assert.AreEqual(c4.Fitness, actual[1].Fitness); // 0.55
            Assert.AreEqual(c4.Fitness, actual[2].Fitness); // 0.80
            Assert.AreEqual(c1.Fitness, actual[3].Fitness); // 0.05

            // Step size 1/5 = 0.20
            actual = target.SelectChromosomes(5, generation);
            Assert.AreEqual(5, actual.Count);
            Assert.AreEqual(c2.Fitness, actual[0].Fitness); // 0.3
            Assert.AreEqual(c4.Fitness, actual[1].Fitness); // 0.5
            Assert.AreEqual(c4.Fitness, actual[2].Fitness); // 0.7
            Assert.AreEqual(c4.Fitness, actual[3].Fitness); // 0.9
            Assert.AreEqual(c2.Fitness, actual[4].Fitness); // 0.1
        }
    }
}