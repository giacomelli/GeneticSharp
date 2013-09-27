using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;

namespace GeneticSharp.Runner.ConsoleApp.Samples
{
    public interface ISampleController
    {
        /// <summary>
        /// Creates the fitness.
        /// </summary>
        /// <returns>The fitness.</returns>
        IFitness CreateFitness();

        /// <summary>
        /// Creates the chromosome.
        /// </summary>
        /// <returns>The chromosome.</returns>
        IChromosome CreateChromosome();

        /// <summary>
        /// Draws the sample;
        /// </summary>
        void Draw(IChromosome bestChromosome);
    }
}
