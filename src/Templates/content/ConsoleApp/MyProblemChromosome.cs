using GeneticSharp.Domain.Chromosomes;
using System;

namespace ConsoleApp
{
    class MyProblemChromosome : ChromosomeBase
    {
        // TODO: Change the argument value passed to base construtor to change the length 
        // of your chromosome.
        public MyProblemChromosome() 
            : base(10)
        {
            CreateGenes();
        }

        public override Gene GenerateGene(int geneIndex)
        {
            throw new NotImplementedException("// TODO: Generate a gene base on MyProblemChromosome representation.");
        }

        public override IChromosome CreateNew()
        {
            return new MyProblemChromosome();
        }
    }
}
