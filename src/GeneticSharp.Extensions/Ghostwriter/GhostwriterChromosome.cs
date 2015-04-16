using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Extensions.Ghostwriter
{
    public class GhostwriterChromosome : ChromosomeBase
    {
        private IList<string> m_words;

        public GhostwriterChromosome(int maxTextWordLength, IList<string> words)
            : base(maxTextWordLength)
        {            
            m_words = words;

            for (int i = 0; i < maxTextWordLength; i++)
            {
                ReplaceGene(i, GenerateGene(i));
            }
        }

        public override Gene GenerateGene(int geneIndex)
        {
            return new Gene(m_words[RandomizationProvider.Current.GetInt(0, m_words.Count)]);
        }

        public override IChromosome CreateNew()
        {
            return new GhostwriterChromosome(Length, m_words);
        }

        public override IChromosome Clone()
        {
            return base.Clone();
        }

        public string GetText()
        {
            return String.Join(" ", GetGenes().Select(g => g.Value.ToString()).ToArray());
        }
    }
}
