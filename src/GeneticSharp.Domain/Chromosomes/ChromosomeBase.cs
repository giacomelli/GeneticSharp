using System;
using System.Collections.Generic;
using System.Diagnostics;
using HelperSharp;
using System.Linq;

namespace GeneticSharp.Domain.Chromosomes
{
    [DebuggerDisplay("{Id}: {Fitness}")]
    public abstract class ChromosomeBase : IChromosome
    {
		#region Fields
		private Gene[] m_genes;
        private int m_length;
		#endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ChromosomeBase"/> class.
        /// <param name="length">The length, in genes, of the chromosome.</param>
        /// </summary>
        protected ChromosomeBase(int length)
        {
            m_length = length;
            Id = Guid.NewGuid().ToString();
			m_genes = new Gene[length];
       }
        #endregion

        #region Properties
        public string Id { get; private set;  }
      
        public double? Fitness { get; set; }

        public int Age { get; set; }

        public int Length { get { return m_length; } }
        #endregion

        #region Methods
        public abstract Gene GenerateGene (int geneIndex);
        public abstract IChromosome CreateNew();
	       
		public void ReplaceGene(int index, Gene gene)
		{
            if (index < 0 || index >= m_length)
			{
				throw new ArgumentOutOfRangeException ("index", "There is no Gene on index {0} to be replaced.".With(index));
			}

			m_genes [index] = gene;
			Fitness = null;
		}

		public void ReplaceGenes(int startIndex, Gene[] genes)
		{
            if (startIndex < 0 || startIndex >= m_length)
			{
				throw new ArgumentOutOfRangeException ("index", "There is no Gene on index {0} to be replaced.".With(startIndex));
			}

			Array.Copy (genes, 0, m_genes, startIndex, genes.Length);
			Fitness = null;
		}

		public Gene GetGene(int index)
		{
			return m_genes[index];
		}

		public Gene[] GetGenes()
		{
			return m_genes;
		}    
        
        public int CompareTo(IChromosome other)
        {
            if (other == null)
            {
                return -1;
            }

            var otherFitness = other.Fitness;

            if (Fitness == otherFitness)
            {
                return 0;
            }

            return Fitness > otherFitness ? 1 : -1;
        }

        public override bool Equals(object obj)
        {
            var other = obj as IChromosome;

            if (other == null)
            {
                return false;
            }

            return CompareTo(other) == 0;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(ChromosomeBase first, ChromosomeBase second)
        {
            if (Object.ReferenceEquals(first, second))
            {
                return true;
            }

            if (((object)first == null) || ((object)second == null))
            {
                return false;
            }

            return first.CompareTo(second) == 0;
        }

        public static bool operator !=(ChromosomeBase first, ChromosomeBase second)
        {
            return !(first == second);
        }

        public static bool operator <(ChromosomeBase first, ChromosomeBase second)
        {
            if (Object.ReferenceEquals(first, second))
            {
                return false;
            }
            else if ((object)first == null)
            {
                return true;
            }
            else if ((object)second == null)
            {
                return false;
            }

            return first.CompareTo(second) < 0;
        }

        public static bool operator >(ChromosomeBase first, ChromosomeBase second)
        {
            return !(first == second) && !(first < second);
        }
        #endregion      
    }
}
