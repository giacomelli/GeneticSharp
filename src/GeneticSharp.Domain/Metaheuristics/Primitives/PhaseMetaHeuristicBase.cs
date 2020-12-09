using System.Collections.Generic;

namespace GeneticSharp.Domain.Metaheuristics
{
    /// <summary>
    /// Abstract Metaheuristic to provide a base class for distinct subheuristics depending on a phase state
    /// The phase state can depend on the population (e.g. generation nb), the individual index (distinct sets), or genes (see EukaryotypeMetaheuristic)
    /// </summary>
    public abstract class PhaseMetaHeuristicBase<TIndex> : ScopedMetaHeuristic
    {

        public PhaseMetaHeuristicBase()
        {
           
            PhaseHeuristics = new Dictionary<TIndex, IMetaHeuristic>();
        }

        //public PhaseMetaHeuristicBase(int phaseSize, params IMetaHeuristic[] phaseHeuristics)
        //{
        //    PhaseSizes = Enumerable.Repeat(phaseSize, phaseHeuristics.Length).ToList();
        //    PhaseHeuristics = phaseHeuristics.ToList();
        //}


        //public PhaseMetaHeuristicBase( int phaseSize, int repeatNb, params IMetaHeuristic[] phaseHeuristics): this(phaseSize, Enumerable.Repeat(phaseHeuristics, repeatNb/phaseHeuristics.Length).SelectMany(x=>x).ToArray()) { }



        public Dictionary<TIndex,IMetaHeuristic> PhaseHeuristics { get; set; }



        public override void RegisterParameters(IEvolutionContext ctx)
        {
            base.RegisterParameters(ctx);
            foreach (var keyValuePair in PhaseHeuristics)
            {
                ((MetaHeuristicBase)keyValuePair.Value).RegisterParameters(ctx);
            }
            
        }

    }
}