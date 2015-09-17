using System;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;

namespace GeneticSharp.Extensions.Mathematic
{
    /// <summary>
    /// Equation solver fitness.
    /// </summary>
    public class EquationSolverFitness : IFitness
    {
        #region Fields
        private int m_expectedResult;
        private Func<Gene[], int> m_getEquationResult;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Extensions.Mathematic.EquationSolverFitness"/> class.
        /// </summary>
        /// <param name="expectedResult">Expected result.</param>
        /// <param name="getEquationResult">Get equation result.</param>
        public EquationSolverFitness(int expectedResult, Func<Gene[], int> getEquationResult)
        {
            m_expectedResult = expectedResult;
            m_getEquationResult = getEquationResult;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Performs the evaluation against the specified chromosome.
        /// </summary>
        /// <param name="chromosome">The chromosome to be evaluated.</param>
        /// <returns>The fitness of the chromosome.</returns>
        public double Evaluate(IChromosome chromosome)
        {
            var equalityChromosome = chromosome as EquationChromosome;

            var fitness = Math.Abs(m_getEquationResult(equalityChromosome.GetGenes()) - m_expectedResult);

            return fitness * -1;
        }
        #endregion
    }
}
