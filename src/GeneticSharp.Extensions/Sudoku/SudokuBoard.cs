using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GeneticSharp.Extensions.Sudoku
{
    /// <summary>
    /// Class that represents a Sudoku, fully or partially completed
    /// Holds a list of 81 int for cells, with 0 for empty cells
    /// Can parse strings and files from most common formats and displays the sudoku in an easy to read format
    /// </summary>
    public class SudokuBoard
    {

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
            if (enumerable.Count() != 81)
            {
                throw new ArgumentException(nameof(cells));
            }
            _cellsList = new List<int>(enumerable);
        }

        // We use a list for easier access to cells,
        private IList<int> _cellsList = Enumerable.Repeat(0, 81).ToList();

        /// <summary>
        /// Easy access by row and column number
        /// </summary>
        /// <param name="x">row number (between 0 and 8)</param>
        /// <param name="y">column number (between 0 and 8)</param>
        /// <returns>value of the cell</returns>
        public int GetCell(int x, int y)
        {
            return _cellsList[(9 * x) + y];
        }

        /// <summary>
        /// Easy setter by row and column number
        /// </summary>
        /// <param name="x">row number (between 0 and 8)</param>
        /// <param name="y">column number (between 0 and 8)</param>
        /// <param name="value">value of the cell to set</param>
        public void SetCell(int x, int y, int value)
        {
            _cellsList[(9 * x) + y] = value;
        }

        /// <summary>
        /// The array property is to be used in linq to Z3
        /// </summary>
        public int[] Cells
        {
            get => _cellsList.ToArray();
            set => _cellsList = new List<int>(value);
        }

        public IList<int> CellsList
        {
            get => _cellsList;
            set => _cellsList = value;
        }

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

            for (int row = 1; row <= 9; row++)
            {
                output.Append("| ");
                for (int column = 1; column <= 9; column++)
                {

                    var value = _cellsList[(row - 1) * 9 + (column - 1)];
                    output.Append(value);
                    output.Append(column % 3 == 0 ? " | " : "  ");
                }

                output.AppendLine();
                if (row % 3 == 0)
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
                    if (IsSudokuChar(c))
                    {
                        if (char.IsDigit(c))
                        {
                            cells.Add((int)Char.GetNumericValue(c));
                        }
                        else
                        {
                            cells.Add(0);
                        }
                    }

                    if (cells.Count == 81)
                    {
                        toReturn.Add(new SudokuBoard() { _cellsList = new List<int>(cells) });
                        cells.Clear();
                    }

                }
            }

            return toReturn;
        }

        private static bool IsSudokuChar(char c)
        {
            return char.IsDigit(c) || c == '.' || c == 'X' || c == '-';
        }

    }


}