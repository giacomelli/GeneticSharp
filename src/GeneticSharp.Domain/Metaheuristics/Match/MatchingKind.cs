namespace GeneticSharp.Domain.Metaheuristics.Matching
{
    public enum MatchingKind
    {
        Current = 0,
        Neighbor = 1,
        Random = 2,
        RouletteWheel = 3,
        Best = 4,
        Worst = 5,
        Child = 6,
        Custom = 7
    }
}