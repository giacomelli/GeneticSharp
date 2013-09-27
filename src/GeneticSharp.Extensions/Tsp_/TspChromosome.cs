using System;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using System.Diagnostics;

namespace GeneticSharp.Extensions.Tsp
{
	/// <summary>
	/// Travelling Salesman Problem chromosome.
	/// <remarks>
	/// Each gene represents a city index.
	/// </remarks>
	/// </summary>
	public class TspChromosome : ChromosomeBase
	{
		#region Fields
		private int m_numberOfCities;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="GeneticSharp.Extensions.Tsp.TspChromosome"/> class.
		/// </summary>
		/// <param name="numberOfCities">Number of cities.</param>
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
		/// <summary>
		/// Gets or sets the distance.
		/// </summary>
		/// <value>The distance.</value>
		public double Distance { get; internal set; }
		#endregion

		#region implemented abstract members of ChromosomeBase
		/// <summary>
		/// Generates the gene for the specified index.
		/// </summary>
		/// <returns>The gene.</returns>
		/// <param name="geneIndex">Gene index.</param>
		public override Gene GenerateGene (int geneIndex)
		{
			return new Gene (RandomizationProvider.Current.GetInt (0, m_numberOfCities));
		}

		/// <summary>
		/// Creates a new chromosome using the same structure of this.
		/// </summary>
		/// <returns>The new chromosome.</returns>
		public override IChromosome CreateNew ()
		{
			return new TspChromosome(m_numberOfCities);
		}

		/// <summary>
		/// Creates a clone.
		/// </summary>
		public override IChromosome Clone ()
		{
			var clone = base.Clone () as TspChromosome;
			clone.Distance = Distance;

			return clone;
		}
		#endregion
	}
}

