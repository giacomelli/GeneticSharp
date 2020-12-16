using System.Collections.Generic;
using System.ComponentModel;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
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

        public DefaultMetaHeuristic(): base(new NoOpMetaHeuristic()){}
        

        public override IList<IChromosome> ScopedSelectParentPopulation(IEvolutionContext ctx, ISelection selection)
        {
            return selection.SelectChromosomes(ctx.Population.MinSize, ctx.Population.CurrentGeneration);
        }

        public override IList<IChromosome> ScopedMatchParentsAndCross(IEvolutionContext ctx, ICrossover crossover, float crossoverProbability,
            IList<IChromosome> parents)
        {

            // If match the probability cross is made, otherwise the offspring is an exact copy of the parents.
            // Checks if the number of selected parents is equal which the crossover expect, because the in the end of the list we can
            // have some rest chromosomes.
            if (parents.Count - ctx.Index >= crossover.ParentsNumber && RandomizationProvider.Current.GetDouble() <= crossoverProbability)
            {
                var selectedParents = new List<IChromosome>(crossover.ParentsNumber);
                for (int i = 0; i < crossover.ParentsNumber; i++)
                {
                    selectedParents.Add(parents[ctx.Index + i]);
                }
                return crossover.Cross(selectedParents);

            }

            return new List<IChromosome>();
        }

        public override void ScopedMutateChromosome(IEvolutionContext ctx, IMutation mutation, float mutationProbability, IList<IChromosome> offSprings)
        {
            mutation.Mutate(offSprings[ctx.Index], mutationProbability);
        }

        public override IList<IChromosome> ScopedReinsert(IEvolutionContext ctx, IReinsertion reinsertion, IList<IChromosome> offspring, IList<IChromosome> parents)
        {
            return reinsertion.SelectChromosomes(ctx.Population, offspring, parents);
        }
    }


    


}