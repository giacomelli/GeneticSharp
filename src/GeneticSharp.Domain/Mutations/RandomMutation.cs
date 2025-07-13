using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticSharp.Domain.Mutations
{
    /// <summary>
    /// Performs a random mutation of one supplied.
    /// </summary>
    public class RandomMutation : IMutation
    {
        private readonly bool _isOrdered;
        private readonly IMutation[] _mutations;
        private RandomMutation() { }

        public RandomMutation(bool isOrdered, params IMutation[] mutations)
        {
            _isOrdered = isOrdered;
            _mutations = mutations;

            if (_mutations.Length == 0) { throw new ArgumentException("Requires at least one mutation type."); }
        }
        public bool IsOrdered => _isOrdered;

        public void Mutate(IChromosome chromosome, float probability)
        {
            int i = Random.Shared.Next(0, _mutations.Length);
            _mutations[i].Mutate(chromosome, probability);


        }
    }
}
