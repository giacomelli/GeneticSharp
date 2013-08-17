using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using System.ComponentModel;

namespace GeneticSharp.Domain.Crossovers
{
    /// <summary>
	/// The Uniform Crossover uses a fixed mixing ratio between two parents. 
    /// Unlike one-point and two-point crossover, the Uniform Crossover enables the parent chromosomes to contribute the gene level rather than the segment level.
    /// <remarks>
    /// If the mix probability is 0.5, the offspring has approximately half of the genes from first parent and the other half from second parent, although cross over points can be randomly chosen.
    /// </remarks>
	/// <see href="http://en.wikipedia.org/wiki/Crossover_(genetic_algorithm)#Uniform_Crossover_and_Half_Uniform_Crossover">Wikipedia</see>
    /// </summary>
    [DisplayName("Uniform")]
	public class UniformCrossover : CrossoverBase
    {
        #region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="GeneticSharp.Domain.Crossovers.UniformCrossover"/> class.
		/// </summary>
		/// <param name="mixProbability">Mix probability.</param>
        public UniformCrossover(float mixProbability)
            : base(2, 2)
        {
            MixProbability = mixProbability;
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="GeneticSharp.Domain.Crossovers.UniformCrossover"/> class.
		/// <remarks>
		/// The default mix probability is 0.5.
		/// </remarks>
		/// </summary>
		public UniformCrossover() : this(0.5f)
		{
		}
        #endregion

        #region Properties
		/// <summary>
		/// Gets the mix probability.
		/// </summary>
		/// <value>The mix probability.</value>
        public float MixProbability { get; set; }
        #endregion

        #region Methods
		/// <summary>
		/// Performs the cross with specified parents generating the children.
		/// </summary>
		/// <param name="parents">Parents.</param>
		/// <returns>The offspring (children) of the parents.</returns>
        protected override IList<IChromosome> PerformCross(IList<IChromosome> parents)        
        {
            var firstParent = parents[0];
            var secondParent = parents[1];
            var firstChild = firstParent.CreateNew();
            var secondChild = secondParent.CreateNew();

            for (int i = 0; i < firstParent.Length; i++)
            {
                if (RandomizationProvider.Current.GetDouble() < MixProbability)
                {
                    firstChild.ReplaceGene(i, firstParent.GetGene(i));
                    secondChild.ReplaceGene(i, secondParent.GetGene(i));
                }
                else
                {
					firstChild.ReplaceGene(i, secondParent.GetGene(i));
					secondChild.ReplaceGene(i, firstParent.GetGene(i));
                }
            }

            return new List<IChromosome> { firstChild, secondChild };
        }
        #endregion
    }
}
