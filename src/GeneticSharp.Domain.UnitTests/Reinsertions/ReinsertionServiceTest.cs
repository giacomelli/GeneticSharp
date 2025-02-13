﻿using System;
using GeneticSharp.Domain.LifeSpans;
using NUnit.Framework;

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

            Assert.AreEqual(5, actual.Count);
            Assert.AreEqual(typeof(ElitistReinsertion), actual[0]);
            Assert.AreEqual(typeof(FitnessBasedReinsertion), actual[1]);
            Assert.AreEqual(typeof(LifespanReinsertionDecorator), actual[2]);
            Assert.AreEqual(typeof(PureReinsertion), actual[3]);
            Assert.AreEqual(typeof(UniformReinsertion), actual[4]);
        }

        [Test()]
        public void GetReinsertionNames_NoArgs_AllAvailableReinsertionsNames()
        {
            var actual = ReinsertionService.GetReinsertionNames();

            Assert.AreEqual(5, actual.Count);
            Assert.AreEqual("Elitist", actual[0]);
            Assert.AreEqual("Fitness Based", actual[1]);
            Assert.AreEqual("LifespanReinsertion", actual[2]);
            Assert.AreEqual("Pure", actual[3]);
            Assert.AreEqual("Uniform", actual[4]);
            
        }

        [Test()]
        public void CreateReinsertionByName_InvalidName_Exception()
        {
            Assert.Catch<ArgumentException>(() =>
            {
                ReinsertionService.CreateReinsertionByName("Test");
            }, "There is no IReinsertion implementation with name 'Test'.");
        }

        [Test()]
        public void CreateReinsertionByName_ValidNameButInvalidConstructorArgs_Exception()
        {
            Assert.Catch<ArgumentException>(() =>
            {
                ReinsertionService.CreateReinsertionByName("Elitist", 1, 2, 3);
            }, "A IReinsertion's implementation with name 'Elitist' was found, but seems the constructor args were invalid.");
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
            Assert.Catch<ArgumentException>(() =>
            {
                ReinsertionService.GetReinsertionTypeByName("Test");
            }, "There is no IReinsertion implementation with name 'Test'.");
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