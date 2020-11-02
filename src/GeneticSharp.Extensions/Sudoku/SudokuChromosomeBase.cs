using System.Collections.Generic;
using System.Linq;
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
        /// The cell domains updated from the initial mask for the board to solve
        /// </summary>
        private  Dictionary<int, List<int>> _extendedMask;


        /// <summary>
        /// Constructor that accepts a Sudoku to solve
        /// </summary>
        /// <param name="targetSudokuBoard">the target sudoku to solve</param>
        /// <param name="length">The number of genes for the sudoku chromosome</param>
        public SudokuChromosomeBase(SudokuBoard targetSudokuBoard, int length) : base(length)
        {
            _targetSudokuBoard = targetSudokuBoard;
            for (int i = 0; i < Length; i++)
            {
                ReplaceGene(i, GenerateGene(i));
            }
        }

        /// <summary>
        /// Constructor that accepts an additional extended mask for quick cloning
        /// </summary>
        /// <param name="targetSudokuBoard">the target sudoku to solve</param>
        /// <param name="extendedMask">The cell domains after initial constraint propagation</param>
        /// <param name="length">The number of genes for the sudoku chromosome</param>
        public SudokuChromosomeBase(SudokuBoard targetSudokuBoard, Dictionary<int, List<int>> extendedMask, int length) : base(length)
        {
            _targetSudokuBoard = targetSudokuBoard;
            _extendedMask = extendedMask;
            for (int i = 0; i < Length; i++)
            {
                ReplaceGene(i, GenerateGene(i));
            }
        }


        /// <summary>
        /// The target sudoku board to solve
        /// </summary>
        public SudokuBoard TargetSudokuBoard => _targetSudokuBoard;

        /// <summary>
        /// The cell domains updated from the initial mask for the board to solve
        /// </summary>
        public Dictionary<int, List<int>> ExtendedMask
        {
            get
            {
                if (_extendedMask == null)
                {

                    var invertedMask = new Dictionary<int, List<int>>();
                    List<int> targetList = null;
                    for (var index = 0; index < _targetSudokuBoard.Cells.Count; index++)
                    {
                        var targetCell = _targetSudokuBoard.Cells[index];
                        if (targetCell != 0)
                        {
                            var row = index / 9;
                            var col = index % 9;
                            var boxStartIdx = (index / 27 * 27) + (index % 9 / 3 * 3);

                            for (int i = 0; i < 9; i++)
                            {
                                var boxtargetIdx = boxStartIdx + (i % 3) + ((i / 3) * 9);
                                var targetIndices = new[] { (row * 9) + i, i * 9 + col, boxtargetIdx };
                                foreach (var targetIndex in targetIndices)
                                {
                                    if (targetIndex != index)
                                    {
                                        if (!invertedMask.TryGetValue(targetIndex, out targetList))
                                        {
                                            targetList = new List<int>();
                                            invertedMask[targetIndex] = targetList;
                                        }
                                        if (!targetList.Contains(targetCell))
                                        {
                                            targetList.Add(targetCell);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    _extendedMask = new Dictionary<int, List<int>>();
                    var indices = Enumerable.Range(1, 9).ToList();
                    for (var index = 0; index < _targetSudokuBoard.Cells.Count; index++)
                    {
                        _extendedMask[index] = indices.Where(i => !invertedMask[index].Contains(i)).ToList();
                    }


                }
                return _extendedMask;
            }
        }

        public abstract IList<SudokuBoard> GetSudokus();

    }
}