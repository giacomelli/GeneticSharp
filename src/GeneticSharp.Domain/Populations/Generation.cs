using System;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using HelperSharp;

namespace GeneticSharp.Domain.Populations
{
	public sealed class Generation
	{
		#region Constructors
		public Generation(int number, IList<IChromosome> chromosomes)
		{
			if(number < 1)
			{
				throw new ArgumentOutOfRangeException (
					"number", 
					"Generation number {0} is invalid. Generation number should be positive and start in 1.".With(number));
			}

			if(chromosomes == null || chromosomes.Count < 2)
			{
				throw new ArgumentOutOfRangeException ("chromosomes", "A generation should have at least 2 chromosomes.");
			}

			Number = number;
			Chromosomes = chromosomes;
		}
		#endregion

		#region Properties
		public int Number { get; private set; }
		public IList<IChromosome> Chromosomes { get; internal set; }
		#endregion
	}
}

