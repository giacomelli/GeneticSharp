using System;
using GeneticSharp.Domain.Terminations;
using NUnit.Framework;
using Rhino.Mocks;
using TestSharp;

namespace GeneticSharp.Domain.UnitTests.Terminations
{
    [TestFixture]
    [Category("Terminations")]
    public class OrTerminationTest
    {
        [Test()]
        public void AddTermination_Null_Exception()
        {
            var target = new OrTermination();

            ExceptionAssert.IsThrowing(new ArgumentNullException("termination"), () =>
            {
                target.AddTermination(null);
            });
        }

        [Test()]
        public void HasReached_LessThan2Terminations_Exception()
        {
            var target = new OrTermination();
            target.AddTermination(MockRepository.GenerateMock<ITermination>());

            ExceptionAssert.IsThrowing(new InvalidOperationException("The OrTermination needs at least 2 terminations to perform. Please, add the missing terminations."), () =>
            {
                target.HasReached(MockRepository.GenerateMock<IGeneticAlgorithm>());
            });
        }

        [Test()]
        public void HasReached_AllTerminationsHasNotReached_False()
        {
            var target = new OrTermination();
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

        [Test()]
        public void HasReached_OnlyOneTerminationsHasReached_True()
        {
            var target = new OrTermination();
            var ga = MockRepository.GenerateMock<IGeneticAlgorithm>();

            var t1 = MockRepository.GenerateMock<ITermination>();
            t1.Expect(t => t.HasReached(ga)).IgnoreArguments().Return(false);
            target.AddTermination(t1);

            var t2 = MockRepository.GenerateMock<ITermination>();
            t2.Expect(t => t.HasReached(ga)).IgnoreArguments().Return(true);
            target.AddTermination(t2);

            var t3 = MockRepository.GenerateMock<ITermination>();
            t3.Expect(t => t.HasReached(ga)).IgnoreArguments().Return(false);
            target.AddTermination(t3);

            Assert.IsTrue(target.HasReached(ga));
        }
    }
}