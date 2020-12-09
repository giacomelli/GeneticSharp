using GeneticSharp.Domain.Metaheuristics.Primitives;

namespace GeneticSharp.Domain.Metaheuristics.Parameters
{
    /// <summary>
    /// A IMetaHeuristicParameter is a typed entity factory based on a paramName and the evolution context.
    /// It has a scope defining when it should be cached and reused, and when a new value should be computed according to the new scope context.
    /// </summary>
    public interface IMetaHeuristicParameter
    {
        /// <summary>
        /// The scope of the parameter within the evolution hierarchy defined by the <see cref="ParamScope"/> enumeration.
        /// For instance, a parameter can be computed on a generation basis, or and individual basis.
        /// </summary>
        ParamScope Scope { get; set; }

        /// <summary>
        /// The typed method to generate the parameter value according to the evolution context
        /// </summary>
        /// <typeparam name="TItemType">The typed of the value to compute</typeparam>
        /// <param name="h">the calling Metaheuristic</param>
        /// <param name="ctx">the current evolution context</param>
        /// <param name="paramName">the name of the parameter</param>
        /// <returns>the value of the named parameter, according to the current context and parameter scope.</returns>
        TItemType Get<TItemType>(IMetaHeuristic h, IEvolutionContext ctx, string paramName);
        


    }
}