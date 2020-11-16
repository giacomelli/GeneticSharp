using System.Collections.Generic;
using System.Linq;

namespace GeneticSharp.Domain.Metaheuristics
{
    /// <summary>
    /// Provides a base class with mechanism to compute the current phase and corresponding phase Metaheuristic from population and current individuals
    /// </summary>
    public class SizeBasedMetaHeuristic : PhaseMetaHeuristic<int>
    {

        public List<int> PhaseSizes { get; set; }

        public SizeBasedMetaHeuristic()
        {
            PhaseSizes = new List<int>();
        }

        public SizeBasedMetaHeuristic(int phaseSize, params IMetaHeuristic[] phaseHeuristics)
        {
            PhaseSizes = Enumerable.Repeat(phaseSize, phaseHeuristics.Length).ToList();
            for (int i = 0; i < phaseHeuristics.Length; i++)
            {
                PhaseHeuristics[i] = phaseHeuristics[i];
            }
        }

        public SizeBasedMetaHeuristic(int phaseSize, int repeatNb, params IMetaHeuristic[] phaseHeuristics) : this(phaseSize, Enumerable.Repeat(phaseHeuristics, repeatNb / phaseHeuristics.Length).SelectMany(x => x).ToArray()) { }

        protected override IMetaHeuristic GetCurrentHeuristic(int phaseItemIndex)
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

            return null;
        }


        private int _totalPhaseSize = -1;
        protected int TotalPhaseSize
        {
            get
            {
                if (_totalPhaseSize == -1)
                {
                    _totalPhaseSize = PhaseSizes.Sum();
                }
                return _totalPhaseSize;
            }
        }


    }
}