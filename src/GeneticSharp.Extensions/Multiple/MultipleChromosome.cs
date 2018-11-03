using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneticSharp.Domain.Chromosomes;

namespace GeneticSharp.Extensions.Multiple
{

    /// <summary>
    /// Compound chromosome to artificially increase genetics diversity by evolving a list of chromosome instead of just one.
    /// Sub-genes are inlined into a single compound list of genes
    /// </summary>
    public class MultipleChromosome : ChromosomeBase
    {

        /// <summary>
        /// Child chromosomes are stored in a list
        /// </summary>
        public IList<IChromosome> Chromosomes { get; set; }

        /// <summary>
        /// Constructor accepting a delegate to create chromosomes
        /// </summary>
        /// <param name="createFunc">a function that create child chromosomes from index</param>
        /// <param name="nbChromosomes">Number of child chromosomes to use</param>
        public MultipleChromosome(Func<int, IChromosome> createFunc, int nbChromosomes) : this(new int[nbChromosomes].Select(createFunc).ToList())
        {
        }

        /// <summary>
        /// Constructor accepting a list of child chromosomes
        /// </summary>
        /// <param name="chromosomes"></param>
        public MultipleChromosome(IList<IChromosome> chromosomes) : base(chromosomes.Count * chromosomes[0].Length)
        {
            Chromosomes = chromosomes;
            for (int i = 0; i < Length; i++)
            {
                ReplaceGene(i, GenerateGene(i));
            }

            UpdateSubGenes();
        }

        /// <summary>
        /// Generates the parent genes by inlining child genes 
        /// </summary>
        /// <param name="geneIndex">the gene index in parent chromosome</param>
        /// <returns></returns>
        public override Gene GenerateGene(int geneIndex)
        {
            return Chromosomes[geneIndex / Chromosomes[0].Length]
              .GenerateGene(geneIndex - ((geneIndex / Chromosomes[0].Length) * Chromosomes[0].Length));
        }


        public override IChromosome CreateNew()
        {
            return new MultipleChromosome(Chromosomes.Select(c => c.CreateNew()).ToList());
        }

        /// <summary>
        /// Since the ReplaceGene is not virtual, a call to this method is required at evaluation time
        /// </summary>
        public void UpdateSubGenes()
        {
            for (int i = 0; i < Length; i++)
            {
                Chromosomes[i / Chromosomes[0].Length].ReplaceGene(i - ((i / Chromosomes[0].Length) * Chromosomes[0].Length), GetGene(i));
            }

        }
    }
}
