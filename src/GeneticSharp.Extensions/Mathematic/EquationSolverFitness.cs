using System;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Infrastructure.Framework.Images;

namespace GeneticSharp.Extensions.Mathematic
{
    /// <summary>
    /// Equation solver fitness.
    /// </summary>
    public class EquationSolverFitness<TValue> : FunctionFitness<TValue>
    {
        #region Fields
        private readonly Func<Gene[], TValue> m_expectedResultFunction;
        
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Extensions.Mathematic.EquationSolverFitness"/> class.
        /// </summary>
        /// <param name="expectedResult">Expected result.</param>
        /// <param name="getEquationResult">Get equation result.</param>
        public EquationSolverFitness(TValue expectedResult, Func<Gene[], TValue> getEquationResult):base(getEquationResult)
        {
            m_expectedResultFunction = x=> expectedResult;
            
        }


        public EquationSolverFitness(Func<Gene[], TValue> expectedResultFunction, Func<Gene[], TValue> getEquationResult) : base(getEquationResult)
        {
            m_expectedResultFunction = expectedResultFunction;

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
            var equationResult = TypedEvaluate(chromosome);

            return CompareValues(m_expectedResultFunction(chromosome.GetGenes()), equationResult);

        }


        protected virtual double CompareValues(TValue expected, TValue equationResult)
        {
            var diff = Math.Abs(expected.To<double>()-equationResult.To<double>());
            return 1 - (diff/1+diff);
        }



        #endregion
    }




    public class EquationSolverFitness : EquationSolverFitness<int>
    {
        public EquationSolverFitness(int expectedResult, Func<Gene[], int> getEquationResult) : base(expectedResult, getEquationResult){}
        protected override double CompareValues(int expected, int equationResult)
        {
            var fitness = Math.Abs(expected - equationResult);

            return fitness * -1;
        }
    }

}
