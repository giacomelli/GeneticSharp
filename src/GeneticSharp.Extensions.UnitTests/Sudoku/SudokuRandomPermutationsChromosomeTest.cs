using GeneticSharp.Extensions.Sudoku;
using NUnit.Framework;

namespace GeneticSharp.Extensions.UnitTests.Sudoku
{
    [TestFixture]
    [Category("Extensions")]
    public class SudokuRandomPermutationsChromosomeTest
    {
        [Test]
        public void Constructor_NoArgs_Length9()
        {
            var target = new SudokuRandomPermutationsChromosome();
            Assert.AreEqual(9, target.Length);

            var genes = target.GetGenes();
            Assert.AreEqual(9, genes.Length);
        }

        /// <summary>
        /// The random permutations chromosome require more individuals and generations, so we only test for significant progresses
        /// </summary>
        [Test]
        public void Evolve_SimpleSudokuRandomPermutationsChromosome_Progressed()
        {
            var sudoku = SudokuTestHelper.CreateBoard();

            //the Random permutations chromosome should make significant progresses over 3 generations with 5 individuals

            var chromosome = new SudokuRandomPermutationsChromosome(sudoku, 2, 3);
            var fitness1 = new SudokuFitness(sudoku).Evaluate((ISudokuChromosome)chromosome);
            var fitness2 = SudokuTestHelper.Eval(chromosome, sudoku, 5, fitness1 + 20, 3);
            Assert.GreaterOrEqual(fitness2, fitness1 + 20);

        }
    }
}