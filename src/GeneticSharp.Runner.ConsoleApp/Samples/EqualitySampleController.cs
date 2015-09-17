using System;
using System.ComponentModel;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Extensions.Mathematic;

namespace GeneticSharp.Runner.ConsoleApp.Samples
{
    /// <summary>
    /// Sample based on this paper: Genetic Algorithm for Solving Simple Mathematical Equality Problem.
    /// <see href="http://arxiv.org/ftp/arxiv/papers/1308/1308.4675.pdf" />
    /// </summary>
    [DisplayName("Equality equation")]
    public class EqualitySampleController : SampleControllerBase
    {
        #region Fields
        private EqualityFitness m_fitness;
        #endregion

        #region Methods        
        /// <summary>
        /// Creates the chromosome.
        /// </summary>
        /// <returns>The sample chromosome.</returns>
        public override IChromosome CreateChromosome()
        {
            return new EquationChromosome(30, 4);
        }

        /// <summary>
        /// Creates the fitness.
        /// </summary>
        /// <returns>The fitness.</returns>
        public override IFitness CreateFitness()
        {
            m_fitness = new EqualityFitness();
            return m_fitness;
        }

        /// <summary>
        /// Draws the specified best chromosome.
        /// </summary>
        /// <param name="bestChromosome">The best chromosome.</param>
        public override void Draw(IChromosome bestChromosome)
        {
            var best = bestChromosome as EquationChromosome;

            var genes = best.GetGenes();
            Console.WriteLine("Equation: {0} + 2*{1} + 3*{2} + 4*{3} = {4}", genes[0], genes[1], genes[2], genes[3], EqualityFitness.GetEquationResult(best));
        }

        /// <summary>
        /// Creates the termination.
        /// </summary>
        /// <returns>
        /// The termination.
        /// </returns>
        public override ITermination CreateTermination()
        {
            return new FitnessThresholdTermination(0);
        }
        #endregion
    }
}
