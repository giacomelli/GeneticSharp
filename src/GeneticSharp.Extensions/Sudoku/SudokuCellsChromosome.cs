using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Extensions.Sudoku
{ 
  /// <summary>
  /// This simple chromosome simply represents each cell by a gene with value between 1 and 9, accounting for the target mask
  /// </summary>
public class SudokuCellsChromosome : ChromosomeBase, ISudokuChromosome
  {
    private Sudoku _targetSudoku;

    public SudokuCellsChromosome():this(null)
    {
    }

    public SudokuCellsChromosome(Sudoku targetSudoku) : base(81)
    {
      _targetSudoku = targetSudoku;
      for (int i = 0; i < Length; i++)
      {
        ReplaceGene(i, GenerateGene(i));
      }
    }

    public override Gene GenerateGene(int geneIndex)
    {
      if (_targetSudoku != null && _targetSudoku.CellsList[geneIndex] != 0)
      {
        return new Gene(_targetSudoku.CellsList[geneIndex]);
      }
      var rnd = RandomizationProvider.Current;
      return new Gene(rnd.GetInt(1, 10));
    }

    public override IChromosome CreateNew()
    {
      return new SudokuCellsChromosome(_targetSudoku);
    }


    public List<Sudoku> GetSudokus()
    {
      var sudoku = new Sudoku(GetGenes().Select(g => (int)g.Value));
      return new List<Sudoku>(new[] { sudoku });
    }
  }
}