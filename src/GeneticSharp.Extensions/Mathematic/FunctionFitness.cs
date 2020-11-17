using System;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Infrastructure.Framework.Commons;

namespace GeneticSharp.Extensions.Mathematic
{
    public class FunctionFitness<TResult> : IFitness
    {

        private readonly Func<Gene[], TResult> m_getEquationResult;


        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionSolverFitness"/> class.
        /// </summary>
        /// <param name="expectedResult">Expected result.</param>
        /// <param name="getEquationResult">Get equation result.</param>
        public FunctionFitness(Func<Gene[], TResult> getEquationResult)
        {
            
            m_getEquationResult = getEquationResult;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Performs the evaluation against the specified chromosome.
        /// </summary>
        /// <param name="chromosome">The chromosome to be evaluated.</param>
        /// <returns>The fitness of the chromosome.</returns>
        public virtual double Evaluate(IChromosome chromosome)
        {
           
            return TypedEvaluate(chromosome).To<double>();
        }
        #endregion

        /// <summary>
        /// Performs the evaluation against the specified chromosome.
        /// </summary>
        /// <param name="chromosome">The chromosome to be evaluated.</param>
        /// <returns>The fitness of the chromosome.</returns>
        public TResult TypedEvaluate(IChromosome chromosome)
        {
            var fitness = m_getEquationResult(chromosome.GetGenes());

            return fitness;
        }


    }
}