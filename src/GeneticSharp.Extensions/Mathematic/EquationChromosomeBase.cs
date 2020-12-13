using System;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;

namespace GeneticSharp.Extensions.Mathematic
{
    /// <summary>
    /// An equation chromosome.
    /// </summary>
    public abstract class EquationChromosomeBase<TValue> : ChromosomeBase where TValue: IComparable
    {

        #region Constructors        


        protected EquationChromosomeBase(int variablesNumber) : base(variablesNumber) {}


        /// <summary>
        /// Initializes a new instance of the <see cref="EquationChromosome"/> class.
        /// </summary>
        /// <param name="expectedResult">The equation expected result.</param>
        /// <param name="variablesNumber">The equation variables number.</param>
        protected EquationChromosomeBase(TValue minValue, TValue maxValue, int variablesNumber) : base(variablesNumber)
        {

            //MinValue = minValue;
            //MaxValue = maxValue;
            Ranges = Enumerable.Repeat((minValue, maxValue), variablesNumber).ToList();


        }
        #endregion

        #region Properties

        public IList<(TValue min, TValue max)> Ranges { get; set; }


        //public TValue MinValue { get; protected set; }

        //public TValue MaxValue { get; protected set; }
        #endregion

        #region Methods        
        

        /// <summary>
        /// Generates the gene.
        /// </summary>
        /// <param name="geneIndex">Index of the gene.</param>
        /// <returns>The generated gene.</returns>
        public override Gene GenerateGene(int geneIndex)
        {
            return new Gene(GetRandomGeneValue(geneIndex, Ranges[geneIndex].min, Ranges[geneIndex].max));
        }

        public abstract TValue GetRandomGeneValue(int geneIndex, TValue min, TValue max);

        #endregion
    }
}