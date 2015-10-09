using System;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Domain.Selections;
using NUnit.Framework;
using Rhino.Mocks;
using TestSharp;

namespace GeneticSharp.Domain.UnitTests.Selections
{
    [TestFixture()]
    [Category("Selections")]
    public class RouletteWheelSelectionTest
    {
        [SetUp]
        public void Cleanup()
        {
            RandomizationProvider.Current = new BasicRandomization();
        }

        [Test()]
        public void SelectChromosomes_InvalidNumber_Exception()
        {
            var target = new RouletteWheelSelection();

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
            var target = new RouletteWheelSelection();

            ExceptionAssert.IsThrowing(new ArgumentNullException("generation"), () =>
            {
                target.SelectChromosomes(2, null);
            });
        }

        [Test()]
        public void SelectChromosomes_Generation_ChromosomesSelected()
        {
            var target = new RouletteWheelSelection();
            var c1 = MockRepository.GeneratePartialMock<ChromosomeBase>(2);
            c1.Fitness = 0.1;

            var c2 = MockRepository.GeneratePartialMock<ChromosomeBase>(2);
            c2.Fitness = 0.5;

            var c3 = MockRepository.GeneratePartialMock<ChromosomeBase>(2);
            c3.Fitness = 0;

            var c4 = MockRepository.GeneratePartialMock<ChromosomeBase>(2);
            c4.Fitness = 0.7;

            var generation = new Generation(1, new List<IChromosome>() {
                c1, c2, c3, c4
            });

            // Just one selected chromosome is c1.
            FlowAssert.IsAtLeastOneAttemptOk(100, () =>
            {
                var actual = target.SelectChromosomes(2, generation);
                Assert.AreEqual(2, actual.Count);
                Assert.AreEqual(1, actual.Count(c => c.Fitness == 0.1));
            });

            // All selected chromosome is c1.
            FlowAssert.IsAtLeastOneAttemptOk(1000, () =>
            {
                var actual = target.SelectChromosomes(2, generation);
                Assert.AreEqual(2, actual.Count);
                Assert.IsTrue(actual.All(c => c.Fitness == 0.1));
            });

            // Just one selected chromosome is c2.
            FlowAssert.IsAtLeastOneAttemptOk(100, () =>
            {
                var actual = target.SelectChromosomes(2, generation);
                Assert.AreEqual(2, actual.Count);
                Assert.AreEqual(1, actual.Count(c => c.Fitness == 0.5));
            });

            // All selected chromosome is c2.
            FlowAssert.IsAtLeastOneAttemptOk(1000, () =>
            {
                var actual = target.SelectChromosomes(2, generation);
                Assert.AreEqual(2, actual.Count);
                Assert.IsTrue(actual.All(c => c.Fitness == 0.5));
            });

            // None selected chromosome is c3.
            FlowAssert.IsAtLeastOneAttemptOk(100, () =>
            {
                var actual = target.SelectChromosomes(2, generation);
                Assert.AreEqual(2, actual.Count);
                Assert.AreEqual(0, actual.Count(c => c.Fitness == 0.0));
            });

            // Just one selected chromosome is c4.
            FlowAssert.IsAtLeastOneAttemptOk(100, () =>
            {
                var actual = target.SelectChromosomes(2, generation);
                Assert.AreEqual(2, actual.Count);
                Assert.AreEqual(1, actual.Count(c => c.Fitness == 0.7));
            });

            // All selected chromosome is c4.
            FlowAssert.IsAtLeastOneAttemptOk(1000, () =>
            {
                var actual = target.SelectChromosomes(2, generation);
                Assert.AreEqual(2, actual.Count);
                Assert.IsTrue(actual.All(c => c.Fitness == 0.7));
            });
        }
    }
}

