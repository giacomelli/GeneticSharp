using System;
using System.Linq;

namespace GeneticSharp
{
    /// <summary>
    /// A base class for binary chromosome of 0 and 1 genes.
    /// </summary>
    public abstract class BinaryChromosomeBase : ChromosomeBase, IBinaryChromosome
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.BinaryChromosomeBase"/> class.
        /// </summary>
        /// <param name="length">The length, in genes, of the chromosome.</param>
        protected BinaryChromosomeBase(int length) 
            : base(length)
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Flips the gene.
        /// </summary>
        /// <remarks>>
        /// If gene's value is 0, the it will be flip to 1 and vice-versa.</remarks>
        /// <param name="index">The gene index.</param>
        public virtual void FlipGene (int index)    
        {
            var value = (int) GetGene (index).Value;

            ReplaceGene (index, new Gene (value == 0 ? 1 : 0));
        }

        /// <summary>
        /// Generates the gene for the specified index.
        /// </summary>
        /// <returns>The gene.</returns>
        /// <param name="geneIndex">Gene index.</param>
        public override Gene GenerateGene (int geneIndex)
        {
            return new Gene (RandomizationProvider.Current.GetInt (0, 2));
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="GeneticSharp.BinaryChromosomeBase"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="GeneticSharp.BinaryChromosomeBase"/>.</returns>
        public override string ToString ()
        {
            return String.Join (string.Empty, GetGenes ().Select (g => g.Value.ToString()).ToArray());
        }
        #endregion
    }
}

