using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;

namespace GeneticSharp.Extensions.Sudoku
{
    /// <summary>
    /// Evaluates a sudoku chromosome for completion by counting duplicates in rows, columns, boxes, and differences from the target mask
    /// </summary>
    public class SudokuFitness : IFitness
    {
        /// <summary>
        /// The target Sudoku Mask to solve.
        /// </summary>
        private readonly SudokuBoard _targetSudokuBoard;

        public SudokuFitness(SudokuBoard targetSudokuBoard)
        {
            _targetSudokuBoard = targetSudokuBoard;
        }

        /// <summary>
        /// Evaluates a chromosome according to the IFitness interface. Simply reroutes to a typed version.
        /// </summary>
        /// <param name="chromosome"></param>
        /// <returns></returns>
        public double Evaluate(IChromosome chromosome)
        {
            return Evaluate((ISudokuChromosome)chromosome);
        }

        /// <summary>
        /// Evaluates a ISudokuChromosome by summing over the fitnesses of its corresponding Sudoku boards.
        /// </summary>
        /// <param name="chromosome">a Chromosome that can build Sudokus</param>
        /// <returns>the chromosome's fitness</returns>
        public double Evaluate(ISudokuChromosome chromosome)
        {
            List<double> scores = new List<double>();

            var sudokus = chromosome.GetSudokus();
            foreach (var sudoku in sudokus)
            {
                scores.Add(Evaluate(sudoku));
            }

            return scores.Sum();
        }

        /// <summary>
        /// Evaluates a single Sudoku board by counting the duplicates in rows, boxes
        /// and the digits differing from the target mask.
        /// </summary>
        /// <param name="testSudokuBoard">the board to evaluate</param>
        /// <returns>the number of mistakes the Sudoku contains.</returns>
        public double Evaluate(SudokuBoard testSudokuBoard)
        {
            // We use a large lambda expression to count duplicates in rows, columns and boxes
            var cells = testSudokuBoard.Cells.Select((c, i) => new { index = i, cell = c });
            var toTest = cells.GroupBy(x => x.index / 9).Select(g => g.Select(c => c.cell)) // rows
              .Concat(cells.GroupBy(x => x.index % 9).Select(g => g.Select(c => c.cell))) //columns
              .Concat(cells.GroupBy(x => x.index / 27 * 27 + x.index % 9 / 3 * 3).Select(g => g.Select(c => c.cell))); //boxes
            var toReturn = -toTest.Sum(test => test.GroupBy(x => x).Select(g => g.Count() - 1).Sum()); // Summing over duplicates
            toReturn -= cells.Count(x => _targetSudokuBoard.Cells[x.index] > 0 && _targetSudokuBoard.Cells[x.index] != x.cell); // Mask
            return toReturn;
        }



    }



}