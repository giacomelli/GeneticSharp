using System;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Domain.Reinsertions;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Infrastructure.Framework.Commons;

namespace GeneticSharp.Domain.Metaheuristics
{
    /// <summary>
    /// The Eukaryote Meta Heuristic uses Eurkaryote Chromosomes to split the original individual into Karyotypes of child sub chromosomes and apply distinct phase Metaheuristics to the child chromosomes before building back the resulting parents
    /// </summary>
    public class EukaryoteMetaHeuristic : SizeBasedMetaHeuristic
    {
       
        public EukaryoteMetaHeuristic() : base() { }

        public EukaryoteMetaHeuristic(int subChromosomeSize, params IMetaHeuristic[] phaseHeuristics) : base(subChromosomeSize, phaseHeuristics) { }

        public EukaryoteMetaHeuristic(int phaseSize, int repeatNb, params IMetaHeuristic[] phaseHeuristics) : base(phaseSize, repeatNb, phaseHeuristics) { }

        public override IList<IChromosome> ScopedSelectParentPopulation(IMetaHeuristicContext ctx, ISelection selection)
        {
            IList<IList<IChromosome>> subPopulations = EukaryoteChromosome.GetSubPopulations(ctx.Population.CurrentGeneration.Chromosomes, PhaseSizes);
            var selectedParents = PerformSubOperator(subPopulations, (subHeuristic, subChromosomes) =>
            {
                var subPopulation = new EukaryotePopulation(ctx.Population, subChromosomes) ;
                //todo: deal with parameters (delegate?)
                var newCtx = new MetaHeuristicContext(){GA = ctx.GA, Population = subPopulation, Count = ctx.Count};
                return subHeuristic.SelectParentPopulation(newCtx, selection);

            });
           
            return selectedParents;
        }

        public ParameterScope SubPopulationCachingScope { get; set; } = ParameterScope.Generation | ParameterScope.MetaHeuristic;



        public override IList<IChromosome> ScopedMatchParentsAndCross(IMetaHeuristicContext ctx, ICrossover crossover, float crossoverProbability, IList<IChromosome> parents, int firstParentIndex)
        {

            var dynamicSubPopulationParameter = new MetaHeuristicParameter<IList<IList<IChromosome>>>()
            {
                Scope = SubPopulationCachingScope,
                Generator = (h, c) => EukaryoteChromosome.GetSubPopulations(parents, PhaseSizes)
            };
            var subPopulations = dynamicSubPopulationParameter.GetOrAdd(this, ctx, "subPopulations");

            if (RandomizationProvider.Current.GetDouble() <= crossoverProbability)
            {
                var offsprings = PerformSubOperator(subPopulations, (subHeuristic, subPopulation) => subHeuristic.MatchParentsAndCross(ctx, crossover,
                    1, subPopulation, firstParentIndex));

                return offsprings;
            }

            return null;
        }


        public override void ScopedMutateChromosome(IMetaHeuristicContext ctx, IMutation mutation, float mutationProbability, IList<IChromosome> offSprings, int offspringIndex)
        {
            var karyotype = EukaryoteChromosome.GetKaryotype(offSprings[offspringIndex], PhaseSizes);
            for (var subChromosomeIdx = 0; subChromosomeIdx < karyotype.Count; subChromosomeIdx++)
            {
                var subChromosome = karyotype[subChromosomeIdx];
                PhaseHeuristics[subChromosomeIdx].MutateChromosome(ctx, mutation, mutationProbability,new List<IChromosome>(new []{subChromosome}),0 );
            }
            EukaryoteChromosome.UpdateParent(karyotype);
        }

        public override IList<IChromosome> ScopedReinsert(IMetaHeuristicContext ctx, IReinsertion reinsertion, IList<IChromosome> offspring, IList<IChromosome> parents)
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
                this.CreateNewGeneration(subPopulation);
                GenerationsNumber = parentPopulation.GenerationsNumber;
            }
        }
    }
}