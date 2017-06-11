using System;
using System.Linq;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Infrastructure.Framework.Commons;

namespace GeneticSharp.Domain.Chromosomes
{
	/// <summary>
	/// Floating point chromosome with binary values (0 and 1).
	/// </summary>
	public class FloatingPointChromosome : BinaryChromosomeBase
	{
		private double[] m_minValue;
		private double[] m_maxValue;
		private int[] m_totalBits;
		private int[] m_fractionBits;
		private string m_originalValueStringRepresentation;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GeneticSharp.Domain.Chromosomes.FloatingPointChromosome"/> class.
		/// </summary>
		/// <param name="minValue">Minimum value.</param>
		/// <param name="maxValue">Max value.</param>
		/// <param name="fractionBits">Decimals.</param>
		public FloatingPointChromosome (double minValue, double maxValue, int fractionBits) 
			: this(new double[] { minValue }, new double[] { maxValue }, new int[] { 32 }, new int[] { fractionBits })
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GeneticSharp.Domain.Chromosomes.FloatingPointChromosome"/> class.
		/// </summary>
		/// <param name="minValue">Minimum value.</param>
		/// <param name="maxValue">Max value.</param>
		/// <param name="totalBits">Total bits.</param>
		/// <param name="fractionBits">Fraction bits.</param>
		public FloatingPointChromosome(double[] minValue, double[] maxValue, int[] totalBits, int[] fractionBits)
			: base(totalBits.Sum())
		{
			m_minValue = minValue;
			m_maxValue = maxValue;
			m_totalBits = totalBits;
			m_fractionBits = fractionBits;

			var originalValues = new double[minValue.Length];
			var rnd = RandomizationProvider.Current;

			for (int i = 0; i < originalValues.Length; i++)
			{
				originalValues[i] = rnd.GetDouble(minValue[i], maxValue[i]);
			}

			m_originalValueStringRepresentation = String.Join(
				"", 
				BinaryStringRepresentation.ToRepresentation(
				originalValues, 
				totalBits,
					fractionBits));

			CreateGenes();
		}

		/// <summary>
		/// Creates the new.
		/// </summary>
		/// <returns>The new.</returns>
		public override IChromosome CreateNew ()
		{
			return new FloatingPointChromosome (m_minValue, m_maxValue, m_totalBits, m_fractionBits);
		}

		/// <summary>
		/// Generates the gene.
		/// </summary>
		/// <returns>The gene.</returns>
		/// <param name="geneIndex">Gene index.</param>
		public override Gene GenerateGene (int geneIndex)
		{
			return new Gene (Convert.ToInt32(m_originalValueStringRepresentation [geneIndex].ToString()));
		}

		/// <summary>
		/// Converts the chromosome to the floating point representation.
		/// </summary>
		/// <returns>The floating point.</returns>
		public double ToFloatingPoint()
		{
			return EnsureMinMax(
				BinaryStringRepresentation.ToDouble(ToString()),
				0);

		}

		/// <summary>
		/// Converts the chromosome to the floating points representation.
		/// </summary>
		/// <returns>The floating points.</returns>
		public double[] ToFloatingPoints()
		{
			return BinaryStringRepresentation
				.ToDouble(ToString(), m_totalBits, m_fractionBits)
				.Select(EnsureMinMax)
				.ToArray();
		}

		private double EnsureMinMax(double value, int index)
		{
			if (value < m_minValue[index])
			{
				return m_minValue[index];
			}
			else if (value > m_maxValue[index])
			{
				return m_maxValue[index];
			}

			return value;
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:GeneticSharp.Domain.Chromosomes.FloatingPointChromosome"/>.
		/// </summary>
		/// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:GeneticSharp.Domain.Chromosomes.FloatingPointChromosome"/>.</returns>
		public override string ToString()
		{
			return String.Join("", GetGenes().Select(g => g.Value.ToString()).ToArray());
		}
	}
}