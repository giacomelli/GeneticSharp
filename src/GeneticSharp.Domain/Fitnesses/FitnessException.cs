using System;
using HelperSharp;

namespace GeneticSharp.Domain.Fitnesses
{
	/// <summary>
	/// Exception throw when an error occurs during the execution of fitness evaluation.
	/// </summary>
	public sealed class FitnessException : Exception
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="GeneticSharp.Domain.Fitnesses.FitnessException"/> class.
		/// </summary>
		/// <param name="fitness">The fitness where ocurred the error.</param>
		/// <param name="message">The error message.</param>
		public FitnessException (IFitness fitness, string message) : base("{0}: {1}".With(fitness.GetType().Name, message))
		{
			Fitness = fitness;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GeneticSharp.Domain.Fitnesses.FitnessException"/> class.
		/// </summary>
		/// <param name="fitness">The fitness where ocurred the error.</param>
		/// <param name="message">The error message.</param>
		/// <param name="innerException">Inner exception.</param>
		public FitnessException (IFitness fitness, string message, Exception innerException) : base("{0}: {1}".With(fitness.GetType().Name, message), innerException)
		{
			Fitness = fitness;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the fitness.
		/// </summary>
		/// <value>The fitness.</value>
		public IFitness Fitness { get; private set; }
		#endregion
	}
}