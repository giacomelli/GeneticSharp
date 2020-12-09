using GeneticSharp.Domain.Metaheuristics.Primitives;

namespace GeneticSharp.Domain.Metaheuristics.Parameters
{

    public interface IMetaHeuristicParameter
    {
        ParamScope Scope { get; set; }

        TItemType GetOrAdd<TItemType>(IMetaHeuristic h, IEvolutionContext ctx, string key);
        
        object ComputeParameter(IMetaHeuristic h, IEvolutionContext ctx);


    }
}