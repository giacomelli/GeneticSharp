using System;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Infrastructure.Framework.Commons;

namespace GeneticSharp.Extensions.Mathematic
{
    public class EquationChromosome<TValue> : EquationChromosomeBase<TValue> where TValue : IComparable
    {
        private readonly IRandomization _random = RandomizationProvider.Current;

        public EquationChromosome(int variablesNumber) : base(variablesNumber)
        {
            GetGeneValueFunction = DefaultGetGeneValueFunction;
        }

        public EquationChromosome(TValue minValue, TValue maxValue, int variablesNumber) : base(minValue, maxValue, variablesNumber)
        {
            GetGeneValueFunction = DefaultGetGeneValueFunction;
        }

        public override IChromosome CreateNew()
        {
            return new EquationChromosome<TValue>(Length){Ranges = new List<(TValue min, TValue max)>(Ranges)};
        }

        public override TValue GetRandomGeneValue(int geneIndex, TValue min, TValue max)
        {
            return GetGeneValueFunction(geneIndex, min.To<double>() + _random.GetDouble()* (max.To<double>() - min.To<double>()));
        }

        public Func<int, double, TValue> GetGeneValueFunction { get; set; } 

        private TValue DefaultGetGeneValueFunction(int geneIndex, double geneValue)
        {
            var toReturn = geneValue.To<TValue>();
            if (toReturn.CompareTo(Ranges[geneIndex].min) < 0)
            {
                toReturn = Ranges[geneIndex].min;
            }
            else if (toReturn.CompareTo(Ranges[geneIndex].max) > 0)
            {
                toReturn = Ranges[geneIndex].max;
            }

            return toReturn;
        }
    }

    /// <summary>
    /// An equation chromosome.
    /// </summary>
    public sealed class EquationChromosome : EquationChromosomeBase<int>
    {

        public EquationChromosome(int length):base(length){}

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EquationChromosome"/> class.
        /// </summary>
        /// <param name="expectedResult">The equation expected result.</param>
        /// <param name="variablesNumber">The equation variables number.</param>
        public EquationChromosome(int expectedResult, int variablesNumber) : base(variablesNumber)
        {
            if (expectedResult >= int.MaxValue / 2)
            {
                throw new ArgumentOutOfRangeException(nameof(expectedResult), expectedResult,
                    "EquationChromosome expected value must be lower");
            }
            Ranges = Enumerable.Repeat((-Math.Abs(expectedResult * 2), Math.Abs(expectedResult * 2)), variablesNumber).ToList();
            ResultIsNegative = expectedResult < 0;
        }

        #endregion

        #region Properties

        public bool ResultIsNegative { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Creates the new.
        /// </summary>
        /// <returns>The new chromosome.</returns>
        public override IChromosome CreateNew()
        {
            if (ResultIsNegative)
            {
                return new EquationChromosome(Length);
            }

            return new EquationChromosome(Length){Ranges = new List<(int min, int max)>(Ranges)};
        }


        public override int GetRandomGeneValue(int geneIndex, int min, int max)
        {
            return RandomizationProvider.Current.GetInt(min, max + 1);
        }

        #endregion
    }
}