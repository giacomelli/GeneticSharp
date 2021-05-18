using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using GeneticSharp.Infrastructure.Framework.Collections;
using GeneticSharp.Infrastructure.Framework.Commons;

namespace GeneticSharp.Extensions.Sudoku
{
    /// <summary>
    /// Class that represents a Sudoku, fully or partially completed
    /// Holds a list of 81 int for cells, with 0 for empty cells
    /// Can parse strings and files from most common formats and displays the sudoku in an easy to read format
    /// </summary>
    public class SudokuBoard
    {
        /// <summary>
        /// The list of cells is used many times and thus stored for quicker access.
        /// </summary>
        public static readonly ReadOnlyCollection<int> CellIndex = new ReadOnlyCollection<int>(Enumerable.Range(0, 81).ToArray());

        /// <summary>
        /// The list of row indexes is used many times and thus stored for quicker access.
        /// </summary>
        public static readonly ReadOnlyCollection<int> NeighborhoodIndex = new ReadOnlyCollection<int>(Enumerable.Range(0, 9).ToArray());

        private static readonly List<List<int>> _rowsNeighborhoods =
            CellIndex.GroupBy(x => x / 9).Select(g => g.ToList()).ToList();

        private static readonly List<List<int>> _colNeighborhoods =
            CellIndex.GroupBy(x => x % 9).Select(g => g.ToList()).ToList();

        private static readonly List<List<int>> _boxNeighborhoods =
            CellIndex.GroupBy(x => x / 27 * 27 + x % 9 / 3 * 3).Select(g => g.ToList()).ToList();

        public static readonly List<List<int>> AllNeighborhoods =
            _rowsNeighborhoods.Concat(_colNeighborhoods).Concat(_boxNeighborhoods).ToList();


        /// <summary>
        /// The empty constructor assumes no mask
        /// </summary>
        public SudokuBoard()
        {
        }

        /// <summary>
        /// constructor that initializes the board with 81 cells
        /// </summary>
        /// <param name="cells"></param>
        public SudokuBoard(IEnumerable<int> cells)
        {
            var enumerable = cells.ToList();
            if (enumerable.Count != 81)
            {
                throw new ArgumentException("Sudoku should have exactly 81 cells", nameof(cells));
            }
            Cells = new List<int>(enumerable);
        }

        // We use a list for easier access to cells,

        /// <summary>
        /// Easy access by row and column number
        /// </summary>
        /// <param name="x">row number (between 0 and 8)</param>
        /// <param name="y">column number (between 0 and 8)</param>
        /// <returns>value of the cell</returns>
        public int GetCell(int x, int y)
        {
            return Cells[9 * x + y];
        }

        /// <summary>
        /// Easy setter by row and column number
        /// </summary>
        /// <param name="x">row number (between 0 and 8)</param>
        /// <param name="y">column number (between 0 and 8)</param>
        /// <param name="value">value of the cell to set</param>
        public void SetCell(int x, int y, int value)
        {
            Cells[9 * x + y] = value;
        }

        /// <summary>
        /// Sudoku cells are initialized with zeros standing for empty cells
        /// </summary>
        public IList<int> Cells { get; set; } = Enumerable.Repeat(0, 81).ToList();

        /// <summary>
        /// Displays a Sudoku in an easy to read format
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var lineSep = new string('-', 31);

            var output = new StringBuilder();
            output.Append(lineSep);
            output.AppendLine();
            foreach (var row in NeighborhoodIndex)
            {
                // we start each line with |
                output.Append("| ");
                foreach (var column in NeighborhoodIndex)
                {
                    // we obtain the 81-cell index from the 9x9 row/column index
                    var value = Cells[row * 9 + column];
                    output.Append(value);
                    //we identify boxes with | within lines
                    output.Append((column + 1 ) % 3 == 0 ? " | " : "  ");
                }

                output.AppendLine();
                //we identify boxes with - within columns
                if ((row+1) % 3 == 0)
                {
                    output.Append(lineSep);
                }

                output.AppendLine();
            }

            return output.ToString();
        }

        /// <summary>
        /// Parses a single Sudoku
        /// </summary>
        /// <param name="sudokuAsString">the string representing the sudoku</param>
        /// <returns>the parsed sudoku</returns>
        public static SudokuBoard Parse(string sudokuAsString)
        {
            return ParseMulti(new[] { sudokuAsString })[0];
        }

        /// <summary>
        /// Parses a file with one or several sudokus
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>the list of parsed Sudokus</returns>
        public static List<SudokuBoard> ParseFile(string fileName)
        {
            return ParseMulti(File.ReadAllLines(fileName));
        }

        /// <summary>
        /// Parses a list of lines into a list of sudoku, accounting for most cases usually encountered
        /// </summary>
        /// <param name="lines">the lines of string to parse</param>
        /// <returns>the list of parsed Sudokus</returns>
        public static List<SudokuBoard> ParseMulti(string[] lines)
        {
            var toReturn = new List<SudokuBoard>();
            var cells = new List<int>(81);
            // we ignore lines not starting with a sudoku character
            foreach (var line in lines.Where(l => l.Length > 0
                                                 && IsSudokuChar(l[0])))
            {
                foreach (char c in line)
                {
                    //we ignore lines not starting with cell chars
                    if (IsSudokuChar(c))
                    {
                        if (char.IsDigit(c))
                        {
                            // if char is a digit, we add it to a cell
                            cells.Add((int)Char.GetNumericValue(c));
                        }
                        else
                        {
                            // if char represents an empty cell, we add 0
                            cells.Add(0);
                        }
                    }
                    // when 81 cells are entered, we create a sudoku and start collecting cells again.
                    if (cells.Count == 81)
                    {
                        toReturn.Add(new SudokuBoard { Cells = new List<int>(cells) });
                        // we empty the current cell collector to start building a new Sudoku
                        cells.Clear();
                    }

                }
            }

            return toReturn;
        }


        /// <summary>
        /// identifies characters to be parsed into sudoku cells
        /// </summary>
        /// <param name="c">a character to test</param>
        /// <returns>true if the character is a cell's char</returns>
        private static bool IsSudokuChar(char c)
        {
            return char.IsDigit(c) || c == '.' || c == 'X' || c == '-';
        }








        /// <summary>
        /// The cell domains updated from the initial mask for the board to solve
        /// </summary>
        private Dictionary<int, List<int>> _extendedMask;


       

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
                    var indices = Enumerable.Range(1, 9);
                    var extendedMask = new Dictionary<int, List<int>>(81);

                    //If target sudoku mask is provided, we generate an inverted mask with forbidden values by propagating rows, columns and boxes constraints
                    var forbiddenMask = new Dictionary<int, List<int>>();
                    List<int> targetList = null;
                    for (var index = 0; index < Cells.Count; index++)
                    {
                        var targetCell = Cells[index];
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
                    for (var index = 0; index < Cells.Count; index++)
                    {
                        extendedMask[index] = indices.Where(i => !forbiddenMask[index].Contains(i)).ToList();
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
        public virtual IList<int> GetPermutation(int rowIndex, int permIDx)
        {
            // we use a modulo operator in case the gene was swapped:
            // It may contain a number higher than the number of available permutations. 

            var perm = GetRowsPermutations()[rowIndex][permIDx.PositiveMod(GetRowsPermutations()[rowIndex].Count)];
            return perm;

        }


        /// <summary>
        /// This method computes for each row the list of digit permutations that respect the target mask, that is the list of valid rows discarding columns and boxes
        /// </summary>
        /// <param name="sudokuBoard">the target sudoku to account for</param>
        /// <returns>the list of permutations available</returns>
        public IList<IList<IList<int>>> GetRowsPermutations()
        {

            // we store permutations to compute them once only for each target Sudoku
            if (_rowsPermutations == null)
            {
                // Since this is a static member we use a lock to prevent parallelism.
                // This should be computed once only.
                lock (this)
                {
                    _rowsPermutations = GetRowsPermutationsUncached();
                }
            }
            return _rowsPermutations;
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
                    if (NeighborhoodIndex.All(j => ExtendedMask[i * 9 + j].Contains(perm[j])))
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
                            _unfilteredPermutations = NeighborhoodIndex.Select(i => AllPermutations).ToList();
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
                            _allPermutations = Enumerable.Range(1, 9).GetPermutations(9);
                        }
                    }
                }
                return _allPermutations;
            }
        }

       
        /// <summary>
        /// The list of compatible permutations for a given Sudoku is stored for fast retrieval
        /// </summary>
        private  IList<IList<IList<int>>> _rowsPermutations;

       
       
        /// <summary>
        /// The complete list of unfiltered permutations is stored for quicker access
        /// </summary>
        private static IList<IList<int>> _allPermutations = new List<IList<int>>();
        private static IList<IList<IList<int>>> _unfilteredPermutations = new List<IList<IList<int>>>();



    }


}