using GeneticSharp.Domain;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Infrastructure.Framework.Threading;
using GeneticSharp.Runner.UnityApp.Commons;
using UnityEngine;

namespace GeneticSharp.Runner.UnityApp.WallBuilder
{
    public class WallBuilderController : SampleControllerBase
    {

        public int BricksCount = 10;
        public Vector3 MinPosition;
        public Vector3 MaxPosition;
        public Object BrickPrefab;
        public int SecondsForEvaluation = 5;
        public float TimeScale = 1;
        public int NumberOfSimultaneousEvaluations = 100;
        public Vector3 EvaluationDistance = new Vector3(0, 0, 2);

        private WallBuilderFitness m_fitness;
        private Vector3 m_lastPosition = Vector3.zero;
        private PrefabPool m_brickPool;

        protected override GeneticAlgorithm CreateGA()
        {
            m_fitness = new WallBuilderFitness(SecondsForEvaluation / TimeScale);
            var chromosome = new WallBuilderChromosome(BricksCount, MinPosition, MaxPosition);

            var crossover = new RandomCrossover()
                .AddCrossover(new UniformCrossover(), 0.9f)
                .AddCrossover(new SectionCrossover(chromosome.Length / chromosome.BricksCount, true), 0.1f);

            var mutation = new RandomMutation()
                .AddMutation(new FlipBitMutation(), .9f)
                .AddMutation(new UniformMutation(), .1f);
            
            var selection = new EliteSelection();
            var population = new Population(NumberOfSimultaneousEvaluations, NumberOfSimultaneousEvaluations, chromosome);
            var ga = new GeneticAlgorithm(population, m_fitness, selection, crossover, mutation);
            ga.Termination = new TimeEvolvingTermination(System.TimeSpan.FromDays(1));
            ga.TaskExecutor = new ParallelTaskExecutor
            {
                MinThreads = population.MinSize,
                MaxThreads = population.MaxSize * 2
            };
            ga.GenerationRan += delegate
            {
                m_lastPosition = Vector3.zero;
            };

            return ga;
        }

		protected override void StartSample()
		{
            ChromosomesCleanupEnabled = true;
            ShowPreviousInfoEnabled = false;
            m_brickPool = new PrefabPool(BrickPrefab);
            Time.timeScale = TimeScale;
		}

        protected override void UpdateSample()
        {
             // end evaluation.
            while (m_fitness.ChromosomesToEndEvaluation.Count > 0)
            {
                WallBuilderChromosome c;
                m_fitness.ChromosomesToEndEvaluation.TryTake(out c);
                var container = GameObject.Find(c.ID);

                if (container.transform.childCount == 0)
                {
                    Debug.LogError("Bricks not found on container");
                }

                if (container.transform.childCount != BricksCount)
                {
                    Debug.LogWarning("Less bricks on container than expected");
                }

                while (container.transform.childCount > 0)
                {
                    var child = container.transform.GetChild(0);
                    var ctrl = child.GetComponent<BrickController>();

                    if (ctrl.HitFloor)
                    {
                        c.FloorHits++;
                    }

                    c.BrickHits += ctrl.HitBricksCount;
                    c.BricksEndPositions.Add(child.transform.position);

                    m_brickPool.Release(child.gameObject);
                }

                Object.Destroy(container);
                c.Evaluated = true;

                Debug.Assert(c.FloorHits != 0, "At least one brick should hit the floor");

                if (c.FloorHits == 0)
                {
                    Debug.LogWarning("Chromosome did not touch the floor");
                }
            }

            // begin evaluation.
            while (m_fitness.ChromosomesToBeginEvaluation.Count > 0)
            {
                WallBuilderChromosome c;
                m_fitness.ChromosomesToBeginEvaluation.TryTake(out c);
                c.Evaluated = false;
                c.FloorHits = 0;
                c.BrickHits = 0;
                c.BricksEndPositions.Clear();
            
                var container = new GameObject(c.ID);
                container.transform.position = m_lastPosition;
                m_lastPosition += EvaluationDistance;
                var bricksPhenotypes = c.GetPhenotypes();

                foreach (var p in bricksPhenotypes)
                {
                    var brick = m_brickPool.Get(p.Position);
                    brick.transform.SetParent(container.transform, false);
                }
            }
        }
    }
}