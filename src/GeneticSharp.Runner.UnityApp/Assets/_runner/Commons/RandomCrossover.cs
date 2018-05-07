using GeneticSharp.Domain.Chromosomes;
using System;
using System.Linq;
using GeneticSharp.Domain.Randomizations;
using System.Collections.Generic;
using GeneticSharp.Domain.Crossovers;

namespace GeneticSharp.Runner.UnityApp.Commons
{
    public class RandomCrossover : ICrossover
    {
        private Dictionary<ICrossover, Tuple<double, double>> m_crossoverChanceRange = new Dictionary<ICrossover, Tuple<double, double>>();
        private float m_chanceSum;

        public RandomCrossover()
        {
            IsOrdered = true;
        }

        public int ParentsNumber { get; private set; }

        public int ChildrenNumber { get; private set; }

        public int MinChromosomeLength { get; private set; }

        public bool IsOrdered { get; private set; }

        public RandomCrossover AddCrossover(ICrossover crossover, float chance)
        {
            if (IsOrdered && !crossover.IsOrdered)
            {
                IsOrdered = false;
            }

            ParentsNumber = Math.Max(ParentsNumber, crossover.ParentsNumber);
            ChildrenNumber = Math.Max(ChildrenNumber, crossover.ChildrenNumber);
            MinChromosomeLength = Math.Max(MinChromosomeLength, crossover.MinChromosomeLength);

            var chanceRange = new Tuple<double, double>(m_chanceSum, m_chanceSum + chance);
            m_chanceSum += chance;

            m_crossoverChanceRange.Add(crossover, chanceRange);

            return this;
        }

        public IList<IChromosome> Cross(IList<IChromosome> parents)
        {
            if (m_crossoverChanceRange.Count == 0)
            {
                throw new InvalidOperationException("There is no randomization added to RandomCrossover. Please add them using the AddCrossover method.");
            }

            var chance = RandomizationProvider.Current.GetDouble(0, m_chanceSum);

            // Selects the crossover in the chance rage.
            var selectedMutation = m_crossoverChanceRange.First(m => chance >= m.Value.Item1 && chance < m.Value.Item2);
            return selectedMutation.Key.Cross(parents);
        }
    }
}