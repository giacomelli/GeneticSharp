namespace GeneticSharp.Domain.Metaheuristics
{
    /// <summary>
    /// Allows to define distinct MetaHeuristic for different scopes
    /// </summary>
    public class StageSwitchMetaHeuristic : SwitchMetaHeuristic<EvolutionStage>
    {
        public StageSwitchMetaHeuristic()
        {
            DynamicParameter = new ExpressionMetaHeuristicParameter<EvolutionStage> { Scope = ParamScope.Generation, DynamicGenerator = (h, ctx) => ctx.CurrentStage };
        }
    }
}