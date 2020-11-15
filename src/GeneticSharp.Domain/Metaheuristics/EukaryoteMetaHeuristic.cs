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
    public class EukaryoteMetaHeuristic : PhaseBasedMetaHeuristic
    {
        /// <summary>
        /// The Eukaryote population serves applying genetic operators to populations of Eurkaryote chromosomes
        /// </summary>
        public sealed class EukaryotePopulation: Population
        {
            public IPopulation ParentPopulation { get; set; }

            public EukaryotePopulation(IPopulation parentPopulation, IList<IChromosome> subPopulation) : base(parentPopulation.MinSize, parentPopulation.MaxSize, subPopulation[0])
            {
                ParentPopulation = parentPopulation;
                this.CreateNewGeneration(subPopulation);
                GenerationsNumber = parentPopulation.GenerationsNumber;
            }
        }


        public EukaryoteMetaHeuristic() : base() { }

        public EukaryoteMetaHeuristic(int subChromosomeSize, params IMetaHeuristic[] phaseHeuristics) : base(subChromosomeSize, phaseHeuristics) { }

        public EukaryoteMetaHeuristic(int phaseSize, int repeatNb, params IMetaHeuristic[] phaseHeuristics) : base(phaseSize, repeatNb, phaseHeuristics) { }

        public override IList<IChromosome> ScopedSelectParentPopulation(IPopulation population, ISelection selection)
        {
            IList<IList<IChromosome>> subPopulations = EukaryoteChromosome.GetSubPopulations(population.CurrentGeneration.Chromosomes, PhaseSizes);
            var selectedParents = PerformSubOperator(subPopulations, (subHeuristic, subChromosomes) =>
            {
                var subPopulation = new EukaryotePopulation(population, subChromosomes) ;
                return subHeuristic.SelectParentPopulation(subPopulation, selection);

            });
           
            return selectedParents;
        }

        public override IList<IChromosome> ScopedMatchParentsAndCross(IPopulation population, ICrossover crossover, float crossoverProbability, IList<IChromosome> parents, int firstParentIndex)
        {
            var subPopulations = this.GetOrAddContextItem<IList<IList<IChromosome>>>(true, population,"subPopulations", () => EukaryoteChromosome.GetSubPopulations(parents, PhaseSizes));
            if (RandomizationProvider.Current.GetDouble() <= crossoverProbability)
            {
                var offsprings = PerformSubOperator(subPopulations, (subHeuristic, subPopulation) => subHeuristic.MatchParentsAndCross(population, crossover,
                    1, subPopulation, firstParentIndex));

                return offsprings;
            }

            return null;
        }


        public override void ScopedMutateChromosome(IPopulation population, IMutation mutation, float mutationProbability, IList<IChromosome> offSprings, int offspringIndex)
        {
            var karyotype = EukaryoteChromosome.GetKaryotype(offSprings[offspringIndex], PhaseSizes);
            for (var subChromosomeIdx = 0; subChromosomeIdx < karyotype.Count; subChromosomeIdx++)
            {
                var subChromosome = karyotype[subChromosomeIdx];
                PhaseHeuristics[subChromosomeIdx].MutateChromosome(population, mutation, mutationProbability,new List<IChromosome>(new []{subChromosome}),0 );
            }
            EukaryoteChromosome.UpdateParent(karyotype);
        }

        public override IList<IChromosome> ScopedReinsert(IPopulation population, IReinsertion reinsertion, IList<IChromosome> offspring, IList<IChromosome> parents)
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

            throw new InvalidOperationException("Eukaryote doesn't support reinsertion");


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


    }
}