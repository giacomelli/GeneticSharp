﻿using System;

namespace GeneticSharp.Extensions
{
    /// <summary>
    /// Equation solver fitness.
    /// </summary>
    public class EquationSolverFitness : IFitness
    {
        #region Fields
        private readonly int m_expectedResult;
        private readonly Func<Gene[], int> m_getEquationResult;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Extensions.EquationSolverFitness"/> class.
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

            var fitness = Math.Abs(m_getEquationResult(equalityChromosome!.GetGenes()) - m_expectedResult);

            return fitness * -1;
        }
        #endregion
    }
}
