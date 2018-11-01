using System;
using System.Collections.Generic;
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
        private readonly Func<IEnumerable<IChromosome>, Func<IChromosome, double>, double> _aggregator;

        /// <summary>
        /// constructor that accepts a child fitness class to be used on child chromosomes
        /// </summary>
        /// <param name="individualFitness">the IFitness class to be used for children chromosomes</param>
        public MultipleFitness(IFitness individualFitness): this(individualFitness, (chromosomes, fitnessFunc) => chromosomes.Sum(fitnessFunc))
        {
        }

        /// <summary>
        /// Constructor that specifies a custom aggregator to the children fitness evaluations
        /// </summary>
        /// <param name="individualFitness">the IFitness class to be used for children chromosomes</param>
        /// <param name="aggregator">the function to aggregate child chromosomes fitness results</param>
        public MultipleFitness(IFitness individualFitness, Func<IEnumerable<IChromosome>, Func<IChromosome, double>, double> aggregator)
        {
            _individualFitness = individualFitness;
            _aggregator = aggregator;
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
            return _aggregator(chromosome.Chromosomes.Where(c => c.Fitness.HasValue), c => c.Fitness.Value);
        }




    }
}