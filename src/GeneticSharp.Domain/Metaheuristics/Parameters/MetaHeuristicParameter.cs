using GeneticSharp.Domain.Metaheuristics.Primitives;

namespace GeneticSharp.Domain.Metaheuristics.Parameters
{

    public delegate TParamType ParameterGenerator<out TParamType>(IMetaHeuristic h, IEvolutionContext ctx);
    public delegate TParamType ParameterGenerator<out TParamType, in TArg1>(IMetaHeuristic h, IEvolutionContext ctx, TArg1 arg1);
    public delegate TParamType ParameterGenerator<out TParamType, in TArg1, in TArg2>(IMetaHeuristic h, IEvolutionContext ctx, TArg1 arg1, TArg2 arg2);
    public delegate TParamType ParameterGenerator<out TParamType, in TArg1, in TArg2, in TArg3>(IMetaHeuristic h, IEvolutionContext ctx, TArg1 arg1, TArg2 arg2, TArg3 arg3);

    /// <summary>
    /// The default MetaHeuristic implementation has parameter type defined with generics, has a delegate based generator, and relies on key masking according to the scope defined to cache the parameter value withing the evolution context
    /// </summary>
    /// <typeparam name="TParamType">The type for the parameter value</typeparam>
    public class MetaHeuristicParameter<TParamType> : NamedEntity, IMetaHeuristicParameterGenerator<TParamType>
    {
        /// <inheritdoc />
        public ParamScope Scope { get; set; }

        /// <summary>
        /// A function that generates the parameter value from the context
        /// </summary>
        public ParameterGenerator<TParamType> Generator { get; set; }

        /// <summary>
        /// A non generic version of the Get method that uses the actual parameter type
        /// </summary>
        /// <param name="h">the calling metaheuristic</param>
        /// <param name="ctx">the current evolution context</param>
        /// <param name="paramName">the name of the parameter</param>
        /// <returns>the value of the parameter, according to the context, and parameter scope</returns>
        public TParamType Get(IMetaHeuristic h, IEvolutionContext ctx, string paramName)
        {
            var toReturn = Get<TParamType>(h, ctx, paramName);
            return toReturn;
        }


        /// <inheritdoc />
        public TItemType Get<TItemType>(IMetaHeuristic h, IEvolutionContext ctx, string paramName)
        {
            if (Scope == ParamScope.None)
            {
                return (TItemType)ComputeParameter(h, ctx);
            }
            var maskedTuple = (paramName, ctx.Population?.GenerationsNumber ?? 0, ctx.CurrentStage, h, ctx.OriginalIndex);
            GetScopeMask(ref maskedTuple);

            var toReturn = (TItemType)ctx.GetOrAdd(maskedTuple, () => ComputeParameter(h, ctx));
            return toReturn;
        }



       private  object ComputeParameter(IMetaHeuristic h, IEvolutionContext ctx)
        {
            var toReturn = GetGenerator(ctx)(h, ctx);
            return toReturn;
        }

        public virtual ParameterGenerator<TParamType> GetGenerator(IEvolutionContext ctx)
        {
            return Generator;
        }

        private void GetScopeMask(ref (string key, int generation, EvolutionStage stage, IMetaHeuristic heuristic, int individual) input)
        {
            if ((Scope & ParamScope.Generation) != ParamScope.Generation)
            {
                input.generation = 0;

            }
            if ((Scope & ParamScope.Stage) != ParamScope.Stage)
            {
                input.stage = EvolutionStage.All;
            }
            if ((Scope & ParamScope.MetaHeuristic) != ParamScope.MetaHeuristic)
            {
                input.heuristic = null;
            }
            if ((Scope & ParamScope.Individual) != ParamScope.Individual)
            {
                input.individual = 0;
            }
        }

      
    }
}