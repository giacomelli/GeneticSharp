using GeneticSharp.Domain.Chromosomes;
using System;
using System.Linq;
using GeneticSharp.Infrastructure.Framework.Commons;
using GeneticSharp.Domain.Randomizations;
using System.Collections.Generic;
using System.Diagnostics;
using GeneticSharp.Domain.Mutations;

namespace GeneticSharp.Runner.UnityApp.Commons
{
    public class RandomMutation : IMutation
    {
        private Dictionary<IMutation, float> m_mutationsChance = new Dictionary<IMutation, float>();

        public bool IsOrdered => m_mutationsChance.All(m => m.Key.IsOrdered);

        public RandomMutation AddMutation(IMutation mutation, float chance)
        {
            m_mutationsChance.Add(mutation, chance);

            return this;
        }
        public void Mutate(IChromosome chromosome, float probability)
        {
            throw new NotImplementedException();
        }
    }
}