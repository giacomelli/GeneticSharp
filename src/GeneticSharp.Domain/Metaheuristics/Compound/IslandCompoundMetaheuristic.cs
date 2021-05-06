using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Metaheuristics.Primitives;

namespace GeneticSharp.Domain.Metaheuristics.Compound
{
    /// <summary>
    /// IslandCompoundMetaheuristic allows building an IslandMetaheuristic from a list of compound metaheuristics
    /// </summary>
    public class IslandCompoundMetaheuristic : ICompoundMetaheuristic
    {

        public IslandCompoundMetaheuristic(int islandSize, int islandNb, params ICompoundMetaheuristic[] phaseCompounds)
        {
            var compounds = new List<(int islandSize, ICompoundMetaheuristic islandCompoundMetaheuristic)>();
            var repeatNb = islandNb / phaseCompounds.Length;
            for (int i = 0; i < repeatNb; i++)
            {
                compounds.AddRange(phaseCompounds.Select(pc=>(islandSize, pc)));
            }
            IslandCompounds = compounds;
        }

        public IslandCompoundMetaheuristic(int totalPopulation, params (int shares, ICompoundMetaheuristic compoundMetaheuristic)[] phaseCompounds)
        {
            var compounds = new List<(int islandSize, ICompoundMetaheuristic islandCompoundMetaheuristic)>();
            var totalShares = phaseCompounds.Sum(tuple => tuple.shares);
            var shareSize = totalPopulation / totalShares;
            foreach (var phaseCompound in phaseCompounds)
            {
                var islandConfig = (shareSize * phaseCompound.shares, phaseCompound.compoundMetaheuristic);
                    compounds.Add(islandConfig);
            }

            IslandCompounds = compounds;
        }
        public IslandCompoundMetaheuristic(params (int islandSize, ICompoundMetaheuristic islandCompoundMetaheuristic)[] islands)
        {
            IslandCompounds = new List<(int islandSize, ICompoundMetaheuristic islandCompoundMetaheuristic)>(islands);
        }

        public List<(int islandSize, ICompoundMetaheuristic islandCompoundMetaheuristic)> IslandCompounds { get; set; }


        public IContainerMetaHeuristic Build()
        {
            var phaseheuristics = IslandCompounds.Select(c => (c.islandSize, (IMetaHeuristic) c.islandCompoundMetaheuristic.Build())).ToArray(); 
            var islandMetaHeuristic = new IslandMetaHeuristic(phaseheuristics) { GlobalMigrationRate = GlobalMigrationRate, MigrationMode = MigrationMode, MigrationsGenerationPeriod = MigrationsGenerationPeriod};
            return islandMetaHeuristic;
        }


        public MigrationMode MigrationMode { get; set; } = MigrationMode.RandomRing;




        public int MigrationsGenerationPeriod { get; set; } = 10;

        public double GlobalMigrationRate { get; set; } = IslandMetaHeuristic.DefaultGlobalMigrationRate;


        public List<int> IslandSizes { get; set; }
    }
}