using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Extensions.Mathematic
{
    /// <summary>
    /// An equation chromosome.
    /// </summary>
    public class EquationChromosome : ChromosomeBase
    {
        #region Fields
        private int m_expectedResult;
        #endregion

        #region Constructors        
        /// <summary>
		/// Initializes a new instance of the <see cref="EquationChromosome"/> class.
        /// </summary>
        public EquationChromosome(int expectedResult, int variablesNumber) : base(variablesNumber)
        {
            m_expectedResult = expectedResult + 10;

            for (int i = 0; i < Length; i++)
            {
                ReplaceGene(i, GenerateGene(i));
            }
        }
        #endregion

        #region Methods        
        /// <summary>
        /// Creates the new.
        /// </summary>
        /// <returns></returns>
        public override IChromosome CreateNew()
        {
            return new EquationChromosome(m_expectedResult, Length);
        }

        /// <summary>
        /// Generates the gene.
        /// </summary>
        /// <param name="geneIndex">Index of the gene.</param>
        /// <returns></returns>
        public override Gene GenerateGene(int geneIndex)
        {
            return new Gene(RandomizationProvider.Current.GetInt(m_expectedResult * -1, m_expectedResult + 1));
        }
        #endregion
    }
}
