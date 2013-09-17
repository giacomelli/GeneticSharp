using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;

namespace GeneticSharp.Extensions.Ghostwriter
{
    public class GhostwriterFitness : IFitness
    {
        public Func<string, double> EvaluateFunc { get; set; }

        public double Evaluate(IChromosome chromosome)
        {
            var c = chromosome as GhostwriterChromosome;
            var text = c.GetText();

            return EvaluateFunc(text);
        }
    }
}
