using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticSharp.Domain.UnitTests.Selections
{
    class SelectionStubChromosome : ChromosomeBase
    {
        public SelectionStubChromosome() : base(2)
        {
            Fitness = 0;
        }

        public override IChromosome CreateNew() => new SelectionStubChromosome();

        public override Gene GenerateGene(int geneIndex) => new Gene(0);
    }
}
