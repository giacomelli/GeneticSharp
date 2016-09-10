using System;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;

namespace Issue9Sample
{
	public class PreloadPopulation : Population
	{
		private IList<IChromosome> m_preloadChromosome;

		public PreloadPopulation(int minSize, int maxSize, IList<IChromosome> preloadChromosomes) 
			: base(minSize, maxSize, preloadChromosomes.FirstOrDefault())
		{
			m_preloadChromosome = preloadChromosomes;	
		}

		public override void CreateInitialGeneration()
		{
			Generations = new List<Generation>();
			GenerationsNumber = 0;

			foreach(var c in m_preloadChromosome) 
			{
				c.ValidateGenes();
			}

			CreateNewGeneration(m_preloadChromosome);
		}
	}
}

