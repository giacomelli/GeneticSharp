using System.Collections.Generic;

namespace GeneticSharp.Extensions.Mathematic
{
    /// <summary>
    /// Function builder input.
    /// </summary>
    public class FunctionBuilderInput
    {
        #region Constructors        
        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionBuilderInput"/> class.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <param name="expectedResult">The expected result.</param>
        public FunctionBuilderInput(IList<double> arguments, double expectedResult)
        {
            Arguments = arguments;
            ExpectedResult = expectedResult;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the arguments.
        /// </summary>
        /// <value>The arguments.</value>
        public IList<double> Arguments { get; private set; }

        /// <summary>
        /// Gets the expected result.
        /// </summary>
        /// <value>The expected result.</value>
        public double ExpectedResult { get; private set; }
        #endregion
    }
}