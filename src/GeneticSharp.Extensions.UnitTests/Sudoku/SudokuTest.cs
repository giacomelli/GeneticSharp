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


    [Test()]
    public void SolveSimpleSudoku()
    {
      // checking that a simple Sudoku can be tackled using various strategies
     var sudoku = Extensions.Sudoku.Sudoku.Parse("9.2..54.31...63.255.84.7.6..263.9..1.57.1.29..9.67.53.24.53.6..7.52..3.4.8..4195.");
      Assert.AreEqual(sudoku.CellsList[0], 9);
      Assert.AreEqual(sudoku.CellsList[sudoku.CellsList.Count-1], 0);

      //the permutation chromosome should always solve the sudoku in less than 30 generations with 1000 chromosomes 
      IChromosome chromosome = new SudokuPermutationsChromosome(sudoku);
      var fitness = EvaluatesSudokuChromosome(chromosome, sudoku, 1000, 30);
      Assert.AreEqual(fitness, 0);

      // Other chromosomes would require more individuals thus more time, so we simply test for significant progresses

      //the cells chromosome should solve the sudoku or nearly in less than 50 generations with 500 chromosomes
      chromosome = new SudokuCellsChromosome(sudoku);
      fitness = EvaluatesSudokuChromosome(chromosome, sudoku, 500, 50);
      Assert.GreaterOrEqual(fitness, -20);
      
      //the Random permutations chromosome should make significant progresses over 50 generations with 50 individuals
      
      chromosome = new SudokuRandomPermutationsChromosome(sudoku, 2, 5);
      var fitness1 = EvaluatesSudokuChromosome(chromosome, sudoku, 50, 1);
      var fitness2 = EvaluatesSudokuChromosome(chromosome, sudoku, 50, 51);
      Assert.GreaterOrEqual(fitness2, fitness1 + 5);


    }

    private double EvaluatesSudokuChromosome(IChromosome sudokuChromosome, Extensions.Sudoku.Sudoku sudoku, int populationSize, int generationNb)
    {
      var fitness = new SudokuFitness(sudoku);
      var selection = new EliteSelection();
      var crossover = new UniformCrossover();
      var mutation = new UniformMutation();
      
      var population = new Population(populationSize, populationSize, sudokuChromosome);
      var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
      ga.Termination = new GenerationNumberTermination(generationNb);

      ga.Start();

      var bestIndividual = ((ISudokuChromosome)ga.Population.BestChromosome);
      var solutions = bestIndividual.GetSudokus();
      return solutions.Max(solutionSudoku => fitness.Evaluate(solutionSudoku));
    }

  }


  

}
