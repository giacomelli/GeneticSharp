using System;
using System.Reflection;
using System.Runtime.Serialization;
using GeneticSharp.Domain.Fitnesses;
using NUnit.Framework;
using Rhino.Mocks;

namespace GeneticSharp.Domain.UnitTests.Chromosomes
{
    [TestFixture]
    public class FitnessExceptionTest
    {
        [Test]
        public void Constructor_NoArgs_DefaultValue()
        {
            var target = new FitnessException();
            Assert.IsTrue(target.Message.Contains("FitnessException"));
        }

        [Test]
        public void Constructor_Message_Message()
        {
            var target = new FitnessException("1");
            Assert.AreEqual("1", target.Message);
        }

        [Test]
        public void Constructor_MessageAndInnerException_MessageAndInnerExcetion()
        {
            var target = new FitnessException("1", new Exception("2"));
            Assert.AreEqual("1", target.Message);
            Assert.AreEqual("2", target.InnerException.Message);
        }

        [Test]
        public void Constructor_FitnessAndMessageAndInnerException_FitnessAndMessageAndInnerExcetion()
        {
            var target = new FitnessException(MockRepository.GenerateMock<IFitness>(), "1", new Exception("2"));
            Assert.IsNotNull(target.Fitness);
            Assert.AreEqual(target.Fitness.GetType().Name + ": 1", target.Message);
            Assert.AreEqual("2", target.InnerException.Message);
        }

        [Test]
        public void Constructor_InfoAndContext_InfoAndContext()
        {
            var constructor = typeof(FitnessException).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0];

            var serializationInfo = new SerializationInfo(typeof(int), MockRepository.GenerateMock<IFormatterConverter>());
            serializationInfo.AddValue("ClassName", "FitnessException");
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
                new StreamingContext() }) as FitnessException;

            Assert.AreEqual("2", target.InnerException.Message);
        }

        [Test]
        public void GetObjectData_InfoAndContext_Property()
        {
            var fitness = new FitnessStub();
            var target = new FitnessException(fitness, "1");
            var serializationInfo = new SerializationInfo(typeof(int), MockRepository.GenerateMock<IFormatterConverter>());
            target.GetObjectData(serializationInfo, new StreamingContext());

            Assert.AreEqual(fitness, serializationInfo.GetValue("Fitness", typeof(IFitness)));
        }
    }
}