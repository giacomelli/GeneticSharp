using System;
using System.Collections.Generic;
using System.Diagnostics;
using HelperSharp;

namespace GeneticSharp.Domain.Chromosomes
{
    [DebuggerDisplay("{Id}: {Fitness}")]
    public abstract class ChromosomeBase : IChromosome
    {
		#region Fields
		private List<Gene> m_genes;
		#endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ChromosomeBase"/> class.
        /// </summary>
        protected ChromosomeBase()
        {
            Id = Guid.NewGuid().ToString();
			m_genes = new List<Gene>();
       }
        #endregion

        #region Properties
        public string Id { get; private set;  }
      
        public double? Fitness { get; set; }

        public int Age { get; set; }

		public int Length { get { return m_genes.Count; } }
        #endregion

        #region Methods
        public abstract Gene GenerateGene (int geneIndex);
        public abstract IChromosome CreateNew();

        public void AddGene(Gene gene)
		{
			m_genes.Add (gene);
			Fitness = null;
		}

		public void ReplaceGene(int index, Gene gene)
		{
			if(index < 0 || index >= m_genes.Count)
			{
				throw new ArgumentOutOfRangeException ("index", "There is no Gene on index {0} to be replaced.".With(index));
			}

			m_genes [index] = gene;
			Fitness = null;
		}

		public Gene GetGene(int index)
		{
			return m_genes[index];
		}

		public IList<Gene> GetGenes()
		{
			return m_genes;
		}

		public void AddGenes (IEnumerable<Gene> genes)
		{
			m_genes.AddRange (genes);
			Fitness = null;
		}

		public void ClearGenes ()
		{
			m_genes.Clear ();
			Fitness = null;
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
