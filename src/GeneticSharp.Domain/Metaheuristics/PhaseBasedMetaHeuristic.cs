using System.Collections.Generic;
using System.Linq;

namespace GeneticSharp.Domain.Metaheuristics
{
    /// <summary>
    /// Abstract Metaheuristic to provide a base class for distinct subheuristics depending on a phase state
    /// The phase state can depend on the population (e.g. generation nb), the individual index (distinct sets), or genes (see EukaryotypeMetaheuristic)
    /// </summary>
    public abstract class PhaseBasedMetaHeuristic : ScopedBasedMetaHeuristic
    {

        public PhaseBasedMetaHeuristic()
        {
            PhaseSizes = new List<int>();
            PhaseHeuristics = new List<IMetaHeuristic>();
        }

        public PhaseBasedMetaHeuristic(int phaseSize, params IMetaHeuristic[] phaseHeuristics)
        {
            PhaseSizes = Enumerable.Repeat(phaseSize, phaseHeuristics.Length).ToList();
            PhaseHeuristics = phaseHeuristics.ToList();
        }


        public PhaseBasedMetaHeuristic( int phaseSize, int repeatNb, params IMetaHeuristic[] phaseHeuristics): this(phaseSize, Enumerable.Repeat(phaseHeuristics, repeatNb/phaseHeuristics.Length).SelectMany(x=>x).ToArray()) { }


        public List<int> PhaseSizes { get; set; }

        public List<IMetaHeuristic> PhaseHeuristics { get; set; }

        /// <summary>
        /// A fluent constructor allows to define phase heuristics in sequence
        /// </summary>
        /// <param name="phaseSize">the phase size for the next phase heuristic</param>
        /// <param name="subMetaHeuristic">the phase heuristic to add</param>
        /// <returns>the current phase based Metaheuristic</returns>
        public PhaseBasedMetaHeuristic WithPhaseHeuristic(int phaseSize, IMetaHeuristic subMetaHeuristic)
        {
            PhaseSizes.Add(phaseSize);
            PhaseHeuristics.Add(subMetaHeuristic);
            return this;
        }


    }
}