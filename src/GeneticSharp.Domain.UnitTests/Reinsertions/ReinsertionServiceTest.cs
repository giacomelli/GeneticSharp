using System;
using GeneticSharp.Domain.Reinsertions;
using NUnit.Framework;
using TestSharp;

namespace GeneticSharp.Domain.UnitTests.Reinsertions
{
    [TestFixture()]
    [Category("Reinsertions")]
    public class ReinsertionServiceTest
    {
        [Test()]
        public void GetReinsertionTypes_NoArgs_AllAvailableReinsertions()
        {
            var actual = ReinsertionService.GetReinsertionTypes();

            Assert.AreEqual(4, actual.Count);
            Assert.AreEqual(typeof(ElitistReinsertion), actual[0]);
            Assert.AreEqual(typeof(FitnessBasedReinsertion), actual[1]);
            Assert.AreEqual(typeof(PureReinsertion), actual[2]);
            Assert.AreEqual(typeof(UniformReinsertion), actual[3]);
        }

        [Test()]
        public void GetReinsertionNames_NoArgs_AllAvailableReinsertionsNames()
        {
            var actual = ReinsertionService.GetReinsertionNames();

            Assert.AreEqual(4, actual.Count);
            Assert.AreEqual("Elitist", actual[0]);
            Assert.AreEqual("Fitness Based", actual[1]);
            Assert.AreEqual("Pure", actual[2]);
            Assert.AreEqual("Uniform", actual[3]);
        }

        [Test()]
        public void CreateReinsertionByName_InvalidName_Exception()
        {
            ExceptionAssert.IsThrowing(new ArgumentException("There is no IReinsertion implementation with name 'Test'.", "name"), () =>
            {
                ReinsertionService.CreateReinsertionByName("Test");
            });
        }

        [Test()]
        public void CreateReinsertionByName_ValidNameButInvalidConstructorArgs_Exception()
        {
            ExceptionAssert.IsThrowing(new ArgumentException("A IReinsertion's implementation with name 'Elitist' was found, but seems the constructor args were invalid.", "constructorArgs"), () =>
            {
                ReinsertionService.CreateReinsertionByName("Elitist", 1, 2, 3);
            });
        }

        [Test()]
        public void CreateReinsertionByName_ValidName_ReinsertionCreated()
        {
            IReinsertion actual = ReinsertionService.CreateReinsertionByName("Elitist") as ElitistReinsertion;
            Assert.IsNotNull(actual);

            actual = ReinsertionService.CreateReinsertionByName("Fitness Based") as FitnessBasedReinsertion;
            Assert.IsNotNull(actual);

            actual = ReinsertionService.CreateReinsertionByName("Pure") as PureReinsertion;
            Assert.IsNotNull(actual);

            actual = ReinsertionService.CreateReinsertionByName("Uniform") as UniformReinsertion;
            Assert.IsNotNull(actual);
        }

        [Test()]
        public void GetReinsertionTypeByName_InvalidName_Exception()
        {
            ExceptionAssert.IsThrowing(new ArgumentException("There is no IReinsertion implementation with name 'Test'.", "name"), () =>
            {
                ReinsertionService.GetReinsertionTypeByName("Test");
            });
        }

        [Test()]
        public void GetReinsertionTypeByName_ValidName_ReinsertionTpe()
        {
            var actual = ReinsertionService.GetReinsertionTypeByName("Elitist");
            Assert.AreEqual(typeof(ElitistReinsertion), actual);

            actual = ReinsertionService.GetReinsertionTypeByName("Fitness Based");
            Assert.AreEqual(typeof(FitnessBasedReinsertion), actual);

            actual = ReinsertionService.GetReinsertionTypeByName("Pure");
            Assert.AreEqual(typeof(PureReinsertion), actual);

            actual = ReinsertionService.GetReinsertionTypeByName("Uniform");
            Assert.AreEqual(typeof(UniformReinsertion), actual);
        }
    }
}