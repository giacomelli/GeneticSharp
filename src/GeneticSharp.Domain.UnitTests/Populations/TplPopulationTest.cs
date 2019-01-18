using System;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Infrastructure.Framework.Collections;
using NSubstitute;
using NUnit.Framework;

namespace GeneticSharp.Domain.UnitTests.Populations
{
    [TestFixture()]
    [NUnit.Framework.Category("Populations")]
    public class TplPopulationTest
    {
        [TearDown]
        public void Cleanup()
        {
            RandomizationProvider.Current = new BasicRandomization();
        }

        [Test]
        public void Constructor_NullAdamChromosome_Exception()
        {
            var actual = Assert.Catch<ArgumentNullException>(() =>
            {
                new TplPopulation(2, 2, null);
            });

            Assert.AreEqual("adamChromosome", actual.ParamName);
        }

        [Test]
        public void CreateInitialGeneration_AdamChromosomeCreateNewNull_Exception()
        {
            var c = Substitute.For<ChromosomeBase>(4);
            c.CreateNew().Returns((IChromosome)null);
            var population = new TplPopulation(2, 2, c);

            Assert.Catch<InvalidOperationException>(() =>
            {
                try
                {
                    population.CreateInitialGeneration();
                }
                catch (AggregateException e)
                {
                    throw e.InnerException;
                }
            }, "The Adam chromosome's 'CreateNew' method generated a null chromosome. This is a invalid behavior, please, check your chromosome code.");
        }

        [Test]
        public void EndCurrentGeneration_BestChromosomeChanged_ChangeEventRaise()
        {
            var target = new TplPopulation(2, 2, new ChromosomeStub());
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
    }
}
