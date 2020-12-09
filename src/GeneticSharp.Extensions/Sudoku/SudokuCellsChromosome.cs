using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Extensions.Sudoku
{
    /// <summary>
	/// This simple chromosome simply represents each cell by a gene with value between 1 and 9, accounting for the target mask if given
	/// </summary>
	public class SudokuCellsChromosome : SudokuChromosomeBase, ISudokuChromosome
    {
       

        public SudokuCellsChromosome() : this(null, true)
        {
        }

        /// <summary>
        /// Basic constructor with target sudoku to solve
        /// </summary>
        /// <param name="targetSudokuBoard">the target sudoku to solve</param>
        /// <param name="initWithPermutations">defines if the chromosome gene cells should be initialized with row permutations</param>
        public SudokuCellsChromosome(SudokuBoard targetSudokuBoard, bool initWithPermutations) : this( targetSudokuBoard, initWithPermutations, null) {}

        /// <summary>
        /// Constructor with additional precomputed domains for faster cloning
        /// </summary>
        /// <param name="targetSudokuBoard">the target sudoku to solve</param>
        /// <param name="initWithPermutations">defines if the chromosome gene cells should be initialized with row permutations</param>
        /// <param name="extendedMask">The cell domains after initial constraint propagation</param>
        public SudokuCellsChromosome(SudokuBoard targetSudokuBoard, bool initWithPermutations, Dictionary<int, List<int>> extendedMask) : base(targetSudokuBoard, extendedMask, 81)
        {
            InitWithPermutations = initWithPermutations;
        }

        /// <summary>
        /// Defines if the chromosome cell should be initialized with row permutations, allowing for ordered subcrossovers with metaheuristics
        /// </summary>
        public bool InitWithPermutations { get; set; }


        public override Gene GenerateGene(int geneIndex)
        {
            //If a target mask exist and has a digit for the cell, we use it.
            if (TargetSudokuBoard != null && TargetSudokuBoard.Cells[geneIndex] != 0)
            {
                return new Gene(TargetSudokuBoard.Cells[geneIndex]);
            }
            // otherwise we use a random digit amongts those permitted.
			var rnd = RandomizationProvider.Current;
	        var targetIdx = rnd.GetInt(0, ExtendedMask[geneIndex].Count);
			return new Gene(ExtendedMask[geneIndex][targetIdx]);
        }

        public override IChromosome CreateNew()
        {
            return new SudokuCellsChromosome(TargetSudokuBoard, InitWithPermutations, ExtendedMask);
        }

        /// <summary>
        /// Builds a single Sudoku from the 81 genes
        /// </summary>
        /// <returns>A Sudoku board built from the 81 genes</returns>
        public override IList<SudokuBoard> GetSudokus()
        {
            var sudoku = new SudokuBoard(GetGenes().Select(g => (int)g.Value));
            return new List<SudokuBoard>(new[] { sudoku });
        }


        /// <summary>
        /// Creates the initial cell genes, either random accounting for the target Sudoku Mask, or according to row permutations with the same constraint
        /// </summary>
        protected override void CreateGenes()
        {
            if (InitWithPermutations)
            {
                for (int rowIndex = 0; rowIndex < 9; rowIndex++)
                {
                    var rowPerms = TargetRowsPermutations[rowIndex];
                    var rndIndx = RandomizationProvider.Current.GetInt(0, rowPerms.Count);
                    var rowPerm = rowPerms[rndIndx];
                    for (int colIndex = 0; colIndex < 9; colIndex++)
                    {
                        ReplaceGene(9*rowIndex+colIndex, new Gene(rowPerm[colIndex]));
                    }
                }
            }
            else
            {
                base.CreateGenes();
            }
            
        }
    }
}