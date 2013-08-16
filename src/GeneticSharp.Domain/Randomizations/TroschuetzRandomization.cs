using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Troschuetz.Random;

namespace GeneticSharp.Domain.Randomizations
{
    /// <summary>
	/// An IRandomization using Troschuetz (http://www.codeproject.com/Articles/15102/NET-random-number-generators-and-distributions) has pseudo-number generator.
    /// </summary>
    public class TroschuetzRandomization : RandomizationBase
    {
		#region Fields
        private Generator m_rnd = new XorShift128Generator(DateTime.Now.Millisecond);
		#endregion

		#region Methods
		/// <summary>
		/// Gets an integer value between minimum value (inclusive) and maximum value (exclusive).
		/// </summary>
		/// <returns>The integer.</returns>
		/// <param name="min">Minimum value (inclusive).</param>
		/// <param name="max">Maximum value (exclusive).</param>
        public override int GetInt(int min, int max)
        {
            return m_rnd.Next(min, max);
        }

		/// <summary>
		/// Gets a double value between 0.0 and 1.0.
		/// </summary>
		/// <returns>The double value.</returns>
        public override double GetDouble()
        {
            return m_rnd.NextDouble();
        }

		/// <summary>
		/// Gets a double value between minimum value (inclusive) and maximum value (exclusive).
		/// </summary>
		/// <returns>The double value.</returns>
		/// <param name="min">Minimum value.</param>
		/// <param name="max">Max value.</param>
        public override double GetDouble(double min, double max)
        {
            return m_rnd.NextDouble(min, max);
        }     
		#endregion
    }
}
