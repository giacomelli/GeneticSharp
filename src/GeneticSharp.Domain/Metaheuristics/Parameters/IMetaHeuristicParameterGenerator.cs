namespace GeneticSharp.Domain.Metaheuristics
{
    public interface IMetaHeuristicParameterGenerator<out TParamType>: IMetaHeuristicParameter
    {

        ParameterGenerator<TParamType> GetGenerator(IEvolutionContext ctx);

    }
}