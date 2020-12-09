using GeneticSharp.Domain.Metaheuristics.Primitives;

namespace GeneticSharp.Domain.Metaheuristics
{
    public interface IMetaHeuristicParameter
    {
        ParamScope Scope { get; set; }

        TItemType GetOrAdd<TItemType>(IMetaHeuristic h, IEvolutionContext ctx, string key);
        
        object ComputeParameter(IMetaHeuristic h, IEvolutionContext ctx);


    }
}