using System;
using GeneticSharp.Domain.Chromosomes;

namespace GeneticSharp.Domain.Mutations
{
	/// <summary>
	/// http://en.wikipedia.org/wiki/Mutation_(genetic_algorithm)
	/// </summary>
	public interface IMutation
	{
		void Mutate(IChromosome chromosome);
	}
}

