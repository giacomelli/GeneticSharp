using System;

namespace GeneticSharp.Domain.Randomizations
{
	public class BasicRandomization : IRandomization
	{
		#region Fields
		private Random m_random = new Random (DateTime.Now.Millisecond);
		#endregion

		#region Methods
		public int GetInt(int min, int max)
		{
			return m_random.Next(min, max);
		}

		public int[] GetInts(int length, int min, int max)
		{
			var ints = new int[length];

			for (int i = 0; i < length; i++) {
				ints [i] = GetInt (min, max);
			}

			return ints;
		}

		public double GetDouble()
		{
			return m_random.NextDouble ();
		}

		public  double GetDouble(double min, double max)
		{
			return min + ((max - min) * m_random.NextDouble ());
		}
		#endregion
	}
}

