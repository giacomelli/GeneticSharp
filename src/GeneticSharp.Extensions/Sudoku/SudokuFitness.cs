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
            var nbErrors = GetNbErrors(testSudokuBoard);
            return 1 - (nbErrors / 100);
        }

        //private int GetWorstCaseError(SudokuBoard testSudokuBoard)
        //{
        //    //Nothing fancy for now
        //    return 100;
        //}

        /// <summary>
        /// Evaluates a single Sudoku board by counting the duplicates in rows, boxes
        /// and the digits differing from the target mask.
        /// </summary>
        /// <param name="testSudokuBoard">the board to evaluate</param>
        /// <returns>the number of mistakes the Sudoku contains.</returns>
        public int GetNbErrors(SudokuBoard testSudokuBoard)
        {
            //Summing other duplicates on each neighborhood
            var toReturn = SudokuBoard.AllNeighborhoods.Select(n => n.Select(nx => testSudokuBoard.Cells[nx])).Sum(n => n.GroupBy(x => x).Select(g => g.Count() - 1).Sum());

            toReturn += SudokuBoard.CellIndex.Count(c => _targetSudokuBoard.Cells[c] > 0 && _targetSudokuBoard.Cells[c] != testSudokuBoard.Cells[c]); // Mask
            return toReturn;
        }

        



    }



}