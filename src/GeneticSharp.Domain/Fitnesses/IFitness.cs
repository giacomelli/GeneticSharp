using System;
using GeneticSharp.Domain.Chromosomes;

namespace GeneticSharp.Domain.Fitnesses
{
	public interface IFitness
	{
		bool SupportsParallel { get; }
		double Evaluate(IChromosome chromosome);
	}
}

