using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Reinsertions;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Extensions.Mathematic;

namespace GeneticSharp.Runner.ConsoleApp.Samples
{
    [DisplayName("Function Builder")]
    public class FunctionBuilderSampleController : SampleControllerBase
    {
        #region Fields
        private FunctionBuilderFitness m_fitness;
        private List<FunctionBuilderInput> m_inputs;
        private int m_maxOperations;
        #endregion

        #region Methods    
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public override void Initialize()
        {
            Console.WriteLine("Function arguments and expected result: arg1,arg2=expected result.");
            Console.WriteLine("Sample1: 1,2,3=6");
            Console.WriteLine("Sample2: 2,3,4=24");
            Console.WriteLine("When finish, type ENTER to start the GA.");

            m_inputs = new List<FunctionBuilderInput>();
            do
            {
                var parts = Console.ReadLine().Split('=');

                if (parts.Length != 2)
                {
                    Console.WriteLine("Max number of operations?");
                    m_maxOperations = Convert.ToInt32(Console.ReadLine());

                    break;
                }

                var arguments = parts[0].Split(',');

                var input = new FunctionBuilderInput(
                    arguments.Select(a => Convert.ToDouble(a)).ToList(),
                    Convert.ToDouble(parts[1]));

                m_inputs.Add(input);
            }
            while (true);

            m_fitness = new FunctionBuilderFitness(m_inputs.ToArray());
        }

        /// <summary>
        /// Configure the Genetic Algorithm.
        /// </summary>
        /// <param name="ga">The genetic algorithm.</param>
        public override void ConfigGA(GeneticAlgorithm ga)
        {
            ga.CrossoverProbability = 0.1f;
            ga.MutationProbability = 0.4f;
            ga.Reinsertion = new ElitistReinsertion();
        }

        /// <summary>
        /// Draws the specified best chromosome.
        /// </summary>
        /// <param name="bestChromosome">The best chromosome.</param>
        public override void Draw(IChromosome bestChromosome)
        {
            var best = bestChromosome as FunctionBuilderChromosome;

            foreach (var input in m_inputs)
            {
                Console.WriteLine("{0} = {1}", string.Join(", ", input.Arguments), input.ExpectedResult);
            }

            Console.WriteLine("Max operations: {0}", m_maxOperations);
            Console.WriteLine("Function: {0}", best.BuildFunction());
        }

        /// <summary>
        /// Creates the chromosome.
        /// </summary>
        /// <returns>The sample chromosome.</returns>
        public override IChromosome CreateChromosome()
        {
            return new FunctionBuilderChromosome(m_fitness.AvailableOperations, m_maxOperations);
        }

        /// <summary>
        /// Creates the fitness.
        /// </summary>
        /// <returns>The fitness.</returns>
        public override IFitness CreateFitness()
        {
            return m_fitness;
        }

        /// <summary>
        /// Creates the crossover.
        /// </summary>
        /// <returns>The crossover.</returns>
        public override ICrossover CreateCrossover()
        {
            return new ThreeParentCrossover();
        }

        /// <summary>
        /// Creates the mutation.
        /// </summary>
        /// <returns>The mutation.</returns>
        public override IMutation CreateMutation()
        {
            return new UniformMutation(true);
        }

        /// <summary>
        /// Creates the selection.
        /// </summary>
        /// <returns>The selection.</returns>
        public override ISelection CreateSelection()
        {
            return new EliteSelection();
        }

        /// <summary>
        /// Creates the termination.
        /// </summary>
        /// <returns> 
        /// The termination.
        /// </returns>
        public override ITermination CreateTermination()
        {
            return new FitnessThresholdTermination(0);
        }
        #endregion
    }
}
