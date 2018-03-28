using System;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using NUnit.Framework;
using NSubstitute;

namespace GeneticSharp.Domain.UnitTests.Selections
{
    [TestFixture()]
    [Category("Selections")]
    public class EliteSelectionTest
    {
        [Test()]
        public void SelectChromosomes_InvalidNumber_Exception()
        {
            var target = new EliteSelection();

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
            var target = new EliteSelection();

            var actual = Assert.Catch<ArgumentNullException>(() =>
            {
                target.SelectChromosomes(2, null);
            });

            Assert.AreEqual("generation", actual.ParamName);
        }

        [Test()]
        public void SelectChromosomes_Generation_ChromosomesSelected()
        {
            var target = new EliteSelection();
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


            var actual = target.SelectChromosomes(2, generation);
            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(0.7, actual[0].Fitness);
            Assert.AreEqual(0.5, actual[1].Fitness);

            actual = target.SelectChromosomes(3, generation);
            Assert.AreEqual(3, actual.Count);
            Assert.AreEqual(0.7, actual[0].Fitness);
            Assert.AreEqual(0.5, actual[1].Fitness);
            Assert.AreEqual(0.1, actual[2].Fitness);
        }
    }
}

