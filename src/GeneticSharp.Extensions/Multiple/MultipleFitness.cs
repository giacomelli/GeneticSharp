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

        private IFitness _individualFitness;

        public MultipleFitness(IFitness individualFitness)
        {
            _individualFitness = individualFitness;
        }

        public double Evaluate(IChromosome chromosome)
        {
            return Evaluate((MultipleChromosome)chromosome);
        }

        public double Evaluate(MultipleChromosome chromosome)
        {
            chromosome.UpdateSubGenes();
            chromosome.Chromosomes.ForEach(c => c.Fitness = _individualFitness.Evaluate(c));
            return chromosome.Chromosomes.Where(c => c.Fitness.HasValue).Sum(c => c.Fitness.Value);
        }




    }
}