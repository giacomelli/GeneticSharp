using System;

namespace GeneticSharp.Domain.Randomizations
{
	public interface IRandomization
	{
		int GetInt (int min, int max);
		int[] GetInts (int length, int min, int max);
		double GetDouble();
		double GetDouble(double min, double max);
	}
}