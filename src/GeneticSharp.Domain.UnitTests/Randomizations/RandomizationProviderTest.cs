using NUnit.Framework;
using System;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Domain.UnitTests.Randomizations
{
	[TestFixture()]
	public class RandomizationProviderTest
	{
		[Test()]
		public void Current_Default_IsNotNull ()
		{
			Assert.IsNotNull (RandomizationProvider.Current);
		}
	}
}

