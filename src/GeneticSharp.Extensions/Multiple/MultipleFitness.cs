using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;

namespace GeneticSharp.Extensions.Multiple
{

    /// <summary>
    /// Fitness class that can evaluate a compound chromosome by summing over the evaluation of its sub-chromosomes. 
    /// </summary>
    public class MultipleFitness : IFitness
    {

        private readonly IFitness _individualFitness;

        /// <summary>
        /// constructor that accepts a child fitness class to be used on child chromosomes
        /// </summary>
        /// <param name="individualFitness"></param>
        public MultipleFitness(IFitness individualFitness)
        {
            _individualFitness = individualFitness;
        }

        public double Evaluate(IChromosome chromosome)
        {
            return Evaluate((MultipleChromosome)chromosome);
        }

        /// <summary>
        /// Sums over the child chromosome fitnesses to obtain the global fitness
        /// </summary>
        /// <param name="chromosome"></param>
        /// <returns></returns>
        public double Evaluate(MultipleChromosome chromosome)
        {
            chromosome.UpdateSubGenes();
         foreach (var childChromosome in chromosome.Chromosomes)
         {
          childChromosome.Fitness = _individualFitness.Evaluate(childChromosome);
         }
            return chromosome.Chromosomes.Where(c => c.Fitness.HasValue).Sum(c => c.Fitness.Value);
        }




    }
}