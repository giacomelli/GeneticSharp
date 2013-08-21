using System;
using NUnit.Framework;
using System.ComponentModel;
using GeneticSharp.Domain.Populations;

namespace GeneticSharp.Domain.Terminations
{
	/// <summary>
	/// Fitness Threshold Termination
	/// <remarks>
	/// The genetic algorithm will be terminate when the best chromosome reach the expected fitness.
	/// </remarks>
	/// </summary>
	[DisplayName("Fitness Threshold")]
	public class FitnessThresholdTermination : TerminationBase
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="GeneticSharp.Domain.Terminations.FitnessThresholdTermination"/> class.
		/// </summary>
		/// <remarks>
		/// The default expected fitness is 1.00.
		/// </remarks>
		public FitnessThresholdTermination() : this(1.00)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GeneticSharp.Domain.Terminations.FitnessThresholdTermination"/> class.
		/// </summary>
		/// <param name="expectedFitness">Expected fitness.</param>
		public FitnessThresholdTermination(double expectedFitness)
		{
			ExpectedFitness = expectedFitness;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the expected fitness to consider that termination has been reached.
		/// </summary>
		public double ExpectedFitness { get; set; }
		#endregion

		#region implemented abstract members of TerminationBase
		/// <summary>
		/// Determines whether the specified generation reached the termination condition.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		/// <param name="generation">The generation.</param>
		protected override bool PerformHasReached (Generation generation)
		{
			return generation.BestChromosome.Fitness >= ExpectedFitness;
		}
		#endregion
	
	}
}

