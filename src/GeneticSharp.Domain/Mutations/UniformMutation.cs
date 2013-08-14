using System;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using HelperSharp;
using System.Linq;

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
		private int[] m_mutableGenesIndexes;
		private bool m_allGenesMutables;
		#endregion

		#region Constructors
		public UniformMutation(params int[] mutableGenesIndexes)
		{
			m_mutableGenesIndexes = mutableGenesIndexes;
		}

		public UniformMutation(bool allGenesMutables)
		{
			m_allGenesMutables = allGenesMutables;
		}
		#endregion

		#region IMutation implementation
		public void Mutate (IChromosome chromosome, float probability)
		{
            ExceptionHelper.ThrowIfNull("chromosome", chromosome);

			var genesLength = chromosome.Length;

			if (m_mutableGenesIndexes == null || m_mutableGenesIndexes.Length == 0) {
				if (m_allGenesMutables) {
					m_mutableGenesIndexes = Enumerable.Range (0, genesLength).ToArray ();
				} else {
					m_mutableGenesIndexes = RandomizationProvider.Current.GetInts (1, 0, genesLength);
				}
			}

			foreach (var i in m_mutableGenesIndexes) {
				if (i >= genesLength) {
					throw new MutationException (this, "The chromosome has no gene on index {0}. The chromosome genes lenght is {1}.".With(i, genesLength));
				}

				if (RandomizationProvider.Current.GetDouble () <= probability) {
					chromosome.ReplaceGene (i, chromosome.GenerateGene (i));
				}
			}
		}
		#endregion
	}
}