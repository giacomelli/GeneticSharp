using System;
using System.Collections;
using System.Linq;

namespace GeneticSharp
{
    /// <summary>
    /// IntegerArray chromosome, uses binary values (0 and 1).
    /// </summary>
    public class IntegerArrayChromosome : BinaryChromosomeBase
    {
        private readonly int[] m_maxValues;
        private readonly int[] m_totalBits;
        private readonly int[] m_cumulativeBits;
        private readonly BitArray[] m_originalValues;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:GeneticSharp.Domain.Chromosomes.IntegerArrayChromosome"/> class.
        /// </summary>
        /// <param name="maxValue"> array of int[] maximum values; this chromosome assumes that the minvalue >= 0 </param>
        public IntegerArrayChromosome(int[] maxValues, int[] seedingValues = null)
            : base(maxValues.Sum(x => Convert.ToString(x, 2).Length)) {
            int length = maxValues.Length;
            m_maxValues = maxValues;
            m_totalBits = new int[length];
            m_originalValues = new BitArray[length];
            m_cumulativeBits = new int[length];
            int cumulativeBitsSum = 0;
            for (int i = 0; i < length; i++) {
                int bitLength = Convert.ToString(maxValues[i], 2).Length;
                m_totalBits[i] = bitLength;
                cumulativeBitsSum += bitLength;
                m_cumulativeBits[i] = cumulativeBitsSum;

                int intValue = (seedingValues != null && seedingValues.Length == length) ? seedingValues[i] : RandomizationProvider.Current.GetInt(0, maxValues[i]);
                m_originalValues[i] = new BitArray(new int[] { intValue });
            }
            CreateGenes();
        }

        /// <summary>
        /// Generates the gene.
        /// </summary>
        /// <returns>The gene.</returns>
        /// <param name="geneIndex">Gene index.</param>
        public override Gene GenerateGene(int geneIndex) {
            int arrayIndex = Array.BinarySearch(m_cumulativeBits, geneIndex);
            if (arrayIndex < 0) arrayIndex = ~arrayIndex;
            int localIndex = (arrayIndex == 0) ? geneIndex : (geneIndex - m_cumulativeBits[arrayIndex - 1]);
            return new Gene(m_originalValues[arrayIndex][localIndex]);
        }

        /// <summary>
        /// Fast flip of the gene.
        /// </summary>
        /// <param name="index">The Gene index.</param>
        /// If gene's value is true, it will flip it to false and vice-versa.</remarks>
        public override void FlipGene(int index) {
            int arrayIndex = Array.BinarySearch(m_cumulativeBits, index);
            if (arrayIndex < 0) arrayIndex = ~arrayIndex;
            var value = (bool)GetGene(index).Value;
            ReplaceGene(index, new Gene(!value));
        }

        /// <summary>
        /// Creates a new Chromosome.
        /// </summary>
        /// <returns>a new instance of Chromosome of type IntegerArrayChromosome.</returns>
        public override IChromosome CreateNew() {
            return new IntegerArrayChromosome(m_maxValues);
        }

        /// <summary>
        /// Converts a chromosome to an array of integers.
        /// </summary>
        /// <returns>int[] array.</returns>
        public int[] ToIntegerArray() {
            int[] result = new int[m_maxValues.Length];
            int bitCounter = 0;
            for (int i = 0; i < m_maxValues.Length; i++) {
                int[] array = new int[1];
                bool[] genes = new bool[m_totalBits[i]];
                for (int j = 0; j < m_totalBits[i]; j++) {
                    genes[j] = (bool)GetGene(bitCounter + j).Value;
                }
                BitArray bitArray = new BitArray(genes);
                bitArray.CopyTo(array, 0);
                result[i] = Math.Min(array[0], m_maxValues[i]);
                bitCounter += m_totalBits[i];
            }
            return result;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:GeneticSharp.Domain.Chromosomes.IntegerArrayChromosome"/>.
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:GeneticSharp.Domain.Chromosomes.IntegerArrayChromosome"/>.</returns>
        public override string ToString(){
            return String.Join("", GetGenes().Select(g => (bool)g.Value ? "1" : "0").ToArray());
        }
    }

}

