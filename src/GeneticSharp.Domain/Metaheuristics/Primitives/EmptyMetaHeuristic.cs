using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Reinsertions;
using GeneticSharp.Domain.Selections;

namespace GeneticSharp.Domain.Metaheuristics
{


   /// <summary>
    /// This MetaHeuristic doesn't perform any operation at all and returns null elements
    /// </summary>
    public class EmptyMetaHeuristic : MetaHeuristicBase
    {
        public override IList<IChromosome> SelectParentPopulation(IEvolutionContext ctx, ISelection selection)
        {
            return null;
        }

        public override IList<IChromosome> MatchParentsAndCross(IEvolutionContext ctx, ICrossover crossover, float crossoverProbability, IList<IChromosome> parents)
        {
            return null;
        }

        public override void MutateChromosome(IEvolutionContext ctx, IMutation mutation, float mutationProbability, IList<IChromosome> offSprings)
        {
        }

        public override IList<IChromosome> Reinsert(IEvolutionContext ctx, IReinsertion reinsertion, IList<IChromosome> offspring, IList<IChromosome> parents)
        {
            return null;
        }
    }
}