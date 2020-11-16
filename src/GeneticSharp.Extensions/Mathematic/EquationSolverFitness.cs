using System;
using GeneticSharp.Domain.Chromosomes;

namespace GeneticSharp.Extensions.Mathematic
{
    /// <summary>
    /// Equation solver fitness.
    /// </summary>
    public class EquationSolverFitness : EquationFitness<int>
    {
        #region Fields
        private readonly int m_expectedResult;
        
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Extensions.Mathematic.EquationSolverFitness"/> class.
        /// </summary>
        /// <param name="expectedResult">Expected result.</param>
        /// <param name="getEquationResult">Get equation result.</param>
        public EquationSolverFitness(int expectedResult, Func<Gene[], int> getEquationResult):base(getEquationResult)
        {
            m_expectedResult = expectedResult;
            
        }
        #endregion

        #region Methods
        /// <summary>
        /// Performs the evaluation against the specified chromosome.
        /// </summary>
        /// <param name="chromosome">The chromosome to be evaluated.</param>
        /// <returns>The fitness of the chromosome.</returns>
        public override double Evaluate(IChromosome chromosome)
        {
            var equationResult = base.Evaluate(chromosome);

            var fitness = Math.Abs(equationResult - m_expectedResult);

            return fitness * -1;
        }
        #endregion
    }
}
