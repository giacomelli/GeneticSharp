namespace GeneticSharp.Domain.Metaheuristics
{
    /// <summary>
    /// The generation based Metaheuristcs allows applying distinct Metaheuristics depending on the generation number. By default, it cycles alternating between phases during generation periods corresponding to the phase sizes
    /// </summary>
    public class GenerationMetaHeuristic : SizeBasedMetaHeuristic
    {


        public GenerationMetaHeuristic() : base()
        {
            Init();
        }

        public GenerationMetaHeuristic(int phaseDuration, params IMetaHeuristic[] phaseHeuristics) : base(phaseDuration, phaseHeuristics)
        {
            Init();
        }

        private void Init()
        {
            IndexGenerator = (h, ctx) => ctx.Population.GenerationsNumber % TotalPhaseSize;
        }
    }
}