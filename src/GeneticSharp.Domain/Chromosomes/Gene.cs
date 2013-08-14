using System;
using System.Diagnostics;

namespace GeneticSharp.Domain.Chromosomes
{
    [DebuggerDisplay("{Value}")]
	public sealed class Gene : IComparable<Gene>
	{
		#region Constructors
        public Gene()
        {
        }

		public Gene(IComparable value)
		{
			Value = value;
		}
		#endregion

		#region Properties
		public IComparable Value { get; set; }
		#endregion

		#region Methods
        
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

		public static bool operator !=(Gene first, Gene second)
		{
			return !(first == second);
		}

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

		public static bool operator >(Gene first, Gene second)
		{
			return !(first == second) && !(first < second);
		}
		#endregion
	}
}

