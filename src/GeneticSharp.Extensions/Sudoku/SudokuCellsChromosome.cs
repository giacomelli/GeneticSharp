using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Extensions.Sudoku
{
    /// <summary>
    /// This simple chromosome simply represents each cell by a gene with value between 1 and 9, accounting for the target mask if given
    /// </summary>
    public class SudokuCellsChromosome : ChromosomeBase, ISudokuChromosome
    {
        /// <summary>
        /// The target sudoku board to solve
        /// </summary>
        private readonly SudokuBoard _targetSudokuBoard;
		private Dictionary<int, List<int>> _extendedMask;



        public SudokuCellsChromosome() : this(null)
        {
        }

        /// <summary>
        /// Constructor that accepts a Sudoku to solve
        /// </summary>
        /// <param name="targetSudokuBoard">the target sudoku to solve</param>
        public SudokuCellsChromosome(SudokuBoard targetSudokuBoard) : base(81)
        {
            _targetSudokuBoard = targetSudokuBoard;
            for (int i = 0; i < Length; i++)
            {
                ReplaceGene(i, GenerateGene(i));
            }
        }

	    public SudokuCellsChromosome(SudokuBoard targetSudokuBoard, Dictionary<int, List<int>> mask) : base(81)
	    {
            _targetSudokuBoard = targetSudokuBoard;
		    _extendedMask = mask;
		    for (int i = 0; i < Length; i++)
		    {
			    ReplaceGene(i, GenerateGene(i));
		    }
	    }

		public Dictionary<int, List<int>> ExtendedMask
	    {
		    get
		    {
			    if (_extendedMask == null)
			    {
				   
					var invertedMask = new Dictionary<int, List<int>>();
				    List<int> targetList = null;
					for (var index = 0; index < _targetSudokuBoard.Cells.Count; index++)
				    {
					    var targetCell = _targetSudokuBoard.Cells[index];
					    if (targetCell!=0)
					    {
						    var row = index / 9;
						    var col = index % 9;
						    var boxStartIdx = (index / 27 * 27) + (index % 9 / 3 * 3);

							for (int i = 0; i < 9; i++)
							{
								var boxtargetIdx = boxStartIdx + (i % 3) +  ((i / 3) * 9);
							    var targetIndices = new[] {(row * 9) + i, i *9 + col, boxtargetIdx };
							    foreach (var targetIndex in targetIndices)
							    {
								    if (targetIndex != index)
								    {
									    if (!invertedMask.TryGetValue(targetIndex, out targetList))
									    {
										    targetList = new List<int>();
										    invertedMask[targetIndex] = targetList;
									    }
									    if (!targetList.Contains(targetCell))
									    {
										    targetList.Add(targetCell);
									    }
								    }
								}
							}
					    }
				    }
				    _extendedMask = new Dictionary<int, List<int>>();
				    var indices = Enumerable.Range(1, 9).ToList();
					for (var index = 0; index < _targetSudokuBoard.CellsList.Count; index++)
					{
						_extendedMask[index] = indices.Where(i => !invertedMask[index].Contains(i)).ToList();
					}
					

				}
				return _extendedMask;
		    }
	    }


	    public override Gene GenerateGene(int geneIndex)
        {
            //If a target mask exist and has a digit for the cell, we use it.
            if (_targetSudokuBoard != null && _targetSudokuBoard.Cells[geneIndex] != 0)
            {
                return new Gene(_targetSudokuBoard.Cells[geneIndex]);
            }
            // otherwise we use a random digit amongts those permitted.
			var rnd = RandomizationProvider.Current;
	        var targetIdx = rnd.GetInt(0, ExtendedMask[geneIndex].Count);
			return new Gene(ExtendedMask[geneIndex][targetIdx]);
        }

        public override IChromosome CreateNew()
        {
            return new SudokuCellsChromosome(_targetSudokuBoard, ExtendedMask);
        }

        /// <summary>
        /// Builds a single Sudoku from the 81 genes
        /// </summary>
        /// <returns>A Sudoku board built from the 81 genes</returns>
        public IList<SudokuBoard> GetSudokus()
        {
            var sudoku = new SudokuBoard(GetGenes().Select(g => (int)g.Value));
            return new List<SudokuBoard>(new[] { sudoku });
        }
    }
}