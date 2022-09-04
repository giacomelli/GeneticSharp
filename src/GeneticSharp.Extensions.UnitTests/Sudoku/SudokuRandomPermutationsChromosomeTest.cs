using System;
using GeneticSharp.Domain.Chromosomes;
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

        ///// <summary>
        ///// The random permutations chromosome require more individuals and generations, so we only test for significant progresses
        ///// </summary>
        //[Test]
        //public void Evolve_SimpleSudokuRandomPermutationsChromosome_Progressed()
        //{
        //    var sudoku = SudokuTestHelper.CreateBoard();

        //    //the Random permutations chromosome should make significant progresses over 3 generations with 5 individuals

        //    var chromosome = new SudokuRandomPermutationsChromosome(sudoku, 2, 3);
        //    var fitness1 = new SudokuFitness(sudoku).Evaluate((ISudokuChromosome)chromosome);
        //    var targetFitness = Math.Min(0, fitness1 + 10);
        //    var fitness2 = SudokuTestHelper.Eval(chromosome, sudoku, 5, targetFitness, 3);
        //    Assert.GreaterOrEqual(fitness2, targetFitness);

        //}


        /// <summary>
        /// The permutation chromosome should always solve the very easy sudoku with small population in few generations
        /// </summary>
        [Test]
        public void Evolve_RandomPermutationsChromosome_VeryEasySudoku_Solved()
        {
            var sudoku = SudokuTestHelper.CreateBoard(SudokuTestDifficulty.VeryEasy);

            IChromosome chromosome = new SudokuRandomPermutationsChromosome(sudoku,2,3);
            var fitness = SudokuTestHelper.Eval(chromosome, sudoku, 50, 0, 100);
            Assert.AreEqual( 0, fitness);
        }

        /// <summary>
        /// The permutation chromosome should always solve the easy sudoku with population 500
        /// </summary>
        [Test]
        public void Evolve_RandomPermutationsChromosome_EasySudoku_Solved() {
            var sudoku = SudokuTestHelper.CreateBoard(SudokuTestDifficulty.Easy);

            IChromosome chromosome = new SudokuPermutationsChromosome(sudoku);
            var fitness = SudokuTestHelper.Eval(chromosome, sudoku, 1000, 0, 40);
            Assert.AreEqual( 0, fitness);
        }


    }
}