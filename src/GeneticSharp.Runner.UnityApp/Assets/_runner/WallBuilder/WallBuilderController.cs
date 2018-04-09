using GeneticSharp.Domain;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Infrastructure.Framework.Threading;
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
            var crossover = new UniformCrossover();
            var mutation = new UniformMutation(true);
            var selection = new EliteSelection();
            var population = new Population(NumberOfSimultaneousEvaluations, NumberOfSimultaneousEvaluations, chromosome);
            var ga = new GeneticAlgorithm(population, m_fitness, selection, crossover, mutation);
            ga.Termination = new FitnessStagnationTermination(100000);
            ga.TaskExecutor = new ParallelTaskExecutor
            {
                MinThreads = population.MinSize,
                MaxThreads = population.MaxSize * 2
            };
            ga.GenerationRan += delegate
            {
                Debug.Log($"Generation: {GA.GenerationsNumber} - Best: ${GA.BestChromosome.Fitness}");
                m_lastPosition = Vector3.zero;
            };

            return ga;
        }

		protected override void StartSample()
		{
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

                foreach (Transform child in container.transform)
                {
                    c.FloorHits += child.GetComponent<BrickController>().FloorHits;
                    m_brickPool.Release(child.gameObject);
                }

                GameObject.Destroy(container);
                c.Evaluated = true;

                if (c.FloorHits == 0)
                {
                    Debug.LogWarning("Chromosome did not touch the floor");
                }
            }

            // in evaluation.
            while (m_fitness.ChromosomesToBeginEvaluation.Count > 0)
            {
                WallBuilderChromosome c;
                m_fitness.ChromosomesToBeginEvaluation.TryTake(out c);
                c.Evaluated = false;
                c.FloorHits = 0;

                var container = new GameObject(c.ID);
                container.transform.position = m_lastPosition;
                m_lastPosition += EvaluationDistance;
                var bricksPositions = c.GetBricksPositions();

                foreach (var p in bricksPositions)
                {
                    var brick = m_brickPool.Get(p);
                    brick.transform.SetParent(container.transform, false);
                }
            }
        }
    }
}