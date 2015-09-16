using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Terminations;

namespace GeneticSharp.Runner.ConsoleApp.Samples
{
    public abstract class SampleControllerBase : ISampleController
    {      
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
        /// Draws the sample;
        /// </summary>
        /// <param name="bestChromosome"></param>
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
        /// <returns></returns>
        public virtual ICrossover CreateCrossover()
        {
            return new UniformCrossover();
        }

        /// <summary>
        /// Creates the mutation.
        /// </summary>
        /// <returns></returns>
        public virtual IMutation CreateMutation()
        {
            return new UniformMutation(true);
        }
    }
}
