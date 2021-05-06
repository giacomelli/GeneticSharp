using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;

namespace GeneticSharp.Extensions.Sudoku
{
    /// <summary>
    /// This abstract chromosome accounts for the target mask if given, and generates an extended mask with cell domains updated according to original mask
    /// </summary>
    public abstract class SudokuChromosomeBase : ChromosomeBase, ISudokuChromosome
    {


        /// <summary>
        /// The target sudoku board to solve
        /// </summary>
        private readonly SudokuBoard _targetSudokuBoard;

        
        /// <summary>
        /// Constructor that accepts a Sudoku to solve
        /// </summary>
        /// <param name="targetSudokuBoard">the target sudoku to solve</param>
        /// <param name="length">The number of genes for the sudoku chromosome</param>
        protected SudokuChromosomeBase(SudokuBoard targetSudokuBoard, int length) : base(length)
        {
            _targetSudokuBoard = targetSudokuBoard;
        }


        /// <summary>
        /// The target sudoku board to solve
        /// </summary>
        public SudokuBoard TargetSudokuBoard => _targetSudokuBoard;



        public abstract IList<SudokuBoard> GetSudokus();

    }
}