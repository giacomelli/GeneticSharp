using System;
using GeneticSharp.Domain.Randomizations;
using System.Text;
using System.Linq;
using System.Collections;
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

		// TODO: should we call as decimals or fractionbits?
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
			return BinaryStringRepresentation.ToDouble (ToString());
		}

		public double[] ToFloatingPoints()
		{
			return BinaryStringRepresentation.ToDouble(ToString(), m_totalBits, m_fractionBits);
		}

		public override string ToString()
		{
			return String.Join("", GetGenes().Select(g => g.Value.ToString()).ToArray());
		}
	}
}

