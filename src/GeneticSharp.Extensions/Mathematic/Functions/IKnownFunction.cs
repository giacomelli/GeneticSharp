using System;
using System.Collections.Generic;
using GeneticSharp.Domain.Metaheuristics;

namespace GeneticSharp.Extensions.Mathematic.Functions
{
    /// <summary>
    /// A known function is a double vector to double function with ranges, and fitness information
    /// </summary>
    public interface IKnownFunction:INamedEntity
    {
        Func<double[], double> Function { get; }
        Func<int, IList<(double min, double max)>> Ranges { get; }

        Func<double, double> Fitness { get; }
    }
}