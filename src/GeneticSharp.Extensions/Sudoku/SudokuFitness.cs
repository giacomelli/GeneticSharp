using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;

namespace GeneticSharp.Extensions.Sudoku
{
  /// <summary>
  /// Evaluates a sudoku chromosome for completion by counting duplicates in rows, columns, boxes, and differences from the target mask
  /// </summary>
  public class SudokuFitness : IFitness
  {
    private readonly Sudoku _targetSudoku;

    public SudokuFitness(Sudoku targetSudoku)
    {
      _targetSudoku = targetSudoku;
    }


    public double Evaluate(IChromosome chromosome)
    {
      return Evaluate((ISudokuChromosome)chromosome);
    }

    public double Evaluate(ISudokuChromosome chromosome)
    {
      List<double> scores = new List<double>();

      var sudokus = chromosome.GetSudokus();
      foreach (var sudoku in sudokus)
      {
        scores.Add(Evaluate(sudoku));
      }

      return scores.Sum();
    }

    public double Evaluate(Sudoku testSudoku)
    {
      // We use a large lambda expression to count duplicates in rows, columns and boxes
      var cells = testSudoku.CellsList.Select((c, i) => new { index = i, cell = c });
      var toTest = cells.GroupBy(x => x.index / 9).Select(g => g.Select(c => c.cell)) // rows
        .Concat(cells.GroupBy(x => x.index % 9).Select(g => g.Select(c => c.cell))) //columns
        .Concat(cells.GroupBy(x => x.index / 27 * 27 + x.index % 9 / 3 * 3).Select(g => g.Select(c => c.cell))); //boxes
      var toReturn = -toTest.Sum(test => test.GroupBy(x => x).Select(g => g.Count() - 1).Sum()); // Summing over duplicates
      toReturn -= cells.Count(x => _targetSudoku.CellsList[x.index] > 0 && _targetSudoku.CellsList[x.index] != x.cell); // Mask
      return toReturn;
    }



  }



}