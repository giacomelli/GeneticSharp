using System;

namespace GeneticSharp.Domain.Chromosomes
{
    /// <summary>
    /// Defines a interface of a binary chromosome of 0 and 1 genes.
    /// </summary>
    public interface IBinaryChromosome : IChromosome
    {
        /// <summary>
        /// Flips the gene.
        /// </summary>
        /// <remarks>>
        /// If gene's value is 0, the it will be flip to 1 and vice-versa.
        /// </remarks>
        /// <param name="index">The gene index.</param>
        void FlipGene(int index);
    }
}

