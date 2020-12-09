using GeneticSharp.Domain.Metaheuristics.Primitives;

namespace GeneticSharp.Domain.Metaheuristics
{

    public delegate TParamType ParameterGenerator<out TParamType>(IMetaHeuristic h, IEvolutionContext ctx);
    public delegate TParamType ParameterGenerator<out TParamType, in TArg1>(IMetaHeuristic h, IEvolutionContext ctx, TArg1 arg1);
    public delegate TParamType ParameterGenerator<out TParamType, in TArg1, in TArg2>(IMetaHeuristic h, IEvolutionContext ctx, TArg1 arg1, TArg2 arg2);
    public delegate TParamType ParameterGenerator<out TParamType, in TArg1, in TArg2, in TArg3>(IMetaHeuristic h, IEvolutionContext ctx, TArg1 arg1, TArg2 arg2, TArg3 arg3);


    public class MetaHeuristicParameter<TParamType> : NamedEntity, IMetaHeuristicParameterGenerator<TParamType>
    {
       

        public ParamScope Scope { get; set; }


        private (string key, int generation, EvolutionStage stage, IMetaHeuristic heuristic, int individual) GetScopeMask((string key, int generation, EvolutionStage stage, IMetaHeuristic heuristic, int individual) input)
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

            return input;
        }

        public TItemType GetOrAdd<TItemType>(IMetaHeuristic h, IEvolutionContext ctx, string key)
        {

            var maskedTuple = GetScopeMask((key, ctx.Population?.GenerationsNumber ?? 0, ctx.CurrentStage, h, ctx.Index));

            var toReturn = (TItemType)ctx.GetOrAdd(maskedTuple, () => ComputeParameter(h, ctx));
            return toReturn;
        }



       public  object ComputeParameter(IMetaHeuristic h, IEvolutionContext ctx)
        {
            var toReturn = GetGenerator(ctx)(h, ctx);
            return toReturn;
        }

        public virtual ParameterGenerator<TParamType> GetGenerator(IEvolutionContext ctx)
        {
            return Generator;
        }


        public ParameterGenerator<TParamType> Generator { get; set; }

        public TParamType GetOrAdd(IMetaHeuristic h, IEvolutionContext ctx, string key)
        {
          var toReturn =  GetOrAdd<TParamType>(h, ctx, key);
          return toReturn;
        }
    }
}