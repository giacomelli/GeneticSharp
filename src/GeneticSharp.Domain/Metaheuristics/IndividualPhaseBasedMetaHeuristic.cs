using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Reinsertions;
using GeneticSharp.Domain.Selections;

namespace GeneticSharp.Domain.Metaheuristics
{

    public delegate int PhaseGenerator(IPopulation population, int individualCount, int individualIndex);

    /// <summary>
    /// Provides a base class with mechanism to compute the current phase and corresponding phase Metaheuristic from population and current individuals
    /// </summary>
    public class IndividualPhaseBasedMetaHeuristic : PhaseBasedMetaHeuristic
    {
        
        public IndividualPhaseBasedMetaHeuristic() : base(){}

        public IndividualPhaseBasedMetaHeuristic(PhaseGenerator phaseGenerator) : base()
        {
            PhaseGenerator = phaseGenerator;
        }

        public IndividualPhaseBasedMetaHeuristic(int phaseSize, params IMetaHeuristic[] phaseHeuristics):base(){}

        public PhaseGenerator PhaseGenerator { get; set; } 

        public override IList<IChromosome> ScopedSelectParentPopulation(IPopulation population, ISelection selection)
        {
            var phaseItemIdx = PhaseGenerator(population, population.CurrentGeneration.Chromosomes.Count, 0);
            var currentHeuristic = GetOrAddContextItem<IMetaHeuristic>(true, population, phaseItemIdx.ToString(CultureInfo.InvariantCulture), () => GetCurrentHeuristic(phaseItemIdx));
            return currentHeuristic.SelectParentPopulation(population, selection);
        }

        public override IList<IChromosome> ScopedMatchParentsAndCross(IPopulation population, ICrossover crossover, float crossoverProbability, IList<IChromosome> parents,
            int firstParentIndex)
        {
            var phaseItemIdx = PhaseGenerator(population, parents.Count, firstParentIndex);
            var currentHeuristic = GetOrAddContextItem<IMetaHeuristic>(true, population, phaseItemIdx.ToString(CultureInfo.InvariantCulture) ,() => GetCurrentHeuristic(phaseItemIdx));
            return currentHeuristic.MatchParentsAndCross(population, crossover, crossoverProbability, parents,
                firstParentIndex);
        }

        public override void ScopedMutateChromosome(IPopulation population, IMutation mutation, float mutationProbability, IList<IChromosome> offSprings,
            int offspringIndex)
        {
            var phaseItemIdx = PhaseGenerator(population, offSprings.Count, offspringIndex);
            var currentHeuristic = GetOrAddContextItem<IMetaHeuristic>(true, population,phaseItemIdx.ToString(CultureInfo.InvariantCulture), () => GetCurrentHeuristic(phaseItemIdx));
            currentHeuristic.MutateChromosome(population, mutation, mutationProbability, offSprings, offspringIndex);
        }

        public override IList<IChromosome> ScopedReinsert(IPopulation population, IReinsertion reinsertion, IList<IChromosome> offspring, IList<IChromosome> parents)
        {
            var phaseItemIdx = PhaseGenerator(population, offspring.Count, 0);
            var currentHeuristic = GetOrAddContextItem<IMetaHeuristic>(true, population, phaseItemIdx.ToString(CultureInfo.InvariantCulture), () => GetCurrentHeuristic(phaseItemIdx));
           return currentHeuristic.Reinsert(population, reinsertion, offspring, parents);
        }

        private IMetaHeuristic GetCurrentHeuristic(int phaseItemIndex)
        {
            var cumulativeGens = 0;
            for (int phaseIdx = 0; phaseIdx < PhaseSizes.Count; phaseIdx++)
            {
                cumulativeGens += PhaseSizes[phaseIdx];
                if (phaseItemIndex < cumulativeGens)
                {
                    return PhaseHeuristics[phaseIdx];
                }
            }
            throw new ApplicationException("Generation number should correspond to an existing phase");
        }
       

        private int _totalPhaseSize = -1;
        protected int TotalPhaseSize
        {
            get
            {
                if (_totalPhaseSize==-1)
                {
                    _totalPhaseSize = PhaseSizes.Sum();
                }
                return _totalPhaseSize;
            }
        }
    }
}