using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneticSharp.Extensions.Mathematic
{
	/// <summary>
	/// Function builder input.
	/// </summary>
    public class FunctionBuilderInput
    {
		/// <summary>
		/// Gets or sets the arguments.
		/// </summary>
		/// <value>The arguments.</value>
        public double[] Arguments { get; set; }

		/// <summary>
		/// Gets or sets the expected result.
		/// </summary>
		/// <value>The expected result.</value>
        public double ExpectedResult { get; set; }
    }
}

