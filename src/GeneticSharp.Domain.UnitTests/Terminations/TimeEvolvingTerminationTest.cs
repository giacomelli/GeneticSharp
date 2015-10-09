using System;
using GeneticSharp.Domain.Terminations;
using NUnit.Framework;
using Rhino.Mocks;

namespace GeneticSharp.Domain.UnitTests.Terminations
{
    [TestFixture()]
    [Category("Terminations")]
    public class TimeEvolvingTerminationTest
    {
        [Test()]
        public void HasReached_TimeLowerThanMaxTime_False()
        {
            var repository = new MockRepository();
            var ga = repository.StrictMock<IGeneticAlgorithm>();

            using (repository.Ordered())
            {
                ga.Expect(g => g.TimeEvolving).Return(TimeSpan.FromSeconds(0.1));
                ga.Expect(g => g.TimeEvolving).Return(TimeSpan.FromSeconds(0.9));
            }
            repository.ReplayAll();

            var target = new TimeEvolvingTermination(TimeSpan.FromSeconds(1));
            Assert.IsFalse(target.HasReached(ga));
            Assert.IsFalse(target.HasReached(ga));
        }

        [Test()]
        public void HasReached_TimeGreaterOrEqualMaxTime_True()
        {
            var repository = new MockRepository();
            var ga = repository.StrictMock<IGeneticAlgorithm>();

            using (repository.Ordered())
            {
                ga.Expect(g => g.TimeEvolving).Return(TimeSpan.FromSeconds(0.1));
                ga.Expect(g => g.TimeEvolving).Return(TimeSpan.FromSeconds(0.9));
                ga.Expect(g => g.TimeEvolving).Return(TimeSpan.FromSeconds(1));
                ga.Expect(g => g.TimeEvolving).Return(TimeSpan.FromSeconds(1.1));
            }
            repository.ReplayAll();

            var target = new TimeEvolvingTermination(TimeSpan.FromSeconds(1));

            Assert.IsFalse(target.HasReached(ga));
            Assert.IsFalse(target.HasReached(ga));
            Assert.IsTrue(target.HasReached(ga));
            Assert.IsTrue(target.HasReached(ga));
        }
    }
}

