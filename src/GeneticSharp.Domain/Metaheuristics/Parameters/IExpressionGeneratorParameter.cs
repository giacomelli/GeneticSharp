using System.Linq.Expressions;

namespace GeneticSharp.Domain.Metaheuristics.Parameters
{
    public interface IExpressionGeneratorParameter: IMetaHeuristicParameter
    {

        LambdaExpression GetExpression(IEvolutionContext ctx, string paramName);

    }
}