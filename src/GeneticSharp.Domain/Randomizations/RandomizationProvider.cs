using System;

namespace GeneticSharp.Domain.Randomizations
{
	public static class RandomizationProvider
	{
		#region Constructors
		static RandomizationProvider ()
		{
			Current = new BasicRandomization ();
		}
		#endregion

		#region Properties
		public static IRandomization Current { get; set; }
		#endregion
	}
}