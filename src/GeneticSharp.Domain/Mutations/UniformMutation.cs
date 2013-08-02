using System;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using HelperSharp;

namespace GeneticSharp.Domain.Mutations
{
	/// <summary>
	/// This operator replaces the value of the chosen gene with a uniform random value selected 
	/// between the user-specified upper and lower bounds for that gene. 
	/// http://en.wikipedia.org/wiki/Mutation_(genetic_algorithm)
	/// </summary>
	public class UniformMutation : IMutation
	{
		#region Fields
		private int[] m_genesIndexes;
		#endregion

		#region Constructors
		public UniformMutation(params int[] genesIndexes)
		{
			m_genesIndexes = genesIndexes;
		}
		#endregion

		#region IMutation implementation
		public void Mutate (IChromosome chromosome)
		{
            ExceptionHelper.ThrowIfNull("chromosome", chromosome);

			var genesLength = chromosome.Length;

			if (m_genesIndexes == null || m_genesIndexes.Length == 0) {
				m_genesIndexes = RandomizationProvider.Current.GetInts (1, 0, genesLength);
			}

			foreach (var i in m_genesIndexes) {
				if (i >= genesLength) {
					throw new ArgumentOutOfRangeException ("chromosome", 
					                                       "The chromosome has no gene on index {0}. The chromosome genes lenght is {1}.".With(i, genesLength));
				}

				chromosome.ReplaceGene(i, chromosome.GenerateGene(i));
			}
		}
		#endregion
	}
}