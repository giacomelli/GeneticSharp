using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Troschuetz.Random;

namespace GeneticSharp.Domain.Randomizations
{
    /// <summary>
    /// <see href="http://www.codeproject.com/Articles/15102/NET-random-number-generators-and-distributions"/>Article</see>
    /// </summary>
    public class TroschuetzRandomization : RandomizationBase
    {
        private Generator m_rnd = new XorShift128Generator(DateTime.Now.Millisecond);

        public override int GetInt(int min, int max)
        {
            return m_rnd.Next(min, max);
        }

        public override double GetDouble()
        {
            return m_rnd.NextDouble();
        }

        public override double GetDouble(double min, double max)
        {
            return m_rnd.NextDouble(min, max);
        }        
    }
}
