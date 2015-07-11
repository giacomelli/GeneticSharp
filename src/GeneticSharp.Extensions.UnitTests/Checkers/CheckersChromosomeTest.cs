using System;
using GeneticSharp.Extensions.Checkers;
using NUnit.Framework;
using TestSharp;
using System.Collections.Generic;

namespace GeneticSharp.Extensions.UnitTests.Checkers
{
    [TestFixture]
	public class CheckersChromosomeTest
    {
		[Test()]
		public void Clone_NoArgs_Cloned()
		{
			var target = new CheckersChromosome (2, 10);

			var actual = target.Clone () as CheckersChromosome;
			Assert.IsFalse (Object.ReferenceEquals (target, actual));
			Assert.AreEqual(2, actual.Moves.Count);
		}
    }
}