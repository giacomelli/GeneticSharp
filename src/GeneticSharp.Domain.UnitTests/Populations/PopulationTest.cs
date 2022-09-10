using System;
using NUnit.Framework;
using NSubstitute;

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
            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                new Population(-1, 1, null);
            }, "The minimum size for a population is 2 chromosomes.");

            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                new Population(0, 1, null);
            }, "The minimum size for a population is 2 chromosomes.");

            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                new Population(1, 1, null);
            }, "The minimum size for a population is 2 chromosomes.");
        }

        [Test]
        public void Constructor_InvalidMaxSize_Exception()
        {
            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                new Population(2, 1, null);
            }, "The maximum size for a population should be equal or greater than minimum size.");
        }

        [Test]
        public void Constructor_NullAdamChromosome_Exception()
        {
            var actual = Assert.Catch<ArgumentNullException>(() =>
            {
                new Population(2, 2, null);
            });

            Assert.AreEqual("adamChromosome", actual.ParamName);
        }

        [Test]
        public void CreateInitialGeneration_AdamChromosomeCreateNewNull_Exception()
        {
            var c = Substitute.For<ChromosomeBase>(4);
            c.CreateNew().Returns((IChromosome)null);
            var population = new Population(2, 2, c);

            Assert.Catch<InvalidOperationException>(() =>
            {
                population.CreateInitialGeneration();
            }, "The Adam chromosome's 'CreateNew' method generated a null chromosome. This is a invalid behavior, please, check your chromosome code.");
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
            target.EndCurrentGeneration();

            Assert.IsTrue(eventRaise);
        }

        /// <summary>
        /// https://github.com/giacomelli/GeneticSharp/issues/70
        /// </summary>
        [Test]
        public void EndCurrentGeneration_Issue70_Solved()
        {
            var target = new Population(100, 100, new ChromosomeStub());

            var eventRaise = false;
            IChromosome lastChromosome = null;

            target.BestChromosomeChanged += (e, a) =>
            {
                eventRaise = true;

                if (lastChromosome != null && lastChromosome.Fitness == target.BestChromosome.Fitness)
                    Assert.Fail("BestChromosomeChanged should only be raised if fitness is different.");

                lastChromosome = target.BestChromosome;
            };

            target.CreateInitialGeneration();
            target.CurrentGeneration.Chromosomes.Each(c => c.Fitness = 0);
            target.CurrentGeneration.Chromosomes[1].Fitness = 1;
            target.EndCurrentGeneration();

            target.CurrentGeneration.Chromosomes[0].Fitness = 1;            
            target.EndCurrentGeneration();

            Assert.IsTrue(eventRaise);
        }
    }
}

