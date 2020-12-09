using System.Linq.Expressions;

namespace GeneticSharp.Domain.Metaheuristics
{
    public interface IExpressionGeneratorParameter: IMetaHeuristicParameter
    {

        LambdaExpression GetExpression(IEvolutionContext ctx, string paramName);

    }
}