using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Reinsertions;
using GeneticSharp.Domain.Selections;

namespace GeneticSharp.Domain.Metaheuristics
{
    /// <summary>
    /// This MetaHeuristic doesn't perform any operation and can be used together with other Metaheuristic to Cancel certain operators
    /// </summary>
    public class EmptyMetaHeuristic : MetaHeuristicBase
    {
        public override IList<IChromosome> SelectParentPopulation(IPopulation population, ISelection selection)
        {
            return null;
        }

        public override IList<IChromosome> MatchParentsAndCross(IPopulation population, ICrossover crossover, float crossoverProbability, IList<IChromosome> parents,
            int firstParentIndex)
        {
            return null;
        }

        public override void MutateChromosome(IPopulation population, IMutation mutation, float mutationProbability, IList<IChromosome> offSprings,
            int offspringIndex)
        {
        }

        public override IList<IChromosome> Reinsert(IPopulation population, IReinsertion reinsertion, IList<IChromosome> offspring, IList<IChromosome> parents)
        {
            return null;
        }
    }
}