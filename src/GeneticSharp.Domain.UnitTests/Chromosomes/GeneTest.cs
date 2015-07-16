using GeneticSharp.Domain.Chromosomes;
using NUnit.Framework;

namespace GeneticSharp.Domain.UnitTests.Chromosomes
{
    [TestFixture]
    public class GeneTest
    {
        [Test]
        public void ToString_NoArgs_Value()
        {
            var target = new Gene();
            Assert.AreEqual("", target.ToString());

            target = new Gene(1);
            Assert.AreEqual("1", target.ToString());
        }

        [Test]
        public void Equals_OtherValue_False()
        {
            var target = new Gene(1);
            var other = new Gene(2);
            Assert.IsFalse(target.Equals(other));
        }

        [Test]
        public void Equals_SameValue_True()
        {
            var target = new Gene(1);
            var other = new Gene(1);
            Assert.IsTrue(target.Equals(other));
        }

        [Test]
        public void GetHashCode_NoValue_Zero()
        {
            var target = new Gene();
            Assert.AreEqual(0, target.GetHashCode());
        }

        [Test]
        public void OperatorEquals_SameInstance_True()
        {
            var target = new Gene(1);
            var other = target;
            Assert.IsTrue(target == other);
        }

        [Test]
        public void OperatorEquals_AnyNull_False()
        {
            var target = new Gene();
            var other = new Gene();
            Assert.IsFalse(null == other);
            Assert.IsFalse(target == null);
        }

        [Test]
        public void OperatorDiff_Diff_True()
        {
            var target = new Gene();
            var other = new Gene();
            Assert.IsTrue(null != other);
        }
    }
}