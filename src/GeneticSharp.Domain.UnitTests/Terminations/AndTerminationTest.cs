using System;
using GeneticSharp.Domain.Terminations;
using NUnit.Framework;
using NSubstitute;

namespace GeneticSharp.Domain.UnitTests.Terminations
{
    [TestFixture]
    [Category("Terminations")]
    public class AndTerminationTest
    {
        [Test]
        public void Constructors_Terminations_Added()
        {
            var ga = Substitute.For<IGeneticAlgorithm>();
            var t1 = Substitute.For<ITermination>();
            t1.HasReached(ga).ReturnsForAnyArgs(true);

            var t2 = Substitute.For<ITermination>();
            t2.HasReached(ga).ReturnsForAnyArgs(true);

            var t3 = Substitute.For<ITermination>();
            t3.HasReached(ga).ReturnsForAnyArgs(false);

            var target = new AndTermination(t1, t2, t3);

            Assert.IsFalse(target.HasReached(ga));
        }

        [Test]
        public void AddTermination_Null_Exception()
        {
            var target = new AndTermination();

            Assert.Catch<ArgumentNullException>(() =>
            {
                target.AddTermination(null);
            });
        }

        [Test]
        public void HasReached_LessThan2Terminations_Exception()
        {
            var target = new AndTermination();
            target.AddTermination(Substitute.For<ITermination>());

            Assert.Catch<InvalidOperationException>(() =>
            {
                target.HasReached(Substitute.For<IGeneticAlgorithm>());
            }, "The AndTermination needs at least 2 terminations to perform. Please, add the missing terminations.");
        }

        [Test]
        public void HasReached_AllTerminationsHasNotReached_False()
        {
            var target = new AndTermination();
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

        [Test]
        public void HasReached_OnlyOneTerminationsHasNotReached_False()
        {
            var target = new AndTermination();
            var ga = Substitute.For<IGeneticAlgorithm>();

            var t1 = Substitute.For<ITermination>();
            t1.HasReached(ga).ReturnsForAnyArgs(true);
            target.AddTermination(t1);

            var t2 = Substitute.For<ITermination>();
            t2.HasReached(ga).ReturnsForAnyArgs(true);
            target.AddTermination(t2);

            var t3 = Substitute.For<ITermination>();
            t3.HasReached(ga).ReturnsForAnyArgs(false);
            target.AddTermination(t3);

            Assert.IsFalse(target.HasReached(ga));
        }

        [Test]
        public void HasReached_AllTerminationsHasReached_True()
        {
            var target = new AndTermination();
            var ga = Substitute.For<IGeneticAlgorithm>();

            var t1 = Substitute.For<ITermination>();
            t1.HasReached(ga).ReturnsForAnyArgs(true);
            target.AddTermination(t1);

            var t2 = Substitute.For<ITermination>();
            t2.HasReached(ga).ReturnsForAnyArgs(true);
            target.AddTermination(t2);

            var t3 = Substitute.For<ITermination>();
            t3.HasReached(ga).ReturnsForAnyArgs(true);
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