using System;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Infrastructure.Framework.Collections;
using GeneticSharp.Infrastructure.Framework.Commons;

namespace GeneticSharp.Extensions.Tsp
{
    /// <summary>
    /// The TspPermutationEmbedding maps a TSP problem by applying a simple permutation to input city indices. The target permutation can be set to the shortest path result obtained previously, and a simple algorithm can be invoked to register a simple candidate.
    /// </summary>
    public class TspPermutationEmbedding : TspOrderedEmbedding
    {
        private readonly object mLock = new object();
       
       
        private IList<int> _TargetPermutation;
        private List<SortedDictionary<int, double>> _sortedPairDistances;

        public TspPermutationEmbedding(TspFitness fitness):base(fitness) {}

       

        /// <summary>
        /// The offspring chromosome is built from geometric values permuted back into gene space.
        /// </summary>
        /// <param name="parents">the parents producing the offspring</param>
        /// <param name="values">the offspring geometric values to convert back to gene values</param>
        /// <returns></returns>
        public override IChromosome MapFromGeometryOrdered(IList<IChromosome> parents, IList<int> values)
        {
            if (_TargetPermutation != null)
            {
                values = FromMetricSpace(values);
            }
            return base.MapFromGeometryOrdered(parents, values);
        }

        public override IList<int> MapToGeometry(IChromosome parent)
        {
            if (_TargetPermutation != null)
            {
                return ToMetricSpace(base.MapToGeometry(parent));
            }

            return base.MapToGeometry(parent);
        }

        /// <summary>
        /// When a permutation exists, creates the corresponding TSP candidate chromosome
        /// </summary>
        public TspChromosome GetMetricChromosome()
        {
            if (_TargetPermutation==null)
            {
                throw new InvalidOperationException("Cannot generate metric chromosome because Embedding permutation was not assigned yet");
            }
            var toReturn = new TspChromosome(TargetPermutation.Count);
            return (TspChromosome)IdentityMapFromGeometry(new []{toReturn}, TargetPermutation);
        }

        /// <summary>
        /// Uses the invert target permutation to convert parent values from gene space into metric-space 
        /// </summary>
        private IList<int> ToMetricSpace(IList<int> geneValues)
        {
            return geneValues.Select(g => InvertTargetPermutation[g]).ToList();
        }


        /// <summary>
        /// Uses the target permutation to convert offspring values from metric-space into gene space
        /// </summary>
        private IList<int> FromMetricSpace(IList<int> metricValues)
        {
            return metricValues.Select(p => TargetPermutation[p]).ToList();
        }

       

        /// <summary>
        /// The lazy-loaded sorted dictionary of all city distances with cities sorted by distances (typically using the Skip() linq construct)
        /// </summary>
        public List<SortedDictionary<int, double>> SortedPairDistances
        {
            get
            {
                if (_sortedPairDistances == null)
                {
                    lock (mLock)
                    {
                        if (_sortedPairDistances == null)
                        {
                            _sortedPairDistances = BuildSortedPairDistances(Fitness.CityDistances);
                        }
                    }
                }
                return _sortedPairDistances;

            }
        }

       

        private IList<int> _invertTargetPermutation;

        /// <summary>
        /// The Invert target permutation performs the invert operation from the TargetPermutation.
        /// Together they serve projecting to and from metric space.
        /// </summary>
        public IList<int> InvertTargetPermutation
        {
            get
            {
                if (_invertTargetPermutation == null && _TargetPermutation!= null)
                {
                    lock (mLock)
                    {
                        var invertTargetPermutation = new List<int>(TargetPermutation);
                        for (int i = 0; i < Fitness.Cities.Count; i++)
                        {
                            invertTargetPermutation[i] = TargetPermutation.IndexOf(i);
                        }

                        _invertTargetPermutation = invertTargetPermutation;
                    }
                }
                return _invertTargetPermutation;
            }
        }

        /// <summary>
        /// The target permutation maps original indices to a candidate TSP solution
        /// </summary>
        public IList<int> TargetPermutation
        {
            get
            {
                return _TargetPermutation;
            }
            set
            {
                lock (mLock)
                {
                    _TargetPermutation = value;
                    _invertTargetPermutation = null;
                }
            }
        }

      
        /// <summary>
        /// Default number of scans per distance ranks in the default embedding computation
        /// </summary>
        public int DefaultNbScans { get; set; } = 1;

        /// <summary>
        /// When computing the default permutation, returns the number of sorted neighbors to skip, from rank and repeatNb parameters. Optional
        /// </summary>
        public Func<int, int, int> SwapSkipsPicker { get; set; }

        /// <summary>
        /// Fluent accessors to initialize the current permutation to the default embedding
        /// </summary>
        /// <returns>the current embedding</returns>
        public TspPermutationEmbedding WithDefaultEmbedding()
        {
            RegisterDefaultEmbedding();
            return this;
        }

        /// <summary>
        /// Initializes the current permutation to the default embedding
        /// </summary>
        public virtual void RegisterDefaultEmbedding()
        {
            TargetPermutation = ComputeSimpleEmbedding();
        }

        /// <summary>
        /// Computes a TSP solution naively by running through all cities several times, and swaping immediat neighboors if larger than a moving ranked distance.
        /// </summary>
        /// <returns>a candidate solution to the TSP problem</returns>
        public IList<int> ComputeSimpleEmbedding()
        {
            var targetChromosome = new TspChromosome(Enumerable.Range(0, Fitness.Cities.Count).ToList());
            Fitness.ComputeDistance(targetChromosome, out var targetPermutation);

            for (int rank = 2; rank < Fitness.Cities.Count - 2; rank++)
            {
                for (int repeatIndex = 0; repeatIndex < DefaultNbScans; repeatIndex++)
                {
                    for (int indexCity = 0; indexCity < Fitness.Cities.Count; indexCity++)
                    {
                        var next = (indexCity + 1).PositiveMod(Fitness.Cities.Count);
                        var minDist = SortedPairDistances[indexCity].Skip(Fitness.Cities.Count - 1 - rank).First();
                        if (Fitness.CityDistances[indexCity][next] > minDist.Value)
                        {
                            int swapIndex;
                            if (SwapSkipsPicker != null)
                            {
                                var skipNb = SwapSkipsPicker(rank, repeatIndex);
                                swapIndex = SortedPairDistances[indexCity].Skip(skipNb).First().Key;
                            }
                            else
                            {
                                swapIndex = minDist.Key;
                            }

                            var gainPotential = GetDistanceGainFromSwap(targetChromosome, next, swapIndex);
                            if (gainPotential > 0)
                            {
                                targetPermutation.Swap(next, swapIndex);
                                targetChromosome = new TspChromosome(targetPermutation);
                            }
                        }
                    }
                }
            }
            return targetPermutation;
        }

        /// <summary>
        /// Computes a sorted list of sorted distances between cities, for easier recall with distance rank parameter
        /// </summary>
        /// <param name="pairDistances">The city pair distances as obtained from the fitness class instance</param>
        private List<SortedDictionary<int, double>> BuildSortedPairDistances(double[][] pairDistances)
        {
            var toReturn = new List<SortedDictionary<int, double>>(Fitness.Cities.Count);

            for (var i = 0; i < pairDistances.Length; i++)
            {
                var pairDistance = pairDistances[i];
                var distComparer = new DynamicComparer<int>((firstIndex, secondIndex) =>
                {
                    var dist = pairDistance[firstIndex] - pairDistance[secondIndex];

                    if (dist == 0)
                    {
                        dist = (firstIndex - secondIndex) * Math.Log(firstIndex + 1 / (secondIndex + 1));
                    }

                    if (dist == 0)
                        return 0;
                    if (dist < 0)
                        return -1;
                    return 1;
                });
                var pairDictionary = pairDistance.Select((d, j) => (d, j))
                    .ToDictionary(tuple => tuple.j, tuple => tuple.d);
                var keyDictionary = new SortedDictionary<int, double>(pairDictionary, distComparer);
                toReturn.Add(keyDictionary);
            }

            return toReturn;
        }


       
    }
}