using System.Collections.Generic;
using System.Linq;

namespace GeneticSharp.Extensions
{
    /// <summary>
	/// This simple chromosome simply represents each cell by a gene with value between 1 and 9, accounting for the target mask if given
	/// </summary>
	public class SudokuCellsChromosome : SudokuChromosomeBase, ISudokuChromosome
    {
       

        public SudokuCellsChromosome() : this(null)
        {
        }

        /// <summary>
        /// Basic constructor with target sudoku to solve
        /// </summary>
        /// <param name="targetSudokuBoard">the target sudoku to solve</param>
        public SudokuCellsChromosome(SudokuBoard? targetSudokuBoard) : this( targetSudokuBoard, null) {}

        /// <summary>
        /// Constructor with additional precomputed domains for faster cloning
        /// </summary>
        /// <param name="targetSudokuBoard">the target sudoku to solve</param>
        /// <param name="extendedMask">The cell domains after initial constraint propagation</param>
        public SudokuCellsChromosome(SudokuBoard? targetSudokuBoard, Dictionary<int, List<int>>? extendedMask) : base(targetSudokuBoard, extendedMask, 81)
	    {
	    }


	    public override Gene GenerateGene(int geneIndex)
        {
            //If a target mask exist and has a digit for the cell, we use it.
            if (TargetSudokuBoard != null && TargetSudokuBoard.Cells[geneIndex] != 0)
            {
                return new Gene(TargetSudokuBoard.Cells[geneIndex]);
            }
            // otherwise we use a random digit amongts those permitted.
			var rnd = RandomizationProvider.Current;
	        var targetIdx = rnd.GetInt(0, ExtendedMask[geneIndex].Count);
			return new Gene(ExtendedMask[geneIndex][targetIdx]);
        }

        public override IChromosome CreateNew()
        {
            return new SudokuCellsChromosome(TargetSudokuBoard, ExtendedMask);
        }

        /// <summary>
        /// Builds a single Sudoku from the 81 genes
        /// </summary>
        /// <returns>A Sudoku board built from the 81 genes</returns>
        public override IList<SudokuBoard> GetSudokus()
        {
            var sudoku = new SudokuBoard(GetGenes().Select(g => (int)g.Value!));
            return new List<SudokuBoard>(new[] { sudoku });
        }
    }
}