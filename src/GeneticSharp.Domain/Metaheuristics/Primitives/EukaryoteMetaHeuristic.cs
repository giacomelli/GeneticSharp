using System;
using System.Collections.Generic;
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
    /// The Eukaryote Meta Heuristic uses Eurkaryote Chromosomes to split the original individual into Karyotypes of child sub chromosomes and apply distinct phase Metaheuristics to the child chromosomes before building back the resulting parents
    /// </summary>
    public class EukaryoteMetaHeuristic : SizeBasedMetaHeuristic
    {
       
        public EukaryoteMetaHeuristic()
        { }

        public EukaryoteMetaHeuristic(int subChromosomeSize, params IMetaHeuristic[] phaseHeuristics) : base(subChromosomeSize, phaseHeuristics) { }

        public EukaryoteMetaHeuristic(int phaseSize, int repeatNb, params IMetaHeuristic[] phaseHeuristics) : base(phaseSize, repeatNb, phaseHeuristics) { }

        public override IList<IChromosome> ScopedSelectParentPopulation(IEvolutionContext ctx, ISelection selection)
        {
            IList<IList<IChromosome>> subPopulations = EukaryoteChromosome.GetSubPopulations(ctx.Population.CurrentGeneration.Chromosomes, PhaseSizes);
            var selectedParents = PerformSubOperator(subPopulations, (subHeuristic, subChromosomes) =>
            {
                var subPopulation = new EukaryotePopulation(ctx.Population, subChromosomes) ;
                //todo: deal with parameters (delegate?)
                var newCtx = new EvolutionContext {GA = ctx.GA, Population = subPopulation};
                return subHeuristic.SelectParentPopulation(newCtx, selection);

            });
           
            return selectedParents;
        }

        public ParamScope SubPopulationCachingScope { get; set; } = ParamScope.Generation | ParamScope.MetaHeuristic;



        public override IList<IChromosome> ScopedMatchParentsAndCross(IEvolutionContext ctx, ICrossover crossover, float crossoverProbability, IList<IChromosome> parents)
        {

            var dynamicSubPopulationParameter = new MetaHeuristicParameter<IList<IList<IChromosome>>>
            {
                Scope = SubPopulationCachingScope,
                Generator = (h, c) => EukaryoteChromosome.GetSubPopulations(parents, PhaseSizes)
            };
            var subPopulations = dynamicSubPopulationParameter.GetOrAdd(this, ctx, "subPopulations");

            if (RandomizationProvider.Current.GetDouble() <= crossoverProbability)
            {
                var offsprings = PerformSubOperator(subPopulations, (subHeuristic, subPopulation) => subHeuristic.MatchParentsAndCross(ctx, crossover,
                    1, subPopulation));

                return offsprings;
            }

            return null;
        }


        public override void ScopedMutateChromosome(IEvolutionContext ctx, IMutation mutation, float mutationProbability, IList<IChromosome> offSprings)
        {
            var karyotype = EukaryoteChromosome.GetKaryotype(offSprings[ctx.Index], PhaseSizes);
            for (var subChromosomeIdx = 0; subChromosomeIdx < karyotype.Count; subChromosomeIdx++)
            {
                var subChromosome = karyotype[subChromosomeIdx];
                var subContext = ctx.GetIndividual(0);
                PhaseHeuristics[subChromosomeIdx].MutateChromosome(subContext, mutation, mutationProbability,new List<IChromosome>(new []{subChromosome}));
            }
            EukaryoteChromosome.UpdateParent(karyotype);
        }

        public override IList<IChromosome> ScopedReinsert(IEvolutionContext ctx, IReinsertion reinsertion, IList<IChromosome> offspring, IList<IChromosome> parents)
        {
            ////In order to use suboperator, we will temporarily concatenate offspring and parents
            //var offSpringCount = offspring.Count;
            //var offspringAndParents = offspring.Concat(parents);
            //IList<IList<IChromosome>> subOffspringAndParents = EukaryoteChromosome.GetSubPopulations(offspringAndParents, PhaseSizes);
            

            //var selectedParents = PerformSubOperator(subOffspringAndParents, (subHeuristic, subChromosomes) =>
            //{
            //    var subOffsprings = subChromosomes.Take(offSpringCount).ToList();
            //    var subParents = subChromosomes.Skip(offSpringCount).ToList();
            //    var subPopulation = new EukaryotePopulation(population, subParents);
            //    return subHeuristic.Reinsert(subPopulation, reinsertion, subOffsprings, subParents);

            //});

            //return selectedParents;

            throw new InvalidOperationException("Eukaryote MetaHeuristic doesn't support reinsertion");


        }


        private IList<IChromosome> PerformSubOperator(IList<IList<IChromosome>> subPopulations, Func<IMetaHeuristic, IList<IChromosome>, IList<IChromosome>> subPopulationOperator)
        {
            var resultSubPopulations = new List<IList<IChromosome>>();
            for (var subChromosomeIndex = 0; subChromosomeIndex < subPopulations.Count; subChromosomeIndex++)
            {
                var subPopulation = subPopulations[subChromosomeIndex];
                var subHeuristic = PhaseHeuristics[subChromosomeIndex];
                var subResults = subPopulationOperator(subHeuristic, subPopulation);
                resultSubPopulations.Add(subResults);
            }

            var resultPopulation = EukaryoteChromosome.GetNewIndividuals(resultSubPopulations);
            return resultPopulation;
        }

        /// <summary>
        /// The Eukaryote population serves applying genetic operators to populations of Eurkaryote chromosomes
        /// </summary>
        public sealed class EukaryotePopulation : Population
        {
            public IPopulation ParentPopulation { get; set; }

            public EukaryotePopulation(IPopulation parentPopulation, IList<IChromosome> subPopulation) : base(parentPopulation.MinSize, parentPopulation.MaxSize, subPopulation[0])
            {
                ParentPopulation = parentPopulation;
                CreateNewGeneration(subPopulation);
                GenerationsNumber = parentPopulation.GenerationsNumber;
            }
        }
    }
}