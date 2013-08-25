using System;
using NUnit.Framework;
using Rhino.Mocks;
using GeneticSharp.Domain.Terminations;
using TestSharp;

namespace GeneticSharp.Domain.UnitTests.Terminations
{
	[TestFixture()]
	public class TerminationBaseTest
	{
		[Test()]
		public void HasReached_NullGeneration_Exception ()
		{
			var target = MockRepository.GenerateStub<TerminationBase> ();

			ExceptionAssert.IsThrowing (new ArgumentNullException ("geneticAlgorithm"), () => {
				target.HasReached(null);
			});
		}
	}
}

