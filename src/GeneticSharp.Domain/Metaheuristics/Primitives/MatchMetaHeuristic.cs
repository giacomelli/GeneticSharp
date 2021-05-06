using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Metaheuristics.Matching;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Reinsertions;

namespace GeneticSharp.Domain.Metaheuristics.Primitives
{
    /// <summary>
    /// The matching MetaHeuristic offers various techniques to match specific parents for mating and applies the crossover operator to them
    /// </summary>
    [DisplayName("Match")]
    public class MatchMetaHeuristic : ContainerMetaHeuristic
    {



        public MatchPicker Picker { get; set; } = new MatchPicker();

        /// <summary>
        ///Hyperspeed allows for skiping twin parents with same fitness and skip them assuming they are identical and the offspring will be clones.
        /// This will occur more and more after mode collapse, and accordingly, generations will be accelerated. 
        /// The concept is inspired from game of life runner golly's<see href="http://golly.sourceforge.net/Help/control.html"> Hyperspeed feature</see>, which speeds up generations expontentially on regular patterns by storing the long term evolution.
        /// </summary>
        public bool EnableHyperSpeed { get; set; }

       

        public MatchMetaHeuristic() : this(new DefaultMetaHeuristic())
        {
        }

        public MatchMetaHeuristic(IMetaHeuristic subMetaHeuristic/*, int numberOfMatches*/) : base(subMetaHeuristic)
        {
            //NumberOfMatches = numberOfMatches;
            CrossoverProbabilityStrategy =
                ProbabilityStrategy.TestProbability | ProbabilityStrategy.OverwriteProbability;
        }

      
        protected override IList<IChromosome> DoMatchParentsAndCross(IEvolutionContext ctx, ICrossover crossover,
            float crossoverProbability,
            IList<IChromosome> parents)
        {
            var toReturn = new List<IChromosome>(crossover.ParentsNumber * crossover.ChildrenNumber);

            //todo: crossover.ParentsNumber comes from the OperatorsStrategy legacy skipping indexer
            for (int matchIndex = 0; matchIndex < crossover.ParentsNumber; matchIndex++)
            {
                var referenceIndex = ctx.LocalIndex + matchIndex;
                if (referenceIndex < parents.Count)
                {
                    var selectedParents = Picker.SelectMatches(this, ctx, referenceIndex, crossover, parents);

                    //var  matchResult = new List<IChromosome>();
                    if (EnableHyperSpeed
                        && selectedParents.All(c =>
                            c.Fitness != null && selectedParents[0].Fitness != null &&
                            Math.Abs(c.Fitness.Value - selectedParents[0].Fitness.Value) <= double.Epsilon)
                        //todo:investigate Other possibilities (costly)
                        //&& (selectedParents.All(c => c.Equals(selectedParents[0]))
                        /*|| selectedParents.All(c => Enumerable.Range(0, c.Length).All(i => c.GetGene(i).Value.Equals(selectedParents[0].GetGene(i).Value)))*/
                        /*)*/)
                    {
                        break;
                        //Other possibilities to investigate
                        //toReturn.AddRange(Enumerable.Repeat(firstParent, crossover.ChildrenNumber));
                    }
                    else
                    {
                        var subContext = ctx.GetLocal(0);
                        subContext.SelectedParents = selectedParents;
                        var matchResult =
                            base.DoMatchParentsAndCross(subContext, crossover, crossoverProbability, selectedParents);
                        if (matchResult != null)
                        {
                            toReturn.AddRange(matchResult);
                        }
                    }
                }
            }

            return toReturn;
        }

       
    }
}