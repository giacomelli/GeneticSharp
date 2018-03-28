using System;
using GeneticSharp.Domain.Terminations;
using NUnit.Framework;
using NSubstitute;

namespace GeneticSharp.Domain.UnitTests.Terminations
{
    [TestFixture()]
    [Category("Terminations")]
    public class TimeEvolvingTerminationTest
    {
        [Test()]
        public void HasReached_TimeLowerThanMaxTime_False()
        {
            var ga = Substitute.For<IGeneticAlgorithm>();

            ga.TimeEvolving.Returns(TimeSpan.FromSeconds(0.1),
                                    TimeSpan.FromSeconds(00.9));

            var target = new TimeEvolvingTermination(TimeSpan.FromSeconds(1));
            Assert.IsFalse(target.HasReached(ga));
            Assert.IsFalse(target.HasReached(ga));
        }

        [Test()]
        public void HasReached_TimeGreaterOrEqualMaxTime_True()
        {
            var ga = Substitute.For<IGeneticAlgorithm>();

            ga.TimeEvolving.Returns(TimeSpan.FromSeconds(0.1),
                                    TimeSpan.FromSeconds(0.9),
                                    TimeSpan.FromSeconds(1),
                                    TimeSpan.FromSeconds(1.1));

            var target = new TimeEvolvingTermination(TimeSpan.FromSeconds(1));

            Assert.IsFalse(target.HasReached(ga));
            Assert.IsFalse(target.HasReached(ga));
            Assert.IsTrue(target.HasReached(ga));
            Assert.IsTrue(target.HasReached(ga));
        }
    }
}

