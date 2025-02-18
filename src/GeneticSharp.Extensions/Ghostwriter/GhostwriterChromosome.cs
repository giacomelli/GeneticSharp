﻿using System.Collections.Generic;
using System.Linq;

namespace GeneticSharp.Extensions
{
    /// <summary>
    /// Ghostwriter chromosome.
    /// </summary>
    public sealed class GhostwriterChromosome : ChromosomeBase
    {
        #region Fields
        private readonly IList<string> m_words;
        #endregion 

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Extensions.GhostwriterChromosome"/> class.
        /// </summary>
        /// <param name="maxTextWordLength">Max text word length.</param>
        /// <param name="words">The words.</param>
        public GhostwriterChromosome(int maxTextWordLength, IList<string> words)
            : base(maxTextWordLength)
        {
            m_words = words;

            for (int i = 0; i < maxTextWordLength; i++)
            {
                ReplaceGene(i, GenerateGene(i));
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Generates the gene for the specified index.
        /// </summary>
        /// <returns>The gene.</returns>
        /// <param name="geneIndex">Gene index.</param>
        public override Gene GenerateGene(int geneIndex)
        {
            return new Gene(m_words[RandomizationProvider.Current.GetInt(0, m_words.Count)]);
        }

        /// <summary>
        /// Creates a new chromosome using the same structure of this.
        /// </summary>
        /// <returns>The new chromosome.</returns>
        public override IChromosome CreateNew()
        {
            return new GhostwriterChromosome(Length, m_words);
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <returns>The text.</returns>
        public string BuildText()
        {
            return string.Join(" ", GetGenes().Select(g => g.Value!.ToString()).ToArray());
        }
        #endregion
    }
}
