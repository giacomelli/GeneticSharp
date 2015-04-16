using System;
using System.Diagnostics;

namespace GeneticSharp.Domain.Chromosomes
{
    /// <summary>
    /// Represents a gene of a chromosome.
    /// </summary>
    [DebuggerDisplay("{Value}")]
    public struct Gene : IEquatable<Gene>
    {
        #region Fields
        private object m_value;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Chromosomes.Gene"/> class.
        /// </summary>
        /// <param name="value">The gene intial value.</param>
        public Gene(object value)
        {
            m_value = value;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public Object Value { get { return m_value; } }
        #endregion

        #region Methods
        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="GeneticSharp.Domain.Chromosomes.Gene"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="GeneticSharp.Domain.Chromosomes.Gene"/>.</returns>
        public override string ToString()
        {
            return Value != null ? Value.ToString() : "";
        }

        /// <summary>
        /// Determines whether the specified <see cref="GeneticSharp.Domain.Chromosomes.Gene"/> is equal to the current <see cref="GeneticSharp.Domain.Chromosomes.Gene"/>.
        /// </summary>
        /// <param name="other">The <see cref="GeneticSharp.Domain.Chromosomes.Gene"/> to compare with the current <see cref="GeneticSharp.Domain.Chromosomes.Gene"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="GeneticSharp.Domain.Chromosomes.Gene"/> is equal to the current
        /// <see cref="GeneticSharp.Domain.Chromosomes.Gene"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(Gene other)
        {
            return Value.Equals(other.Value);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="GeneticSharp.Domain.Chromosomes.Gene"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="GeneticSharp.Domain.Chromosomes.Gene"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
        /// <see cref="GeneticSharp.Domain.Chromosomes.Gene"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            var other = (Gene)obj;

            return Value.Equals(other.Value);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            if (Value == null)
            {
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

            return first.Equals(second);
        }

        /// <param name="first">First.</param>
        /// <param name="second">Second.</param>
        public static bool operator !=(Gene first, Gene second)
        {
            return !(first == second);
        }
        #endregion
    }
}

