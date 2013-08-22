using System;
using System.ComponentModel;
using GeneticSharp.Domain.Populations;

namespace GeneticSharp.Domain.Terminations
{
    /// <summary>
    /// Fitness Stagnation Termination.    
    /// <remarks>
    /// The genetic algorithm will be terminate when the best chromosome's fitness has no change in the last generations specified.
    /// </remarks>
    /// </summary>
    [DisplayName("Fitness Stagnation")]
    public class FitnessStagnationTermination : TerminationBase
    {
        #region Fields
        private double m_lastFitness;
        private int m_stagnantGenerationsCount;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FitnessStagnationTermination"/> class.
        /// </summary>
        /// <remarks>
        /// The ExpectedStagnantGenerationsNumber default value is 100.
        /// </remarks>
        public FitnessStagnationTermination() : this(100)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="FitnessStagnationTermination"/> class.
        /// </summary>
        /// <param name="expectedStagnantGenerationsNumber">The expected stagnant generations number to reach the termination.</param>
        public FitnessStagnationTermination(int  expectedStagnantGenerationsNumber)
        {
			ExpectedStagnantGenerationsNumber = expectedStagnantGenerationsNumber;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the expected stagnant generations number to reach the termination.
        /// </summary>
        public int ExpectedStagnantGenerationsNumber { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Determines whether the specified generation reached the termination condition.
        /// </summary>
        /// <param name="generation">The generation.</param>
        /// <returns>
        /// True if termination has been reached, otherwise false.
        /// </returns>
        protected override bool PerformHasReached(Generation generation)
        {
            var bestFitness = generation.BestChromosome.Fitness.Value;

            if (m_lastFitness == bestFitness)
            {
                m_stagnantGenerationsCount++;
            }
            else
            {
                m_stagnantGenerationsCount = 1;
            }

			m_lastFitness = bestFitness;

			return m_stagnantGenerationsCount >= ExpectedStagnantGenerationsNumber;
        }
        #endregion
    }
}
