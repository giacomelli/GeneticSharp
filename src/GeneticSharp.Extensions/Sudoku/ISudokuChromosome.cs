
using System.Collections.Generic;

namespace GeneticSharp.Extensions.Sudoku
{ 
  /// <summary>
  /// Each type of chromosome for solving a sudoku is simply required to output a sudoku
  /// </summary>
public interface ISudokuChromosome
  {
    List<Sudoku> GetSudokus();
  }
}