using NUnit.Framework;
using System;
using GeneticSharp.Domain.Randomizations;
using TestSharp;

namespace GeneticSharp.Domain.UnitTests
{
	[TestFixture()]
	public class BasicRandomizationTest
	{
		[Test()]
		public void GetDouble_Range_RandomInRangeResult ()
		{
			var target = new BasicRandomization ();

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

		[Test()]
		public void GetDouble_NoArgs_RandomResult ()
		{
			var target = new BasicRandomization ();

			Assert.AreNotEqual (target.GetDouble (), target.GetDouble ());
		}

		[Test()]
		public void GetInt_Range_RandomInRangeResult ()
		{
			var target = new BasicRandomization ();

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

		[Test()]
		public void GetInts_Length_ArrayWithLength ()
		{
			var target = new BasicRandomization ();
			Assert.AreEqual(1, target.GetInts(1, 0, 10).Length);
			Assert.AreEqual(2, target.GetInts(2, 0, 10).Length);
			Assert.AreEqual(3, target.GetInts(3, 0, 10).Length);
		}
	}
}

