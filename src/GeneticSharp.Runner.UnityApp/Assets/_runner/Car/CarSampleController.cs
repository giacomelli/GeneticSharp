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
        public int SecondsForEvaluation = 5;
        private int NumberOfSimultaneousEvaluations = 100;
        public Vector2Int SimulationsGrid = new Vector2Int(5, 5);
        public Vector3 EvaluationDistance = new Vector3(0, 0, 2);
        public int VectorsCount = 8;
        public float VectorSize = 10;
        public int WheelsCount = 2;
        public float MaxWheelRadius = 1;
        public Object EvaluationPrefab;
        public FollowChromosomeCam FollowCamera;

        private CarFitness m_fitness;
        private Vector3 m_lastPosition = Vector3.zero;
        private PrefabPool m_evaluationPool;

        protected override GeneticAlgorithm CreateGA()
        {
            NumberOfSimultaneousEvaluations = SimulationsGrid.x * SimulationsGrid.y;
            m_fitness = new CarFitness(SecondsForEvaluation);
            var chromosome = new CarChromosome(VectorsCount, VectorSize, WheelsCount, MaxWheelRadius);
            var crossover = new UniformCrossover();
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
            };

            ga.MutationProbability = 0.1f;

            return ga;
        }

        protected override void StartSample()
        {
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

                c.Distance = evaluation.GetComponentInChildren<CarController>().Distance;

                m_evaluationPool.Release(evaluation);
                c.Evaluated = true;
            }

               
            // in evaluation.
            while (m_fitness.ChromosomesToBeginEvaluation.Count > 0)
            {
                CarChromosome c;
                m_fitness.ChromosomesToBeginEvaluation.TryTake(out c);
                c.Evaluated = false;
                c.Distance = 0;

                var evaluation = m_evaluationPool.Get(m_lastPosition);
                evaluation.name = c.ID;

                var car = evaluation.GetComponentInChildren<CarController>();
                car.transform.position = m_lastPosition;
                car.SimulationsGrid = SimulationsGrid;
                car.SetChromosome(c);

                m_lastPosition += EvaluationDistance;
            }
        }
    }
}