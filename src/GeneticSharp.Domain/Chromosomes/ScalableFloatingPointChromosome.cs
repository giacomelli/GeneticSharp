using GeneticSharp.Domain.Randomizations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneticSharp.Domain.Chromosomes
{
    [Serializable]
    public class ScalableFloatingPointChromosome : BinaryChromosomeBase
    {
        private double[] m_minValue;
        private double[] m_maxValue;
        private int[] m_totalBits;
        private int[] m_chromosomeBits;

        public ScalableFloatingPointChromosome(double[] minValue, double[] maxValue, int[] totalBits, int[] chromosomeBits = null) : base(totalBits.Sum())
        {
            if(minValue.Length != maxValue.Length
                || minValue.Length != totalBits.Length)
            {
                throw new ArgumentOutOfRangeException("All arrays have to be equal length.");
            }
            if(chromosomeBits!=null && totalBits.Sum() != chromosomeBits.Length)
            {
                throw new ArgumentOutOfRangeException("Chromosome bits needs to be the size of totalBits sum.");
            }
            m_minValue = minValue;
            m_maxValue = maxValue;
            m_totalBits = totalBits;
            if (chromosomeBits == null)
            {
                chromosomeBits = new int[totalBits.Sum()];
                var rnd = RandomizationProvider.Current;

                for (int i = 0; i < chromosomeBits.Length; i++)
                {
                    chromosomeBits[i] = rnd.GetInt(0, 2);
                }
            }
            m_chromosomeBits = chromosomeBits;
            CreateGenes();
        }

        public override IChromosome CreateNew()
        {
            return new ScalableFloatingPointChromosome(m_minValue, m_maxValue, m_totalBits);
        }

        public double[] ToFloatingPoints()
        {
            var result = new double[m_minValue.Length];
            int bitsToSkip = 0;
            for(var i = 0; i < m_minValue.Length; ++i)
            {
                var bitsCount = m_totalBits[i];
                double singleNumber = 0;
                for (int k = 0; k < bitsCount; ++k)
                {
                    singleNumber += Math.Pow(2, k) * m_chromosomeBits[bitsToSkip + bitsCount - k - 1];
                }
                bitsToSkip += bitsCount;
                var scaledNumber = (m_minValue[i] + (m_maxValue[i] - m_minValue[i]) / (Math.Pow(2, bitsCount) - 1) * singleNumber);
                result[i] = scaledNumber;
            }
            return result;
        }

        public override string ToString()
        {
            return String.Join("", GetGenes().Select(g => g.Value.ToString()).ToArray());
        }

        public override Gene GenerateGene(int geneIndex)
        {
            return new Gene(Convert.ToInt32(m_chromosomeBits[geneIndex].ToString()));
        }
    }
}
