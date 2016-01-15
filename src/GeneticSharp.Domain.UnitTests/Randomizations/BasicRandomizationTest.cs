using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using GeneticSharp.Domain.Randomizations;
using NUnit.Framework;
using TestSharp;

namespace GeneticSharp.Domain.UnitTests.Randomizations
{
    [TestFixture()]
    [Category("Randomizations")]
    public class BasicRandomizationTest
    {
        [Test]
        public void GetFloat_Range_RandomInRangeResult()
        {
            var target = new BasicRandomization();

            FlowAssert.IsAtLeastOneAttemptOk(100, () =>
            {
                Assert.IsTrue(target.GetFloat(0, 2.2f) < 1);
            });

            FlowAssert.IsAtLeastOneAttemptOk(1000, () =>
            {
                Assert.IsTrue(target.GetFloat(0, 2.2f) > 2.1);
            });

            for (int i = 0; i < 100; i++)
            {
                Assert.AreNotEqual(2.3, target.GetFloat(0, 2.2f));
            }
        }

        [Test]
        public void GetFloat_NoArgs_RandomResult()
        {
            var target = new BasicRandomization();

            Assert.AreNotEqual(target.GetFloat(), target.GetFloat());
        }

        [Test]
        public void GetDouble_Range_RandomInRangeResult()
        {
            var target = new BasicRandomization();

            FlowAssert.IsAtLeastOneAttemptOk(100, () =>
            {
                Assert.IsTrue(target.GetDouble(0, 2.2) < 1);
            });

            FlowAssert.IsAtLeastOneAttemptOk(1000, () =>
            {
                Assert.IsTrue(target.GetDouble(0, 2.2) > 2.1);
            });

            for (int i = 0; i < 100; i++)
            {
                Assert.AreNotEqual(2.3, target.GetDouble(0, 2.2));
            }
        }

        [Test]
        public void GetDouble_NoArgs_RandomResult()
        {
            var target = new BasicRandomization();

            Assert.AreNotEqual(target.GetDouble(), target.GetDouble());
        }

        [Test]
        public void GetDouble_ManyThreads_DiffRandomResult()
        {
            var target = new BasicRandomization();
            var actual = new BlockingCollection<int>();

            Parallel.For(0, 1000, (i) =>
            {
                actual.Add(target.GetInt(0, int.MaxValue));
            });

            Assert.AreEqual(1000, actual.Count);
            Assert.AreEqual(1000, actual.Distinct().Count());
        }

        [Test]
        public void GetInt_Range_RandomInRangeResult()
        {
            var target = new BasicRandomization();

            FlowAssert.IsAtLeastOneAttemptOk(100, () =>
            {
                Assert.AreEqual(0, target.GetInt(0, 2));
            });

            FlowAssert.IsAtLeastOneAttemptOk(100, () =>
            {
                Assert.AreEqual(1, target.GetInt(0, 2));
            });

            for (int i = 0; i < 100; i++)
            {
                Assert.AreNotEqual(2, target.GetInt(0, 2));
            }
        }

        [Test]
        public void GetInts_Length_ArrayWithLength()
        {
            var target = new BasicRandomization();
            var actual = target.GetInts(1, 0, 10);
            Assert.AreEqual(1, actual.Length);
            Assert.IsTrue(actual[0] >= 0 && actual[0] < 10);

            actual = target.GetInts(2, 0, 10);
            Assert.AreEqual(2, actual.Length);
            Assert.IsTrue(actual[0] >= 0 && actual[0] < 10);
            Assert.IsTrue(actual[1] >= 0 && actual[1] < 10);

            actual = target.GetInts(3, 0, 10);
            Assert.AreEqual(3, actual.Length);
            Assert.IsTrue(actual[0] >= 0 && actual[0] < 10);
            Assert.IsTrue(actual[1] >= 0 && actual[1] < 10);
            Assert.IsTrue(actual[2] >= 0 && actual[2] < 10);
        }


        [Test]
        public void GetUniqueInts_RangeLowerThanLength_Exception()
        {
            var target = new BasicRandomization();

            ExceptionAssert.IsThrowing(new ArgumentOutOfRangeException("length", "The length is 5, but the possible unique values between 0 (inclusive) and 4 (exclusive) are 4."), () =>
            {
                target.GetUniqueInts(5, 0, 4);
            });


        }

        [Test]
        public void GetUniqueInts_Length_ArrayWithUniqueInts()
        {
            var target = new BasicRandomization();
            var actual = target.GetUniqueInts(10, 0, 10);
            Assert.AreEqual(10, actual.Length);
            Assert.AreEqual(10, actual.Distinct().Count());

            for (int i = 0; i < 10; i++)
            {
                Assert.IsTrue(actual[i] >= 0 && actual[i] < 10);
            }

            actual = target.GetUniqueInts(10, 10, 20);
            Assert.AreEqual(10, actual.Length);
            Assert.AreEqual(10, actual.Distinct().Count());

            for (int i = 0; i < 10; i++)
            {
                Assert.IsTrue(actual[i] >= 10 && actual[i] < 20);
            }

            actual = target.GetUniqueInts(2, 0, 20);
            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual(2, actual.Distinct().Count());

            for (int i = 0; i < 2; i++)
            {
                Assert.IsTrue(actual[i] >= 0 && actual[i] < 20);
            }

            FlowAssert.IsAtLeastOneAttemptOk(100, () =>
            {
                actual = target.GetUniqueInts(2, 0, 20);
                Assert.AreEqual(2, actual.Length);
                Assert.AreEqual(2, actual.Distinct().Count());

                Assert.IsTrue(actual[0] >= 2);
            });
        }
    }
}

