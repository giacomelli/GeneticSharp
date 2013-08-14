using System;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using System.Diagnostics;

namespace GeneticSharp.Extensions.Tsp
{
	public class TspChromosome : ChromosomeBase
	{
		#region Fields
		private int m_numberOfCities;
		#endregion

		#region Constructors
		public TspChromosome(int numberOfCities) : base(numberOfCities)
		{
            if (numberOfCities < 2)
            {
                throw new ArgumentOutOfRangeException("The number of cities should be greater than 1.");
            }

			m_numberOfCities = numberOfCities;
            var citiesIndexes = RandomizationProvider.Current.GetUniqueInts(numberOfCities, 0, numberOfCities);

			for (int i = 0; i < numberOfCities; i++) {
				ReplaceGene(i, new Gene(citiesIndexes[i]));
			}
		}
		#endregion

		#region Properties
		public double Distance { get; internal set; }
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

