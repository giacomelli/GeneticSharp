using System;
using System.Diagnostics;

namespace GeneticSharp.Domain.Chromosomes
{
	/// <summary>
	/// Represents a gene of a chromosome.
	/// </summary>
    [DebuggerDisplay("{Value}")]
	public sealed class Gene : IComparable<Gene>
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="GeneticSharp.Domain.Chromosomes.Gene"/> class.
		/// </summary>
        public Gene()
        {
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="GeneticSharp.Domain.Chromosomes.Gene"/> class.
		/// </summary>
		/// <param name="value">The gene intial value.</param>
		public Gene(IComparable value)
		{
			Value = value;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		public IComparable Value { get; set; }
		#endregion

		#region Methods
        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>The to.</returns>
        /// <param name="other">Other.</param>
		public int CompareTo(Gene other)
		{
			if (other == null)
			{
				return -1;
			}

			if (Value == null) {
				return 1;
			}

			var otherValue = other.Value;

			return Value.CompareTo (otherValue);
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="GeneticSharp.Domain.Chromosomes.Gene"/>.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="GeneticSharp.Domain.Chromosomes.Gene"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
		/// <see cref="GeneticSharp.Domain.Chromosomes.Gene"/>; otherwise, <c>false</c>.</returns>
		public override bool Equals(object obj)
		{
			var other = obj as Gene;

			if (other == null)
			{
				return false;
			}

			return CompareTo(other) == 0;
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode()
		{
			if (Value == null) {
				return 0;
			}

			return Value.GetHashCode();
		}

		/// <param name="first">First.</param>
		/// <param name="second">Second.</param>
		public static bool operator ==(Gene first, Gene second)
		{
			if (Object.ReferenceEquals(first, second))
			{
				return true;
			}

			if (((object)first == null) || ((object)second == null))
			{
				return false;
			}

			return first.CompareTo(second) == 0;
		}

		/// <param name="first">First.</param>
		/// <param name="second">Second.</param>
		public static bool operator !=(Gene first, Gene second)
		{
			return !(first == second);
		}

		/// <param name="first">First.</param>
		/// <param name="second">Second.</param>
		public static bool operator <(Gene first, Gene second)
		{
			if (Object.ReferenceEquals(first, second))
			{
				return false;
			}
			else if ((object)first == null)
			{
				return true;
			}
			else if ((object)second == null)
			{
				return false;
			}

			return first.CompareTo(second) < 0;
		}

		/// <param name="first">First.</param>
		/// <param name="second">Second.</param>
		public static bool operator >(Gene first, Gene second)
		{
			return !(first == second) && !(first < second);
		}
		#endregion
	}
}

