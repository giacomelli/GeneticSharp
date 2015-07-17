using System;
using System.Reflection;
using System.Runtime.Serialization;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Crossovers;
using NUnit.Framework;
using Rhino.Mocks;

namespace GeneticSharp.Domain.UnitTests.Chromosomes
{
    [TestFixture]
    public class CrossoverExceptionTest
    {
        [Test]
        public void Constructor_NoArgs_DefaultValue()
        {
            var target = new CrossoverException();
            Assert.IsTrue(target.Message.Contains("CrossoverException"));
        }

        [Test]
        public void Constructor_Message_Message()
        {
            var target = new CrossoverException("1");
            Assert.AreEqual("1", target.Message);
        }

        [Test]
        public void Constructor_MessageAndInnerException_MessageAndInnerExcetion()
        {
            var target = new CrossoverException("1", new Exception("2"));
            Assert.AreEqual("1", target.Message);
            Assert.AreEqual("2", target.InnerException.Message);
        }

        [Test]
        public void Constructor_CrossoverAndMessageAndInnerException_CrossoverAndMessageAndInnerExcetion()
        {
            var target = new CrossoverException(MockRepository.GenerateMock<ICrossover>(), "1", new Exception("2"));
            Assert.IsNotNull(target.Crossover);
            Assert.AreEqual(target.Crossover.GetType().Name + ": 1", target.Message);
            Assert.AreEqual("2", target.InnerException.Message);
        }

        [Test]
        public void Constructor_InfoAndContext_InfoAndContext()
        {
            var constructor = typeof(CrossoverException).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0];

            var serializationInfo = new SerializationInfo(typeof(int), MockRepository.GenerateMock<IFormatterConverter>());
            serializationInfo.AddValue("ClassName", "CrossoverException");
            serializationInfo.AddValue("Message", "1");
            serializationInfo.AddValue("InnerException", new Exception("2"));
            serializationInfo.AddValue("HelpURL", "");
            serializationInfo.AddValue("StackTraceString", "");
            serializationInfo.AddValue("RemoteStackTraceString", "");
            serializationInfo.AddValue("RemoteStackIndex", 1);
            serializationInfo.AddValue("ExceptionMethod", 1);
            serializationInfo.AddValue("HResult", 1);
            serializationInfo.AddValue("Source", 1);

            var target = constructor.Invoke(new object[] {
                serializationInfo,
                new StreamingContext() }) as CrossoverException;

            Assert.AreEqual("2", target.InnerException.Message);
        }

        [Test]
        public void GetObjectData_InfoAndContext_Property()
        {
            var propertyValue = MockRepository.GenerateMock<ICrossover>();
            var target = new CrossoverException(propertyValue, "1");
            var serializationInfo = new SerializationInfo(typeof(int), MockRepository.GenerateMock<IFormatterConverter>());
            target.GetObjectData(serializationInfo, new StreamingContext());

            Assert.AreEqual(propertyValue, serializationInfo.GetValue("Crossover", typeof(ICrossover)));
        }
    }
}