using GeneticSharp.Extensions.Sudoku;
using NUnit.Framework;

namespace GeneticSharp.Extensions.UnitTests.Sudoku
{
    [TestFixture]
    [Category("Extensions")]
    public class SudokuCellsChromosomeTest
    {
        [Test]
        public void Constructor_NoArgs_Length81()
        {
            var target = new SudokuCellsChromosome();
            Assert.AreEqual(81, target.Length);

            var genes = target.GetGenes();
            Assert.AreEqual(81, genes.Length);
        }

        /// <summary>
        /// The cells chromosome should always solve the very easy Sudoku with a population of 500 chromosomes in less than 30 generations
        /// </summary>
        [Test]
        public void Evolve_CellsChromosome_VeryEasySudoku_Solved()
        {
            var sudoku = SudokuTestHelper.CreateBoard(SudokuTestDifficulty.VeryEasy);

            //the cells chromosome should solve the sudoku in less than 30 generations with 500 chromosomes
            var chromosome = new SudokuCellsChromosome(sudoku);
            var fitness = SudokuTestHelper.Eval(chromosome, sudoku, 500, 0, 30);
            Assert.GreaterOrEqual(fitness, 0);

        }
    }
}