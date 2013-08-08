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
        public ChromosomeStub()
        {
            AddGene(GenerateGene(0));
            AddGene(GenerateGene(1));
			AddGene(GenerateGene(2));
			AddGene(GenerateGene(3));
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
