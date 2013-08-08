using System;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;
using HelperSharp;

namespace GeneticSharp.Domain.Selections
{
	public abstract class SelectionBase : ISelection
	{
		#region Fields
		private int m_minNumberChromosomes;
		#endregion

		#region Constructors
		protected SelectionBase(int minNumberChromosomes)
		{
			m_minNumberChromosomes = minNumberChromosomes;
		}
		#endregion

		#region ISelection implementation
		public IList<IChromosome> SelectChromosomes (int number, Generation generation)
		{
			if (number < m_minNumberChromosomes) {
				throw new ArgumentOutOfRangeException ("number", "The number of selected chromosomes should be at least {0}.".With(m_minNumberChromosomes));
			}

			ExceptionHelper.ThrowIfNull ("generation", generation);

			return PerformSelectChromosomes (number, generation);
		}

		protected abstract IList<IChromosome> PerformSelectChromosomes (int number, Generation generation);
		#endregion
	}
}