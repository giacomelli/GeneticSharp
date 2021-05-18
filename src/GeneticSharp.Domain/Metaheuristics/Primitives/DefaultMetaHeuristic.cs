using System.Collections.Generic;
using System.ComponentModel;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Metaheuristics.Matching;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Domain.Reinsertions;
using GeneticSharp.Domain.Selections;

namespace GeneticSharp.Domain.Metaheuristics.Primitives
{
    /// <summary>
    /// This meta heuristic does not do any extra steps from the baseline GeneticAlgorithm. For crossover it pick parents in the original selection order and applies the crossover operator depending on the given probabiliy.
    /// For mutation it applies the input mutator with given probability to the target chromosome, and for reinsertion, the reinsertion operator is also simply applied.
    /// </summary>
    [DisplayName("Default")]
    public class DefaultMetaHeuristic : ScopedMetaHeuristic
    {
        private MatchMetaHeuristic _matchMetaHeuristic;

        public DefaultMetaHeuristic(): base(new NoOpMetaHeuristic()){}

        /// <summary>
        /// The default metaheuristic, which recycles the original Operator strategy routine, can instead use a dedicated match meta heuristic offering more flexibility in the match making process
        /// Simply touch the property at config time to trigger the creation of the metaheuristic and subsequent use.
        /// </summary>
        public MatchMetaHeuristic MatchMetaHeuristic
        {
            get
            {
                if (_matchMetaHeuristic == null)
                {
                    lock (this)
                    {
                        _matchMetaHeuristic = new MatchMetaHeuristic().WithMatches(MatchingKind.Current, MatchingKind.Random);
                    }
                }
                return _matchMetaHeuristic;
            }
        }

        protected override IList<IChromosome> ScopedSelectParentPopulation(IEvolutionContext ctx, ISelection selection)
        {
            return selection.SelectChromosomes(ctx.Population.MinSize, ctx.Population.CurrentGeneration);
        }

        protected override IList<IChromosome> ScopedMatchParentsAndCross(IEvolutionContext ctx, ICrossover crossover, float crossoverProbability,
            IList<IChromosome> parents)
        {
            if (_matchMetaHeuristic == null)
            {
                // If match the probability cross is made, otherwise the offspring is an exact copy of the parents.
                // Checks if the number of selected parents is equal which the crossover expect, because the in the end of the list we can
                // have some rest chromosomes.
                if (parents.Count - ctx.LocalIndex >= crossover.ParentsNumber && RandomizationProvider.Current.GetDouble() <= crossoverProbability)
                {
                    var selectedParents = new List<IChromosome>(crossover.ParentsNumber);
                    for (int i = 0; i < crossover.ParentsNumber; i++)
                    {
                        selectedParents.Add(parents[ctx.LocalIndex + i]);
                    }
                    return crossover.Cross(selectedParents);

                }
            }
            else
            {
                return _matchMetaHeuristic.MatchParentsAndCross(ctx, crossover, crossoverProbability, parents);
            }
            

            return new List<IChromosome>();
        }

        protected override void ScopedMutateChromosome(IEvolutionContext ctx, IMutation mutation, float mutationProbability, IList<IChromosome> offSprings)
        {
            mutation.Mutate(offSprings[ctx.LocalIndex], mutationProbability);
        }

        protected override IList<IChromosome> ScopedReinsert(IEvolutionContext ctx, IReinsertion reinsertion, IList<IChromosome> offspring, IList<IChromosome> parents)
        {
            return reinsertion.SelectChromosomes(ctx.Population, offspring, parents);
        }
    }


    


}