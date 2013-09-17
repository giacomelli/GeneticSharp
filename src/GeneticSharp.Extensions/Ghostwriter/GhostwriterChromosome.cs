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
        public GhostwriterChromosome()
            : base(140)
        {
            for (int i = 0; i < 140; i++)
            {
                ReplaceGene(i, GenerateGene(i));
            }
        }

        public override Gene GenerateGene(int geneIndex)
        {
            return new Gene((char)RandomizationProvider.Current.GetInt(65, 92));
        }

        public override IChromosome CreateNew()
        {
            return new GhostwriterChromosome();
        }

        public override IChromosome Clone()
        {
            return base.Clone();
        }

        public string GetText()
        {
            return new string(GetGenes().Select(g => (char) g.Value).ToArray()).Replace("[", " ");
        }
    }
}
