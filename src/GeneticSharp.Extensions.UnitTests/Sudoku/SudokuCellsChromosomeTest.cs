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
        /// The cells chromosome might need more individuals, so in order to keep execution time low, we only expect near completion
        /// </summary>
        [Test]
        public void Evolve_SimpleSudokuCellsChromosome_NearlySolved()
        {
            var sudoku = SudokuTestHelper.CreateBoard();

            //the cells chromosome should solve the sudoku or nearly in less than 50 generations with 500 chromosomes
            var chromosome = new SudokuCellsChromosome(sudoku);
            var fitness = SudokuTestHelper.Eval(chromosome, sudoku, 500, -20, 30);
            Assert.GreaterOrEqual(fitness, -20);

        }
    }
}