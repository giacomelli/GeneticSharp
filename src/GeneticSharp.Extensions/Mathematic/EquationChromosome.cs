using System;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Infrastructure.Framework.Commons;

namespace GeneticSharp.Extensions.Mathematic
{
    public class EquationChromosome<TValue> : EquationChromosomeBase<TValue>
    {

        private readonly IRandomization _random = RandomizationProvider.Current;

        public EquationChromosome(int variablesNumber) : base(variablesNumber)
        {
        }

        public EquationChromosome(TValue minValue, TValue maxValue, int variablesNumber) : base(minValue, maxValue, variablesNumber)
        {
        }

        public override IChromosome CreateNew()
        {
            return new EquationChromosome<TValue>(MinValue, MaxValue, Length);
        }

        public override TValue GetRandomGeneValue(TValue min, TValue max)
        {
            return GetGeneValueFunction(min.To<double>() + _random.GetDouble()* max.To<double>());
        }


        public Func<double, TValue> GetGeneValueFunction { get; set; } = geneValue => geneValue.To<TValue>();
        
    }

    /// <summary>
    /// An equation chromosome.
    /// </summary>
    public sealed class EquationChromosome : EquationChromosomeBase<int>
    {
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

            MinValue = -Math.Abs(expectedResult * 2);
            MaxValue = Math.Abs(expectedResult * 2);
            ResultIsNegative = expectedResult < 0;
        }

        #endregion

        #region Properties

        public bool ResultIsNegative { get; set; }

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
                return new EquationChromosome(-MaxValue / 2, Length);
            }

            return new EquationChromosome(MaxValue / 2, Length);
        }


        public override int GetRandomGeneValue(int min, int max)
        {
            return RandomizationProvider.Current.GetInt(min, max + 1);
        }

        #endregion
    }
}