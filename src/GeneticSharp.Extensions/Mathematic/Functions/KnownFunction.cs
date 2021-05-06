using System;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Metaheuristics;

namespace GeneticSharp.Extensions.Mathematic.Functions
{
    /// <summary>
    /// Default KnownFunction implementation tailored for KnownFunctionFactory, with default parameters
    /// </summary>
    public class KnownFunction : NamedEntity, IKnownFunction
    {

        public KnownFunction()
        {
            Fitness = (genes, d) => d;
        }

        public KnownFunction(double range):this()
        {
            Ranges = i => Enumerable.Repeat((-range, range), i).ToList();
        }

        public Func<double[], double> Function { get; set; }
        public Func<int, IList<(double min, double max)>> Ranges { get; set; }
        public Func<double[], double, double> Fitness { get; set; }
    }


    public static class KnownFunctionExtensions 
    {
        public static Func<double[], double> Shift(this Func<double[], double> function, double shift)
        {
            return coords => function(coords.Select(d => d - shift).ToArray());
        }

    }
}