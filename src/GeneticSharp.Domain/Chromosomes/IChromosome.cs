using System;
using System.Collections.Generic;

namespace GeneticSharp.Domain.Chromosomes
{
	/// <summary>
	/// Defines an interface for a chromosome.
	/// <remarks>
	/// In genetic algorithms, a chromosome (also sometimes called a genome) is a set of parameters 
	/// which define a proposed solution to the problem that the genetic algorithm is trying to solve. 
	/// The chromosome is often represented as a simple string, although a wide variety of other data 
	/// structures are also used.
    /// <see href="http://en.wikipedia.org/wiki/Chromosome_(genetic_algorithm)">http://en.wikipedia.org/wiki/Chromosome_(genetic_algorithm)</see> 
	/// </remarks>
	/// </summary>
	public interface IChromosome : IComparable<IChromosome>
	{
		#region Properties
		/// <summary>
		/// Gets the identifier.
		/// </summary>
		/// <value>The identifier.</value>
        string Id { get; }

		/// <summary>
		/// Gets or sets the fitness.
		/// </summary>
		/// <value>The fitness.</value>
		double? Fitness { get; set; }

		/// <summary>
		/// Gets or sets the age.
		/// </summary>
		/// <value>The age.</value>
        int Age { get; set; }

		/// <summary>
		/// Gets the length.
		/// </summary>
		/// <value>The length.</value>
		int Length { get; }
		#endregion

		#region Methods
		/// <summary>
		/// Generates the gene for the specified index.
		/// </summary>
		/// <returns>The gene.</returns>
		/// <param name="geneIndex">Gene index.</param>
		Gene GenerateGene(int geneIndex);

		/// <summary>
		/// Adds the gene in the end of the chromosome.
		/// </summary>
		/// <param name="gene">Gene.</param>
		//void AddGene (Gene gene);

		/// <summary>
		/// Adds the genes in the end of the chromosome.
		/// </summary>
		/// <param name="genes">Genes.</param>
		//void AddGenes (IEnumerable<Gene> genes);

		/// <summary>
		/// Replaces the gene in the specified index.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="gene">Gene.</param>
		void ReplaceGene (int index, Gene gene);

		void ReplaceGenes (int startIndex, Gene[] genes);

		/// <summary>
		/// Clears the genes.
		/// </summary>
		//void ClearGenes();

		/// <summary>
		/// Gets the gene in the specified index.
		/// </summary>
		/// <returns>The gene.</returns>
		/// <param name="index">Index.</param>
		Gene GetGene (int index);

		/// <summary>
		/// Gets the genes.
		/// </summary>
		/// <returns>The genes.</returns>
		Gene[] GetGenes();

		/// <summary>
		/// Creates a new chromosome using the same structure of this.
		/// </summary>
		/// <returns>The new chromosome.</returns>
        IChromosome CreateNew();
		#endregion
	}
}