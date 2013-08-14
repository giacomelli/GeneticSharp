using System;
using HelperSharp;

namespace GeneticSharp.Domain.Fitnesses
{
	public sealed class FitnessException : Exception
	{
		#region Constructors
		public FitnessException (IFitness fitness, string message) : base("{0}: {1}".With(fitness.GetType().Name, message))
		{
			Fitness = fitness;
		}

		public FitnessException (IFitness fitness, string message, Exception innerException) : base("{0}: {1}".With(fitness.GetType().Name, message), innerException)
		{
			Fitness = fitness;
		}
		#endregion

		#region Properties
		public IFitness Fitness { get; private set; }
		#endregion
	}
}