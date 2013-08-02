using System;
using System.Diagnostics;

namespace GeneticSharp.Domain.Chromosomes
{
    [DebuggerDisplay("{Value}")]
	public sealed class Gene
	{
        public Gene()
        {
        }

        public Gene(object value)
        {
            Value = value;
        }

		public object Value { get; set; }
	}
}

