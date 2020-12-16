using System.ComponentModel;
using GeneticSharp.Domain.Metaheuristics.Parameters;

namespace GeneticSharp.Domain.Metaheuristics.Primitives
{
    /// <summary>
    /// The generation based Metaheuristcs allows applying distinct Metaheuristics depending on the generation number. By default, it cycles alternating between phases during generation periods corresponding to the phase sizes
    /// </summary>
    [DisplayName("Generation")]
    public class GenerationMetaHeuristic : SizeBasedMetaHeuristic
    {


        public GenerationMetaHeuristic()
        {
            Init();
        }


        public GenerationMetaHeuristic(int phaseDuration, params IMetaHeuristic[] phaseHeuristics) : base(phaseDuration, phaseHeuristics)
        {
            Init();
        }

        private void Init()
        {
            DynamicParameter = new ExpressionMetaHeuristicParameter<int> {DynamicGenerator = (h, ctx) => ctx.Population.GenerationsNumber, Scope = ParamScope.Generation};
        }
    }
}