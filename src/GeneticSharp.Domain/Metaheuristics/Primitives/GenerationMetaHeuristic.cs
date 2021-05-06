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
            DynamicParameter = new ExpressionMetaHeuristicParameter<int>
            {
                DynamicGenerator = (IMetaHeuristic h, IEvolutionContext ctx) =>
                    GetGenerationPhase(ctx), Scope = ParamScope.Generation | ParamScope.MetaHeuristic
            };
        }

        private int GetGenerationPhase(IEvolutionContext ctx)
        {
            return (ctx.Population.GenerationsNumber - 1) % PhaseSizes.TotalPhaseSize;
        }
    }
}