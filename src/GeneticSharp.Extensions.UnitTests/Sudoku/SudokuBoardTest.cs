using System;
using System.Globalization;
using System.IO;
using System.Linq;
using GeneticSharp.Extensions.Sudoku;
using NUnit.Framework;

namespace GeneticSharp.Extensions.UnitTests.Sudoku
{


    [TestFixture()]
    [Category("Extensions")]
    public class SudokuBoardTest
    {
        [Test()]
        public void Constructor_TooManyCells_Exception()
        {
            var tooMuchCells = Enumerable.Repeat(0, 82);
            var actual = Assert.Catch<ArgumentException>(() =>
            {
                new SudokuBoard(tooMuchCells);
            });
            Assert.AreEqual("cells", actual.ParamName);
        }

        /// <summary>
        /// The sample sudoku string should parse properly into corresponding cells
        /// </summary>
        [Test()]
        public void Parse_SampleString_ConsistantCells()
        {

            var sudoku = SudokuTestHelper.CreateBoard();

            Assert.AreEqual(sudoku.Cells[0], 9);
            Assert.AreEqual(sudoku.Cells[1], 0);
            Assert.AreEqual(sudoku.Cells[2], 2);
            Assert.AreEqual(sudoku.Cells[sudoku.Cells.Count - 2], 5);
            Assert.AreEqual(sudoku.Cells[sudoku.Cells.Count - 1], 0);

            sudoku.SetCell(0,0,0);
            Assert.AreEqual(sudoku.Cells[0], 0);
            var stringExport = sudoku.ToString();
            var currentIndex = 0;
            foreach (var sudokuCell in sudoku.Cells)
            {
                var newIndex = stringExport.IndexOf(sudokuCell.ToString(CultureInfo.InvariantCulture), currentIndex, StringComparison.Ordinal);
                Assert.Greater(newIndex, currentIndex);
                currentIndex = newIndex+1;
            }
        }
               

        /// <summary>
        /// The sample sudoku file should parse properly into corresponding individual Sudokus
        /// </summary>
        [Test()]  
        public void Parse_SampleFile_SudokusAreParsedFromFile()
        {

            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sudoku", "SudokuList.sdk");
            var sudokus = SudokuBoard.ParseFile(fileName);
            Assert.AreEqual(sudokus.Count, 16002);
            Assert.AreEqual(sudokus[0].Cells[2], 7);
            Assert.AreEqual(sudokus[1].Cells[1], 2);
        }
    }
}