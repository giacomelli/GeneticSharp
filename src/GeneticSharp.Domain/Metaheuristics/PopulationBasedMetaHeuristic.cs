namespace GeneticSharp.Domain.Metaheuristics
{
    /// <summary>
    /// The population based Metaheuristcs allows applying distinct Metaheuristics depending on the individual index. By default, it divides the individuals into distinct phase sets proportional to the phase sizes 
    /// </summary>
    public class PopulationBasedMetaHeuristic : IndividualPhaseBasedMetaHeuristic
    {

        public PopulationBasedMetaHeuristic() : base()
        {
            Init();
        }



        public PopulationBasedMetaHeuristic(int groupSize, params IMetaHeuristic[] phaseHeuristics) : base(groupSize,
            phaseHeuristics)
        {
            Init();
        }

        private void Init()
        {
            PhaseGenerator = (population, individualCount, individualIndex) => (int)(individualIndex / (float)individualCount) * TotalPhaseSize;
        }
    }
}