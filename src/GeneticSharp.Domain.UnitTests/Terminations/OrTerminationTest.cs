using System;
using GeneticSharp.Domain.Terminations;
using NUnit.Framework;
using NSubstitute;

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
            var actual = Assert.Catch<ArgumentNullException>(() =>
            {
                target.AddTermination(null);
            });

            Assert.AreEqual("termination", actual.ParamName);
        }

        [Test()]
        public void HasReached_LessThan2Terminations_Exception()
        {
            var target = new OrTermination();
            target.AddTermination(Substitute.For<ITermination>());

            Assert.Catch<InvalidOperationException>(() =>
            {
                target.HasReached(Substitute.For<IGeneticAlgorithm>());
            }, "The OrTermination needs at least 2 terminations to perform. Please, add the missing terminations.");
        }

        [Test()]
        public void HasReached_AllTerminationsHasNotReached_False()
        {
            var target = new OrTermination();
            var ga = Substitute.For<IGeneticAlgorithm>();

            var t1 = Substitute.For<ITermination>();
            t1.HasReached(ga).ReturnsForAnyArgs(false);
            target.AddTermination(t1);

            var t2 = Substitute.For<ITermination>();
            t2.HasReached(ga).ReturnsForAnyArgs(false);
            target.AddTermination(t2);

            var t3 = Substitute.For<ITermination>();
            t3.HasReached(ga).ReturnsForAnyArgs(false);
            target.AddTermination(t3);

            Assert.IsFalse(target.HasReached(ga));
        }

        [Test()]
        public void HasReached_OnlyOneTerminationsHasReached_True()
        {
            var target = new OrTermination();
            var ga = Substitute.For<IGeneticAlgorithm>();

            var t1 = Substitute.For<ITermination>();
            t1.HasReached(ga).ReturnsForAnyArgs(false);
            target.AddTermination(t1);

            var t2 = Substitute.For<ITermination>();
            t2.HasReached(ga).ReturnsForAnyArgs(true);
            target.AddTermination(t2);

            var t3 = Substitute.For<ITermination>();
            t3.HasReached(ga).ReturnsForAnyArgs(false);
            target.AddTermination(t3);

            Assert.IsTrue(target.HasReached(ga));
        }
    }
}