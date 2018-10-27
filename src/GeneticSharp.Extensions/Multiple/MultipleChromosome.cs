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

      public List<IChromosome> Chromosomes { get; set; }

      public MultipleChromosome(Func<IChromosome> createFunc, int nbChromosomes) : this(new int[nbChromosomes].Select(x => createFunc()).ToList())
      {
      }

      public MultipleChromosome(List<IChromosome> chromosomes) : base(chromosomes.Count * chromosomes[0].Length)
      {
         Chromosomes = chromosomes;
         for (int i = 0; i < Length; i++)
         {
            ReplaceGene(i, GenerateGene(i));
         }

         UpdateSubGenes();
      }

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
