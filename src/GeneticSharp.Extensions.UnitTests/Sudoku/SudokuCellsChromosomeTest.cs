using System.Linq;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Metaheuristics;
using GeneticSharp.Domain.Mutations;
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

            var chromosome = new SudokuCellsChromosome(sudoku, false);
            var fitness = SudokuTestHelper.Eval(chromosome, sudoku, 500, 1, 30, out int genNb);
            Assert.Less(genNb, 30);
            Assert.AreEqual(1, fitness);

        }

        /// <summary>
        /// The cells chromosome initialized with row permutations should always solve the easy Sudoku with a population of 500 chromosomes in less than 30 generations
        /// </summary>
        [Test]
        public void Evolve_CellsChromosome_EasySudoku_Solved()
        {
            var sudoku = SudokuTestHelper.CreateBoard(SudokuTestDifficulty.Easy);

            var chromosome = new SudokuCellsChromosome(sudoku, true);
            var fitness = SudokuTestHelper.Eval(chromosome, sudoku, 2000, 1, 50, out int genNb);
            Assert.Less(genNb, 50);
            Assert.AreEqual(1, fitness);

        }


        /// <summary>
        /// The cells chromosome initialized with row permutations, used with a EurkaryoteMetaHeuristics and row subchromosomes together with ordered crossovers and mutations to keep yielding row permutations, should always solve the Medium Sudoku with a population of 5000 chromosomes in less than 50 generations
        /// </summary>
        [Test]
        public void Evolve_CellsChromosome_Metaheuristics_EasySudoku_Solved()
        {
            var sudoku = SudokuTestHelper.CreateBoard(SudokuTestDifficulty.Easy);

            var chromosome = new SudokuCellsChromosome(sudoku, true);

            // We split the original 81 genes/cells chromosome into a 9x9genes chromosomes Karyotype
            var metaHeuristics = new EukaryoteMetaHeuristic(9, 9, new DefaultMetaHeuristic()) { Scope = MetaHeuristicsStage.Crossover | MetaHeuristicsStage.Mutation };
            //Since we used rows permutations at init and the solution is also a row permutation, we used ordered crossovers and mutations to keep yielding permutations
            var crossover = new CycleCrossover();
            var mutation = new TworsMutation();

            var fitness = SudokuTestHelper.Eval(chromosome, sudoku, metaHeuristics, crossover, mutation, 1000, 1, 50, out int genNb);
            Assert.Less(genNb, 50);
            Assert.AreEqual(1, fitness);

        }



    }
}