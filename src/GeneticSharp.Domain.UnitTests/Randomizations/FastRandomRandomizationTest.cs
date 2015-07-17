using NUnit.Framework;
using System;
using GeneticSharp.Domain.Randomizations;
using TestSharp;
using System.Linq;

namespace GeneticSharp.Domain.UnitTests.Randomizations
{
	[TestFixture()]
	public class FastRandomRandomizationTest
	{
        [Test]
        public void GetFloat_Range_RandomInRangeResult()
        {
            var target = new FastRandomRandomization();

            FlowAssert.IsAtLeastOneAttemptOk(100, () =>
            {
                Assert.IsTrue(target .GetFloat(0, 2.2f) < 1);
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
            var target = new FastRandomRandomization();

            Assert.AreNotEqual(target.GetFloat(), target.GetFloat());
        }

		[Test]
		public void GetDouble_Range_RandomInRangeResult ()
		{
			var target = new FastRandomRandomization ();

			FlowAssert.IsAtLeastOneAttemptOk (100, () => {
				Assert.IsTrue(target.GetDouble(0, 2.2) < 1);
			});

			FlowAssert.IsAtLeastOneAttemptOk (1000, () => {
				Assert.IsTrue(target.GetDouble(0, 2.2) > 2.1);
			});

			for (int i = 0; i < 100; i++) {
				Assert.AreNotEqual(2.3, target.GetDouble(0, 2.2));
			}
		}

		[Test]
		public void GetDouble_NoArgs_RandomResult ()
		{
			var target = new FastRandomRandomization ();

			Assert.AreNotEqual (target.GetDouble (), target.GetDouble ());
		}

		[Test]
		public void GetInt_Range_RandomInRangeResult ()
		{
			var target = new FastRandomRandomization ();

			FlowAssert.IsAtLeastOneAttemptOk (100, () => {
				Assert.AreEqual(0, target.GetInt(0, 2));
			});

			FlowAssert.IsAtLeastOneAttemptOk (100, () => {
				Assert.AreEqual(1, target.GetInt(0, 2));
			});

			for (int i = 0; i < 100; i++) {
				Assert.AreNotEqual(2, target.GetInt(0, 2));
			}
		}

		[Test]
		public void GetInts_Length_ArrayWithLength ()
		{
			var target = new FastRandomRandomization ();
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
            var target = new FastRandomRandomization();

            ExceptionAssert.IsThrowing(new ArgumentOutOfRangeException("length", "The length is 5, but the possible unique values between 0 (inclusive) and 4 (exclusive) are 4."), () =>
            {
                target.GetUniqueInts(5, 0, 4);
            });
            
          
        }

		[Test]
		public void GetUniqueInts_Length_ArrayWithUniqueInts ()
		{
			var target = new FastRandomRandomization ();
			var actual = target.GetUniqueInts (10, 0, 10);
			Assert.AreEqual (10, actual.Length);
			Assert.AreEqual (10, actual.Distinct().Count());

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

			FlowAssert.IsAtLeastOneAttemptOk (100, () => {
				actual = target.GetUniqueInts (2, 0, 20);
				Assert.AreEqual (2, actual.Length);
				Assert.AreEqual (2, actual.Distinct ().Count ());

				Assert.IsTrue(actual[0] >= 2); 
			});
		}

		[Test]
		public void GetInt_MinGreaterThanMaxEquals_Exception ()
		{
			var target = new FastRandomRandomization ();

			ExceptionAssert.IsThrowing (new ArgumentOutOfRangeException ("upperBound", 1, "upperBound must be >=lowerBound"), () => {
				target.GetInt(2, 1);
			});
		}

		[Test]
		public void GetInt_NegativeValues_Negative ()
		{
			var target = new FastRandomRandomization ();

			var actual = target.GetInt (-10, -9);
			Assert.AreEqual (-10, actual);
		}
	}
}

