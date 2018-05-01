using GeneticSharp.Domain;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Infrastructure.Framework.Threading;
using UnityEngine;

namespace GeneticSharp.Runner.UnityApp.Car
{
    public class CarSampleController : SampleControllerBase
    {
        private int NumberOfSimultaneousEvaluations = 100;
        public Vector2Int SimulationsGrid = new Vector2Int(5, 5);
        public Vector3 EvaluationDistance = new Vector3(0, 0, 2);
     
        public Object EvaluationPrefab;
        public CarSampleConfig Config;

        private CarFitness m_fitness;
        private Vector3 m_lastPosition;
        private PrefabPool m_evaluationPool;

        protected override GeneticAlgorithm CreateGA()
        {
            NumberOfSimultaneousEvaluations = SimulationsGrid.x * SimulationsGrid.y;
            m_fitness = new CarFitness();
            var chromosome = new CarChromosome(Config);

            //var swapPointOne = Config.VectorsCount / 3;
            //var crossover = new TwoPointCrossover(swapPointOne, swapPointOne * 2);
            var crossover = new UniformCrossover(.75f);
            var mutation = new UniformMutation(true);
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
                m_evaluationPool.ReleaseAll();
            };

            ga.MutationProbability = 0.1f;

            return ga;
        }

        protected override void StartSample()
        {
            ChromosomesCleanupEnabled = true;
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
                var evaluation = GameObject.Find(c.ID);
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