namespace GeneticSharp.Domain.Metaheuristics
{
    /// <summary>
    /// Allows to define distinct MetaHeuristic for different scopes
    /// </summary>
    public class StageSwitchMetaHeuristic : SwitchMetaHeuristic<MetaHeuristicsStage>
    {
        public StageSwitchMetaHeuristic()
        {
            IndexGenerator = (h,ctx) => ctx.CurrentStage;
        }
    }
}