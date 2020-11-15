using System;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Infrastructure.Framework.Collections;

namespace GeneticSharp.Domain.Chromosomes
{
    /// <summary>
    /// A EukaryoteChromosome is a child chromosome made from a parent individual. It keeps track of the parent chromosome and a complete individual can be built back from a Karyotype of EukaryoteChromosomes
    /// </summary>
    public class EukaryoteChromosome : ChromosomeBase, IChromosome
    {

        /// <summary>
        /// The parent chromosome this child chromosome belongs to
        /// </summary>
        public IChromosome ParentIndividual { get; set; }

        /// <summary>
        /// The gene start index of the child chromosome in its parent
        /// </summary>
        public int StartGeneIndex { get; set; }

        public EukaryoteChromosome(IChromosome parent, int startGeneIdx, int length) : base(length)
        {
            ParentIndividual = parent;
            StartGeneIndex = startGeneIdx;
            var parentGenes = new System.ArraySegment<Gene>(ParentIndividual.GetGenes(), startGeneIdx, length) ;
           
            this.ReplaceGenes(0, parentGenes.ToArray());
           
            if (parent.Fitness.HasValue)
            {
                Fitness = parent.Fitness;
            }
        }
        
        
        public override Gene GenerateGene(int geneIndex)
        {
            if (geneIndex>=Length)
            {
                throw new ApplicationException("Eukaryote chromosme size overflow");
            }
            return ParentIndividual.GenerateGene(StartGeneIndex + geneIndex);
        }


        public override IChromosome CreateNew()
        {
            return new EukaryoteChromosome(ParentIndividual, StartGeneIndex, Length);
        }

       

        /// <summary>
        /// Extracts child Eurokaryote chromosomes from a parent individual, givent the length of child chromosomes
        /// </summary>
        /// <param name="parentIndividual">the parent chromosome to slice</param>
        /// <param name="subChromosomeLengths">The list of lengths of the child chromosomes</param>
        /// <returns></returns>
        public static IList<IChromosome> GetKaryotype(IChromosome parentIndividual, IList<int> subChromosomeLengths)
        {
            var toReturn = new List<IChromosome>(subChromosomeLengths.Count);
            var currentGeneIdx = 0;
            foreach (var subChromosomeLength in subChromosomeLengths)
            {
                toReturn.Add(new EukaryoteChromosome(parentIndividual, currentGeneIdx, subChromosomeLength));
                currentGeneIdx += subChromosomeLength;
            }

            return toReturn;
        }

        /// <summary>
        /// Takes a parent population, splits individuals into karyotypes, and groups child chromosomes to return the child populations for genetic operations
        /// </summary>
        /// <param name="parents"></param>
        /// <param name="subChromosomeLengths"></param>
        /// <returns></returns>
        public static IList<IList<IChromosome>> GetSubPopulations(IEnumerable<IChromosome> parents, IList<int> subChromosomeLengths)
        {
            var karyotypes = parents.Select(parent => EukaryoteChromosome.GetKaryotype(parent, subChromosomeLengths));
            var subPopulations = Enumerable.Range(0, subChromosomeLengths.Count)
                .Select(i => (IList<IChromosome>)karyotypes.Select(p => p[i]).ToList()).ToList();
            return subPopulations;
        }

        /// <summary>
        /// Updates the parent genes from the current child
        /// </summary>
        public void UpdateParent()
        {
            this.ParentIndividual.ReplaceGenes(StartGeneIndex, GetGenes());
        }

        /// <summary>
        /// Update the parent genes from a collection of Eukaryote children
        /// </summary>
        /// <param name="children"></param>
        public static void UpdateParent(IList<IChromosome> children)
        {
            children.Each(objEukaryoteChromomosome => ((EukaryoteChromosome)objEukaryoteChromomosome).UpdateParent());
        }

        /// <summary>
        /// Builds a complete individual back from its karyotype
        /// </summary>
        /// <param name="karyotype">the karyotype of child chromosomes for the individual to build</param>
        /// <returns></returns>
        public static IChromosome GetNewIndividual(IList<EukaryoteChromosome> karyotype)
        {
            var newParent = (karyotype[0]).ParentIndividual.CreateNew();
            foreach (var subChromosome in karyotype)
            {
                newParent.ReplaceGenes(subChromosome.StartGeneIndex, subChromosome.GetGenes());
            }

            return newParent;
        }

        /// <summary>
        /// Builds a population of parent individual chromosome back from a list of population of child chromosomes
        /// </summary>
        /// <param name="subPopulations"></param>
        /// <returns></returns>
        public static IList<IChromosome> GetNewIndividuals(IList<IList<IChromosome>> subPopulations)
        {
            var toReturn = new List<IChromosome>();
            var karyotypes = Enumerable.Range(0, subPopulations[0].Count)
                .Select(i => subPopulations.Select(subPopulation => subPopulation[i]).Cast<EukaryoteChromosome>().ToList()).ToList();
            foreach (var karyotype in karyotypes)
            {
                var newParent = GetNewIndividual(karyotype);
                toReturn.Add(newParent);
            }

            return toReturn;
        }

    }
}