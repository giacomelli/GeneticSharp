namespace GeneticSharp.Domain.Metaheuristics
{
    /// <summary>
    /// Allows to define distinct MetaHeuristic for different scopes
    /// </summary>
    public class StageSwitchMetaHeuristic : SwitchMetaHeuristic<MetaHeuristicsStage>
    {
        public StageSwitchMetaHeuristic()
        {
            DynamicParameter = new ExpressionMetaHeuristicParameter<MetaHeuristicsStage>() { Scope = ParameterScope.Generation, DynamicGenerator = (h, ctx) => ctx.CurrentStage };
        }
    }
}