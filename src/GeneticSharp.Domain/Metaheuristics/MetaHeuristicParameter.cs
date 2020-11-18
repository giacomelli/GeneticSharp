namespace GeneticSharp.Domain.Metaheuristics
{
    


    public abstract class MetaHeuristicParameter
    {
        public ParameterScope Scope { get; set; }


        public abstract object ComputeParameter(IMetaHeuristic h, IMetaHeuristicContext ctx);


    }

    public class MetaHeuristicParameter<TParamType> : MetaHeuristicParameter
    {
        public override object ComputeParameter(IMetaHeuristic h, IMetaHeuristicContext ctx)
        {
            return Generator(h, ctx);
        }

        public ParameterGenerator<TParamType> Generator { get; set; }

    }
}