using GeneticSharp.Domain.Chromosomes;

namespace GeneticSharp.Domain.Mutations
{
    /// <summary>
    /// Mutation is a genetic operator used to maintain genetic diversity from one generation of a population of genetic algorithm 
    /// chromosomes to the next. It is analogous to biological mutation. Mutation alters one or more gene values in a chromosome from 
    /// its initial state. In mutation, the solution may change entirely from the previous solution. Hence GA can come to better solution 
    /// by using mutation. Mutation occurs during evolution according to a user-definable mutation probability. This probability should be 
    /// set low. If it is set too high, the search will turn into a primitive random search.
    /// <para>
    /// The classic example of a mutation operator involves a probability that an arbitrary bit in a genetic sequence will be changed 
    /// from its original state. A common method of implementing the mutation operator involves generating a random variable for each bit 
    /// in a sequence. This random variable tells whether or not a particular bit will be modified.This mutation procedure, based on the 
    /// biological point mutation, is called single point mutation. Other types are inversion and floating point mutation. When the gene 
    /// encoding is restrictive as in permutation problems, mutations are swaps, inversions, and scrambles.
    /// </para>
    /// <para>
    /// The purpose of mutation in GAs is preserving and introducing diversity. Mutation should allow the algorithm to avoid local minima 
    /// by preventing the population of chromosomes from becoming too similar to each other, thus slowing or even stopping evolution. 
    /// This reasoning also explains the fact that most GA systems avoid only taking the fittest of the population in generating the next 
    /// but rather a random (or semi-random) selection with a weighting toward those that are fitter.
    /// </para>
    /// <see href="http://en.wikipedia.org/wiki/Mutation_(genetic_algorithm)"/> 
    /// </summary>
    public interface IMutation : IChromosomeOperator
    {
        /// <summary>
        /// Mutate the specified chromosome.
        /// </summary>
        /// <param name="chromosome">The chromosome.</param>
        /// <param name="probability">The probability to mutate each chromosome.</param>
        void Mutate(IChromosome chromosome, float probability);
    }
}
