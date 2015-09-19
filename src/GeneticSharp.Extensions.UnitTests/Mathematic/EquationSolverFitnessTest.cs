using System;
using NUnit.Framework;
using TestSharp;
using GeneticSharp.Extensions.Mathematic;
using GeneticSharp.Domain.Chromosomes;
using System.Linq;

namespace GeneticSharp.Extensions.UnitTests.Mathematic
{
	[TestFixture()]
	[Category("Extensions")]
	public class EquationSolverFitnessTest
	{
		[Test()]
		public void Evaluate_DiffChromosomes_DiffFitness()
		{
			// Equation A + B = 3.
			var target = new EquationSolverFitness (
				             3,
				             (genes) => {
								return genes.Select(g => (int) g.Value).Sum();
							 });

			var chromosome = new EquationChromosome (3, 2);
			chromosome.ReplaceGene (0, new Gene (1));
			chromosome.ReplaceGene (1, new Gene (2));

			var actual = target.Evaluate (chromosome);
			Assert.AreEqual (0, actual);

			chromosome.ReplaceGene (1, new Gene (3));

			actual = target.Evaluate (chromosome);
			Assert.AreEqual (-1, actual);
		}
	}
}