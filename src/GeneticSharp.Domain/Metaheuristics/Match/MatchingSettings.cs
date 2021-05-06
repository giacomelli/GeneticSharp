using System;
using GeneticSharp.Domain.Metaheuristics.Parameters;

namespace GeneticSharp.Domain.Metaheuristics.Matching
{
    public struct MatchingSettings
    {


        public static ParamScope GetDefaultScope(MatchingKind kind)
        {
            switch (kind)
            {
                case MatchingKind.Current:
                case MatchingKind.Neighbor:
                case MatchingKind.Random:
                case MatchingKind.RouletteWheel:
                case MatchingKind.Child:
                case MatchingKind.Custom:
                    return ParamScope.None;
                case MatchingKind.Best:
                case MatchingKind.Worst:
                    return ParamScope.MetaHeuristic | ParamScope.Generation;
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }
        
        public MatchingKind MatchingKind { get; set; }

        public int AdditionalPicks { get; set; } 
        public bool ResetIndex { get; set; }
        
        public ParamScope CachingScope { get; set; }

        public bool RandomIncludesCurrent { get; set; }


        public static int[] GetTechniqueCounter()
        {
            return new int[8];
        }


    }
}