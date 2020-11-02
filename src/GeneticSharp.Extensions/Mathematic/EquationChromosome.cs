using System;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Extensions.Mathematic
{
    /// <summary>
    /// An equation chromosome.
    /// </summary>
    public sealed class EquationChromosome : ChromosomeBase
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
                throw new ArgumentOutOfRangeException("expectedResult", expectedResult, "EquationChromosome expected value must be lower");
            }

            ResultRange = expectedResult * 2;

            for (int i = 0; i < Length; i++)
            {
                ReplaceGene(i, GenerateGene(i));
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the result range.
        /// </summary>
        /// <value>The result range.</value>
        public int ResultRange { get; private set; }
        #endregion

        #region Methods        
        /// <summary>
        /// Creates the new.
        /// </summary>
        /// <returns>The new chromosome.</returns>
        public override IChromosome CreateNew()
        {
            return new EquationChromosome(ResultRange / 2, Length);
        }

        /// <summary>
        /// Generates the gene.
        /// </summary>
        /// <param name="geneIndex">Index of the gene.</param>
        /// <returns>The generated gene.</returns>
        public override Gene GenerateGene(int geneIndex)
        {
            return new Gene(RandomizationProvider.Current.GetInt(ResultRange * -1, ResultRange + 1));
        }
        #endregion
    }
}
