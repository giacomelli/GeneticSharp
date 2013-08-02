using System;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;

namespace GeneticSharp.Domain.Selections
{
	/// <summary>
	/// http://www.ijest.info/docs/IJEST11-03-05-190.pdf
	/// </summary>
	public interface ISelection
	{
		IList<IChromosome> SelectChromosomes (int number, Generation generation);
	}
}