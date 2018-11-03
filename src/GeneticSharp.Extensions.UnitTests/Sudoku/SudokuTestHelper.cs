using System.Linq;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Extensions.Sudoku;

namespace GeneticSharp.Extensions.UnitTests.Sudoku
{
    public static class SudokuTestHelper
    {
        private static readonly string _easySudokuString = "9.2..54.31...63.255.84.7.6..263.9..1.57.1.29..9.67.53.24.53.6..7.52..3.4.8..4195.";

        public static SudokuBoard CreateBoard()
        {
            return SudokuBoard.Parse(_easySudokuString);
        }

        public static double Eval(IChromosome sudokuChromosome, SudokuBoard sudokuBoard, int populationSize, double fitnessThreshold, int generationNb)
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
