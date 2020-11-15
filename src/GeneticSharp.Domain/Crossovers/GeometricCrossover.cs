using System;
using System.Collections.Generic;
using System.ComponentModel;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Infrastructure.Framework.Commons;

namespace GeneticSharp.Domain.Crossovers
{
    /// <summary>
    /// The Geometric crossover yields new genes by applying geometric operators on the parent genes values. Default operator convert genes to doubles computes the middle and converts back to the target type
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    [DisplayName("Geometric")]
    public class GeometricCrossover<TValue> : CrossoverBase
    {
        public GeometricCrossover() : base(2, 1)
        {
            GeometricOperator = (geneVal1, geneVal2) =>
                ((geneVal1.To<double>() + geneVal2.To<double>()) / 2).To<TValue>();
        }

        public Func<TValue, TValue, TValue> GeometricOperator { get; set; }
      

        protected override IList<IChromosome> PerformCross(IList<IChromosome> parents)
        {
            return PerformCross(parents, GeometricOperator);
        }


        public IList<IChromosome> PerformCross(IList<IChromosome> parents, Func<TValue, TValue, TValue> geometricOperator)
        {
            var parent1 = parents[0];
            var parent2 = parents[1];
            var offspring = parent1.CreateNew();
            for (int i = 0; i < parent1.Length; i++)
            {
                offspring.ReplaceGene(i, new Gene(geometricOperator((TValue)parent1.GetGene(i).Value, (TValue)parent2.GetGene(i).Value)));
            }
            return new[] { offspring };
        }


    }
}