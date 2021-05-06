using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;

namespace GeneticSharp.Domain.Populations
{
    public class IslandPopulation : SubPopulation
    {
        public IslandPopulation(IPopulation parentPopulation, IList<IChromosome> subPopulation) : base(parentPopulation, subPopulation)
        {
        }

        public List<double> MigrationRates { get; set; }

    }
}