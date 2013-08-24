using System;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Chromosomes;
using System.Threading;

namespace GeneticSharp.Domain.UnitTests
{
	public class FitnessStub : IFitness
	{
		public FitnessStub()
		{
			ParallelSleep = 500;
		}

		#region IFitness implementation
		public bool SupportsParallel { get; set; }
		public int ParallelSleep { get; set; }
		public double Evaluate (IChromosome chromosome)
		{
			if (SupportsParallel) {
				Thread.Sleep (ParallelSleep);
			}

			var gene1 = (int) chromosome.GetGene (0).Value;
			var gene2 = (int) chromosome.GetGene (1).Value;
			var gene3 = (int) chromosome.GetGene (2).Value;
			var gene4 = (int) chromosome.GetGene (3).Value;

			var f = (gene1 + gene2 + gene3 + gene4) / 20f;

            return f; ;
		}
		#endregion
	}
}