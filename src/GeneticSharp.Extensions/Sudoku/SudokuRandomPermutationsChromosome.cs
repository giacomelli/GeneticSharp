using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Extensions.Sudoku
{
    /// <summary>
    /// This chromosome aims at increasing genetic diversity of SudokuPermutationsChromosome, which exhibits only 9 permutation genes 
    /// Here, instead, an arbitrary number of Sudokus are generated where for each row, a random gene is picked  amongst an arbitrary number of corresponding permutation genes 
    /// </summary>
    public class SudokuRandomPermutationsChromosome : SudokuPermutationsChromosome
    {

        /// <summary>
        /// The number of permutation gene to keep for each row
        /// </summary>
        private readonly int _nbPermutations = 10;

        /// <summary>
        /// The number of Sudokus to generate from the random permutations for evaluation
        /// </summary>
        private readonly int _nbSudokus = 10;



        /// <summary>
        /// The empty constructor assumes no target mask and uses the member initializers as default.
        /// </summary>
        public SudokuRandomPermutationsChromosome()
        {
        }


        /// <summary>
        /// Constructor that takes the target Sudoku, the number of permutation genes per row, and the number of Sudokus to evaluate
        /// </summary>
        /// <param name="targetSudokuBoard">the target sudoku to solve</param>
        /// <param name="nbPermutations">the number of permutation genes per row</param>
        /// <param name="nbSudokus">the number of Sudokus generated for evaluation</param>
        public SudokuRandomPermutationsChromosome(SudokuBoard targetSudokuBoard, int nbPermutations, int nbSudokus) : base(targetSudokuBoard, 9 * nbPermutations)
        {
            _nbPermutations = nbPermutations;
            _nbSudokus = nbSudokus;

        }

        /// <summary>
        /// Overriden from the original permutation chromosome, generates a random permutation for one of th 9 rows,
        /// The row index is given by the rest of the gene index divided by 9  
        /// </summary>
        /// <param name="geneIndex">the gene index amongst all associated to random permutations</param>
        /// <returns>the gene generated for the corresponding index</returns>
        public override Gene GenerateGene(int geneIndex)
        {
            var rnd = RandomizationProvider.Current;

            var rowIndex = geneIndex % 9;

            var permIdx = rnd.GetInt(0, TargetRowsPermutations[rowIndex].Count);
            return new Gene(permIdx);
        }

        /// <summary>
        /// Creates the number of Sudokus defined in the corresponding field, from the random permutations, to be evaluated.
        /// </summary>
        /// <returns>a list of Sudokus for evaluation</returns>
        public override IList<SudokuBoard> GetSudokus()
        {
            var toReturn = new List<SudokuBoard>(_nbSudokus);
            for (int i = 0; i < _nbSudokus; i++)
            {
                toReturn.AddRange(base.GetSudokus());
            }

            return toReturn;
        }

        /// <summary>
        /// Chooses a permutation for a given row, chosen randomly amongst the corresponding genes
        /// </summary>
        /// <param name="rowIndex">the index of the row to find a permutation for</param>
        /// <returns>a permutation index for the corresponding row.</returns>
        protected override int GetPermutationIndex(int rowIndex)
        {
            var rnd = RandomizationProvider.Current;
            var switchIdx = rnd.GetInt(0, _nbPermutations);
            var permGeneIdx = switchIdx * 9 + rowIndex;
            return (int)GetGene(permGeneIdx).Value;
        }

        public override IChromosome CreateNew()
        {
            return new SudokuRandomPermutationsChromosome(TargetSudokuBoard, _nbPermutations, _nbSudokus);
        }
    }
}