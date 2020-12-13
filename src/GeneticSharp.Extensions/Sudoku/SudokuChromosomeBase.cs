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
        /// The list of row permutations accounting for the mask
        /// </summary>
        private IList<IList<IList<int>>> _targetRowsPermutations;


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
        protected SudokuChromosomeBase(SudokuBoard targetSudokuBoard, int length) : this(targetSudokuBoard, null, length) {}

        /// <summary>
        /// Constructor that accepts an additional extended mask for quick cloning
        /// </summary>
        /// <param name="targetSudokuBoard">the target sudoku to solve</param>
        /// <param name="extendedMask">The cell domains after initial constraint propagation</param>
        /// <param name="length">The number of genes for the sudoku chromosome</param>
        protected SudokuChromosomeBase(SudokuBoard targetSudokuBoard, Dictionary<int, List<int>> extendedMask, int length) : base(length)
        {
            _targetSudokuBoard = targetSudokuBoard;
            _extendedMask = extendedMask;
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
                    // We generate 1 to 9 figures for convenience
                    var indices = Enumerable.Range(1, 9).ToList();
                    var extendedMask = new Dictionary<int, List<int>>(81);
                    if (_targetSudokuBoard!=null)
                    {
                        //If target sudoku mask is provided, we generate an inverted mask with forbidden values by propagating rows, columns and boxes constraints
                        var forbiddenMask = new Dictionary<int, List<int>>();
                        List<int> targetList = null;
                        for (var index = 0; index < _targetSudokuBoard.Cells.Count; index++)
                        {
                            var targetCell = _targetSudokuBoard.Cells[index];
                            if (targetCell != 0)
                            {
                                //We parallelize going through all 3 constraint neighborhoods
                                var row = index / 9;
                                var col = index % 9;
                                var boxStartIdx = index / 27 * 27 + index % 9 / 3 * 3;

                                for (int i = 0; i < 9; i++)
                                {
                                    //We go through all 9 cells in the 3 neighborhoods
                                    var boxtargetIdx = boxStartIdx + i % 3 + i / 3 * 9;
                                    var targetIndices = new[] { row * 9 + i, i * 9 + col, boxtargetIdx };
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
                return _extendedMask;
            }
        }


        /// <summary>
        /// Gets the permutation for a row and given a permutation index, according to the corresponding row's available permutations
        /// </summary>
        /// <param name="rowIndex">the row index for the permutation</param>
        /// <param name="permIDx">the permutation index to retrieve</param>
        /// <returns></returns>
        protected virtual IList<int> GetPermutation(int rowIndex, int permIDx)
        {

            // we use a modulo operator in case the gene was swapped:
            // It may contain a number higher than the number of available permutations. 
            var perm = TargetRowsPermutations[rowIndex][permIDx % TargetRowsPermutations[rowIndex].Count];
            return perm;
        }




        /// <summary>
        /// This method computes for each row the list of digit permutations that respect the target mask, that is the list of valid rows discarding columns and boxes
        /// </summary>
        /// <param name="sudokuBoard">the target sudoku to account for</param>
        /// <returns>the list of permutations available</returns>
        public IList<IList<IList<int>>> GetRowsPermutations()
        {
            if (TargetSudokuBoard == null)
            {
                return UnfilteredPermutations;
            }

            // we store permutations to compute them once only for each target Sudoku
            if (!_rowsPermutations.TryGetValue(TargetSudokuBoard, out var toReturn))
            {
                // Since this is a static member we use a lock to prevent parallelism.
                // This should be computed once only.
                lock (_rowsPermutations)
                {
                    if (!_rowsPermutations.TryGetValue(TargetSudokuBoard, out toReturn))
                    {
                        toReturn = GetRowsPermutationsUncached();
                        _rowsPermutations[TargetSudokuBoard] = toReturn;
                    }
                }
            }
            return toReturn;
        }

        private IList<IList<IList<int>>> GetRowsPermutationsUncached()
        {
            var toReturn = new List<IList<IList<int>>>(9);
            for (int i = 0; i < 9; i++)
            {
                var tempList = new List<IList<int>>();
                foreach (var perm in AllPermutations)
                {
                    // Permutation should be compatible with current row extended mask domains
                    if (Range9.All(j => ExtendedMask[i * 9 + j].Contains(perm[j])))
                    {
                        tempList.Add(perm);
                    }
                }
                toReturn.Add(tempList);
            }

            return toReturn;
        }



        /// <summary>
        /// Produces 9 copies of the complete list of permutations
        /// </summary>
        public static IList<IList<IList<int>>> UnfilteredPermutations
        {
            get
            {
                if (!_unfilteredPermutations.Any())
                {
                    lock (_unfilteredPermutations)
                    {
                        if (!_unfilteredPermutations.Any())
                        {
                            _unfilteredPermutations = Range9.Select(i => AllPermutations).ToList();
                        }
                    }
                }
                return _unfilteredPermutations;
            }
        }

        /// <summary>
        /// Builds the complete list permutations for {1,2,3,4,5,6,7,8,9}
        /// </summary>
        public static IList<IList<int>> AllPermutations
        {
            get
            {
                if (!_allPermutations.Any())
                {
                    lock (_allPermutations)
                    {
                        if (!_allPermutations.Any())
                        {
                            _allPermutations = GetPermutations(Enumerable.Range(1, 9), 9);
                        }
                    }
                }
                return _allPermutations;
            }
        }

        /// <summary>
        /// The list of row permutations accounting for the mask
        /// </summary>
        public IList<IList<IList<int>>> TargetRowsPermutations
        {
            get
            {
                if (_targetRowsPermutations == null)
                {
                    _targetRowsPermutations = GetRowsPermutations();
                }
                return _targetRowsPermutations;
            }
        }

        /// <summary>
        /// The list of compatible permutations for a given Sudoku is stored in a static member for fast retrieval
        /// </summary>
        private static readonly IDictionary<SudokuBoard, IList<IList<IList<int>>>> _rowsPermutations = new Dictionary<SudokuBoard, IList<IList<IList<int>>>>();

        /// <summary>
        /// The list of row indexes is used many times and thus stored for quicker access.
        /// </summary>
        private static readonly List<int> Range9 = Enumerable.Range(0, 9).ToList();

        /// <summary>
        /// The complete list of unfiltered permutations is stored for quicker access
        /// </summary>
        private static IList<IList<int>> _allPermutations = new List<IList<int>>();
        private static IList<IList<IList<int>>> _unfilteredPermutations = new List<IList<IList<int>>>();

        /// <summary>
        /// Computes all possible permutation for a given set
        /// </summary>
        /// <typeparam name="T">the type of elements the set contains</typeparam>
        /// <param name="list">the list of elements to use in permutations</param>
        /// <param name="length">the size of the resulting list with permuted elements</param>
        /// <returns>a list of all permutations for given size as lists of elements.</returns>
        static IList<IList<T>> GetPermutations<T>(IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => (IList<T>)new[] { t }.ToList()).ToList();

            var enumeratedList = list.ToList();
            return GetPermutations(enumeratedList, length - 1)
                .SelectMany(t => enumeratedList.Where(e => !t.Contains(e)),
                    (t1, t2) => (IList<T>)t1.Concat(new[] { t2 }).ToList()).ToList();
        }


        public abstract IList<SudokuBoard> GetSudokus();

    }
}