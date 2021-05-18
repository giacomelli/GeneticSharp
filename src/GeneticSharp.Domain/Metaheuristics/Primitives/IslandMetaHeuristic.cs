using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Metaheuristics.Matching;
using GeneticSharp.Domain.Metaheuristics.Parameters;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Domain.Reinsertions;
using GeneticSharp.Domain.Selections;

namespace GeneticSharp.Domain.Metaheuristics.Primitives
{
    public enum MigrationMode
    {
        None,
        Static,
        RandomRing,
        RandomPermutation,
        Reinforced
    }


    /// <summary>
    /// Island-based metaheuristic consider sub-populations evolved independantly with migrations of individuals between Islands
    /// </summary>
    [DisplayName("Island")]
    public class IslandMetaHeuristic : SubPopulationMetaHeuristicBase<IslandPopulation>
    {
        private const string islandsKey = "islands";
        public const double SmallMigrationRate = 0.005;
        public const double MediumMigrationRate = 0.02;
        public const double LargeMigrationRate = 0.1;

        public IslandMetaHeuristic() : base()
        {
            InitMigrationRates();
        }

        public IslandMetaHeuristic(int islandSize, params IMetaHeuristic[] phaseHeuristics) : base(islandSize,
            phaseHeuristics)
        {
            InitMigrationRates();
        }

        public IslandMetaHeuristic(int islandSize, int islandNb, params IMetaHeuristic[] phaseHeuristics) : base(
            islandSize, islandNb, phaseHeuristics)
        {
            InitMigrationRates();
        }

        public IslandMetaHeuristic(params (int islandSize, IMetaHeuristic islandMetaHeuristic)[] islands) : base(islands)
        {
            InitMigrationRates();
        }

        private void InitMigrationRates()
        {
            var migrationRates = new List<List<double>>();
            for (int i = 0; i < PhaseSizes.Phases.Count; i++)
            {
                var currentIslandRates = new List<double>();
                for (int j = 0; j < PhaseSizes.Phases.Count; j++)
                {
                    if (j != i)
                    {
                        currentIslandRates.Add(GlobalMigrationRate / PhaseSizes.Phases.Count);
                    }
                    else
                    {
                        currentIslandRates.Add(1 - GlobalMigrationRate);
                    }
                }
                migrationRates.Add(currentIslandRates);
            }

            StaticMigrationRates = migrationRates;
        }

        public MigrationMode MigrationMode { get; set; } = MigrationMode.RandomRing;

        

        public double GlobalMigrationRate { get; set; } = SmallMigrationRate;

        public int MigrationsGenerationPeriod { get; set; } = 10;

        public List<List<double>> StaticMigrationRates { get; set; }

        public MatchPicker EmigrantPicker { get; set; } = new MatchPicker((10,MatchingKind.Best, ParamScope.None));

        
        public MatchPicker ImigrantReplacePicker { get; set; } = new MatchPicker((10, MatchingKind.Worst, ParamScope.None));




        protected override IList<IslandPopulation> GenerateSubPopulations(IMetaHeuristic h, IEvolutionContext c)
        {
            var islands = new List<IslandPopulation>(PhaseSizes.Phases.Count);
            int skips = 0;
            foreach (var phaseSize in PhaseSizes.Phases)
            {
                var island = new IslandPopulation(c.Population, c.Population.CurrentGeneration.Chromosomes.Skip(skips).Take(phaseSize).ToList());

                islands.Add(island);
                skips += island.CurrentGeneration.Chromosomes.Count;
            }

            if (c.Population.CurrentGeneration.Chromosomes.Count>skips)
            {
                throw new InvalidOperationException(
                    $"Population has {c.Population.CurrentGeneration.Chromosomes.Count} individuals whereas island is configured for {skips}");
            }
            return islands;

        }

        
        


        protected override IList<IChromosome> ScopedSelectParentPopulation(IEvolutionContext ctx, ISelection selection)
        {
            var toReturn = new List<IChromosome>(ctx.Population.MinSize);


            IList<IslandPopulation> islandPopulations = DynamicSubPopulationParameter.Get(this, ctx, islandsKey);

            //When subpopulation scope is broader than generation
            SynchroniseGeneration(islandPopulations, ctx);

            if (ctx.Population.GenerationsNumber % MigrationsGenerationPeriod == 0)
            {
                MigratePopulations(ctx, islandPopulations);
            }

            for (var islandIndex = 0; islandIndex < islandPopulations.Count; islandIndex++)
            {
                var subPopulation = islandPopulations[islandIndex];
                var subHeuristic = PhaseHeuristics[islandIndex];
                var newCtx = subPopulation.GetContext(ctx);
                var islandSelect = subHeuristic.SelectParentPopulation(newCtx, selection);
                newCtx.SelectedParents = islandSelect;
                toReturn.AddRange(islandSelect);
            }

            return toReturn;
        }

       


        protected override IList<IChromosome> ScopedMatchParentsAndCross(IEvolutionContext ctx, ICrossover crossover, float crossoverProbability,
            IList<IChromosome> parents)
        {
            var islandPopulations = DynamicSubPopulationParameter.Get(this, ctx, islandsKey);

            SynchroniseParents(islandPopulations, ctx, parents);

           

            int currentIslandIndex = EnumeratedPhases.GetPhaseIndex(islandPopulations, o => ((IslandPopulation)o).GetContext(ctx).SelectedParents.Count, ctx.LocalIndex, out int localItemIndex);

            //if (!PhaseHeuristics.ContainsKey(currentIslandIndex))
            //{
            //    Debugger.Break();
            //}
            var subHeuristic = PhaseHeuristics[currentIslandIndex];
            var currentIsland = islandPopulations[currentIslandIndex];
            var newCtx = currentIsland.GetContext(ctx).GetLocal(localItemIndex);
            var offspring = subHeuristic.MatchParentsAndCross(newCtx, crossover, 1, newCtx.SelectedParents);

            //foreach (var chromosome in offspring)
            //{
            //    newCtx.GeneratedOffsprings.Add(chromosome);
            //}
            
            return offspring;

        }

       


        protected override void ScopedMutateChromosome(IEvolutionContext ctx, IMutation mutation, float mutationProbability, IList<IChromosome> offSprings)
        {
            var islandPopulations = DynamicSubPopulationParameter.Get(this, ctx, islandsKey);

            SynchroniseOffspring(islandPopulations, ctx, offSprings);

            


            int currentIslandIndex = EnumeratedPhases.GetPhaseIndex(islandPopulations, o => ((IslandPopulation)o).GetContext(ctx).GeneratedOffsprings.Count, ctx.LocalIndex, out int localItemIndex);
           
            var subHeuristic = PhaseHeuristics[currentIslandIndex];
            var currentIsland = islandPopulations[currentIslandIndex];

            var newCtx = currentIsland.GetContext(ctx).GetLocal(localItemIndex);
            subHeuristic.MutateChromosome(newCtx, mutation, mutationProbability, newCtx.GeneratedOffsprings); 
        }


       

        protected override IList<IChromosome> ScopedReinsert(IEvolutionContext ctx, IReinsertion reinsertion, IList<IChromosome> offspring, IList<IChromosome> parents)
        {
            var islandPopulations = DynamicSubPopulationParameter.Get(this, ctx, islandsKey);

            SynchroniseParents(islandPopulations, ctx, parents);
            SynchroniseOffspring(islandPopulations, ctx, offspring);

            var islandReinserts = new List<IList<IChromosome>>(islandPopulations.Count);
            for (var islandIndex = 0; islandIndex < islandPopulations.Count; islandIndex++)
            {
                var island = islandPopulations[islandIndex];
                var subHeuristic = PhaseHeuristics[islandIndex];
                var newCtx = island.GetContext(ctx);
                var localReinserts = subHeuristic.Reinsert(newCtx, reinsertion, newCtx.GeneratedOffsprings, newCtx.SelectedParents);
                islandReinserts.Add(localReinserts);
            }
            var toReturn = islandReinserts.SelectMany(reinserts => reinserts, (list, chromosome) => chromosome).ToList();
            return toReturn;
        }


        private void MigratePopulations(IEvolutionContext ctx, IList<IslandPopulation> islandPopulations)
        {

            switch (MigrationMode)
            {
                case MigrationMode.None:
                    return;
                case MigrationMode.RandomPermutation:
                case MigrationMode.RandomRing:
                    var permutation = RandomizationProvider.Current.GetUniqueInts(islandPopulations.Count, 0,
                        islandPopulations.Count);
                    if (MigrationMode == MigrationMode.RandomRing)
                    {
                        var permutationQueue = new Queue<int>(permutation);
                        var ring = Enumerable.Repeat(-1, permutation.Length).ToArray(); //new int[permutation.Length];  //new List<int>(permutation.Length);
                        int current = 0;
                        //var permIndex = 0;
                        int next; //= ringQueue.Dequeue(); //permutation[0];
                        do
                        {
                            next = permutationQueue.Dequeue();
                            while (next == current || ring[next]!=-1)
                            {
                                //permIndex = (permIndex + 1) % permutation.Length;
                                //next = permutation[permIndex];
                                permutationQueue.Enqueue(next);
                                next = permutationQueue.Dequeue();
                            }
                            ring[current] = next;
                            current = next;
                            
                            //permIndex = current;
                            //next = permutation[permIndex];


                        } while (permutationQueue.Count>1);
                        ring[current] = 0;

                        permutation = ring;
                    }
                    for (int islandIndex = 0; islandIndex < islandPopulations.Count; islandIndex++)
                    {
                        var island = islandPopulations[islandIndex];
                        var islandRates = Enumerable.Repeat(0.0, islandPopulations.Count).ToList();
                        islandRates[permutation[islandIndex]] = GlobalMigrationRate;
                        island.MigrationRates = islandRates;
                    }
                    break;
                case MigrationMode.Static:
                case MigrationMode.Reinforced:
                    for (int islandIndex = 0; islandIndex < islandPopulations.Count; islandIndex++)
                    {
                        var island = islandPopulations[islandIndex];
                        if (island.MigrationRates == null)
                        {
                            island.MigrationRates = new List<double>(StaticMigrationRates[islandIndex]);
                        }

                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            var immigrants = new List<List<IChromosome>>(islandPopulations.Count);

            for (var sourceIslandIndex = 0; sourceIslandIndex < islandPopulations.Count; sourceIslandIndex++)
            {
                var sourceIsland = islandPopulations[sourceIslandIndex];

                
                var sourceReinserts = islandPopulations[sourceIslandIndex].CurrentGeneration.Chromosomes;

                var emigrantNbs = new int[islandPopulations.Count];

                for (var targetIslandIndex = 0; targetIslandIndex < sourceIsland.MigrationRates.Count; targetIslandIndex++)
                {
                    if (targetIslandIndex!=sourceIslandIndex)
                    {
                        var i2iMigrationRate = sourceIsland.MigrationRates[targetIslandIndex];

                        int nbMigrants = 0;
                        if (i2iMigrationRate>0)
                        {
                            nbMigrants = (int)(i2iMigrationRate * sourceReinserts.Count);
                            var leftover = i2iMigrationRate * sourceReinserts.Count - nbMigrants;
                            if (leftover > 0)
                            {
                                if (RandomizationProvider.Current.GetDouble()<leftover)
                                {
                                    nbMigrants += 1;
                                }
                            }
                        }
                        emigrantNbs[targetIslandIndex] = nbMigrants;
                    }
                }

                for (var targetIslandIndex = 0; targetIslandIndex < sourceIsland.MigrationRates.Count; targetIslandIndex++)
                {
                    while (immigrants.Count < targetIslandIndex + 1)
                    {
                        var immigrantList = new List<IChromosome>();
                        immigrants.Add(immigrantList);
                    }
                    if (targetIslandIndex != sourceIslandIndex)
                    {
                        var targetNb = emigrantNbs[targetIslandIndex];
                        if (targetNb>0)
                        {
                            var matches = EmigrantPicker.SelectMatches(this, sourceIsland.GetContext(ctx), 0, ((GeneticAlgorithm)ctx.GeneticAlgorithm).Crossover,
                                sourceReinserts).Take(targetNb);
                            //var targetEmigrantindices = matches.Select(match => sourceReinserts.IndexOf(match));

                            immigrants[targetIslandIndex].AddRange(matches);
                        }
                        
                    }
                }
                
            }

            for (var targetIslandIndex = 0; targetIslandIndex < immigrants.Count; targetIslandIndex++)
            {
                var immigrantPopulation = immigrants[targetIslandIndex];
                if (immigrantPopulation.Count>0)
                {
                    var targetIsland = islandPopulations[targetIslandIndex];
                    var targetPopulation = targetIsland.CurrentGeneration.Chromosomes;

                    var replaced = ImigrantReplacePicker.SelectMatches(this, targetIsland.GetContext(ctx), 0,
                        ((GeneticAlgorithm)ctx.GeneticAlgorithm).Crossover, targetPopulation).Take(immigrantPopulation.Count);
                    var replacedIndices = replaced
                        .Select(replacedIndividual => targetPopulation.IndexOf(replacedIndividual))
                        .OrderByDescending(i => i);
                    foreach (var replacedIndex in replacedIndices)
                    {
                        targetPopulation.RemoveAt(replacedIndex);
                    }

                    foreach (var immigrant in immigrantPopulation)
                    {
                        targetPopulation.Add(immigrant);
                    }
                }
                
            }


           
        }

        private void SynchroniseGeneration(IList<IslandPopulation> islandPopulations, IEvolutionContext ctx)
        {

            if (islandPopulations[0].GenerationsNumber < ctx.Population.GenerationsNumber)
            {
                int skips = 0;
                for (var islandIndex = 0; islandIndex < PhaseSizes.Phases.Count; islandIndex++)
                {
                    var phaseSize = PhaseSizes.Phases[islandIndex];
                    var island = islandPopulations[islandIndex];
                    island.EndCurrentGeneration();
                    island.CreateNewGeneration(ctx.Population.CurrentGeneration.Chromosomes.Skip(skips).Take(phaseSize).ToList());

                    skips += island.CurrentGeneration.Chromosomes.Count;
                }
            }
        }


        private void SynchroniseParents(IList<IslandPopulation> islandPopulations, IEvolutionContext ctx, IList<IChromosome> parents)
        {
            var subContext0 = islandPopulations.Last().GetContext(ctx);
            if (subContext0.SelectedParents == null || subContext0.SelectedParents.Count == 0)
            {
                lock (ctx)
                {
                    if (subContext0.SelectedParents == null || subContext0.SelectedParents.Count == 0)
                    {
                        int skips = 0;
                        for (var islandIndex = 0; islandIndex < PhaseSizes.Phases.Count; islandIndex++)
                        {
                            var phaseSize = PhaseSizes.Phases[islandIndex];
                            var island = islandPopulations[islandIndex];
                            var islandSelectedParents = parents.Skip(skips).Take(phaseSize).ToList();
                            island.GetContext(ctx).SelectedParents = islandSelectedParents;
                            skips += islandSelectedParents.Count;
                        }
                    }
                }
            }
        }

        private void SynchroniseOffspring(IList<IslandPopulation> islandPopulations, IEvolutionContext ctx, IList<IChromosome> offSprings)
        {
            var subContext0 = islandPopulations.Last().GetContext(ctx);
            if (subContext0.GeneratedOffsprings == null || subContext0.GeneratedOffsprings.Count == 0)
            {
                lock (ctx)
                {
                    if (subContext0.GeneratedOffsprings == null || subContext0.GeneratedOffsprings.Count == 0)
                    {
                        int skips = 0;
                        for (var islandIndex = 0; islandIndex < PhaseSizes.Phases.Count; islandIndex++)
                        {
                            var phaseSize = PhaseSizes.Phases[islandIndex];
                            var island = islandPopulations[islandIndex];
                            var islandOffspring = offSprings.Skip(skips).Take(phaseSize).ToList();
                            island.GetContext(ctx).GeneratedOffsprings = islandOffspring;
                            skips += islandOffspring.Count;
                        }
                    }
                }
            }
        }

    }
}