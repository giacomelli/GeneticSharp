namespace GeneticSharp.Domain.Metaheuristics
{
    


    public abstract class MetaHeuristicParameter
    {
        public ParameterScope Scope { get; set; }

       

       

    }

    public class MetaHeuristicParameter<TParamType> : MetaHeuristicParameter
    {

        public ParameterGenerator<TParamType> Generator { get; set; }

    }
}