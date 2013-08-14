using System;
using SharpNeatLib.Maths;

namespace GeneticSharp.Domain.Randomizations
{
    /// <summary>
    /// An IRandomization using Coolgreen's FastRandom (http://www.codeproject.com/Articles/9187/A-fast-equivalent-for-System-Random) has pseudo-number generator.
    /// </summary>
    public class FastRandomRandomization : RandomizationBase
    {
        #region Fields
        private FastRandom m_random = new FastRandom(DateTime.Now.Millisecond);
        #endregion

        #region Methods
        public override int GetInt(int min, int max)
        {
            return m_random.Next(min, max);
        }

        public override double GetDouble()
        {
            return m_random.NextDouble();
        }
        #endregion
    }
}
