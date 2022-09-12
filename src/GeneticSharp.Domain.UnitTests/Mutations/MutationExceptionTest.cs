using System;
using System.Reflection;
using System.Runtime.Serialization;
using NUnit.Framework;
using NSubstitute;

namespace GeneticSharp.Domain.UnitTests.Mutations
{
    [TestFixture]
    [Category("Mutations")]
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
        public void Constructor_MutationAndMessage_MutationAndMessage([Values] bool nullMutation)
        {
            var target = new MutationException(nullMutation ? null : Substitute.For<IMutation>(), "1");
            Assert.AreEqual(nullMutation, target.Mutation == null);
            Assert.AreEqual(nullMutation ? ": 1" : $"{target.Mutation.GetType().Name}: 1", target.Message);
            Assert.IsNull(target.InnerException);
        }

        [Test]   
        public void Constructor_MutationAndMessageAndInnerException_MutationAndMessageAndInnerExcetion([Values] bool nullMutation)
        {
            var target = new MutationException(nullMutation ? null : Substitute.For<IMutation>(), "1", new Exception("2"));
            Assert.AreEqual(nullMutation, target.Mutation == null);
            Assert.AreEqual(nullMutation ? ": 1" : $"{target.Mutation.GetType().Name}: 1", target.Message);
            Assert.AreEqual("2", target.InnerException.Message);
        }

        [Test]
        public void Constructor_InfoAndContext_InfoAndContext()
        {
            var constructor = typeof(MutationException).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0];

            var serializationInfo = new SerializationInfo(typeof(int), Substitute.For<IFormatterConverter>());
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
            var propertyValue = Substitute.For<IMutation>();
            var target = new MutationException(propertyValue, "1");
            var serializationInfo = new SerializationInfo(typeof(int), Substitute.For<IFormatterConverter>());
            target.GetObjectData(serializationInfo, new StreamingContext());

            Assert.AreEqual(propertyValue, serializationInfo.GetValue("Mutation", typeof(IMutation)));
        }
    }
}