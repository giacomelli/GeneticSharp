using System;
using GeneticSharp.Domain.Terminations;
using NUnit.Framework;
using Rhino.Mocks;
using TestSharp;

namespace GeneticSharp.Domain.UnitTests.Terminations
{
    [TestFixture]
    [Category("Terminations")]
    public class AndTerminationTest
    {
        [Test]
        public void Constructors_Terminations_Added()
        {
            var ga = MockRepository.GenerateMock<IGeneticAlgorithm>();
            var t1 = MockRepository.GenerateMock<ITermination>();
            t1.Expect(t => t.HasReached(ga)).IgnoreArguments().Return(true);

            var t2 = MockRepository.GenerateMock<ITermination>();
            t2.Expect(t => t.HasReached(ga)).IgnoreArguments().Return(true);

            var t3 = MockRepository.GenerateMock<ITermination>();
            t3.Expect(t => t.HasReached(ga)).IgnoreArguments().Return(false);

            var target = new AndTermination(t1, t2, t3);

            Assert.IsFalse(target.HasReached(ga));
        }

        [Test]
        public void AddTermination_Null_Exception()
        {
            var target = new AndTermination();

            ExceptionAssert.IsThrowing(new ArgumentNullException("termination"), () =>
            {
                target.AddTermination(null);
            });
        }

        [Test]
        public void HasReached_LessThan2Terminations_Exception()
        {
            var target = new AndTermination();
            target.AddTermination(MockRepository.GenerateMock<ITermination>());

            ExceptionAssert.IsThrowing(new InvalidOperationException("The AndTermination needs at least 2 terminations to perform. Please, add the missing terminations."), () =>
            {
                target.HasReached(MockRepository.GenerateMock<IGeneticAlgorithm>());
            });
        }

        [Test]
        public void HasReached_AllTerminationsHasNotReached_False()
        {
            var target = new AndTermination();
            var ga = MockRepository.GenerateMock<IGeneticAlgorithm>();

            var t1 = MockRepository.GenerateMock<ITermination>();
            t1.Expect(t => t.HasReached(ga)).IgnoreArguments().Return(false);
            target.AddTermination(t1);

            var t2 = MockRepository.GenerateMock<ITermination>();
            t2.Expect(t => t.HasReached(ga)).IgnoreArguments().Return(false);
            target.AddTermination(t2);

            var t3 = MockRepository.GenerateMock<ITermination>();
            t3.Expect(t => t.HasReached(ga)).IgnoreArguments().Return(false);
            target.AddTermination(t3);

            Assert.IsFalse(target.HasReached(ga));
        }

        [Test]
        public void HasReached_OnlyOneTerminationsHasNotReached_False()
        {
            var target = new AndTermination();
            var ga = MockRepository.GenerateMock<IGeneticAlgorithm>();

            var t1 = MockRepository.GenerateMock<ITermination>();
            t1.Expect(t => t.HasReached(ga)).IgnoreArguments().Return(true);
            target.AddTermination(t1);

            var t2 = MockRepository.GenerateMock<ITermination>();
            t2.Expect(t => t.HasReached(ga)).IgnoreArguments().Return(true);
            target.AddTermination(t2);

            var t3 = MockRepository.GenerateMock<ITermination>();
            t3.Expect(t => t.HasReached(ga)).IgnoreArguments().Return(false);
            target.AddTermination(t3);

            Assert.IsFalse(target.HasReached(ga));
        }

        [Test]
        public void HasReached_AllTerminationsHasReached_True()
        {
            var target = new AndTermination();
            var ga = MockRepository.GenerateMock<IGeneticAlgorithm>();

            var t1 = MockRepository.GenerateMock<ITermination>();
            t1.Expect(t => t.HasReached(ga)).IgnoreArguments().Return(true);
            target.AddTermination(t1);

            var t2 = MockRepository.GenerateMock<ITermination>();
            t2.Expect(t => t.HasReached(ga)).IgnoreArguments().Return(true);
            target.AddTermination(t2);

            var t3 = MockRepository.GenerateMock<ITermination>();
            t3.Expect(t => t.HasReached(ga)).IgnoreArguments().Return(true);
            target.AddTermination(t3);

            Assert.IsTrue(target.HasReached(ga));
        }

        [Test]
        public void ToString_NoArgs_State()
        {
            var t1 = new AndTermination();
            var t2 = new OrTermination();
            var t3 = new AndTermination();

            var target = new AndTermination(t1, t2, t3);

            Assert.AreEqual("AndTermination (AndTermination (), OrTermination (), AndTermination ())", target.ToString());
        }
    }
}