using System;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Extensions.Tsp
{
    /// <summary>
    /// Travelling Salesman Problem fitness function.
    /// <remarks>
    /// The travelling salesman problem (TSP) or travelling salesperson problem asks the following question: 
    /// Given a list of cities and the distances between each pair of cities, what is the shortest possible 
    /// route that visits each city exactly once and returns to the origin city?
    /// <see href="http://en.wikipedia.org/wiki/Travelling_salesman_problem">Wikipedia</see> 
    /// </remarks>
    /// </summary>
    public class TspFitness : IFitness
    {

        //This is the max nb of cities to enable caching
        private const int MaxCityNbCachedDistances = 2001;


        #region Private fields


        private double? mMinDistanceApprox;
        private double? mMaxDistanceApprox;
        private (TspCity, TspCity)? mBoundingBox;
        private List<List<double>> _cityDistances;
        private readonly object mLock = new object();
        private bool _cached;

        #endregion


        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Extensions.Tsp.TspFitness"/> class.
        /// </summary>
        /// <param name="numberOfCities">The number of cities.</param>
        /// <param name="minX">The minimum city x coordinate.</param>
        /// <param name="maxX">The maximum city x coordinate.</param>
        /// <param name="minY">The minimum city y coordinate.</param>
        /// <param name="maxY">The maximum city y coordinate..</param>
        public TspFitness(int numberOfCities, int minX, int maxX, int minY, int maxY)
        {
            Cities = new List<TspCity>(numberOfCities);
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;


            if (maxX >= int.MaxValue)
            {
                throw new ArgumentOutOfRangeException("maxX");
            }

            if (maxY >= int.MaxValue)
            {
                throw new ArgumentOutOfRangeException("maxY");
            }

            for (int i = 0; i < numberOfCities; i++)
            {
                var city = new TspCity(RandomizationProvider.Current.GetInt(MinX, maxX + 1), RandomizationProvider.Current.GetInt(MinY, maxY + 1));
                Cities.Add(city);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the cities.
        /// </summary>
        /// <value>The cities.</value>
        public IList<TspCity> Cities { get; private set; }

        /// <summary>
        /// Gets the minimum x.
        /// </summary>
        /// <value>The minimum x.</value>
        public int MinX { get; private set; }

        /// <summary>
        /// Gets the max x.
        /// </summary>
        /// <value>The max x.</value>
        public int MaxX { get; private set; }

        /// <summary>
        /// Gets the minimum y.
        /// </summary>
        /// <value>The minimum y.</value>
        public int MinY { get; private set; }

        /// <summary>
        /// Gets the max y.
        /// </summary>
        /// <value>The max y.</value>
        public int MaxY { get; private set; }

        /// <summary>
        /// Corresponding to edge case with cities aligned 
        /// </summary>
        public double MinDistanceApprox
        {
            get
            {
                if (!mMinDistanceApprox.HasValue)
                {
                    lock (mLock)
                    {
                        mMinDistanceApprox = GetMinDistanceApprox();
                    }
                }

                return mMinDistanceApprox.Value;
            }
        }

       

        /// <summary>
        /// Corresponding to edge case with half cities in opposite corners of bounding box
        /// </summary>
        public double MaxDistanceApprox
        {
            get
            {
                if (!mMaxDistanceApprox.HasValue)
                {
                    lock (mLock)
                    {
                        mMaxDistanceApprox = (Cities.Count) * CalcDistanceTwoCities(BoundingBox.Value.Item1, BoundingBox.Value.Item2);
                    }
                    
                }

                return mMaxDistanceApprox.Value;
            }

        }

        /// <summary>
        /// The smallest rectangle that comprises all cities
        /// </summary>
        public (TspCity, TspCity)? BoundingBox
        {
            get
            {
                if (!mBoundingBox.HasValue)
                {
                    lock (mLock)
                    {
                        mBoundingBox = GetBoundingBox();
                    }
                }
                return mBoundingBox;
            }
        }

        /// <summary>
        /// Defines whether the fitness instance should keep city distances in a cached dictionary for easier retrieval
        /// </summary>
        public bool Cached
        {
            get => _cached;
            set
            {
                if (value && Cities.Count>MaxCityNbCachedDistances)
                {
                    throw new InvalidOperationException($"Cannot use distance caching above {MaxCityNbCachedDistances} cities");
                }
                _cached = value;
            }
        }

        /// <summary>
        /// The lazy-loaded dictionary of all city distances
        /// </summary>
        public List<List<double>> CityDistances
        {
            get
            {
                if (_cityDistances == null)
                {
                    lock (mLock)
                    {
                        if (_cityDistances == null)
                        {
                            _cityDistances = BuildPairDistances();
                        }
                    }
                }
                return _cityDistances;

            }
        }

        #endregion


        #region IFitness implementation
        /// <summary>
        /// Performs the evaluation against the specified chromosome. Assumes gene having int32 city indices.
        /// Computes the distance of the corresponding city path.
        /// Then compares the distance to known minimum bound (with fitness 1) and maximum bound (with fitness 0)
        /// Normalises the result with random path at about fitness 0.5 consistantly among city numbers
        /// If the tour is missing some cities, the resulting fitness is divided by 1+the number of missing cities
        /// </summary>
        /// <param name="chromosome">The chromosome to be evaluated, with integer genes for city indices.</param>
        /// <returns>The fitness of the chromosome.</returns>
        public double Evaluate(IChromosome chromosome)
        {

            var distanceSum = ComputeDistance(chromosome, out var citiesIndexes);

            //Calibrated to close to 1 when distanceSum closes on approx MinDistanceApprox, and 0 when closing on MaxDistanceApprox.
            //With current Min and Max approximations, consistantly yields a mean fitness of 0.631 for a random chromosome with city nb > 50 (higher below, because with small city numbers, random case gets closer to Min case)
            
            var fitness = 1 - (distanceSum - MinDistanceApprox) /MaxDistanceApprox ;   // 1.0 - distanceSum / MaxDistanceApprox;

            ((TspChromosome)chromosome).Distance = distanceSum;

            if (fitness < 0)
            {
                //Worst than worst case
                fitness = 0;
            }

            // There is repeated cities, or missing cities on the indexes?
            var diff = Cities.Count - citiesIndexes.Distinct().Count();

            if (diff > 0)
            {
                fitness =  1.0/(diff+1);
            }
            

            return fitness;
        }

        #endregion


        #region Methods

      

        /// <summary>
        /// Computes the distance of tourning all input cities from a chromosome with integer index genes in a closed circuit 
        /// </summary>
        public double ComputeDistance(IChromosome chromosome, out List<int> cityIndices)
        {
            cityIndices = chromosome.GetGenes().Select(g=>(int?) g.Value ?? 0).ToList();
            return ComputeDistance(cityIndices);
        }

       

        /// <summary>
        /// Computes the distance of tourning all input city indices in a closed circuit 
        /// </summary>
        public double ComputeDistance(IList<int> cityIndices)
        {
            
            var distanceSum = 0.0;
            var lastCityIndex = cityIndices[cityIndices.Count -1];

            for (int i = 0;  i < cityIndices.Count; i++)
            {
                var currentCityIndex = cityIndices[i];

                if (Cached)
                {
                    distanceSum += CityDistances[lastCityIndex][currentCityIndex];
                }
                else
                {
                    distanceSum += CalcDistanceTwoCities(Cities[currentCityIndex], Cities[lastCityIndex]);
                }
                lastCityIndex = currentCityIndex;


            }
            return distanceSum;
        }

        /// <summary>
        /// Calculates the distance between two cities.
        /// </summary>
        /// <returns>The distance two cities.</returns>
        /// <param name="one">City one.</param>
        /// <param name="two">City two.</param>
        public static double CalcDistanceTwoCities(TspCity one, TspCity two)
        {
            return Math.Sqrt(Math.Pow(two.X - one.X, 2) + Math.Pow(two.Y - one.Y, 2));
        }

        private (TspCity, TspCity) GetBoundingBox()
        {
            var xMin = Cities.Min(c => c.X);
            var xMax = Cities.Max(c => c.X);
            var yMin = Cities.Min(c => c.Y);
            var yMax = Cities.Max(c => c.Y);
            return (new TspCity(xMin, yMin), new TspCity(xMax, yMax));
        }


        
        private readonly double mMinDistanceScale = 2 * Math.Sqrt(2);

        /// <summary>
        /// We consider the worst case with cities aligned along a boundingbox diameter or in a diamond shape accross the bounding box
        /// </summary>
        /// <returns></returns>
        private double GetMinDistanceApprox()
        {
            if (Cities.Count>20)
                return mMinDistanceScale * CalcDistanceTwoCities(BoundingBox.Value.Item1, BoundingBox.Value.Item2);

            return 2 *  CalcDistanceTwoCities(BoundingBox.Value.Item1, BoundingBox.Value.Item2);

        }

       


        /// <summary>
        /// Builds a double dictionary structure with all city distance pairs
        /// </summary>
        private List<List<double>> BuildPairDistances()
        {
            //Note: The pairs are computed twice, could be optimized
            var toReturn = new List<List<double>>(Cities.Count);
            for (int i = 0; i < Cities.Count; i++)
            {
                var distFromI = new List<double>(Cities.Count);
                for (int j = 0; j < Cities.Count; j++)
                {
                    if (i==j)
                    {
                        distFromI.Add(0);
                    }
                    else
                    {
                        distFromI.Add(CalcDistanceTwoCities(Cities[i], Cities[j]));
                    }

                }
                toReturn.Add(distFromI);
            }
            return toReturn;
        }

        #endregion
    }
}