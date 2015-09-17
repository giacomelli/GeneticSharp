using System;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;

namespace GeneticSharp.Extensions.Ghostwriter
{
    /// <summary>
    /// Ghostwriter fitness.
    /// </summary>
    public class GhostwriterFitness : IFitness
    {
        /// <summary>
        /// Gets or sets the evaluate function.
        /// </summary>
        /// <value>The evaluate function.</value>
        public Func<string, double> EvaluateFunc { get; set; }

        /// <summary>
        /// Performs the evaluation against the specified chromosome.
        /// </summary>
        /// <param name="chromosome">The chromosome to be evaluated.</param>
        /// <returns>The fitness of the chromosome.</returns>
        public double Evaluate(IChromosome chromosome)
        {
            var c = chromosome as GhostwriterChromosome;
            var text = c.BuildText();

            return EvaluateFunc(text);
        }
    }
}
