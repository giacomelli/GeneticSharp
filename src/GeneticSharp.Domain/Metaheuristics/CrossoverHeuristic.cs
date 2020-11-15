using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Populations;

namespace GeneticSharp.Domain.Metaheuristics
{
    /// <summary>
    /// Metaheuristic to provide a specific Crossover operator
    /// </summary>
    public class CrossoverHeuristic : ContainerMetaHeuristic
    {


        public ICrossover Crossover { get; set; }

        public CrossoverHeuristic() : base() { }

        public CrossoverHeuristic(ICrossover crossover) : base()
        {
            Crossover = crossover;
        }


        public override IList<IChromosome> MatchParentsAndCross(IPopulation population, ICrossover crossover, float crossoverProbability, IList<IChromosome> parents,
            int firstParentIndex)
        {
            return SubMetaHeuristic.MatchParentsAndCross(population, Crossover, crossoverProbability, parents,
                firstParentIndex);
        }
    }
}