using System;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Randomizations;
using NUnit.Framework;
using TestSharp;

namespace GeneticSharp.Domain.UnitTests.Populations
{
    [TestFixture()]
    [Category("Populations")]
    public class PopulationServiceTest
    {
        [SetUp]
        public void Cleanup()
        {
            RandomizationProvider.Current = new BasicRandomization();
        }

        [Test()]
        public void GetGenerationStrategyTypes_NoArgs_AllAvailableIGenerationStrategy()
        {
            var actual = PopulationService.GetGenerationStrategyTypes();

            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(typeof(PerformanceGenerationStrategy), actual[0]);
            Assert.AreEqual(typeof(TrackingGenerationStrategy), actual[1]);
        }

        [Test()]
        public void GetGenerationStrategyNames_NoArgs_AllAvailableGenerationStrategiesNames()
        {
            var actual = PopulationService.GetGenerationStrategyNames();

            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual("Performance", actual[0]);
            Assert.AreEqual("Tracking", actual[1]);
        }

        [Test()]
        public void CreateGenerationStrategyByName_InvalidName_Exception()
        {
            ExceptionAssert.IsThrowing(new ArgumentException("There is no IGenerationStrategy implementation with name 'Test'.", "name"), () =>
            {
                PopulationService.CreateGenerationStrategyByName("Test");
            });
        }

        [Test()]
        public void CreateGenerationStrategyByName_ValidNameButInvalidConstructorArgs_Exception()
        {
            ExceptionAssert.IsThrowing(new ArgumentException("A IGenerationStrategy's implementation with name 'Tracking' was found, but seems the constructor args were invalid.", "constructorArgs"), () =>
            {
                PopulationService.CreateGenerationStrategyByName("Tracking", 1);
            });
        }

        [Test()]
        public void CreateGenerationStrategyByName_ValidName_GenerationStrategyCreated()
        {
            IGenerationStrategy actual = PopulationService.CreateGenerationStrategyByName("Performance", 1) as PerformanceGenerationStrategy;
            Assert.IsNotNull(actual);

            actual = PopulationService.CreateGenerationStrategyByName("Tracking") as TrackingGenerationStrategy;
            Assert.IsNotNull(actual);
        }

        [Test()]
        public void GetGenerationStrategyTypeByName_InvalidName_Exception()
        {
            ExceptionAssert.IsThrowing(new ArgumentException("There is no IGenerationStrategy implementation with name 'Test'.", "name"), () =>
            {
                PopulationService.GetGenerationStrategyTypeByName("Test");
            });
        }

        [Test()]
        public void GetGenerationStrategyTypeByName_ValidName_GenerationStrategyTpe()
        {
            var actual = PopulationService.GetGenerationStrategyTypeByName("Performance");
            Assert.AreEqual(typeof(PerformanceGenerationStrategy), actual);

            actual = PopulationService.GetGenerationStrategyTypeByName("Tracking");
            Assert.AreEqual(typeof(TrackingGenerationStrategy), actual);
        }
    }
}