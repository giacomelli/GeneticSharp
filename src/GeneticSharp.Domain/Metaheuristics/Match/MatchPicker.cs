using System;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Metaheuristics.Parameters;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Domain.Selections;

namespace GeneticSharp.Domain.Metaheuristics.Matching
{
    public class MatchPicker
    {



        public MatchPicker()
        {
            
        }

        public MatchPicker(params (int nbPicks, MatchingKind kind, ParamScope cachingScope)[] matchingKinds)
        {
            MatchPicks = matchingKinds.Select(m => new MatchingSettings() { MatchingKind = m.kind, AdditionalPicks = m.nbPicks - 1, CachingScope = m.cachingScope }).ToList();
        }
      

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

        public ParamScope RouletteCachingScope { get; set; } = ParamScope.Generation | ParamScope.MetaHeuristic;

        public List<MatchingSettings> MatchPicks { get; set; } = new List<MatchingSettings>();

        public IMetaHeuristic ChildMetaHeuristic { get; set; }

        public List<List<MatchingSettings>> CustomMatch { get; set; }

        public IList<IChromosome> SelectMatches(IMetaHeuristic h, IEvolutionContext ctx, int referenceIndex, ICrossover crossover, IList<IChromosome> parents)
        {
            return SelectMatches(MatchPicks, h, ctx, referenceIndex, crossover, parents);
        }

        public IList<IChromosome> SelectMatches(IList<MatchingSettings> settings, IMetaHeuristic h, IEvolutionContext ctx, int referenceIndex, ICrossover crossover, IList<IChromosome> parents)
        {
            var selectedParents = new List<IChromosome>(settings.Count);
            var techniqueCounters = MatchingSettings.GetTechniqueCounter();
            for (int i = 0; i < settings.Count; i++)
            {
                var currentMatchingSettings = settings[i];
                AddOneMatch(h, ctx, selectedParents, referenceIndex, techniqueCounters, currentMatchingSettings, crossover,
                    parents);
            }

            return selectedParents;
        }

        private void AddOneMatch(IMetaHeuristic mh, IEvolutionContext ctx, IList<IChromosome> selectedParents, int referenceIndex,
            int[] techniqueCounters, MatchingSettings currentMatchingSettings, ICrossover crossover,
            IList<IChromosome> parents)
        {
            IList<IChromosome> currentMatches;
            if (currentMatchingSettings.CachingScope != ParamScope.None)
            {
                var dynamicMatchParameter = new MetaHeuristicParameter<IList<IChromosome>>
                {
                    Scope = currentMatchingSettings.CachingScope,
                    Generator = (h, c) => GetMatchesUncached(h, ctx, referenceIndex, techniqueCounters, currentMatchingSettings,
                        crossover, parents)
                };
                currentMatches = dynamicMatchParameter.Get(mh, ctx, $"currentMatches{currentMatchingSettings.MatchingKind}");
            }
            else
            {
                currentMatches = GetMatchesUncached(mh, ctx, referenceIndex, techniqueCounters, currentMatchingSettings,
                    crossover, parents);
            }

            foreach (var currentMatch in currentMatches)
            {
                selectedParents.Add(currentMatch);
            }

        }


        private IList<IChromosome> GetMatchesUncached(IMetaHeuristic mh, IEvolutionContext ctx, int referenceIndex,
            int[] techniqueCounters, MatchingSettings currentMatchingSettings, ICrossover crossover,
            IList<IChromosome> parents)
        {
            var selectedParents = new List<IChromosome>();
            int techniqueIndex;
            if (currentMatchingSettings.ResetIndex)
            {
                techniqueCounters[(int)currentMatchingSettings.MatchingKind] = 0;
                techniqueIndex = 0;
            }
            else
            {
                techniqueIndex = techniqueCounters[(int)currentMatchingSettings.MatchingKind];
                techniqueCounters[(int)currentMatchingSettings.MatchingKind] += currentMatchingSettings.AdditionalPicks + 1;
            }
            switch (currentMatchingSettings.MatchingKind)
            {
                case MatchingKind.Current:

                    var currentIdx = referenceIndex;
                    if (currentIdx < parents.Count)
                    {
                        if (currentMatchingSettings.AdditionalPicks == 0)
                        {
                            selectedParents.Add(parents[currentIdx]);
                        }
                        else
                        {
                            selectedParents.AddRange(Enumerable.Repeat(parents[currentIdx], currentMatchingSettings.AdditionalPicks + 1));
                        }
                    }
                    break;
                case MatchingKind.Neighbor:
                    var newParentIdx = referenceIndex + techniqueIndex + 1;
                    if (currentMatchingSettings.AdditionalPicks == 0)
                    {
                        if (newParentIdx < parents.Count)
                        {
                            selectedParents.Add(parents[newParentIdx]);
                        }
                    }
                    else
                    {
                        if (newParentIdx + currentMatchingSettings.AdditionalPicks < parents.Count)
                        {
                            selectedParents.AddRange(parents.Skip(newParentIdx).Take(currentMatchingSettings.AdditionalPicks + 1));
                        }
                    }
                    break;
                case MatchingKind.Random:
                    if (currentMatchingSettings.AdditionalPicks == 0)
                    {
                        int targetIdx;
                        if (currentMatchingSettings.RandomIncludesCurrent)
                        {
                            targetIdx = RandomizationProvider.Current.GetInt(0, parents.Count);
                        }
                        else
                        {
                            targetIdx = RandomizationProvider.Current.GetInt(0, parents.Count - 1);
                            if (targetIdx == referenceIndex)
                            {
                                targetIdx = parents.Count-1;
                            }
                        }
                        
                        
                        selectedParents.Add(parents[targetIdx]);
                    }
                    else
                    {
                        int[] targetIndices;
                        if (currentMatchingSettings.RandomIncludesCurrent)
                        {
                            targetIndices = RandomizationProvider.Current.GetInts(currentMatchingSettings.AdditionalPicks + 1, 0, parents.Count);
                        }
                        else
                        {
                            targetIndices = RandomizationProvider.Current.GetInts(currentMatchingSettings.AdditionalPicks + 1, 0, parents.Count-1);
                            for (int i = 0; i < targetIndices.Length; i++)
                            {
                                if (targetIndices[i] == referenceIndex)
                                {
                                    targetIndices[i] = parents.Count - 1;
                                }
                            }
                        }
                       
                        selectedParents.AddRange(targetIndices.Select(targetIdx => parents[targetIdx]));
                    }
                    break;
                case MatchingKind.RouletteWheel:
                    var dynamicRouletteParameter = new MetaHeuristicParameter<IList<double>>
                    {
                        Scope = RouletteCachingScope,
                        Generator = (h, c) =>
                        {
                            var tempRoulette = new List<double>(parents.Count);
                            ReuseRouletteWheelSelection.CalculateCumulativePercentFitness(parents, tempRoulette);
                            return tempRoulette;
                        }
                    };
                    var currentRoulette = dynamicRouletteParameter.Get(mh, ctx, "currentRouletteWheel");

                    if (currentMatchingSettings.AdditionalPicks == 0)
                    {
                        selectedParents.Add(ReuseRouletteWheelSelection.SelectFromWheel(1, parents, currentRoulette,
                            () => RandomizationProvider.Current.GetDouble())[0]);
                    }
                    else
                    {
                        selectedParents.AddRange(ReuseRouletteWheelSelection.SelectFromWheel(currentMatchingSettings.AdditionalPicks + 1, parents, currentRoulette,
                            () => RandomizationProvider.Current.GetDouble()));
                    }
                    break;
                case MatchingKind.Best:
                    if (techniqueIndex == 0 && currentMatchingSettings.AdditionalPicks == 0)
                    {
                        if (ctx.Population.CurrentGeneration.BestChromosome != null)
                        {
                            selectedParents.Add(ctx.Population.CurrentGeneration.BestChromosome);
                        }
                        else
                        {
                            var fallbackTargetId = RandomizationProvider.Current.GetInt(0, parents.Count);
                            selectedParents.Add(parents[fallbackTargetId]);
                        }
                    }
                    else
                    {
                        var picks = ctx.Population.CurrentGeneration.GetBestChromosomes(techniqueIndex +
                            currentMatchingSettings.AdditionalPicks + 1);
                        selectedParents.AddRange(picks.Skip(techniqueIndex));
                    }
                    break;
                case MatchingKind.Worst:
                    var worstPicks = ctx.Population.CurrentGeneration.GetWorstChromosomes(techniqueIndex +
                        currentMatchingSettings.AdditionalPicks + 1);
                    selectedParents.AddRange(worstPicks.Skip(techniqueIndex));
                    break;
                case MatchingKind.Child:
                    if (ChildMetaHeuristic == null)
                    {
                        throw new ArgumentOutOfRangeException(nameof(currentMatchingSettings),
                            $"ChildMetaHeuristic cannot be null for matching technique {currentMatchingSettings}");
                    }
                    for (int i = 0; i < currentMatchingSettings.AdditionalPicks + 1; i++)
                    {
                        var matchResult = ChildMetaHeuristic.MatchParentsAndCross(ctx, crossover, 1, parents);
                        if (matchResult == null || matchResult.Count == 0)
                        {
                            throw new ApplicationException("child match heuristic didn't produce any offspring");
                        }
                        selectedParents.AddRange(matchResult);
                    }
                    break;
                case MatchingKind.Custom:
                    if (CustomMatch == null)
                    {
                        throw new ArgumentOutOfRangeException(nameof(currentMatchingSettings),
                            $"CustomMatch cannot be null for matching technique {currentMatchingSettings}");
                    }
                    for (int i = 0; i < currentMatchingSettings.AdditionalPicks + 1; i++)
                    {
                        var subContext = ctx;
                        var subReference = referenceIndex;
                        var stepParents = parents;
                        foreach (var stepMatches in CustomMatch)
                        {
                            stepParents = SelectMatches(stepMatches, mh, subContext, subReference, crossover, stepParents);
                            subContext = subContext.GetLocal(0);
                            subContext.SelectedParents = stepParents;
                            subReference = 0;
                        }
                        selectedParents.AddRange(stepParents);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(currentMatchingSettings),
                        $"Unsupported matching process: {currentMatchingSettings}");
            }

            return selectedParents;
        }

    }
}