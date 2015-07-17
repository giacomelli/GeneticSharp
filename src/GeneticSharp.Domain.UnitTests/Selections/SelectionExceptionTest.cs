using System;
using System.Reflection;
using System.Runtime.Serialization;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Selections;
using NUnit.Framework;
using Rhino.Mocks;

namespace GeneticSharp.Domain.UnitTests.Chromosomes
{
    [TestFixture]
    public class SelectionExceptionTest
    {
        [Test]
        public void Constructor_NoArgs_DefaultValue()
        {
            var target = new SelectionException();
            Assert.IsTrue(target.Message.Contains("SelectionException"));
        }

        [Test]
        public void Constructor_Message_Message()
        {
            var target = new SelectionException("1");
            Assert.AreEqual("1", target.Message);
        }

        [Test]
        public void Constructor_MessageAndInnerException_MessageAndInnerExcetion()
        {
            var target = new SelectionException("1", new Exception("2"));
            Assert.AreEqual("1", target.Message);
            Assert.AreEqual("2", target.InnerException.Message);
        }

        [Test]
        public void Constructor_SelectionAndMessageAndInnerException_SelectionAndMessageAndInnerExcetion()
        {
            var target = new SelectionException(MockRepository.GenerateMock<ISelection>(), "1", new Exception("2"));
            Assert.IsNotNull(target.Selection);
            Assert.AreEqual(target.Selection.GetType().Name + ": 1", target.Message);
            Assert.AreEqual("2", target.InnerException.Message);
        }

        [Test]
        public void Constructor_InfoAndContext_InfoAndContext()
        {
            var constructor = typeof(SelectionException).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0];

            var serializationInfo = new SerializationInfo(typeof(int), MockRepository.GenerateMock<IFormatterConverter>());
            serializationInfo.AddValue("ClassName", "SelectionException");
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
                new StreamingContext() }) as SelectionException;

            Assert.AreEqual("2", target.InnerException.Message);
        }

        [Test]
        public void GetObjectData_InfoAndContext_Property()
        {
            var propertyValue = MockRepository.GenerateMock<ISelection>();
            var target = new SelectionException(propertyValue, "1");
            var serializationInfo = new SerializationInfo(typeof(int), MockRepository.GenerateMock<IFormatterConverter>());
            target.GetObjectData(serializationInfo, new StreamingContext());

            Assert.AreEqual(propertyValue, serializationInfo.GetValue("Selection", typeof(ISelection)));
        }
    }
}