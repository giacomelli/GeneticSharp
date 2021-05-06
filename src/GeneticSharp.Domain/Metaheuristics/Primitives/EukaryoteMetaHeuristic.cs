using System;
using System.Collections.Generic;
using System.ComponentModel;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Metaheuristics.Parameters;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Reinsertions;
using GeneticSharp.Domain.Selections;

namespace GeneticSharp.Domain.Metaheuristics.Primitives
{
    /// <summary>
    /// The Eukaryote Meta Heuristic uses Eurkaryote Chromosomes to split the original individual into Karyotypes of child sub chromosomes and apply distinct phase Metaheuristics to the child chromosomes before building back the resulting parents
    /// </summary>
    [DisplayName("Eukaryote")]
    public class EukaryoteMetaHeuristic : SubPopulationMetaHeuristicBase<SubPopulation>
    {
       
        public EukaryoteMetaHeuristic()
        { }

        public EukaryoteMetaHeuristic(int subChromosomeSize, params IMetaHeuristic[] phaseHeuristics) : base(subChromosomeSize, phaseHeuristics) { }

        public EukaryoteMetaHeuristic(int phaseSize, int islandNb, params IMetaHeuristic[] phaseHeuristics) : base(phaseSize, islandNb, phaseHeuristics) { }




        protected override IList<SubPopulation> GenerateSubPopulations(IMetaHeuristic h, IEvolutionContext c)
        {
            var subPopulations = new List<SubPopulation>(PhaseSizes.Phases.Count);
            var subPopulationChromosomes = EukaryoteChromosome.GetSubPopulations(c.Population.CurrentGeneration.Chromosomes, PhaseSizes.Phases);
            for (int subPopulationIndex = 0; subPopulationIndex < subPopulationChromosomes.Count; subPopulationIndex++)
            {
                var subPopulation = new SubPopulation(c.Population, subPopulationChromosomes[subPopulationIndex]);
                subPopulations.Add(subPopulation);
            }
            return subPopulations;
        }


        private const string subPopulationsKey = "eukaryoteSubPopulations";

        protected override IList<IChromosome> ScopedSelectParentPopulation(IEvolutionContext ctx, ISelection selection)
        {
            //IList<IList<IChromosome>> subPopulations = EukaryoteChromosome.GetSubPopulations(ctx.Population.CurrentGeneration.Chromosomes, PhaseSizes.Phases);
            var subPopulations = DynamicSubPopulationParameter.Get(this, ctx, subPopulationsKey);
            var selectedParents = PerformSubOperator(subPopulations, (subHeuristic, subPopulation) =>
            {
                //todo:check parameter scope
                var newCtx = subPopulation.GetContext(ctx);
                var toReturn = subHeuristic.SelectParentPopulation(newCtx, selection);
                newCtx.SelectedParents = toReturn;
                
                return toReturn;
            });
           
            return selectedParents;
        }


        protected override IList<IChromosome> ScopedMatchParentsAndCross(IEvolutionContext ctx, ICrossover crossover, float crossoverProbability, IList<IChromosome> parents)
        {

            var subPopulations = DynamicSubPopulationParameter.Get(this, ctx, subPopulationsKey);

            if (subPopulations[0].GetContext(ctx).SelectedParents.Count==0)
            {
                var selectedSubParents = EukaryoteChromosome.GetSubPopulations(parents, PhaseSizes.Phases);
                for (int subPopulationIndex = 0; subPopulationIndex < selectedSubParents.Count; subPopulationIndex++)
                {
                    subPopulations[subPopulationIndex].GetContext(ctx).SelectedParents = selectedSubParents[subPopulationIndex];
                }
            }

            var offsprings = PerformSubOperator(subPopulations, (subHeuristic, subPopulation) =>
            {
                var newCtx = subPopulation.GetContext(ctx);
                var toReturn = subHeuristic.MatchParentsAndCross(ctx, crossover,
                    1, newCtx.SelectedParents);
                newCtx.GeneratedOffsprings = toReturn;
                return toReturn;
            });

            return offsprings;

        }


        protected override void ScopedMutateChromosome(IEvolutionContext ctx, IMutation mutation, float mutationProbability, IList<IChromosome> offSprings)
        {
            var karyotype = EukaryoteChromosome.GetKaryotype(offSprings[ctx.LocalIndex], PhaseSizes.Phases);
            for (var subChromosomeIdx = 0; subChromosomeIdx < karyotype.Count; subChromosomeIdx++)
            {
                var subChromosome = karyotype[subChromosomeIdx];
                var subContext = ctx.GetLocal(0);
                PhaseHeuristics[subChromosomeIdx].MutateChromosome(subContext, mutation, mutationProbability,new List<IChromosome>(new []{subChromosome}));
            }
            EukaryoteChromosome.UpdateParent(karyotype);
        }

        protected override IList<IChromosome> ScopedReinsert(IEvolutionContext ctx, IReinsertion reinsertion, IList<IChromosome> offspring, IList<IChromosome> parents)
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

       
    }
}