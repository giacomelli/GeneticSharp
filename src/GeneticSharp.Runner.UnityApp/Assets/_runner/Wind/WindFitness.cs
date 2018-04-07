using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Chromosomes;
using System.Threading;
using UnityEngine;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System;
using System.Linq;

namespace GeneticSharp.Runner.UnityApp.Wind
{
    public class WindFitness : IFitness
    {
        private float m_secondsForEvaluation = 5;

        public WindFitness(float secondsForEvaluation)
        {
            m_secondsForEvaluation = secondsForEvaluation;
            ChromosomesToBeginEvaluation = new BlockingCollection<WindChromosome>();
            ChromosomesToEndEvaluation = new BlockingCollection<WindChromosome>();
        }

        public BlockingCollection<WindChromosome> ChromosomesToBeginEvaluation { get; private set; }
        public BlockingCollection<WindChromosome> ChromosomesToEndEvaluation { get; private set; }

        public double Evaluate(IChromosome chromosome)
        {
            var c = chromosome as WindChromosome;
            ChromosomesToBeginEvaluation.Add(c);
            Thread.Sleep(TimeSpan.FromSeconds(m_secondsForEvaluation));

            ChromosomesToEndEvaluation.Add(c);

            do
            {
                Thread.Sleep(100);
            } while (!c.Evaluated);

            return c.Turns;
        }

    }
}
