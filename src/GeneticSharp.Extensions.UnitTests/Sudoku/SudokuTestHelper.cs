using System;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Metaheuristics;
using GeneticSharp.Domain.Metaheuristics.Primitives;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Extensions.Sudoku;

namespace GeneticSharp.Extensions.UnitTests.Sudoku
{
    public enum SudokuTestDifficulty
    {
        VeryEasy,
        Easy,
        Medium
    }

    public static class SudokuTestHelper
    {
        private static readonly string _veryEasySudokuString = "9.2..54.31...63.255.84.7.6..263.9..1.57.1.29..9.67.53.24.53.6..7.52..3.4.8..4195.";
        private static readonly string _easySudokuString = "..48...1767.9.....5.8.3...43..74.1...69...78...1.69..51...8.3.6.....6.9124...15..";
        private static readonly string _mediumSudokuString = "..6.......8..542...4..9..7...79..3......8.4..6.....1..2.3.67981...5...4.478319562";


        public static SudokuBoard CreateBoard(SudokuTestDifficulty difficulty)
        {
            string sudokuToParse;
            switch (difficulty)
            {
                case SudokuTestDifficulty.VeryEasy:
                    sudokuToParse = _veryEasySudokuString;
                    break;
                case SudokuTestDifficulty.Easy:
                    sudokuToParse = _easySudokuString;
                    break;
                case SudokuTestDifficulty.Medium:
                    sudokuToParse = _mediumSudokuString;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(difficulty), difficulty, null);
            }
            return SudokuBoard.Parse(sudokuToParse);
        }

        public static double Eval(IChromosome sudokuChromosome, SudokuBoard sudokuBoard, int populationSize, double fitnessThreshold, int maxGenerationNb, out int generationNb)
        {
            return Eval(sudokuChromosome, sudokuBoard, new DefaultMetaHeuristic(), new UniformCrossover(), new UniformMutation(), 
                populationSize, fitnessThreshold, maxGenerationNb, out generationNb);
        }


        public static double Eval(IChromosome sudokuChromosome, SudokuBoard sudokuBoard, IMetaHeuristic metaHeuristic, ICrossover crossover, IMutation mutation,  int populationSize, double fitnessThreshold, int maxGenerationNb,  out int generationNb)
        {
            var fitness = new SudokuFitness(sudokuBoard);
            var selection = new EliteSelection();

            var population = new Population(populationSize, populationSize, sudokuChromosome);
            var ga = new MetaGeneticAlgorithm(population, fitness, selection, crossover, mutation, metaHeuristic)
            {
                Termination = new OrTermination(new FitnessThresholdTermination(fitnessThreshold), new GenerationNumberTermination(maxGenerationNb))
            };

            ga.Start();

            var bestIndividual = (ISudokuChromosome)ga.Population.BestChromosome;
            var solutions = bestIndividual.GetSudokus();
            generationNb = ga.Population.GenerationsNumber;
            return solutions.Max(solutionSudoku => fitness.Evaluate(solutionSudoku));
        }
    }
}
