using System;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Chromosomes;

namespace GeneticSharp.Domain.UnitTests
{
	public class FitnessStub : IFitness
	{
		#region IFitness implementation
		public bool SupportsParallel { get; set; }
		public double Evaluate (IChromosome chromosome)
		{
			var gene1 = (int) chromosome.GetGene (0).Value;
			var gene2 = (int) chromosome.GetGene (1).Value;

			return (gene1 + gene2) / 10f;
		}
		#endregion
	}
}