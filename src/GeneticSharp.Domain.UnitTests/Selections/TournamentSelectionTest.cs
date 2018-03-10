using System;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Domain.Selections;
using NUnit.Framework;
using NSubstitute;

namespace GeneticSharp.Domain.UnitTests.Selections
{
    [TestFixture]
    [Category("Selections")]
    public class TournamentSelectionTest
    {
        [TearDown]
        public void Cleanup()
        {
            RandomizationProvider.Current = new BasicRandomization();
        }

        [Test()]
        public void SelectChromosomes_InvalidNumber_Exception()
        {
            var target = new TournamentSelection();

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
            var target = new TournamentSelection();
            var actual = Assert.Catch<ArgumentNullException>(() =>
            {
                target.SelectChromosomes(2, null);
            });

            Assert.AreEqual("generation", actual.ParamName);
        }

        [Test()]
        public void SelectChromosomes_TournamentSizeGreaterThanAvailableChromosomes_Exception()
        {
            var target = new TournamentSelection(3, true);

            var c0 = Substitute.ForPartsOf<ChromosomeBase>(2);
            c0.Fitness = 0.1;

            var c1 = Substitute.ForPartsOf<ChromosomeBase>(2);
            c1.Fitness = 0.5;


            var generation = new Generation(1, new List<IChromosome>() {
                c0, c1
            });

            Assert.Catch<SelectionException>(() =>
            {
                target.SelectChromosomes(2, generation);
            }, "The tournament size is greater than available chromosomes. Tournament size is 3 and generation 1 available chromosomes are 2.");
        }

        [Test()]
        public void SelectChromosomes_TournamentSize3AllowWinnerCompeteNextTournamentTrue_ChromosomesSelected()
        {
            var target = new TournamentSelection(3, true);

            var c0 = Substitute.ForPartsOf<ChromosomeBase>(2);
            c0.Fitness = 0.1;

            var c1 = Substitute.ForPartsOf<ChromosomeBase>(2);
            c1.Fitness = 0.5;

            var c2 = Substitute.ForPartsOf<ChromosomeBase>(2);
            c2.Fitness = 0;

            var c3 = Substitute.ForPartsOf<ChromosomeBase>(2);
            c3.Fitness = 0.7;

            var c4 = Substitute.ForPartsOf<ChromosomeBase>(2);
            c4.Fitness = 0.3;

            var c5 = Substitute.ForPartsOf<ChromosomeBase>(2);
            c5.Fitness = 0.2;

            var generation = new Generation(1, new List<IChromosome>() {
                c0, c1, c2, c3, c4, c5
            });

            var rnd = Substitute.For<IRandomization>();
            rnd.GetUniqueInts(3, 0, 6).Returns(
                new int[] { 0, 1, 2 },
                new int[] { 3, 4, 5 },
                new int[] { 0, 2, 4 },
                new int[] { 1, 3, 5 });

            RandomizationProvider.Current = rnd;
      
            var actual = target.SelectChromosomes(4, generation);
            Assert.AreEqual(4, actual.Count);
            Assert.AreEqual(c1, actual[0]);
            Assert.AreEqual(c3, actual[1]);
            Assert.AreEqual(c4, actual[2]);
            Assert.AreEqual(c3, actual[3]);
        }

        [Test()]
        public void SelectChromosomes_TournamentSize3AllowWinnerCompeteNextTournamentFalse_ChromosomesSelected()
        {
            var target = new TournamentSelection(3, false);

            var c0 = Substitute.ForPartsOf<ChromosomeBase>(2);
            c0.Fitness = 0.1;

            var c1 = Substitute.ForPartsOf<ChromosomeBase>(2);
            c1.Fitness = 0.5;

            var c2 = Substitute.ForPartsOf<ChromosomeBase>(2);
            c2.Fitness = 0;

            var c3 = Substitute.ForPartsOf<ChromosomeBase>(2);
            c3.Fitness = 0.7;

            var c4 = Substitute.ForPartsOf<ChromosomeBase>(2);
            c4.Fitness = 0.3;

            var c5 = Substitute.ForPartsOf<ChromosomeBase>(2);
            c5.Fitness = 0.2;

            var generation = new Generation(1, new List<IChromosome>() {
                c0, c1, c2, c3, c4, c5
            });

            var rnd = Substitute.For<IRandomization>();
            rnd.GetUniqueInts(3, 0, 6).Returns(new int[] { 0, 1, 2 });
            rnd.GetUniqueInts(3, 0, 5).Returns(new int[] { 2, 3, 4 });
            rnd.GetUniqueInts(3, 0, 4).Returns(new int[] { 0, 1, 2 });
            rnd.GetUniqueInts(3, 0, 3).Returns(new int[] { 0, 1, 2 });

            RandomizationProvider.Current = rnd;

            var actual = target.SelectChromosomes(4, generation);
            Assert.AreEqual(4, actual.Count);
            Assert.AreEqual(c1, actual[0]);
            Assert.AreEqual(c3, actual[1]);
            Assert.AreEqual(c4, actual[2]);
            Assert.AreEqual(c5, actual[3]);
        }
    }
}
