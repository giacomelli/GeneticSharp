using System;
using System.Reflection;
using System.Runtime.Serialization;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Mutations;
using NUnit.Framework;
using Rhino.Mocks;

namespace GeneticSharp.Domain.UnitTests.Chromosomes
{
    [TestFixture]
    public class MutationExceptionTest
    {
        [Test]
        public void Constructor_NoArgs_DefaultValue()
        {
            var target = new MutationException();
            Assert.IsTrue(target.Message.Contains("MutationException"));
        }

        [Test]
        public void Constructor_Message_Message()
        {
            var target = new MutationException("1");
            Assert.AreEqual("1", target.Message);
        }

        [Test]
        public void Constructor_MessageAndInnerException_MessageAndInnerExcetion()
        {
            var target = new MutationException("1", new Exception("2"));
            Assert.AreEqual("1", target.Message);
            Assert.AreEqual("2", target.InnerException.Message);
        }

        [Test]
        public void Constructor_MutationAndMessageAndInnerException_MutationAndMessageAndInnerExcetion()
        {
            var target = new MutationException(MockRepository.GenerateMock<IMutation>(), "1", new Exception("2"));
            Assert.IsNotNull(target.Mutation);
            Assert.AreEqual(target.Mutation.GetType().Name + ": 1", target.Message);
            Assert.AreEqual("2", target.InnerException.Message);
        }

        [Test]
        public void Constructor_InfoAndContext_InfoAndContext()
        {
            var constructor = typeof(MutationException).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0];

            var serializationInfo = new SerializationInfo(typeof(int), MockRepository.GenerateMock<IFormatterConverter>());
            serializationInfo.AddValue("ClassName", "MutationException");
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
                new StreamingContext() }) as MutationException;

            Assert.AreEqual("2", target.InnerException.Message);
        }

        [Test]
        public void GetObjectData_InfoAndContext_Property()
        {
            var propertyValue = MockRepository.GenerateMock<IMutation>();
            var target = new MutationException(propertyValue, "1");
            var serializationInfo = new SerializationInfo(typeof(int), MockRepository.GenerateMock<IFormatterConverter>());
            target.GetObjectData(serializationInfo, new StreamingContext());

            Assert.AreEqual(propertyValue, serializationInfo.GetValue("Mutation", typeof(IMutation)));
        }
    }
}