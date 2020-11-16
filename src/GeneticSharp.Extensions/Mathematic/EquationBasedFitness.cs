using System;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Infrastructure.Framework.Commons;

namespace GeneticSharp.Extensions.Mathematic
{
    public class EquationFitness<TResult> : IFitness
    {

        private readonly Func<Gene[], TResult> m_getEquationResult;


        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Extensions.Mathematic.EquationSolverFitness"/> class.
        /// </summary>
        /// <param name="expectedResult">Expected result.</param>
        /// <param name="getEquationResult">Get equation result.</param>
        public EquationFitness(Func<Gene[], TResult> getEquationResult)
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
            var equalityChromosome = chromosome as EquationChromosome;

            var fitness = m_getEquationResult(chromosome.GetGenes()) ;

            return fitness.To<double>();
        }
        #endregion

    }
}