using GeneticSharp.Domain;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;

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
        /// Creates the termination.
        /// </summary>
        /// <returns>The termination.</returns>
        ITermination CreateTermination();

        /// <summary>
        /// Creates the crossover.
        /// </summary>
        /// <returns>The crossover.</returns>
        ICrossover CreateCrossover();

        /// <summary>
        /// Creates the mutation.
        /// </summary>
        /// <returns>The mutation.</returns>
        IMutation CreateMutation();

        /// <summary>
        /// Creates the selection.
        /// </summary>
        /// <returns>The selection.</returns>
        ISelection CreateSelection();

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Configure the Genetic Algorithm.
        /// </summary>
        /// <param name="ga">The genetic algorithm.</param>
        void ConfigGA(GeneticAlgorithm ga);

        /// <summary>
        /// Draws the sample.
        /// </summary>
        /// <param name="bestChromosome">The current best chromosome</param>
        void Draw(IChromosome bestChromosome);
    }
}
