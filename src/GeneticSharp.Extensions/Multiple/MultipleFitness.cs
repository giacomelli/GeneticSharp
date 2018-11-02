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

        private readonly Func<IEnumerable<IChromosome>, double> _globalEvaluator;

        /// <summary>
        /// constructor that accepts a child fitness class to be used on child chromosomes
        /// </summary>
        /// <param name="individualFitness">the IFitness class to be used for children chromosomes</param>
        public MultipleFitness(IFitness individualFitness): this(individualFitness, (chromosomes, fitnessFunc) => chromosomes.Sum(fitnessFunc))
        {
        }

        /// <summary>
        /// Constructor that specifies a custom aggregator to the children fitness evaluations together with the individual child fitness implementation
        /// </summary>
        /// <param name="individualFitness">the IFitness class to be used for children chromosomes</param>
        /// <param name="aggregator">the function to aggregate child chromosomes fitness results</param>
        public MultipleFitness(IFitness individualFitness,
            Func<IEnumerable<IChromosome>, Func<IChromosome, double>, double> aggregator)
            : this(chromosomes => ApplyAggregator(chromosomes, individualFitness, aggregator))
        {
        }
       
        /// <summary>
        /// Constructor that specifies a custom global evaluator of the children chromosomes
        /// </summary>
        /// <param name="globalEvaluator">the function to evaluate child chromosomes</param>
        public MultipleFitness(Func<IEnumerable<IChromosome>, double> globalEvaluator)
        {
            _globalEvaluator = globalEvaluator;
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
            return _globalEvaluator(chromosome.Chromosomes);
        }

        /// <summary>
        /// Applies a global aggregator (typically sum, max or average) to child chromosomes fitnesses, given the individual fitness function
        /// </summary>
        /// <param name="childChromosomes">the child chromosomes to be aggregated</param>
        /// <param name="individualFitness">the individual fitness function to evaluate child chromosomes</param>
        /// <param name="aggregator">the global aggregator for the child fitnesses</param>
        /// <returns>an aggregated global fitness</returns>
        private static double ApplyAggregator(IEnumerable<IChromosome> childChromosomes, IFitness individualFitness, Func<IEnumerable<IChromosome>, Func<IChromosome, double>, double> aggregator)
        {
            var chromosomesEnumerated = childChromosomes as IList<IChromosome> ?? childChromosomes.ToList();
            foreach (var childChromosome in chromosomesEnumerated)
            {
                childChromosome.Fitness = individualFitness.Evaluate(childChromosome);
            }

            return aggregator(chromosomesEnumerated, individualFitness.Evaluate);
        }



    }
}