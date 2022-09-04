using System;
using System.Collections;
using System.Linq;

namespace GeneticSharp
{
    /// <summary>
    /// Integer chromosome with binary values (0 and 1).
    /// </summary>
    public class IntegerChromosome : BinaryChromosomeBase
    {
        private readonly int m_minValue;
        private readonly int m_maxValue;
        private readonly BitArray m_originalValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:GeneticSharp.Domain.Chromosomes.IntegerChromosome"/> class.
        /// </summary>
        /// <param name="minValue">Minimum value.</param>
        /// <param name="maxValue">Maximum value.</param>
        public IntegerChromosome(int minValue, int maxValue) : base(32)
        {
            m_minValue = minValue;
            m_maxValue = maxValue;
            var intValue = RandomizationProvider.Current.GetInt(m_minValue, m_maxValue);
            m_originalValue = new BitArray(new int[] { intValue });

            CreateGenes();
        }

        /// <summary>
        /// Generates the gene.
        /// </summary>
        /// <returns>The gene.</returns>
        /// <param name="geneIndex">Gene index.</param>
        public override Gene GenerateGene(int geneIndex)
        {
            var value = m_originalValue[geneIndex];

            return new Gene(value);
        }

        /// <summary>
        /// Creates the new.
        /// </summary>
        /// <returns>The new.</returns>
        public override IChromosome CreateNew()
        {
            return new IntegerChromosome(m_minValue, m_maxValue);
        }

        /// <summary>
        /// Converts the chromosome to its integer representation.
        /// </summary>
        /// <returns>The integer.</returns>
        public int ToInteger()
        {
            var array = new int[1];
            var genes = GetGenes().Select(g => (bool)g.Value).ToArray();
            var bitArray = new BitArray(genes);
            bitArray.CopyTo(array, 0);

            return array[0];
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:GeneticSharp.Domain.Chromosomes.FloatingPointChromosome"/>.
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:GeneticSharp.Domain.Chromosomes.FloatingPointChromosome"/>.</returns>
        public override string ToString()
        {
            return String.Join("", GetGenes().Reverse().Select(g => (bool) g.Value ? "1" : "0").ToArray());
        }

        /// <summary>
        /// Flips the gene.
        /// </summary>
        /// <remarks>>
        /// If gene's value is 0, the it will be flip to 1 and vice-versa.</remarks>
        /// <param name="index">The gene index.</param>
        public override void FlipGene(int index)
        {
            var realIndex = Math.Abs(31 - index);
            var value = (bool)GetGene(realIndex).Value;

            ReplaceGene(realIndex, new Gene(!value));
        }
    }
}

