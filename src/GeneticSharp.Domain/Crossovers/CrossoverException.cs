using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HelperSharp;

namespace GeneticSharp.Domain.Crossovers
{
    public class CrossoverException : Exception
	{
		#region Constructors
        public CrossoverException(ICrossover crossover, string message)
            : base("{0}: {1}".With(crossover.GetType().Name, message))
		{
            Crossover = crossover;
		}
		#endregion

		#region Properties
        public ICrossover Crossover { get; private set; }
		#endregion
	}
}