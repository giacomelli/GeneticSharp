using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Extensions.Sudoku
{
    /// <summary>
    /// This more elaborated chromosome manipulates rows instead of cells, and each of its 9 gene holds an integer for the index of the row's permutation amongst all that respect the target mask.
    /// Permutations are computed once when a new Sudoku is encountered, and stored in a static dictionary for further reference.
    /// </summary>
    public class SudokuPermutationsChromosome : SudokuChromosomeBase, ISudokuChromosome
    {
       

      


        /// <summary>
        /// This constructor assumes no mask
        /// </summary>
        public SudokuPermutationsChromosome() : this(null) {}

        /// <summary>
        /// Constructor with a mask sudoku to solve, assuming a length of 9 genes
        /// </summary>
        /// <param name="targetSudokuBoard">the target sudoku to solve</param>
        public SudokuPermutationsChromosome(SudokuBoard targetSudokuBoard) : this(targetSudokuBoard, 9) {}

        /// <summary>
        /// Constructor with a mask and a number of genes
        /// </summary>
        /// <param name="targetSudokuBoard">the target sudoku to solve</param>
        /// <param name="length">the number of genes</param>
        public SudokuPermutationsChromosome(SudokuBoard targetSudokuBoard, int length) : this(targetSudokuBoard, null, length) {}

        /// <summary>
        /// /// Constructor with a mask and extended mask accounting for initial constraint propagation for faster cloning
        /// </summary>
        /// <param name="targetSudokuBoard">the target sudoku to solve</param>
        /// <param name="extendedMask">The cell domains after initial constraint propagation</param>
        public SudokuPermutationsChromosome(SudokuBoard targetSudokuBoard, Dictionary<int, List<int>> extendedMask) : this(targetSudokuBoard, extendedMask, 9) {}

        /// <param name="targetSudokuBoard">the target sudoku to solve</param>
        /// <param name="extendedMask">The cell domains after initial constraint propagation</param>
        /// <param name="length">The number of genes for the sudoku chromosome</param>
        public SudokuPermutationsChromosome(SudokuBoard targetSudokuBoard, Dictionary<int, List<int>> extendedMask, int length) : base(targetSudokuBoard, extendedMask, length) { }


        /// <summary>
        /// generates a chromosome gene from its index containing a random row permutation
        /// amongst those respecting the target mask. 
        /// </summary>
        /// <param name="geneIndex">the index for the gene</param>
        /// <returns>a gene generated for the index</returns>
        public override Gene GenerateGene(int geneIndex)
        {

            var rnd = RandomizationProvider.Current;
            //we randomize amongst the permutations that account for the target mask.
            var permIdx = rnd.GetInt(0, TargetRowsPermutations[geneIndex].Count);
            return new Gene(permIdx);
        }

        public override IChromosome CreateNew()
        {
            var toReturn = new SudokuPermutationsChromosome(TargetSudokuBoard, ExtendedMask, Length);
            return toReturn;
        }


        /// <summary>
        /// builds a single Sudoku from the given row permutation genes
        /// </summary>
        /// <returns>a list with the single Sudoku built from the genes</returns>
        public override IList<SudokuBoard> GetSudokus()
        {
            var listInt = new List<int>(81);
            for (int i = 0; i < 9; i++)
            {
                var perm = GetPermutation(i);
                listInt.AddRange(perm);
            }
            var sudoku = new SudokuBoard(listInt);
            return new List<SudokuBoard>(new[] { sudoku });
        }



        /// <summary>
        /// Gets the permutation to apply from the index of the row concerned
        /// </summary>
        /// <param name="rowIndex">the index of the row to permute</param>
        /// <returns>the index of the permutation to apply</returns>
        protected virtual List<int> GetPermutation(int rowIndex)
        {
            int permIDx = GetPermutationIndex(rowIndex);
            return GetPermutation(rowIndex, permIDx);
        }


        


        /// <summary>
        /// Gets the permutation to apply from the index of the row concerned
        /// </summary>
        /// <param name="rowIndex">the index of the row to permute</param>
        /// <returns>the index of the permutation to apply</returns>
        protected virtual int GetPermutationIndex(int rowIndex)
        {
            return (int)GetGene(rowIndex).Value;
        }


        



    }
}