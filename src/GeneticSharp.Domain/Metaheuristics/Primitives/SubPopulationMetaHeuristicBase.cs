using System;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Metaheuristics.Parameters;
using GeneticSharp.Domain.Populations;

namespace GeneticSharp.Domain.Metaheuristics.Primitives
{
    public abstract class SubPopulationMetaHeuristicBase<T> : SizeBasedMetaHeuristic where T:SubPopulation
    {

        public SubPopulationMetaHeuristicBase()
        {
            }

        public SubPopulationMetaHeuristicBase(int phaseSize, params IMetaHeuristic[] phaseHeuristics) : base(phaseSize, phaseHeuristics)
        {
        }

        public SubPopulationMetaHeuristicBase(int phaseSize, int phaseNb, params IMetaHeuristic[] phaseHeuristics) : base(phaseSize, phaseNb, phaseHeuristics)
        {
        }

        public SubPopulationMetaHeuristicBase(params (int phaseSize, IMetaHeuristic phaseMetaHeuristic)[] phases) : base(phases)
        {
        }

        public ParamScope SubPopulationCachingScope { get; set; } = ParamScope.Generation | ParamScope.MetaHeuristic;


        protected abstract IList<T> GenerateSubPopulations(IMetaHeuristic h, IEvolutionContext c);


        private MetaHeuristicParameter<IList<T>> _dynamicSubPopulationParameter;

        public MetaHeuristicParameter<IList<T>> DynamicSubPopulationParameter
        {
            get
            {
                if (_dynamicSubPopulationParameter == null)
                {
                    _dynamicSubPopulationParameter = new MetaHeuristicParameter<IList<T>>
                    {
                        Scope = SubPopulationCachingScope,
                        Generator = GenerateSubPopulations
                    };
                }
                return _dynamicSubPopulationParameter;
            }
        }

        protected IList<IChromosome> PerformSubOperator(IList<SubPopulation> subPopulations, Func<IMetaHeuristic, SubPopulation, IList<IChromosome>> subPopulationOperator)
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