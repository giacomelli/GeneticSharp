using System;
using System.Collections.Generic;
using System.Diagnostics;
using HelperSharp;
using System.Linq;

namespace GeneticSharp.Domain.Chromosomes
{
	/// <summary>
	/// A base class for chromosomes.
	/// </summary>
	[DebuggerDisplay("{Id}: {Fitness} = {GetGenes()}")]
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
		/// <summary>
		/// Gets the unique identifier.
		/// </summary>
		/// <value>The identifier.</value>
        public string Id { get; private set;  }
      
		/// <summary>
		/// Gets or sets the fitness of the chromosome in the current problem.
		/// </summary>
        public double? Fitness { get; set; }

		/// <summary>
		/// Gets or sets the age.
		/// </summary>
		/// <value>The age.</value>
        public int Age { get; set; }

		/// <summary>
		/// Gets the length, in genes, of the chromosome.
		/// </summary>
        public int Length { get { return m_length; } }
        #endregion

        #region Methods
		/// <summary>
		/// Generates the gene for the specified index.
		/// </summary>
		/// <returns>The gene.</returns>
		/// <param name="geneIndex">Gene index.</param>
        public abstract Gene GenerateGene (int geneIndex);

		/// <summary>
		/// Creates a new chromosome using the same structure of this.
		/// </summary>
		/// <returns>The new chromosome.</returns>
        public abstract IChromosome CreateNew();
	       
		/// <summary>
		/// Replaces the gene in the specified index.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="gene">Gene.</param>
		public void ReplaceGene(int index, Gene gene)
		{
            if (index < 0 || index >= m_length)
			{
				throw new ArgumentOutOfRangeException ("index", "There is no Gene on index {0} to be replaced.".With(index));
			}

            if (gene == null)
            {
                throw new ArgumentNullException("gene", "A gene can't be replaced by a null gene.");
            }

			m_genes [index] = gene;
			Fitness = null;
		}

		/// <summary>
		/// Replaces the genes starting in the specified index.
		/// </summary>
		/// <remarks>The genes to be replaced can't be greater than the available space between the start index and the end of the chromosome.</remarks>
		/// <param name="startIndex">Start index.</param>
		/// <param name="genes">Genes.</param>
		public void ReplaceGenes(int startIndex, Gene[] genes)
		{
            if (startIndex < 0 || startIndex >= m_length)
			{
				throw new ArgumentOutOfRangeException ("index", "There is no Gene on index {0} to be replaced.".With(startIndex));
			}

            ExceptionHelper.ThrowIfNull("genes", genes);

            var genesToBeReplacedLength = genes.Length;

            for (int i = 0; i < genesToBeReplacedLength; i++)
            {
                if(genes[i] == null)
                {
                    throw new ArgumentException("genes", "A gene can't be replaced by a null gene. The gene on index {0} is null.".With(i));
                }
            }

            var availableSpaceLength = (m_length - startIndex);

            if(genesToBeReplacedLength > availableSpaceLength)
            {
                throw new ArgumentException("genes", "The number of genes to be replaced is greater than available space, there is {0} genes between the index {1} and the end of chromosome, but there is {2} genes to be replaced."
                    .With(availableSpaceLength, startIndex, genesToBeReplacedLength));
            }

            Array.Copy(genes, 0, m_genes, startIndex, genes.Length);

			Fitness = null;
		}

		/// <summary>
		/// Gets the gene in the specified index.
		/// </summary>
		/// <returns>The gene.</returns>
		/// <param name="index">Index.</param>
		public Gene GetGene(int index)
		{
			return m_genes[index];
		}

		/// <summary>
		/// Gets the genes.
		/// </summary>
		/// <returns>The genes.</returns>
		public Gene[] GetGenes()
		{
			return m_genes;
		}    
        
		/// <summary>
		/// Compares the current object with another object of the same type.
		/// </summary>
		/// <returns>The to.</returns>
		/// <param name="other">Other.</param>
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

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="GeneticSharp.Domain.Chromosomes.ChromosomeBase"/>.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="GeneticSharp.Domain.Chromosomes.ChromosomeBase"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
		/// <see cref="GeneticSharp.Domain.Chromosomes.ChromosomeBase"/>; otherwise, <c>false</c>.</returns>
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

		/// <param name="first">First.</param>
		/// <param name="second">Second.</param>
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

		/// <param name="first">First.</param>
		/// <param name="second">Second.</param>
        public static bool operator !=(ChromosomeBase first, ChromosomeBase second)
        {
            return !(first == second);
        }

		/// <param name="first">First.</param>
		/// <param name="second">Second.</param>
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

		/// <param name="first">First.</param>
		/// <param name="second">Second.</param>
        public static bool operator >(ChromosomeBase first, ChromosomeBase second)
        {
            return !(first == second) && !(first < second);
        }
        #endregion      
    }
}
