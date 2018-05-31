using GeneticSharp.Domain;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Infrastructure.Framework.Threading;
using GeneticSharp.Runner.UnityApp.Commons;
using UnityEngine;

namespace GeneticSharp.Runner.UnityApp.Car
{
    public class CarSampleController : SampleControllerBase
    {
        private static CarSampleConfig s_config;

        private int NumberOfSimultaneousEvaluations = 100;
        public Vector2Int SimulationsGrid = new Vector2Int(5, 5);
        public Vector3 EvaluationDistance = new Vector3(0, 0, 2);
     
        public Object EvaluationPrefab;
        public CarSampleConfig Config;
     
        private CarFitness m_fitness;
        private Vector3 m_lastPosition;
        private PrefabPool m_evaluationPool;

        public static void SetConfig(CarSampleConfig config)
        {
            s_config = config;
        }

		private void Awake()
		{
			if(s_config != null)
            {
                Config = s_config;
            }
		}

		protected override GeneticAlgorithm CreateGA()
        {
            NumberOfSimultaneousEvaluations = SimulationsGrid.x * SimulationsGrid.y;
            m_fitness = new CarFitness();
            var chromosome = new CarChromosome(Config);      
            var crossover = new UniformCrossover();
            var mutation = new FlipBitMutation();
            var selection = new EliteSelection();
            var population = new Population(NumberOfSimultaneousEvaluations, NumberOfSimultaneousEvaluations, chromosome)
            {
                GenerationStrategy = new PerformanceGenerationStrategy()
            };

            var ga = new GeneticAlgorithm(population, m_fitness, selection, crossover, mutation);
            ga.Termination = new CarTermination();
            ga.TaskExecutor = new ParallelTaskExecutor
            {
                MinThreads = population.MinSize,
                MaxThreads = population.MaxSize * 2
            };
            ga.GenerationRan += delegate
            {
                m_lastPosition = Vector3.zero;
                m_evaluationPool.ReleaseAll();
            };

            ga.MutationProbability = .1f;

            return ga;
        }

        protected override void StartSample()
        {
            ChromosomesCleanupEnabled = false;
            m_lastPosition = Vector3.zero;
            m_evaluationPool = new PrefabPool(EvaluationPrefab);
        }

        protected override void UpdateSample()
        {
            // end evaluation.
            while (m_fitness.ChromosomesToEndEvaluation.Count > 0)
            {
                CarChromosome c;
                m_fitness.ChromosomesToEndEvaluation.TryTake(out c);
                c.Evaluated = true;
            }

               
            // in evaluation.
            while (m_fitness.ChromosomesToBeginEvaluation.Count > 0)
            {
                CarChromosome c;
                m_fitness.ChromosomesToBeginEvaluation.TryTake(out c);
                c.Evaluated = false;
                c.MaxDistance = 0;

                var evaluation = m_evaluationPool.Get(m_lastPosition);
                evaluation.name = c.ID;

                var road = evaluation.GetComponentInChildren<RoadController>();
                road.Build(Config);

                var car = evaluation.GetComponentInChildren<CarController>();
                car.transform.position = m_lastPosition;
                car.SetChromosome(c, Config);

                m_lastPosition += EvaluationDistance;
            }
        }
    }
}