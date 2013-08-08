using System;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;

namespace GeneticSharp.Domain.Selections
{
	public sealed class EliteSelection : SelectionBase
	{
		#region Constructors
		public EliteSelection() : base(2)
		{
		}
		#endregion

		#region ISelection implementation
		protected override IList<IChromosome> PerformSelectChromosomes (int number, Generation generation)
		{
			return generation.Chromosomes.OrderByDescending (c => c.Fitness).Take (number).ToList ();
		}

		#endregion
	}
}