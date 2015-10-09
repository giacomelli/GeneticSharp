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
            var target = new TournamentSelection();

            ExceptionAssert.IsThrowing(new ArgumentNullException("generation"), () =>
            {
                target.SelectChromosomes(2, null);
            });
        }

        [Test()]
        public void SelectChromosomes_TournamentSizeGreaterThanAvailableChromosomes_Exception()
        {
            var target = new TournamentSelection(3, true);

            var c0 = MockRepository.GeneratePartialMock<ChromosomeBase>(2);
            c0.Fitness = 0.1;

            var c1 = MockRepository.GeneratePartialMock<ChromosomeBase>(2);
            c1.Fitness = 0.5;


            var generation = new Generation(1, new List<IChromosome>() {
                c0, c1
            });

            ExceptionAssert.IsThrowing(new SelectionException(target,
                "The tournament size is greater than available chromosomes. Tournament size is 3 and generation 1 available chromosomes are 2."), () =>
                {
                    target.SelectChromosomes(2, generation);
                });
        }

        [Test()]
        public void SelectChromosomes_TournamentSize3AllowWinnerCompeteNextTournamentTrue_ChromosomesSelected()
        {
            var target = new TournamentSelection(3, true);

            var c0 = MockRepository.GeneratePartialMock<ChromosomeBase>(2);
            c0.Fitness = 0.1;

            var c1 = MockRepository.GeneratePartialMock<ChromosomeBase>(2);
            c1.Fitness = 0.5;

            var c2 = MockRepository.GeneratePartialMock<ChromosomeBase>(2);
            c2.Fitness = 0;

            var c3 = MockRepository.GeneratePartialMock<ChromosomeBase>(2);
            c3.Fitness = 0.7;

            var c4 = MockRepository.GeneratePartialMock<ChromosomeBase>(2);
            c4.Fitness = 0.3;

            var c5 = MockRepository.GeneratePartialMock<ChromosomeBase>(2);
            c5.Fitness = 0.2;

            var generation = new Generation(1, new List<IChromosome>() {
                c0, c1, c2, c3, c4, c5
            });

            var mock = new MockRepository();
            var rnd = mock.StrictMock<IRandomization>();

            using (mock.Ordered())
            {
                rnd.Expect(r => r.GetUniqueInts(3, 0, 6)).Return(new int[] { 0, 1, 2 });
                rnd.Expect(r => r.GetUniqueInts(3, 0, 6)).Return(new int[] { 3, 4, 5 });
                rnd.Expect(r => r.GetUniqueInts(3, 0, 6)).Return(new int[] { 0, 2, 4 });
                rnd.Expect(r => r.GetUniqueInts(3, 0, 6)).Return(new int[] { 1, 3, 5 });
            }

            RandomizationProvider.Current = rnd;
            mock.ReplayAll();

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

            var c0 = MockRepository.GeneratePartialMock<ChromosomeBase>(2);
            c0.Fitness = 0.1;

            var c1 = MockRepository.GeneratePartialMock<ChromosomeBase>(2);
            c1.Fitness = 0.5;

            var c2 = MockRepository.GeneratePartialMock<ChromosomeBase>(2);
            c2.Fitness = 0;

            var c3 = MockRepository.GeneratePartialMock<ChromosomeBase>(2);
            c3.Fitness = 0.7;

            var c4 = MockRepository.GeneratePartialMock<ChromosomeBase>(2);
            c4.Fitness = 0.3;

            var c5 = MockRepository.GeneratePartialMock<ChromosomeBase>(2);
            c5.Fitness = 0.2;

            var generation = new Generation(1, new List<IChromosome>() {
                c0, c1, c2, c3, c4, c5
            });

            var mock = new MockRepository();
            var rnd = mock.StrictMock<IRandomization>();

            using (mock.Ordered())
            {
                rnd.Expect(r => r.GetUniqueInts(3, 0, 6)).Return(new int[] { 0, 1, 2 });
                rnd.Expect(r => r.GetUniqueInts(3, 0, 5)).Return(new int[] { 2, 3, 4 });
                rnd.Expect(r => r.GetUniqueInts(3, 0, 4)).Return(new int[] { 0, 1, 2 });
                rnd.Expect(r => r.GetUniqueInts(3, 0, 3)).Return(new int[] { 0, 1, 2 });
            }

            RandomizationProvider.Current = rnd;
            mock.ReplayAll();


            var actual = target.SelectChromosomes(4, generation);
            Assert.AreEqual(4, actual.Count);
            Assert.AreEqual(c1, actual[0]);
            Assert.AreEqual(c3, actual[1]);
            Assert.AreEqual(c4, actual[2]);
            Assert.AreEqual(c5, actual[3]);
        }
    }
}
