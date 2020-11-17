using System;
using GeneticSharp.Domain.Chromosomes;

namespace GeneticSharp.Extensions.Mathematic
{
    /// <summary>
    /// Equation solver fitness.
    /// </summary>
    public abstract class EquationSolverFitness<TValue> : FunctionFitness<TValue>
    {
        #region Fields
        private readonly TValue m_expectedResult;
        
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Extensions.Mathematic.EquationSolverFitness"/> class.
        /// </summary>
        /// <param name="expectedResult">Expected result.</param>
        /// <param name="getEquationResult">Get equation result.</param>
        public EquationSolverFitness(TValue expectedResult, Func<Gene[], TValue> getEquationResult):base(getEquationResult)
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
            var equationResult = this.TypedEvaluate(chromosome);
            return CompareValues(m_expectedResult, equationResult);

        }


        protected abstract double CompareValues(TValue expected, TValue equationResult);



        #endregion
    }




    public class EquationSolverFitness : EquationSolverFitness<int>
    {
        public EquationSolverFitness(int expectedResult, Func<Gene[], int> getEquationResult) : base(expectedResult, getEquationResult){}
        protected override double CompareValues(int expectedResult, int equationResult)
        {
            var fitness = Math.Abs(expectedResult - equationResult);

            return fitness * -1;
        }
    }

}
