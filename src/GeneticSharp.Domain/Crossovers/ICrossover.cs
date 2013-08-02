using System;
using GeneticSharp.Domain.Chromosomes;
using System.Collections.Generic;

namespace GeneticSharp.Domain.Crossovers
{
	public interface ICrossover
	{
		#region Properties
		int ParentsNumber { get; }
		int ChildrenNumber  { get; }
		#endregion

		#region Methods
		IList<IChromosome> Cross(IList<IChromosome> parents);
		#endregion
	}
}