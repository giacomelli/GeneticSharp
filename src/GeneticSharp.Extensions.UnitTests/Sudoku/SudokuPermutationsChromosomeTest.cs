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
        /// The permutation chromosome should always solve the very easy sudoku with small population in few generations
        /// </summary>
        [Test]
        public void Evolve_PermutationsChromosome_VeryEasySudoku_Solved()
        {
            var sudoku = SudokuTestHelper.CreateBoard(SudokuTestDifficulty.VeryEasy);

            IChromosome chromosome = new SudokuPermutationsChromosome(sudoku);
            var fitness = SudokuTestHelper.Eval(chromosome, sudoku, 50, 1, 10, out int genNb);
            Assert.AreEqual(1, fitness);
        }

        /// <summary>
        /// The random permutation chromosome should always solve the easy sudoku with population 500
        /// </summary>
        [Test]
        public void Evolve_PermutationsChromosome_EasySudoku_Solved()
        {
            var sudoku = SudokuTestHelper.CreateBoard(SudokuTestDifficulty.Easy);

            IChromosome chromosome = new SudokuPermutationsChromosome(sudoku);
            var fitness = SudokuTestHelper.Eval(chromosome, sudoku, 500, 1, 30, out int genNb);
            Assert.AreEqual( 1, fitness);
        }
    }
}