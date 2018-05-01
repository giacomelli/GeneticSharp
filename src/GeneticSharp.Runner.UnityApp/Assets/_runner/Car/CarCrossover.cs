using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Chromosomes;
using System.Threading;
using UnityEngine;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Runner.UnityApp.Car
{
    public class CarCrossover : OnePointCrossover
    {
        public CarCrossover() : base(0)
        {
        }

		protected override IChromosome CreateChild(IChromosome leftParent, IChromosome rightParent)
		{
            var rnd = RandomizationProvider.Current;
            var swapPoint1 = rnd.GetInt(1, leftParent.Length - 1);
            var swapPoint2 = rnd.GetInt(swapPoint1 + 1, leftParent.Length);

            var child = leftParent.CreateNew();
            child.ReplaceGenes(0, leftParent.GetGenes().Take(swapPoint1).ToArray());
            child.ReplaceGenes(swapPoint1, rightParent.GetGenes().Skip(swapPoint1).Take(swapPoint2 - swapPoint1).ToArray());
            child.ReplaceGenes(swapPoint2, leftParent.GetGenes().Skip(swapPoint2).ToArray());

            return child;
		}
    }
}