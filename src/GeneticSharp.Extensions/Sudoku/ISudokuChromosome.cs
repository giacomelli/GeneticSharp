
using System.Collections.Generic;

namespace GeneticSharp.Extensions.Sudoku
{
    /// <summary>
    /// Each type of chromosome for solving a sudoku is simply required to output a list of candidate sudokus
    /// </summary>
    public interface ISudokuChromosome
    {
        IList<SudokuBoard> GetSudokus();
    }
}