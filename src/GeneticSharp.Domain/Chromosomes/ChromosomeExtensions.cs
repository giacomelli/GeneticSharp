using System;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using System.Linq;

namespace GeneticSharp.Domain.Chromosomes
{
	/// <summary>
	/// Chromosome extensions.
	/// </summary>
	public static class ChromosomeExtensions
	{
		/// <summary>
		/// Checks if any of the chromosomes has repeated gene.
		/// </summary>
		/// <remarks>
		/// This can happen when used with a IMutation's implementation that not keep the chromosome ordered, 
		/// like OnePointCrossover, TwoPointCrossover and UniformCrossover is combined with a ICrossover's implementation
		/// that need ordered chromosomes, like OX1 and PMX.
		/// </remarks>
		/// <returns><c>true</c>, if chromosome has repeated gene, <c>false</c> otherwise.</returns>
		/// <param name="chromosomes">Chromosomes.</param>
		public static bool AnyHasRepeatedGene(this IList<IChromosome> chromosomes)
		{
			foreach (var p in chromosomes)
			{
				var notRepeatedGenesLength = p.GetGenes().Distinct().Count();

				if (notRepeatedGenesLength < p.Length)
				{
					return true;
				}
			}

			return false;
		}
	}
}