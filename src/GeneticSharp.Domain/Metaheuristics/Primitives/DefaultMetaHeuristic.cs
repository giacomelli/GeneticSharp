using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Domain.Reinsertions;
using GeneticSharp.Domain.Selections;

namespace GeneticSharp.Domain.Metaheuristics
{
    /// <summary>
    /// This meta heuristic does not do any extra steps from the baseline GA. For crossover it pick parents in the original selection order and applies the crossover operator depending on the given probabiliy.
    /// For mutation it applies the input mutator with given probability to the target chromosome, and for reinsertion, the reinsertion operator is also simply applied.
    /// </summary>
    public class DefaultMetaHeuristic : MetaHeuristicBase
    {
        
        public override IList<IChromosome> SelectParentPopulation(IMetaHeuristicContext ctx, ISelection selection)
        {
            return selection.SelectChromosomes(ctx.Population.MinSize, ctx.Population.CurrentGeneration);
        }

        public override IList<IChromosome> MatchParentsAndCross(IMetaHeuristicContext ctx, ICrossover crossover, float crossoverProbability, IList<IChromosome> parents,
            int firstParentIndex)
        {
            //var selectedParents = parents.Skip(firstParentIndex).Take(crossover.ParentsNumber).ToList();

            // If match the probability cross is made, otherwise the offspring is an exact copy of the parents.
            // Checks if the number of selected parents is equal which the crossover expect, because the in the end of the list we can
            // have some rest chromosomes.
            if (parents.Count - firstParentIndex >= crossover.ParentsNumber && RandomizationProvider.Current.GetDouble() <= crossoverProbability)
            {
                var selectedParents = new List<IChromosome>(crossover.ParentsNumber);
                for (int i = 0; i < crossover.ParentsNumber; i++)
                {
                    selectedParents.Add(parents[firstParentIndex + i]);
                }
                return crossover.Cross(selectedParents);

            }

            return null;
        }

        public override void MutateChromosome(IMetaHeuristicContext ctx, IMutation mutation, float mutationProbability, IList<IChromosome> offSprings,
            int offspringIndex)
        {
            mutation.Mutate(offSprings[offspringIndex], mutationProbability);
        }

        public override IList<IChromosome> Reinsert(IMetaHeuristicContext ctx, IReinsertion reinsertion, IList<IChromosome> offspring, IList<IChromosome> parents)
        {
            return reinsertion.SelectChromosomes(ctx.Population, offspring, parents);
        }
    }
}