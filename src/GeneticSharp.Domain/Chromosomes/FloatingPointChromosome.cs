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
		private double m_minValue;
		private double m_maxValue;
		private int m_decimals;
		private string m_originalValueStringRepresentation;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GeneticSharp.Domain.Chromosomes.FloatingPointChromosome"/> class.
		/// </summary>
		/// <param name="minValue">Minimum value.</param>
		/// <param name="maxValue">Max value.</param>
		/// <param name="decimals">Decimals.</param>
		public FloatingPointChromosome (double minValue, double maxValue, int decimals) 
			: base(32)
		{
			m_minValue = minValue;
			m_maxValue = maxValue;
			m_decimals = decimals;
			var originalValue = RandomizationProvider.Current.GetDouble (minValue, maxValue);
			m_originalValueStringRepresentation = BinaryStringRepresentation.ToRepresentation (originalValue, Length, decimals);

			CreateGenes ();
		}

		/// <summary>
		/// Creates the new.
		/// </summary>
		/// <returns>The new.</returns>
		public override IChromosome CreateNew ()
		{
			return new FloatingPointChromosome (m_minValue, m_maxValue, m_decimals);
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
			var stringRepresentation = String.Join ("", GetGenes ().Select (g => g.Value.ToString ()).ToArray());
			return BinaryStringRepresentation.ToDouble (stringRepresentation);
		}
	}
}

