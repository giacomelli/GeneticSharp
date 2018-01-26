using System;
using GeneticSharp.Domain.Mutations;
using NUnit.Framework;
using TestSharp;

namespace GeneticSharp.Domain.UnitTests.Mutations
{
    [TestFixture()]
    [Category("Mutations")]
    public class MutationServiceTest
    {
        [Test()]
        public void GetMutationTypes_NoArgs_AllAvailableMutations()
        {
            var actual = MutationService.GetMutationTypes();

            Assert.AreEqual(7, actual.Count);
            Assert.AreEqual(typeof(DisplacementMutation), actual[0]);
            Assert.AreEqual(typeof(FlipBitMutation), actual[1]);
            Assert.AreEqual(typeof(InsertionMutation), actual[2]);
            Assert.AreEqual(typeof(PartialShuffleMutation), actual[3]);
            Assert.AreEqual(typeof(ReverseSequenceMutation), actual[4]);
            Assert.AreEqual(typeof(TworsMutation), actual[5]);
            Assert.AreEqual(typeof(UniformMutation), actual[6]);
        }

        [Test()]
        public void GetMutationNames_NoArgs_AllAvailableMutationsNames()
        {
            var actual = MutationService.GetMutationNames();

            Assert.AreEqual(7, actual.Count);
            Assert.AreEqual("Displacement", actual[0]);
            Assert.AreEqual("Flip Bit", actual[1]);
            Assert.AreEqual("Insertion", actual[2]);
            Assert.AreEqual("Partial Shuffle (PSM)", actual[3]);
            Assert.AreEqual("Reverse Sequence (RSM)", actual[4]);
            Assert.AreEqual("Twors", actual[5]);
            Assert.AreEqual("Uniform", actual[6]);
        }

        [Test()]
        public void CreateMutationByName_InvalidName_Exception()
        {
            ExceptionAssert.IsThrowing(new ArgumentException("There is no IMutation implementation with name 'Test'.", "name"), () =>
            {
                MutationService.CreateMutationByName("Test");
            });
        }

        [Test()]
        public void CreateMutationByName_ValidNameButInvalidConstructorArgs_Exception()
        {
            ExceptionAssert.IsThrowing(new ArgumentException("A IMutation's implementation with name 'Uniform' was found, but seems the constructor args were invalid.", "constructorArgs"), () =>
            {
                MutationService.CreateMutationByName("Uniform", 1f);
            });
        }

        [Test()]
        public void CreateMutationByName_ValidName_MutationCreated()
        {
            IMutation actual = MutationService.CreateMutationByName("Reverse Sequence (RSM)") as ReverseSequenceMutation;
            Assert.IsNotNull(actual);

            actual = MutationService.CreateMutationByName("Twors") as TworsMutation;
            Assert.IsNotNull(actual);

            actual = MutationService.CreateMutationByName("Uniform", true) as UniformMutation;
            Assert.IsNotNull(actual);
        }

        [Test()]
        public void GetMutationTypeByName_InvalidName_Exception()
        {
            ExceptionAssert.IsThrowing(new ArgumentException("There is no IMutation implementation with name 'Test'.", "name"), () =>
            {
                MutationService.GetMutationTypeByName("Test");
            });
        }

        [Test()]
        public void GetMutationTypeByName_ValidName_CrossoverTpe()
        {
            var actual = MutationService.GetMutationTypeByName("Reverse Sequence (RSM)");
            Assert.AreEqual(typeof(ReverseSequenceMutation), actual);

            actual = MutationService.GetMutationTypeByName("Twors");
            Assert.AreEqual(typeof(TworsMutation), actual);

            actual = MutationService.GetMutationTypeByName("Uniform");
            Assert.AreEqual(typeof(UniformMutation), actual);
        }
    }
}