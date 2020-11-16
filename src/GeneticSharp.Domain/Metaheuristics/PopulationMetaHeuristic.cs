namespace GeneticSharp.Domain.Metaheuristics
{
    /// <summary>
    /// The population based Metaheuristcs allows applying distinct Metaheuristics depending on the individual index. By default, it divides the individuals into distinct phase sets proportional to the phase sizes 
    /// </summary>
    public class PopulationMetaHeuristic : SizeBasedMetaHeuristic
    {

        public PopulationMetaHeuristic() : base()
        {
            Init();
        }



        public PopulationMetaHeuristic(int groupSize, params IMetaHeuristic[] phaseHeuristics) : base(groupSize,
            phaseHeuristics)
        {
            Init();
        }

        private void Init()
        {
            PhaseGenerator = ctx => (int)(ctx.Index / (float)ctx.Count) * TotalPhaseSize;
        }
    }
}