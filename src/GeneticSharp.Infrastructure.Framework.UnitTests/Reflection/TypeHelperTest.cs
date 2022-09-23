using System;
using NUnit.Framework;
using GeneticSharp;

namespace GeneticSharp.Infrastructure.Framework.UnitTests.Reflection
{
    [TestFixture()]
    [Category("Infrastructure")]
    public class TypeHelperTest
    {
        [Test()]
        public void GetDisplayNamesByInterface_ThereIsTypeWithoutDisplayNameAttribute_Exception()
        {
            Assert.Catch<InvalidOperationException>(() =>
            {
                TypeHelper.GetDisplayNamesByInterface<IRandomization>();
            }, "The member 'BasicRandomization' has no DisplayNameAttribute.");
        }

        [Test()]
        public void GetTypesByInterface_InterfaceType_Types()
        {
            var actual = TypeHelper.GetTypesByInterface<IGenerationStrategy>();
            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(typeof(PerformanceGenerationStrategy), actual[0]);
            Assert.AreEqual(typeof(TrackingGenerationStrategy), actual[1]);
        }

        [Test()]
        public void GetTypesByInterface_Name_Types()
        {
            var actual  = TypeHelper.GetTypesByInterface("IGenerationStrategy");
            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(typeof(PerformanceGenerationStrategy), actual[0]);
            Assert.AreEqual(typeof(TrackingGenerationStrategy), actual[1]);
        }

        [Test()]
        public void GetTypeByName_InterfaceTypeAndImplementationName_Type()
        {
            var actual = TypeHelper.GetTypeByName<IGenerationStrategy>("PerformanceGenerationStrategy");
            Assert.AreEqual(typeof(PerformanceGenerationStrategy), actual);
        }

        [Test()]
        public void GetTypeByName_InterfaceAndImplementationName_Type()
        {
            var actual = TypeHelper.GetTypeByName("IGenerationStrategy", "Performance");
            Assert.AreEqual(typeof(PerformanceGenerationStrategy), actual);
        }
    }
}

