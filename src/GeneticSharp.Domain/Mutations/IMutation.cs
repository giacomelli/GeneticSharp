using System;
using GeneticSharp.Domain.Chromosomes;

namespace GeneticSharp.Domain.Mutations
{
	/// <summary>
	/// http://en.wikipedia.org/wiki/Mutation_(genetic_algorithm)
	/// </summary>
	public interface IMutation
	{
		/// <summary>
		/// Mutate the specified chromosome.
		/// </summary>
		/// <param name="chromosome">The chromosome.</param>
		/// <param name="probability">The probability to mutate each chromosome.</param>
		void Mutate(IChromosome chromosome, float probability);
	}
}

