using System;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Infrastructure.Framework.Commons;

namespace GeneticSharp.Extensions.Tsp
{
    /// <summary>
    /// The default TspOrdered Embedding accounts for gene ordering with a gene swap validator. It has no associated projection space 
    /// </summary>
    public class TspOrderedEmbedding : OrderedEmbedding<int>
    {

        private readonly TspFitness _fitness;

        public TspOrderedEmbedding(TspFitness fitness)
        {
            _fitness = fitness;
        }

        /// <summary>
        /// The TSPGeometry Embedding only works in an ordered scenerio
        /// </summary>
        public override bool IsOrdered
        {
            get => true;
            set
            {
                if (value!=true)
                {
                    throw new InvalidOperationException("Unordered mode is not supported in TspOrderedEmbedding");
                }
            }
        }


        public TspFitness Fitness => _fitness;



        /// <summary>
        /// Replaces the native passthrough to check if the city swap increases the tour distance or not without summing over all cities.
        /// </summary>
        protected override Func<IChromosome, int, int, bool> GetDefaultSwapValidationFunction()
        {
            return (chromosome, swapIndex1, swapIndex2) =>
                GetDistanceGainFromSwap(((TspChromosome)chromosome), swapIndex1, swapIndex2) > 0;
        }

        /// <summary>
        /// Computes the difference in distance by an index swap without having to sum over all the cities. 
        /// </summary>
        /// <param name="initialOrder"></param>
        /// <param name="swapIndex1"></param>
        /// <param name="swapIndex2"></param>
        /// <returns></returns>
        public double GetDistanceGainFromSwap(TspChromosome initialOrder, int swapIndex1, int swapIndex2)
        {

            if (swapIndex1 == swapIndex2)
            {
                return 0;
            }
            var firstTriple = Enumerable.Range(0, 3).Select(i => (int)initialOrder.GetGene((swapIndex1 - 1 + i).PositiveMod(_fitness.Cities.Count)).Value).ToList();
            var secondTriple = Enumerable.Range(0, 3).Select(i => (int)initialOrder.GetGene((swapIndex2 - 1 + i).PositiveMod(_fitness.Cities.Count)).Value).ToList();


            double initDistance, targetDistance;
            var indicesModDifference = (swapIndex1 - swapIndex2).PositiveMod(_fitness.Cities.Count);
            var notContigous = indicesModDifference > 1 && indicesModDifference < _fitness.Cities.Count - 1;

            if (notContigous)
            {
                initDistance =
                    _fitness.CityDistances[firstTriple[0]][firstTriple[1]]
                    + _fitness.CityDistances[firstTriple[1]][firstTriple[2]]
                    + _fitness.CityDistances[secondTriple[0]][secondTriple[1]]
                    + _fitness.CityDistances[secondTriple[1]][secondTriple[1]];


                targetDistance = _fitness.CityDistances[firstTriple[0]][secondTriple[1]]
                                 + _fitness.CityDistances[secondTriple[1]][firstTriple[2]]
                                 + _fitness.CityDistances[secondTriple[0]][firstTriple[1]]
                                 + _fitness.CityDistances[firstTriple[1]][secondTriple[2]];
            }
            else
            {
                initDistance = _fitness.CityDistances[firstTriple[0]][firstTriple[1]]
                               + _fitness.CityDistances[firstTriple[1]][firstTriple[2]]
                               + _fitness.CityDistances[secondTriple[1]][secondTriple[2]];
                targetDistance = _fitness.CityDistances[firstTriple[0]][secondTriple[1]]
                                 + _fitness.CityDistances[secondTriple[1]][firstTriple[1]]
                                 + _fitness.CityDistances[firstTriple[1]][secondTriple[2]];
            }


            var toReturn = initDistance - targetDistance;

            return toReturn;



        }


    }
}