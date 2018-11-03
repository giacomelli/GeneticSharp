using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Extensions.Sudoku;
using NUnit.Framework;

namespace GeneticSharp.Extensions.UnitTests.Sudoku
{
    [TestFixture]
    [Category("Extensions")]
    public class SudokuPermutationsChromosomeTest
    {
        [Test]
        public void Constructor_NoArgs_Length9()
        {
            var target = new SudokuPermutationsChromosome();
            Assert.AreEqual(9, target.Length);

            var genes = target.GetGenes();
            Assert.AreEqual(9, genes.Length);
        }

        /// <summary>
        /// The permutation chromosome should always solve the sudoku in a reasonable time with 1000 chromosomes
        /// </summary>
        [Test]
        public void Evolve_SimpleSudokuPermutationsChromosome_Solved()
        {
            var sudoku = SudokuTestHelper.CreateBoard();

            IChromosome chromosome = new SudokuPermutationsChromosome(sudoku);
            var fitness = SudokuTestHelper.Eval(chromosome, sudoku, 1000, 0, 50);
            Assert.AreEqual(fitness, 0);
        }
    }
}