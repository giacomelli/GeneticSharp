namespace GeneticSharp.Domain.Metaheuristics.Probability
{
    public class OperatorsProbabilityConfig
    {

        public ProbabilityConfig Crossover { get; set; } = new ProbabilityConfig();
        public ProbabilityConfig Mutation { get; set; } = new ProbabilityConfig();



    }
}