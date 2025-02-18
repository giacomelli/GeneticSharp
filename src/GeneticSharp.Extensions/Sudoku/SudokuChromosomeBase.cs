using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace GeneticSharp.Extensions
{
    /// <summary>
    /// This abstract chromosome accounts for the target mask if given, and generates an extended mask with cell domains updated according to original mask
    /// </summary>
    public abstract class SudokuChromosomeBase : ChromosomeBase, ISudokuChromosome
    {

        /// <summary>
        /// The target sudoku board to solve
        /// </summary>
        private readonly SudokuBoard? _targetSudokuBoard;

        /// <summary>
        /// The cell domains updated from the initial mask for the board to solve
        /// </summary>
        private  Dictionary<int, List<int>>? _extendedMask;

        /// <summary>
        /// Constructor that accepts an additional extended mask for quick cloning
        /// </summary>
        /// <param name="targetSudokuBoard">the target sudoku to solve</param>
        /// <param name="extendedMask">The cell domains after initial constraint propagation</param>
        /// <param name="length">The number of genes for the sudoku chromosome</param>
        protected SudokuChromosomeBase(SudokuBoard? targetSudokuBoard, Dictionary<int, List<int>>? extendedMask, int length) : base(length)
        {
            _targetSudokuBoard = targetSudokuBoard;
            _extendedMask = extendedMask;
            CreateGenes();
        }


        /// <summary>
        /// The target sudoku board to solve
        /// </summary>
        public SudokuBoard? TargetSudokuBoard => _targetSudokuBoard;

        /// <summary>
        /// The cell domains updated from the initial mask for the board to solve
        /// </summary>
        public Dictionary<int, List<int>> ExtendedMask
        {
            get
            {
                if (_extendedMask == null)
                    BuildExtenedMask();

                return _extendedMask;
            }
        }

        [MemberNotNull(nameof(_extendedMask))]
        private void BuildExtenedMask()
        {
            // We generate 1 to 9 figures for convenience
            var indices = Enumerable.Range(1, 9).ToList();
            var extendedMask = new Dictionary<int, List<int>>(81);
            if (_targetSudokuBoard != null)
            {
                //If target sudoku mask is provided, we generate an inverted mask with forbidden values by propagating rows, columns and boxes constraints
                var forbiddenMask = new Dictionary<int, List<int>>();
                List<int>? targetList = null;
                for (var index = 0; index < _targetSudokuBoard.Cells.Count; index++)
                {
                    var targetCell = _targetSudokuBoard.Cells[index];
                    if (targetCell != 0)
                    {
                        //We parallelize going through all 3 constraint neighborhoods
                        var row = index / 9;
                        var col = index % 9;
                        var boxStartIdx = (index / 27 * 27) + (index % 9 / 3 * 3);

                        for (int i = 0; i < 9; i++)
                        {
                            //We go through all 9 cells in the 3 neighborhoods
                            var boxtargetIdx = boxStartIdx + (i % 3) + ((i / 3) * 9);
                            var targetIndices = new[] { (row * 9) + i, i * 9 + col, boxtargetIdx };
                            foreach (var targetIndex in targetIndices)
                            {
                                if (targetIndex != index)
                                {
                                    if (!forbiddenMask.TryGetValue(targetIndex, out targetList))
                                    {
                                        //If the current neighbor cell does not have a forbidden values list, we create it
                                        targetList = new List<int>();
                                        forbiddenMask[targetIndex] = targetList;
                                    }
                                    if (!targetList.Contains(targetCell))
                                    {
                                        // We add current cell value to the neighbor cell forbidden values
                                        targetList.Add(targetCell);
                                    }
                                }
                            }
                        }
                    }
                }

                // We invert the forbidden values mask to obtain the cell permitted values domains
                for (var index = 0; index < _targetSudokuBoard.Cells.Count; index++)
                {
                    extendedMask[index] = indices.Where(i => !forbiddenMask[index].Contains(i)).ToList();
                }

            }
            else
            {
                //If we have no sudoku mask, 1-9 numbers are allowed for all cells
                for (int i = 0; i < 81; i++)
                {
                    extendedMask.Add(i, indices);
                }
            }
            _extendedMask = extendedMask;
        }

        public abstract IList<SudokuBoard> GetSudokus();

    }
}