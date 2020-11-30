using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Extensions.Tsp
{
    /// <summary>
    /// Travelling Salesman Problem chromosome.
    /// <remarks>
    /// Each gene represents a city index.
    /// </remarks>
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Distance:{Distance}, Fitness:{Fitness}")]
    public class TspChromosome : ChromosomeBase
    {
        #region Fields
        private readonly int m_numberOfCities;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Extensions.Tsp.TspChromosome"/> class.
        /// </summary>
        /// <param name="numberOfCities">Number of cities.</param>
        public TspChromosome(int numberOfCities) : base(numberOfCities)
        {
            m_numberOfCities = numberOfCities;
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TspChromosome"/> class with a vector of cities
        /// </summary>
        /// <param name="cities">a list of integer to initialize the chromosome genes</param>
        public TspChromosome(IList<int> cities) : base(cities.Count())
        {
            m_numberOfCities = cities.Count();
            for (int i = 0; i < cities.Count; i++)
            {
                ReplaceGene(i, new Gene(cities[i]));
            }
        }


        #endregion

        #region Properties
        /// <summary>
        /// Gets the distance.
        /// </summary>
        /// <value>The distance.</value>
        public double Distance { get; internal set; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the list of city indices.
        /// </summary>
        /// <value>The distance.</value>
        public IList<int> GetCities()
        {
            return GetGenes().Select(g => (int?) g.Value ?? 0).ToList();
        }

        #endregion

        #region implemented abstract members of ChromosomeBase
        /// <summary>
        /// Generates the gene for the specified index.
        /// </summary>
        /// <returns>The gene.</returns>
        /// <param name="geneIndex">Gene index.</param>
        public override Gene GenerateGene(int geneIndex)
        {
            return new Gene(RandomizationProvider.Current.GetInt(0, m_numberOfCities));
        }

        /// <summary>
        /// Creates a new chromosome using the same structure of this.
        /// </summary>
        /// <returns>The new chromosome.</returns>
        public override IChromosome CreateNew()
        {
            return new TspChromosome(m_numberOfCities);
        }

        /// <summary>
        /// Creates a clone.
        /// </summary>
        /// <returns>The chromosome clone.</returns>
        public override IChromosome Clone()
        {
            var clone = base.Clone() as TspChromosome;
            clone.Distance = Distance;

            return clone;
        }

        protected override void CreateGenes()
        {
            var citiesIndexes = RandomizationProvider.Current.GetUniqueInts(m_numberOfCities, 0, m_numberOfCities);

            for (int i = 0; i < m_numberOfCities; i++)
            {
                ReplaceGene(i, new Gene(citiesIndexes[i]));
            }
        }

        #endregion
    }
}