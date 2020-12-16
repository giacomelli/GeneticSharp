namespace GeneticSharp.Domain.Metaheuristics.Primitives
{
    public interface IContainerMetaHeuristic: IMetaHeuristic
    {
        /// <summary>
        /// This sub metaheuristic is used by for all operators, except for those overriden
        /// </summary>
        IMetaHeuristic SubMetaHeuristic { get; set; }
    }
}