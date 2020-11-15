using System;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Domain.Selections;

namespace GeneticSharp.Domain.Metaheuristics
{


    public enum MatchingTechnique
    {
        Neighbor,
        Randomize,
        RouletteWheel,
        Best
    }


    /// <summary>
    /// The matching MetaHeuristic offers various techniques to match specific parents for mating and applies the crossover operator to them
    /// </summary>
    public class MatchingMetaHeuristic : ContainerMetaHeuristic
    {

        /// <summary>
        /// This internal class serves exposing methods to generate a random wheel from population and relative individual fitnesses
        /// </summary>
        private abstract class ReuseRouletteWheelSelection : RouletteWheelSelection
        {

            public new static IList<IChromosome> SelectFromWheel(int number, IList<IChromosome> chromosomes,
                IList<double> rouletteWheel, Func<double> getPointer)
            {
                return RouletteWheelSelection.SelectFromWheel(number, chromosomes, rouletteWheel, getPointer);
            }

            public new static void CalculateCumulativePercentFitness(IList<IChromosome> chromosomes,
                IList<double> rouletteWheel)
            {
                RouletteWheelSelection.CalculateCumulativePercentFitness(chromosomes, rouletteWheel);
            }

        }


        public MatchingMetaHeuristic() : this (new DefaultMetaHeuristic()) { }

        public MatchingMetaHeuristic(IMetaHeuristic subMetaHeuristic) : base(subMetaHeuristic) { }


        public List<MatchingTechnique> MatchingProcesses { get; set; }

        public override IList<IChromosome> MatchParentsAndCross(IPopulation population, ICrossover crossover, float crossoverProbability, IList<IChromosome> parents,
            int firstParentIndex)
        {

            if (RandomizationProvider.Current.GetDouble() <= crossoverProbability)
            {

                var firstParent = parents[firstParentIndex];

                var selectedParents = new List<IChromosome>(crossover.ParentsNumber);
                selectedParents.Add(firstParent);
                for (int i = 0; i < crossover.ParentsNumber - 1; i++)
                {
                    var currentMatchingProcess = MatchingProcesses[i];
                    switch (currentMatchingProcess)
                    {
                        case MatchingTechnique.Neighbor:
                            var newParentIdx = firstParentIndex + i;
                            if (newParentIdx < parents.Count)
                            {
                                selectedParents.Add(parents[newParentIdx]);
                            }
                            break;
                        case MatchingTechnique.Randomize:
                            var targetIdx = RandomizationProvider.Current.GetInt(0, parents.Count);
                            selectedParents.Add(parents[i]);
                            break;
                        case MatchingTechnique.RouletteWheel:
                            var currentRoulette = this.GetOrAddContextItem<IList<double>>(false, population, "currentRouletteWheel",
                                () =>
                                {
                                    var tempRoulette = new List<double>(parents.Count);
                                    ReuseRouletteWheelSelection.CalculateCumulativePercentFitness(parents, tempRoulette);
                                    return tempRoulette;
                                });
                            selectedParents.Add(ReuseRouletteWheelSelection.SelectFromWheel(1, parents, currentRoulette,
                                () => RandomizationProvider.Current.GetDouble())[0]);
                            break;
                        case MatchingTechnique.Best:
                            selectedParents.Add(population.CurrentGeneration.BestChromosome);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                // If match the probability cross is made, otherwise the offspring is an exact copy of the parents.
                // Checks if the number of selected parents is equal which the crossover expect, because the in the end of the list we can
                // have some rest chromosomes.
                if (selectedParents.Count == crossover.ParentsNumber && RandomizationProvider.Current.GetDouble() <= crossoverProbability)
                {
                    return crossover.Cross(selectedParents);
                }

            }

            return null;

        }
      
    }
}