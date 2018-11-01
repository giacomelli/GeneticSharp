using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Extensions.Sudoku;
using NUnit.Framework;

namespace GeneticSharp.Extensions.UnitTests.Sudoku
{


    [TestFixture()]
    [Category("Extensions")]
    public class SudokuTest
    {

        private readonly string _easySudokuString = "9.2..54.31...63.255.84.7.6..263.9..1.57.1.29..9.67.53.24.53.6..7.52..3.4.8..4195.";

        /// <summary>
        /// The sample sudoku string should parse properly into corresponding cells
        /// </summary>
        [Test()]
        public void Parse_SampleString_CellsDefined()
        {

            var sudoku = SudokuBoard.Parse(_easySudokuString);

            Assert.AreEqual(sudoku.Cells[0], 9);
            Assert.AreEqual(sudoku.Cells[1], 0);
            Assert.AreEqual(sudoku.Cells[2], 2);
            Assert.AreEqual(sudoku.Cells[sudoku.Cells.Count - 2], 5);
            Assert.AreEqual(sudoku.Cells[sudoku.Cells.Count - 1], 0);

        }


        /// <summary>
        /// The permutation chromosome should always solve the sudoku in a reasonable time with 1000 chromosomes
        /// </summary>
        [Test()]
        public void Evolve_SimpleSudokuPermutationsChromosome_Solved()
        {
            var sudoku = Extensions.Sudoku.SudokuBoard.Parse(_easySudokuString);

            IChromosome chromosome = new SudokuPermutationsChromosome(sudoku);
            var fitness = EvaluatesSudokuChromosome(chromosome, sudoku, 1000, 0, 50);
            Assert.AreEqual(fitness, 0);

        }

        /// <summary>
        /// The cells chromosome might need more individuals, so in order to keep execution time low, we only expect near completion
        /// </summary>
        [Test()]
        public void Evolve_SimpleSudokuCellsChromosome_NearlySolved()
        {
            var sudoku = Extensions.Sudoku.SudokuBoard.Parse(_easySudokuString);

            //the cells chromosome should solve the sudoku or nearly in less than 50 generations with 500 chromosomes
            var chromosome = new SudokuCellsChromosome(sudoku);
            var fitness = EvaluatesSudokuChromosome(chromosome, sudoku, 500, -20, 30);
            Assert.GreaterOrEqual(fitness, -20);

        }

        /// <summary>
        /// The random permutations chromosome require more individuals and generations, so we only test for significant progresses
        /// </summary>
        [Test()]
        public void Evolve_SimpleSudokuRandomPermutationsChromosome_Progressed()
        {
            var sudoku = Extensions.Sudoku.SudokuBoard.Parse(_easySudokuString);


            //the Random permutations chromosome should make significant progresses over 3 generations with 5 individuals

            var chromosome = new SudokuRandomPermutationsChromosome(sudoku, 2, 3);
            var fitness1 = new SudokuFitness(sudoku).Evaluate((ISudokuChromosome)chromosome);
            var fitness2 = EvaluatesSudokuChromosome(chromosome, sudoku, 5, fitness1 + 20, 3);
            Assert.GreaterOrEqual(fitness2, fitness1 + 20);

        }



        private double EvaluatesSudokuChromosome(IChromosome sudokuChromosome, Extensions.Sudoku.SudokuBoard sudokuBoard, int populationSize, double fitnessThreshold, int generationNb)
        {
            var fitness = new SudokuFitness(sudokuBoard);
            var selection = new EliteSelection();
            var crossover = new UniformCrossover();
            var mutation = new UniformMutation();

            var population = new Population(populationSize, populationSize, sudokuChromosome);
            var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation)
            {
                Termination = new OrTermination(new ITermination[]
                {
                    new FitnessThresholdTermination(fitnessThreshold),
                    new GenerationNumberTermination(generationNb)
                })
            };

            ga.Start();

            var bestIndividual = ((ISudokuChromosome)ga.Population.BestChromosome);
            var solutions = bestIndividual.GetSudokus();
            return solutions.Max(solutionSudoku => fitness.Evaluate(solutionSudoku));
        }

    }




}
