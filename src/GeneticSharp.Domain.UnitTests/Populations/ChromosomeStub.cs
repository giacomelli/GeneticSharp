using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Domain.UnitTests
{
    public class ChromosomeStub : ChromosomeBase
    {
        public ChromosomeStub() : base(4)
        {
            ReplaceGene(0, GenerateGene(0));
			ReplaceGene(1, GenerateGene(1));
			ReplaceGene(2, GenerateGene(2));
			ReplaceGene(3, GenerateGene(3));
        }

        public override Gene GenerateGene(int geneIndex)
        {
            return new Gene(RandomizationProvider.Current.GetInt(0, 6));
        }

        public override IChromosome CreateNew()
        {
            return new ChromosomeStub();
        }
    }
}
