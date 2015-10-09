using System;
using GeneticSharp.Domain.Terminations;
using NUnit.Framework;
using TestSharp;

namespace GeneticSharp.Domain.UnitTests.Terminations
{
    [TestFixture()]
    [Category("Terminations")]
    public class TerminationServiceTest
    {
        [Test()]
        public void GetTerminationTypes_NoArgs_AllAvailableTerminations()
        {
            var actual = TerminationService.GetTerminationTypes();

            Assert.AreEqual(6, actual.Count);
            Assert.AreEqual(typeof(AndTermination), actual[0]);
            Assert.AreEqual(typeof(FitnessStagnationTermination), actual[1]);
            Assert.AreEqual(typeof(FitnessThresholdTermination), actual[2]);
            Assert.AreEqual(typeof(GenerationNumberTermination), actual[3]);
            Assert.AreEqual(typeof(OrTermination), actual[4]);
            Assert.AreEqual(typeof(TimeEvolvingTermination), actual[5]);
        }

        [Test()]
        public void GetTerminationNames_NoArgs_AllAvailableTerminationsNames()
        {
            var actual = TerminationService.GetTerminationNames();

            Assert.AreEqual(6, actual.Count);
            Assert.AreEqual("And", actual[0]);
            Assert.AreEqual("Fitness Stagnation", actual[1]);
            Assert.AreEqual("Fitness Threshold", actual[2]);
            Assert.AreEqual("Generation Number", actual[3]);
            Assert.AreEqual("Or", actual[4]);
            Assert.AreEqual("Time Evolving", actual[5]);

        }

        [Test()]
        public void CreateTerminationByName_InvalidName_Exception()
        {
            ExceptionAssert.IsThrowing(new ArgumentException("There is no ITermination implementation with name 'Test'.", "name"), () =>
            {
                TerminationService.CreateTerminationByName("Test");
            });
        }

        [Test()]
        public void CreateTerminationByName_ValidNameButInvalidConstructorArgs_Exception()
        {
            ExceptionAssert.IsThrowing(new ArgumentException("A ITermination's implementation with name 'Generation Number' was found, but seems the constructor args were invalid.", "constructorArgs"), () =>
            {
                TerminationService.CreateTerminationByName("Generation Number", 1f);
            });
        }

        [Test()]
        public void CreateTerminationByName_ValidName_TerminationCreated()
        {
            ITermination actual = TerminationService.CreateTerminationByName("Fitness Stagnation") as FitnessStagnationTermination;
            Assert.IsNotNull(actual);

            actual = TerminationService.CreateTerminationByName("Fitness Threshold") as FitnessThresholdTermination;
            Assert.IsNotNull(actual);

            actual = TerminationService.CreateTerminationByName("Generation Number") as GenerationNumberTermination;
            Assert.IsNotNull(actual);

            actual = TerminationService.CreateTerminationByName("Time Evolving") as TimeEvolvingTermination;
            Assert.IsNotNull(actual);
        }

        [Test()]
        public void GetTerminationTypeByName_InvalidName_Exception()
        {
            ExceptionAssert.IsThrowing(new ArgumentException("There is no ITermination implementation with name 'Test'.", "name"), () =>
            {
                TerminationService.GetTerminationTypeByName("Test");
            });
        }

        [Test()]
        public void GetTerminationTypeByName_ValidName_CrossoverTpe()
        {
            var actual = TerminationService.GetTerminationTypeByName("Generation Number");
            Assert.AreEqual(typeof(GenerationNumberTermination), actual);

            actual = TerminationService.GetTerminationTypeByName("Time Evolving");
            Assert.AreEqual(typeof(TimeEvolvingTermination), actual);

            actual = TerminationService.GetTerminationTypeByName("Fitness Threshold");
            Assert.AreEqual(typeof(FitnessThresholdTermination), actual);

            actual = TerminationService.GetTerminationTypeByName("Fitness Stagnation");
            Assert.AreEqual(typeof(FitnessStagnationTermination), actual);
        }
    }
}