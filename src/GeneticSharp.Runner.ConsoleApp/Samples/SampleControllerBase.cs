using GeneticSharp.Domain;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;

namespace GeneticSharp.Runner.ConsoleApp.Samples
{
    public abstract class SampleControllerBase : ISampleController
    {
        /// <summary>
        /// Gets the Genetic Algorithm.
        /// </summary>
        /// <value>The Genetic Algorithm.</value>
        protected GeneticAlgorithm GA { get; private set; }

        /// <summary>
        /// Creates the chromosome.
        /// </summary>
        /// <returns>
        /// The chromosome.
        /// </returns>
        public abstract IChromosome CreateChromosome();

        /// <summary>
        /// Creates the fitness.
        /// </summary>
        /// <returns>
        /// The fitness.
        /// </returns>
        public abstract IFitness CreateFitness();

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public virtual void Initialize()
        {
        }

        /// <summary>
        /// Configure the Genetic Algorithm.
        /// </summary>
        /// <param name="ga">The genetic algorithm.</param>
        public virtual void ConfigGA(GeneticAlgorithm ga)
        {
            GA = ga;
        }

        /// <summary>
        /// Draws the sample.
        /// </summary>
        /// <param name="bestChromosome">The current best chromosome</param>
        public abstract void Draw(IChromosome bestChromosome);

        /// <summary>
        /// Creates the termination.
        /// </summary>
        /// <returns>
        /// The termination.
        /// </returns>
        public virtual ITermination CreateTermination()
        {
            return new FitnessStagnationTermination(100);
        }

        /// <summary>
        /// Creates the crossover.
        /// </summary>
        /// <returns>The crossover.</returns>
        public virtual ICrossover CreateCrossover()
        {
            return new UniformCrossover();
        }

        /// <summary>
        /// Creates the mutation.
        /// </summary>
        /// <returns>The mutation.</returns>
        public virtual IMutation CreateMutation()
        {
            return new UniformMutation(true);
        }

        /// <summary>
        /// Creates the selection.
        /// </summary>
        /// <returns>The selection.</returns>
        public virtual ISelection CreateSelection()
        {
            return new EliteSelection();
        }
    }
}
