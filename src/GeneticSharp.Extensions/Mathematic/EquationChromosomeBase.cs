using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Infrastructure.Framework.Commons;

namespace GeneticSharp.Extensions.Mathematic
{
    /// <summary>
    /// An equation chromosome.
    /// </summary>
    public abstract class EquationChromosomeBase<TValue> : ChromosomeBase
    {

        #region Constructors        


        public EquationChromosomeBase(int variablesNumber) : base(variablesNumber) {}


        /// <summary>
        /// Initializes a new instance of the <see cref="EquationChromosome"/> class.
        /// </summary>
        /// <param name="expectedResult">The equation expected result.</param>
        /// <param name="variablesNumber">The equation variables number.</param>
        public EquationChromosomeBase(TValue minValue, TValue maxValue, int variablesNumber) : base(variablesNumber)
        {

            MinValue = minValue;
            MaxValue = maxValue;

        }
        #endregion

        #region Properties
       
        public TValue MinValue { get; protected set; }

        public TValue MaxValue { get; protected set; }
        #endregion

        #region Methods        
        

        /// <summary>
        /// Generates the gene.
        /// </summary>
        /// <param name="geneIndex">Index of the gene.</param>
        /// <returns>The generated gene.</returns>
        public override Gene GenerateGene(int geneIndex)
        {
            return new Gene(GetRandomGeneValue(MinValue, MaxValue));
        }

        public abstract TValue GetRandomGeneValue(TValue min, TValue max);

        #endregion
    }



    public class EquationChromosome<TValue> : EquationChromosomeBase<TValue>
    {

        private IRandomization _random = RandomizationProvider.Current;

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
            return (min.To<double>() + _random.GetDouble()* max.To<double>()).To<TValue>();
        }
    }


}