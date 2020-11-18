using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Metaheuristics;
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

        private static readonly Func<IList<TValue>, TValue> _defaultGeometricOperator = geneValues => (geneValues.Sum(val => val.To<double>()) / 2).To<TValue>();

        public GeometricCrossover() : this(2, false) { }

        public GeometricCrossover(int parentNb, bool generateTwin) : base(parentNb, generateTwin ? 2 : 1)
        {
            GeometricOperator = _defaultGeometricOperator;
        }

        public GeometricCrossover(int parentNb, Func<IList<TValue>, TValue> geometricOperator, bool generateTwin = false) : this(parentNb, generateTwin)
        {
            GeometricOperator = geometricOperator;
        }

        public Func<IList<TValue>, TValue> GeometricOperator { get; set; }
      

        protected override IList<IChromosome> PerformCross(IList<IChromosome> parents)
        {
            return PerformCross(parents, GeometricOperator);
        }


        public IList<IChromosome> PerformCross(IList<IChromosome> parents, Func<IList<TValue>, TValue> geometricOperator)
        {
           var toReturn = new List<IChromosome>(ChildrenNumber);
           var firstChild = CreateOffspring(parents, geometricOperator);
           toReturn.Add(firstChild);
           if (ChildrenNumber==2)
           {
               parents = parents.Reverse().ToList();
               var twinChild = CreateOffspring(parents, geometricOperator);
               toReturn.Add(twinChild);
           }
           return toReturn;

        }

        private IChromosome CreateOffspring(IList<IChromosome> parents, Func<IList<TValue>, TValue> geometricOperator)
        {
            var offspring = parents[0].CreateNew();
            for (int i = 0; i < offspring.Length; i++)
            {
                var newGeneValue = geometricOperator(parents.Select(p => (TValue)p.GetGene(i).Value).ToList());
                offspring.ReplaceGene(i, new Gene(newGeneValue));
            }

            return offspring;
        }


    }
}