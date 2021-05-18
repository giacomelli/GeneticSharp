using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Infrastructure.Framework.Commons;

namespace GeneticSharp.Domain.Metaheuristics.Primitives
{
    /// <summary>
    /// Provides a base class with mechanism to compute the current phase and corresponding phase Metaheuristic from population and current individuals
    /// </summary>
    [DisplayName("SizeBased")]
    public class SizeBasedMetaHeuristic : SwitchMetaHeuristic<int>
    {

        public virtual EnumeratedPhases PhaseSizes { get; set; }

        public SizeBasedMetaHeuristic()
        {
            PhaseSizes = new EnumeratedPhases();
        }

        public SizeBasedMetaHeuristic(int phaseSize, params IMetaHeuristic[] phaseHeuristics)
        {
            PhaseSizes = new EnumeratedPhases(Enumerable.Repeat(phaseSize, phaseHeuristics.Length));
            for (int i = 0; i < phaseHeuristics.Length; i++)
            {
                PhaseHeuristics[i] = phaseHeuristics[i];
            }
        }

        public SizeBasedMetaHeuristic(int phaseSize, int phaseNb, params IMetaHeuristic[] phaseHeuristics) : this(phaseSize, Enumerable.Repeat(phaseHeuristics, phaseNb / phaseHeuristics.Length).SelectMany(x => x).ToArray()) { }

        public SizeBasedMetaHeuristic((int phaseSize, IMetaHeuristic phaseMetaHeuristic)[] phases)
        {
            PhaseSizes = new EnumeratedPhases(phases.Select(p => p.phaseSize));
            for (int i = 0; i < phases.Length; i++)
            {
                PhaseHeuristics[i] = phases[i].phaseMetaHeuristic;
            }
        }

        protected override IMetaHeuristic GetCurrentHeuristic(int phaseItemIndex)
        {
            var phaseIdx = PhaseSizes.GetPhaseIndex(phaseItemIndex, out _);
            if (phaseIdx == -1)
            {
                return null;
            }

            return PhaseHeuristics[phaseIdx];
        }

        public class EnumeratedPhases
        {

            public List<int> Phases { get; set; }


            public EnumeratedPhases()
            {
                Phases = new List<int>();
            }

            public EnumeratedPhases(IEnumerable<int> phaseSizes)
            {
                Phases = new List<int>(phaseSizes);
            }

            public void Add(int phaseIndex, int itemNb)
            {
                while (Phases.Count < phaseIndex)
                {
                    Phases.AddRange(Enumerable.Repeat(0, phaseIndex - Phases.Count));
                }

                Phases[phaseIndex] += itemNb;
                _totalPhaseSize = -1;
            }

            public int GetPhaseIndex(int phaseItemIndex, out int localItemIndex)
            {
                phaseItemIndex = phaseItemIndex.PositiveMod(TotalPhaseSize);
                return GetPhaseIndex(Phases, o => (int)o, phaseItemIndex, out localItemIndex);

            }


            public static int GetPhaseIndex<TPhaseType>(IList<TPhaseType> phases, Func<object, int> phaseCounter, int phaseItemIndex, out int localItemIndex)
            {
                localItemIndex = phaseItemIndex;
               
                for (int phaseIdx = 0; phaseIdx < phases.Count; phaseIdx++)
                {
                    var currentCount = phaseCounter(phases[phaseIdx]);
                    //totalCount += currentCount;
                    localItemIndex -= currentCount;
                    if (localItemIndex < 0)
                    {
                        localItemIndex += currentCount;
                        return phaseIdx;
                    }
                }

                throw new ArgumentOutOfRangeException(nameof(phaseItemIndex), "item index larger than the total phase sizes");
                //return -1;
            }




            private int _totalPhaseSize = -1;
            public int TotalPhaseSize
            {
                get
                {
                    if (_totalPhaseSize == -1)
                    {
                        _totalPhaseSize = Phases.Sum();
                    }
                    return _totalPhaseSize;
                }
            }


        }


    }
}