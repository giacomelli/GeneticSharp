using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Metaheuristics.Parameters;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Infrastructure.Framework.Collections;

namespace GeneticSharp.Domain.Metaheuristics.Primitives
{
    /// <summary>
    /// The matching MetaHeuristic offers various techniques to match specific parents for mating and applies the crossover operator to them
    /// </summary>
    [DisplayName("Match")]
    public class MatchMetaHeuristic : ContainerMetaHeuristic
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

        public int NumberOfMatches { get; set; }

        /// <summary>
        ///Hyperspeed allows for skiping twin parents with same fitness and skip them assuming they are identical and the offspring will be clones.
        /// This will occur more and more after mode collapse, and accordingly, generations will be accelerated. 
        /// The concept is inspired from game of life runner golly's<see href="http://golly.sourceforge.net/Help/control.html"> Hyperspeed feature</see>, which speeds up generations expontentially on regular patterns by storing the long term evolution.
        /// </summary>
        public bool EnableHyperSpeed { get; set; }

        public MatchMetaHeuristic() : this (1) { }

        public MatchMetaHeuristic(int numberOfMatches) : this(new DefaultMetaHeuristic(), numberOfMatches) { }

        public MatchMetaHeuristic(IMetaHeuristic subMetaHeuristic, int numberOfMatches) : base(subMetaHeuristic)
        {
            NumberOfMatches = numberOfMatches;
            CrossoverProbabilityStrategy = ProbabilityStrategy.TestProbability | ProbabilityStrategy.OverwriteProbability;
        }

        public ParamScope RouletteCachingScope { get; set; } = ParamScope.Generation | ParamScope.MetaHeuristic;

        public List<MatchingTechnique> MatchingTechniques { get; set; }

        public override IList<IChromosome> MatchParentsAndCross(IEvolutionContext ctx, ICrossover crossover, float crossoverProbability, IList<IChromosome> parents)
        {

            if (ShouldRun(crossoverProbability, CrossoverProbabilityStrategy, StaticCrossoverProbability, out var subProbability))
            {
                var toReturn = new List<IChromosome>(NumberOfMatches * crossover.ChildrenNumber);

                for (int matchIndex = 0; matchIndex < NumberOfMatches; matchIndex++)
                {
                    var referenceIndex = ctx.Index + matchIndex;
                    if (referenceIndex<parents.Count)
                    {
                        var firstParent = parents[referenceIndex];
                        var firstGenes = firstParent.GetGenes();
                        var selectedParents = new List<IChromosome>(crossover.ParentsNumber) { firstParent };
                        for (int i = 0; i < crossover.ParentsNumber - 1; i++)
                        {
                            var currentMatchingTechnique = MatchingTechniques[i];
                            AddOneMatch(selectedParents, i + matchIndex + 1, ctx, currentMatchingTechnique, parents);
                        }

                        //var  matchResult = new List<IChromosome>();
                        if (EnableHyperSpeed
                            && selectedParents.All(c => c.Fitness.Value ==  firstParent.Fitness.Value)
                                //Other possibilities (costly)
                                //&& (selectedParents.All(c => c.Equals(firstParent))
                                /*|| selectedParents.All(c => Enumerable.Range(0, c.Length).All(i => c.GetGene(i).Value.Equals(firstGenes[i].Value)))*//*)*/)
                        {
                            break;
                            //Other possibilities to investigate
                            //toReturn.AddRange(Enumerable.Repeat(firstParent, crossover.ChildrenNumber));
                        }
                        else
                        {
                            var subContext = ctx.GetIndividual(0);
                            var matchResult =
                                base.MatchParentsAndCross(subContext, crossover, subProbability, selectedParents);
                            if (matchResult != null)
                            {
                                toReturn.AddRange(matchResult);
                            }
                        }
                    }
                }
                return toReturn;
            }
            return null;
        }

        private void AddOneMatch(IList<IChromosome> selectedParents, int matchIndex, IEvolutionContext ctx, MatchingTechnique currentMatchingTechnique, IList<IChromosome> parents)
        {
            switch (currentMatchingTechnique)
            {
                case MatchingTechnique.Neighbor:
                    var newParentIdx = ctx.Index + matchIndex;
                    if (newParentIdx < parents.Count)
                    {
                        selectedParents.Add(parents[newParentIdx]);
                    }
                    break;
                case MatchingTechnique.Randomize:
                    var targetIdx = RandomizationProvider.Current.GetInt(0, parents.Count);
                    selectedParents.Add(parents[targetIdx]);
                    break;
                case MatchingTechnique.RouletteWheel:
                    var dynamicRouletteParameter = new MetaHeuristicParameter<IList<double>>
                    {
                        Scope = RouletteCachingScope,
                        Generator = (h, c) => {
                            var tempRoulette = new List<double>(parents.Count);
                            ReuseRouletteWheelSelection.CalculateCumulativePercentFitness(parents, tempRoulette);
                            return tempRoulette;
                        }
                    };
                    var currentRoulette = dynamicRouletteParameter.Get(this, ctx, "currentRouletteWheel");
                    selectedParents.Add(ReuseRouletteWheelSelection.SelectFromWheel(1, parents, currentRoulette,
                        () => RandomizationProvider.Current.GetDouble())[0]);
                    break;
                case MatchingTechnique.Best:
                    if (ctx.Population.CurrentGeneration.BestChromosome!=null)
                    {
                        selectedParents.Add(ctx.Population.CurrentGeneration.BestChromosome);
                    }
                    else
                    {
                        var fallbackTargetId = RandomizationProvider.Current.GetInt(0, parents.Count);
                        selectedParents.Add(parents[fallbackTargetId]);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(currentMatchingTechnique), $"Unsupported matching process: {currentMatchingTechnique}");
            }


        }
      
    }
}