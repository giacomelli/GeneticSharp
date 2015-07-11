using NUnit.Framework;
using System;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Extensions.Tsp;
using GeneticSharp.Domain.Selections;
using TestSharp;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Domain;

namespace GeneticSharp.Extensions.UnitTests.Tsp
{
	[TestFixture()]
	public class TspFitnessTest
	{
		[Test()]
		public void Evaluate_ChromossomeWithLowerCities_FitnessDividedByDiff ()
		{
			var target = new TspFitness (10, 0, 10, 0, 10);
			var chromossome = new TspChromosome (9);

			var actual = target.Evaluate (chromossome);
			Assert.AreNotEqual (0, actual);
		}

		[Test()]
		public void Evaluate_FitnessLowerThanZero_Zero ()
		{
			var target = new TspFitness (10, 0, 10000000, 0, 10000000);
			var chromossome = new TspChromosome (10);

			var actual = target.Evaluate (chromossome);
			Assert.AreEqual (0, actual);
		}
	}
}

