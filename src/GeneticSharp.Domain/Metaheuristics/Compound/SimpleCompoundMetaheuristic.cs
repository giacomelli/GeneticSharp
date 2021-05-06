using GeneticSharp.Domain.Metaheuristics.Primitives;

namespace GeneticSharp.Domain.Metaheuristics.Compound
{
    /// <summary>
    /// This is a simple Compound metaheuristic holding a primitive or an already built compoundmetaheuristic to be returned upond building
    /// </summary>
    public class SimpleCompoundMetaheuristic : ICompoundMetaheuristic
    {
        public SimpleCompoundMetaheuristic(IContainerMetaHeuristic simpleMetaHeuristic)
        {
            SimpleMetaHeuristic = simpleMetaHeuristic;
        }

        public IContainerMetaHeuristic SimpleMetaHeuristic { get; set; }


        public IContainerMetaHeuristic Build()
        {
            return SimpleMetaHeuristic;
        }
    }
}