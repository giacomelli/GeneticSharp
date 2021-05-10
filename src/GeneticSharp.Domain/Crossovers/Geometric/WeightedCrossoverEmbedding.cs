using System;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Domain.Crossovers.Geometric
{
    public class WeightedCrossoverEmbedding : IGeometryEmbedding<double>
    {

        public IWeightedCrossover WeightedCrossover { get; set; }


        public IChromosome MapFromGeometry(IList<IChromosome> parents, IList<double> offSpringValues)
        {
            //var oneHotGeom = offSpringValues.Select(d => Math.Abs(1 - d));
            //var bestValue = oneHotGeom.Min();
            ////var worstValue = oneHotGeom.Max();
            ////var span = bestValue+worstValue;
            //todo:figure out the right geometrisation
            //var weights = oneHotGeom.Select(v => 1/(1+v)).ToArray();
            var distanceToFirst = Math.Abs(1 - offSpringValues[0]);
            var weights = new double[] {0.5, distanceToFirst};
          
            var children = WeightedCrossover.PerformWeightedCross(parents, weights);
           if (children.Count>1)
           {
               var targetIdx = RandomizationProvider.Current.GetInt(0, children.Count);
               return children[targetIdx];
           }

           return children[0];
        }

        public IList<IList<double>> MapToGeometry(IList<IChromosome> parents)
        {
            //IList<IList<double>> oneHotEncoded = parents.Select((p, pi) =>
            //    Enumerable.Range(0, parents.Count).Select(i => i == pi ? 1.0 : 0.0).ToArray()).ToArray();
            //return oneHotEncoded;
            if (parents.Count>2)
            {
                throw new InvalidOperationException("Only 2 parents crossover are supported");
            }
            return new IList<double>[] {new double[] {1}, new double[] {0}};
        }
    }
}