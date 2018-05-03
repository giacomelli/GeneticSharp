using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Chromosomes;
using System.Threading;
using UnityEngine;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System;
using System.Linq;

namespace GeneticSharp.Runner.UnityApp.Car
{
    public class StringChromosome : BinaryChromosomeBase
    {
        public StringChromosome(int length) : base(length)
        {
        }

        public override IChromosome CreateNew()
        {
            return new StringChromosome(Length);
        }
    }
}