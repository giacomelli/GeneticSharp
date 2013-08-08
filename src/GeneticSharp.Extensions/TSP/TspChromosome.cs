using System;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Extensions.Tsp
{
	public class TspChromosome : ChromosomeBase
	{
		#region Fields
		private int m_numberOfCities;
		#endregion

		#region Constructors
		public TspChromosome(int numberOfCities)
		{
			m_numberOfCities = numberOfCities;

			for (int i = 0; i < numberOfCities; i++) {
				AddGene(GenerateGene(i));
			}
		}
		#endregion

		#region implemented abstract members of ChromosomeBase
		public override Gene GenerateGene (int geneIndex)
		{
			return new Gene (RandomizationProvider.Current.GetInt (0, m_numberOfCities));
		}

		public override IChromosome CreateNew ()
		{
			return new TspChromosome(m_numberOfCities);
		}
		#endregion
	}
}

