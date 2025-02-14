using System.Collections.Generic;
using System.Linq;

namespace GeneticSharp
{
    public class IslandModelStrategy : OperatorsStrategyBase
    {
        private double _migrationProbability; // Probability of a chromosome migrating to another island
        private int _islandCount; // Number of islands (subpopulations)
        private Dictionary<IChromosome, int> _islandMapping = new Dictionary<IChromosome, int>(); // Tracks which island each chromosome belongs to

        public IslandModelStrategy(int islandCount, double migrationProbability = 0.1)
        {
            _islandCount = islandCount;
            _migrationProbability = migrationProbability;
        }

        public override IList<IChromosome> Cross(IPopulation population, ICrossover crossover, float crossoverProbability, IList<IChromosome> parents)
        {
            // Asignar islas a los cromosomas si no tienen una
            int i = 0;
            foreach (var p in parents)
            {
                if (!_islandMapping.ContainsKey(p))
                {
                    _islandMapping[p] = (RandomizationProvider.Current.GetDouble() < _migrationProbability)
                        ? RandomizationProvider.Current.GetInt(0, _islandCount) // Migrar a una isla aleatoria
                        : i++ % _islandCount; // Asignar de manera cíclica
                }
            }

            // Migrar cromosomas a otras islas
            foreach (var c in parents)
            {
                if(RandomizationProvider.Current.GetDouble() < _migrationProbability)
                    _islandMapping[c] = RandomizationProvider.Current.GetInt(0, _islandCount);
            }

            var offspring = new List<IChromosome>();

            // Agrupar padres por isla y hacer crossover dentro de cada isla
            foreach (var island in parents.GroupBy(r => _islandMapping[r]))
            {
                var islandParents = island.OrderBy(r => RandomizationProvider.Current.GetDouble()).ToList();

                // Verificar si hay suficientes padres para hacer cruces
                if (islandParents.Count < crossover.ParentsNumber)
                {
                    continue; // O podrías duplicar padres para completar el grupo
                }

                // Hacer cruces
                for (int j = 0; j <= islandParents.Count - crossover.ParentsNumber; j += crossover.ParentsNumber)
                {
                    var children = SelectParentsAndCross(population, crossover, crossoverProbability, islandParents, j);
                    if (children != null)
                    {
                        // Asignar hijos a la misma isla de sus padres
                        foreach (var c in children)
                            _islandMapping[c] = island.Key;
                        offspring.AddRange(children);
                    }
                }
            }

            // Borrar los elementos de islandMapping que ya no están en la población
            var allChromosomes = new HashSet<IChromosome>(parents.Concat(offspring).Concat(population.CurrentGeneration.Chromosomes));
            var toDelete = _islandMapping.Keys.Except(allChromosomes).ToList();
            foreach (var c in toDelete)
                _islandMapping.Remove(c);

            return offspring;
        }


        public override void Mutate(IMutation mutation, float mutationProbability, IList<IChromosome> chromosomes)
        {
            // Apply mutation to each chromosome individually
            foreach (var c in chromosomes)
            {
                mutation.Mutate(c, mutationProbability);
            }
        }
    }
}
