using System;
using GeneticSharp.Domain.Metaheuristics;

namespace GeneticSharp.Domain.Metaheuristics.Probability
{
    public class ProbabilityConfig
    {

        public ProbabilityStrategy Strategy { get; set; } //= ProbabilityStrategy.TestProbability | ProbabilityStrategy.OverwriteProbability;

        public float StaticProbability { private get; set; } = 1;

        public Func<IEvolutionContext, float, float> DynamicProbability { private get; set; }

        public float GetProbability(IEvolutionContext ctx, float initialProbability)
        {
            return (Strategy & ProbabilityStrategy.OverwriteProbability) == ProbabilityStrategy.OverwriteProbability ? GetCutomProbability(ctx, initialProbability) : initialProbability;
        }

        private float GetCutomProbability(IEvolutionContext ctx, float initialProbability)
        {
            if (DynamicProbability==null)
            {
                return StaticProbability;
            }

            return DynamicProbability(ctx, initialProbability);
        }


    }
}