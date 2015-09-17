using System;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Randomizations;
using HelperSharp;
using NUnit.Framework;
using Rhino.Mocks;
using TestSharp;

namespace GeneticSharp.Domain.UnitTests.Populations
{
    [TestFixture()]
    [Category("Populations")]
    public class PopulationTest
    {
        [TearDown]
        public void Cleanup()
        {
            RandomizationProvider.Current = new BasicRandomization();
        }

        [Test]
        public void Constructor_InvalidMinSize_Exception()
        {
            ExceptionAssert.IsThrowing(new ArgumentOutOfRangeException("minSize", "The minimum size for a population is 2 chromosomes."), () =>
            {
                new Population(-1, 1, null);
            });

            ExceptionAssert.IsThrowing(new ArgumentOutOfRangeException("minSize", "The minimum size for a population is 2 chromosomes."), () =>
            {
                new Population(0, 1, null);
            });

            ExceptionAssert.IsThrowing(new ArgumentOutOfRangeException("minSize", "The minimum size for a population is 2 chromosomes."), () =>
            {
                new Population(1, 1, null);
            });
        }

        [Test]
        public void Constructor_InvalidMaxSize_Exception()
        {
            ExceptionAssert.IsThrowing(new ArgumentOutOfRangeException("maxSize", "The maximum size for a population should be equal or greater than minimum size."), () =>
            {
                new Population(2, 1, null);
            });
        }

        [Test]
        public void Constructor_NullAdamChromosome_Exception()
        {
            ExceptionAssert.IsThrowing(new ArgumentNullException("adamChromosome"), () =>
            {
                new Population(2, 2, null);
            });
        }

        [Test]
        public void CreateInitialGeneration_AdamChromosomeCreateNewNull_Exception()
        {
            var population = new Population(2, 2, MockRepository.GenerateStub<ChromosomeBase>(4));

            ExceptionAssert.IsThrowing(new InvalidOperationException("The Adam chromosome's 'CreateNew' method generated a null chromosome. This is a invalid behavior, please, check your chromosome code."), () =>
            {
                population.CreateInitialGeneration();
            });
        }

        [Test]
        public void EndCurrentGeneration_BestChromosomeChanged_ChangeEventRaise()
        {
            var target = new Population(2, 2, new ChromosomeStub());
            var eventRaise = false;
            target.BestChromosomeChanged += (e, a) =>
            {
                eventRaise = true;
            };

            target.CreateInitialGeneration();
            target.CurrentGeneration.Chromosomes.Each(c => c.Fitness = 1);
            target.CurrentGeneration.BestChromosome = target.CurrentGeneration.Chromosomes[0];
            target.EndCurrentGeneration();

            Assert.IsTrue(eventRaise);
        }
    }
}

