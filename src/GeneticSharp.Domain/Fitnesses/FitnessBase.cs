using GeneticSharp.Domain.Chromosomes;

namespace GeneticSharp.Domain.Fitnesses {
    /// <summary>
    /// The abstract base class for fitness functions
    /// </summary>
    public abstract class FitnessBase : IFitness {
        /// <summary>
        /// Amount of evaluations of the fitness function
        /// </summary>
        public int Evaluations { get; private set; }

        /// <summary>
        /// Performs the evaluation against the specified chromosome.
        /// </summary>
        /// <param name="chromosome">The chromosome to be evaluated.</param>
        /// <returns>The fitness of the chromosome.</returns>
        public double Evaluate(IChromosome chromosome) {
            Evaluations++;
            return PerformEvaluation(chromosome);
        }

        /// <summary>
        /// Performs the evaluation against the specified chromosome.
        /// </summary>
        /// <param name="chromosome">The chromosome to be evaluated.</param>
        /// <returns>The fitness of the chromosome.</returns>
        public abstract double PerformEvaluation(IChromosome chromosome);
    }
}
