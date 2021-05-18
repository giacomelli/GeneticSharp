using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Infrastructure.Framework.Collections;

namespace GeneticSharp.Domain.Results
{
    /// <summary>
    /// The MeanEvolutionResult class allows collecting repeated evolution results and computing mean statistical values, while skipping a percentage of extrema when Results are sorted according to a custom comparer.
    /// </summary>
    [DebuggerDisplay("Fit:{Fitness}  -  {TestSettings}, Time:{TimeEvolvingDisplay}, GenNb:{GenerationsNumber}")]
    public class MeanEvolutionResult : IEvolutionResult
    {
        private SortedSet<IEvolutionResult> _results;

        public object TestSettings { get; set; }

        public Func<IEvolutionResult, IEvolutionResult, int> ResultComparer { get; set; } = CompareSequence(CompareFitness, CompareDuration, (r1, r2) => -CompareGenerations(r1, r2)) ;


        public static Func<IEvolutionResult, IEvolutionResult, int> CompareSequence(params Func<IEvolutionResult, IEvolutionResult, int>[] comparerSequence)
        {
            return (r1, r2) => CompareSequence(r1, r2, comparerSequence);
        }


        private static int CompareSequence(IEvolutionResult result1, IEvolutionResult result2, params Func<IEvolutionResult, IEvolutionResult, int>[] comparerSequence)
        {
            //var sequenceList = new Func<IEvolutionResult, IEvolutionResult, int>[] {CompareFitness, CompareDuration, (r1, r2) => -CompareGenerations(r1, r2) };
            var toReturn = 0;
            foreach (var comparer in comparerSequence)
            {
                toReturn = comparer(result1, result2);
                if (toReturn!=0)
                {
                    return toReturn;
                }
            }
            return toReturn;
        }

        public static int CompareFitness(IEvolutionResult result1, IEvolutionResult result2)
        {
            return Math.Sign(result1.Fitness - result2.Fitness);
        }

        public static int CompareDuration(IEvolutionResult result1, IEvolutionResult result2)
        {
            return Convert.ToInt32(result1.TimeEvolving.Ticks - result2.TimeEvolving.Ticks);
        }

        public static int CompareGenerations(IEvolutionResult result1, IEvolutionResult result2)
        {
            return Convert.ToInt32(result1.GenerationsNumber - result2.GenerationsNumber);
        }


        public double SkipExtremaPercentage { get; set; } = 0.1;

        public SortedSet<IEvolutionResult> Results
        {
            get
            {
                if (_results == null)
                {
                    _results = new SortedSet<IEvolutionResult>(new DynamicComparer<IEvolutionResult>(ResultComparer));
                }
                return _results;
            }
        }

        public double Fitness => Results.Count > 0 ? GetScopedResults().Sum(r => r.Fitness) / GetScopedResults().Count() : 0;

        public IPopulation Population => Results.Count > 0 ? GetScopedResults().First().Population : null;

        public TimeSpan TimeEvolving => Results.Count > 0 ? TimeSpan.FromTicks(GetScopedResults().Sum(r => r.TimeEvolving.Ticks) / GetScopedResults().Count()) : TimeSpan.Zero;

        public string TimeEvolvingDisplay => TimeEvolving.ToString();
        public int GenerationsNumber => Results.Count > 0 ? GetScopedResults().Sum(r => r.Population.GenerationsNumber) / GetScopedResults().Count() : 0;


        protected IEnumerable<IEvolutionResult> GetScopedResults()
        {
            var skipNb = Convert.ToInt32( Math.Floor(Results.Count * SkipExtremaPercentage));
            return Results.Skip(skipNb).Take(Results.Count - 2 * skipNb);
        }


    }
}