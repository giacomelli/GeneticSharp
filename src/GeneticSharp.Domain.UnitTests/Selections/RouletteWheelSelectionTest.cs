using System;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Domain.Selections;
using NUnit.Framework;
using NSubstitute;
using GeneticSharp.Extensions.Tsp;

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

            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                target.SelectChromosomes(-1, null);
            }, "The number of selected chromosomes should be at least 2.");

            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                target.SelectChromosomes(0, null);
            }, "The number of selected chromosomes should be at least 2.");

            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                target.SelectChromosomes(1, null);
            }, "The number of selected chromosomes should be at least 2.");
        }

        [Test()]
        public void SelectChromosomes_NullGeneration_Exception()
        {
            var target = new RouletteWheelSelection();

            var actual = Assert.Catch<ArgumentNullException>(() =>
            {
                target.SelectChromosomes(2, null);
            });

            Assert.AreEqual("generation", actual.ParamName);
        }

        [Test()]
        public void SelectChromosomes_Generation_ChromosomesSelected()
        {
            var target = new RouletteWheelSelection();
            var c1 = Substitute.ForPartsOf<ChromosomeBase>(2);
            c1.Fitness = 0.1;

            var c2 = Substitute.ForPartsOf<ChromosomeBase>(2);
            c2.Fitness = 0.5;

            var c3 = Substitute.ForPartsOf<ChromosomeBase>(2);
            c3.Fitness = 0;

            var c4 = Substitute.ForPartsOf<ChromosomeBase>(2);
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

        [Test]
        public void SelectChromosomes_NullFitness_Exception()
        {
            var target = new RouletteWheelSelection();
            var generation = new Generation(1, new List<IChromosome>
            {
                new TspChromosome(10),
                new TspChromosome(10),
                new TspChromosome(10),
                new TspChromosome(10),
                new TspChromosome(10)
            });

            var actual = Assert.Catch<SelectionException>(() =>
            {
                target.SelectChromosomes(2, generation);
            });

            Assert.AreEqual("RouletteWheelSelection: There are chromosomes with null fitness.", actual.Message);
        }

        [Test()]
        public void SelectChromosomes_Generation_ChromosomesZeroFitness()
        {
            var target = new RouletteWheelSelection();
            var c1 = Substitute.ForPartsOf<ChromosomeBase>(2);
            c1.Fitness = 0;

            var c2 = Substitute.ForPartsOf<ChromosomeBase>(2);
            c2.Fitness = 0;

            var c3 = Substitute.ForPartsOf<ChromosomeBase>(2);
            c3.Fitness = 0;

            var c4 = Substitute.ForPartsOf<ChromosomeBase>(2);
            c4.Fitness = 0;

            var generation = new Generation(1, new List<IChromosome>() {
                c1, c2, c3, c4
            });

            var actual = target.SelectChromosomes(2, generation);
            Assert.AreEqual(0, actual.Count);
        }
    }
}

