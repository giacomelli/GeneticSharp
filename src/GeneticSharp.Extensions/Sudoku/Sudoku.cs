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
    public class Sudoku
    {

        public Sudoku()
        {
        }

        public Sudoku(IEnumerable<int> cells)
        {
            if (cells.Count() != 81)
            {
                throw new ArgumentException(nameof(cells));
            }
            CellsList = new List<int>(cells);
        }

        // We use a list for easier access to cells,
        public List<int> CellsList = Enumerable.Repeat(0, 81).ToList();

        public int GetCell(int x, int y)
        {
            return CellsList[(9 * x) + y];
        }

        public void SetCell(int x, int y, int value)
        {
            CellsList[(9 * x) + y] = value;
        }

        /// <summary>
        /// The array property is to be used in linq to Z3
        /// </summary>
        public int[] Cells
        {
            get => CellsList.ToArray();
            set => CellsList = new List<int>(value);
        }

        /// <summary>
        /// Displays a Sudoku in an easy to read format
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var lineSep = new string('-', 31);
            var blankSep = new string(' ', 8);

            var output = new StringBuilder();
            output.Append(lineSep);
            output.AppendLine();

            for (int row = 1; row <= 9; row++)
            {
                output.Append("| ");
                for (int column = 1; column <= 9; column++)
                {

                    var value = CellsList[(row - 1) * 9 + (column - 1)];
                    output.Append(value);
                    output.Append(column % 3 == 0 ? " | " : "  ");
                }

                output.AppendLine();
                if (row % 3 == 0)
                {
                    output.Append(lineSep);
                }
                // todo:for some reason, GTK does not seem to like the following display
                //else
                //{
                //  output.Append("| ");
                //  for (var i = 0; i < 3; i++)
                //  {
                //    output.Append(blankSep);
                //    output.Append("| ");
                //  }
                //}
                output.AppendLine();
            }

            return output.ToString();
        }

        /// <summary>
        /// Parses a single Sudoku
        /// </summary>
        /// <param name="sudokuAsString">the string representing the sudoku</param>
        /// <returns>the parsed sudoku</returns>
        public static Sudoku Parse(string sudokuAsString)
        {
            return ParseMulti(new[] { sudokuAsString })[0];
        }

        /// <summary>
        /// Parses a file with one or several sudokus
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>the list of parsed Sudokus</returns>
        public static List<Sudoku> ParseFile(string fileName)
        {
            return ParseMulti(File.ReadAllLines(fileName));
        }

        /// <summary>
        /// Parses a list of lines into a list of sudoku, accounting for most cases usually encountered
        /// </summary>
        /// <param name="lines">the lines of string to parse</param>
        /// <returns>the list of parsed Sudokus</returns>
        public static List<Sudoku> ParseMulti(string[] lines)
        {
            var toReturn = new List<Sudoku>();
            var cells = new List<int>(81);
            foreach (var line in lines)
            {
                if (line.Length > 0)
                {
                    if (char.IsDigit(line[0]) || line[0] == '.' || line[0] == 'X' || line[0] == '-')
                    {
                        foreach (char c in line)
                        {
                            int? cellToAdd = null;
                            if (char.IsDigit(c))
                            {
                                var cell = (int)Char.GetNumericValue(c);
                                cellToAdd = cell;
                            }
                            else
                            {
                                if (c == '.' || c == 'X' || c == '-')
                                {
                                    cellToAdd = 0;
                                }
                            }

                            if (cellToAdd.HasValue)
                            {
                                cells.Add(cellToAdd.Value);
                                if (cells.Count == 81)
                                {
                                    toReturn.Add(new Sudoku() { CellsList = new List<int>(cells) });
                                    cells.Clear();
                                }
                            }
                        }
                    }
                }
            }

            return toReturn;
        }
    }
}