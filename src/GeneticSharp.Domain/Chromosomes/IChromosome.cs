using System;
using System.Collections.Generic;

namespace GeneticSharp.Domain.Chromosomes
{
	public interface IChromosome : IComparable<IChromosome>
	{
		#region Properties
        string Id { get; }
		double? Fitness { get; set; }
        int Age { get; set; }
		int Length { get; }
		#endregion

		#region Methods
		Gene GenerateGene(int geneIndex);
		void AddGene (Gene gene);
		void AddGenes (IEnumerable<Gene> genes);
		void ReplaceGene (int index, Gene gene);
		void ClearGenes();
		Gene GetGene (int index);
		IList<Gene> GetGenes();

        IChromosome CreateNew();
		#endregion
	}
}