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
    public class RandomMutation : MutationBase
    {
        private Dictionary<IMutation, Tuple<double, double>> m_mutationChanceRage = new Dictionary<IMutation, Tuple<double, double>>();
        private float m_chanceSum;

        public RandomMutation()
        {
            IsOrdered = true;    
        }

        public RandomMutation AddMutation(IMutation mutation, float chance)
        {
            if (IsOrdered && !mutation.IsOrdered)
            {
                IsOrdered = false;
            }

            var chanceRange = new Tuple<double, double>(m_chanceSum, m_chanceSum + chance);
            m_chanceSum += chance;

            m_mutationChanceRage.Add(mutation, chanceRange);

            return this;
        }

        protected override void PerformMutate(IChromosome chromosome, float probability)
        {
            if (m_mutationChanceRage.Count == 0)
            {
                throw new InvalidOperationException("There is no mutations added to RandomMutation. Please add them using the AddMutation method.");    
            }

            var rnd = RandomizationProvider.Current;

            if (rnd.GetDouble() <= probability)
            {
                var chance = rnd.GetDouble(0, m_chanceSum);

                // Selects the mutation in the chance rage.
                var selectedMutation = m_mutationChanceRage.First(m => chance >= m.Value.Item1 && chance < m.Value.Item2);
                selectedMutation.Key.Mutate(chromosome, probability);
            }
        }
    }
}