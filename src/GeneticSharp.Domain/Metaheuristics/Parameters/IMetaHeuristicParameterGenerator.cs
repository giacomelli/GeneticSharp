namespace GeneticSharp.Domain.Metaheuristics.Parameters
{
    public interface IMetaHeuristicParameterGenerator<out TParamType>: IMetaHeuristicParameter
    {

        ParameterGenerator<TParamType> GetGenerator(IEvolutionContext ctx);

    }
}