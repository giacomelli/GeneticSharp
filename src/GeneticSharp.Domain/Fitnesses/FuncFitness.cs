using System;
using System.Collections.Generic;

namespace GeneticSharp
{
    /// <summary>
    /// An IFitness implementation that defer the fitness evaluation to a Func.
    /// </summary>
    public class FuncFitness : IFitness
    {
        private readonly Func<IChromosome, double> m_func;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:GeneticSharp.Domain.Fitnesses.FuncFitness"/> class.
        /// </summary>
        /// <param name="func">The fitness evaluation Func.</param>
        public FuncFitness (Func<IChromosome, double> func)
        {
            ExceptionHelper.ThrowIfNull("func", func);
            m_func = func;
        }

        #region IFitness implementation
        /// <summary>
        /// Evaluate the specified chromosome.
        /// </summary>
        /// <param name="chromosome">Chromosome.</param>
        public double Evaluate (IChromosome chromosome)
        {
            return m_func (chromosome);
        }
        #endregion
    }

    public class VectorFuncFitness : VectorFitness
    {
        private readonly Func<IList<IChromosome>, double[]> m_func;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:GeneticSharp.Domain.Fitnesses.VectorFuncFitness"/> class.
        /// </summary>
        /// <param name="func">The fitness evaluation Func.</param>
        public VectorFuncFitness(Func<IList<IChromosome>, double[]> func)
        {
            ExceptionHelper.ThrowIfNull("func", func);
            m_func = func;
        }

        /// <summary>
        /// Evaluates the specified chromosomes and returns their fitness values as a vector.
        /// </summary>
        /// <param name="chromosomes">The chromosomes to be evaluated.</param>
        /// <returns>A vector of fitness values corresponding to the input chromosomes.</returns>
        public override double[] Evaluate(IList<IChromosome> chromosomes)
        {
            return m_func(chromosomes);
        }
	}
}
