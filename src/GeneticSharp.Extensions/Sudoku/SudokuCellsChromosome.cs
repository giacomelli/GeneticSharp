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
        private Sudoku _targetSudoku;
		private Dictionary<int, List<int>> _extendedMask;



        public SudokuCellsChromosome() : this(null)
        {
        }

        public SudokuCellsChromosome(Sudoku targetSudoku) : base(81)
        {
            _targetSudoku = targetSudoku;
            for (int i = 0; i < Length; i++)
            {
                ReplaceGene(i, GenerateGene(i));
            }
        }

	    public SudokuCellsChromosome(Sudoku targetSudoku, Dictionary<int, List<int>> mask) : base(81)
	    {
		    _targetSudoku = targetSudoku;
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
					for (var index = 0; index < _targetSudoku.CellsList.Count; index++)
				    {
					    var targetCell = _targetSudoku.CellsList[index];
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
					for (var index = 0; index < _targetSudoku.CellsList.Count; index++)
					{
						_extendedMask[index] = indices.Where(i => !invertedMask[index].Contains(i)).ToList();
					}

				    //Debug.WriteLine(JsonConvert.SerializeObject(_extendedMask));

				}
			    return _extendedMask;
		    }
	    }


	    public override Gene GenerateGene(int geneIndex)
        {
            //If a target mask exist and has a digit for the cell, we use it.
            if (_targetSudoku != null && _targetSudoku.CellsList[geneIndex] != 0)
            {
                return new Gene(_targetSudoku.CellsList[geneIndex]);
            }
            var rnd = RandomizationProvider.Current;
	        var targetIdx = rnd.GetInt(0, ExtendedMask[geneIndex].Count);
			return new Gene(ExtendedMask[geneIndex][targetIdx]);
        }

        public override IChromosome CreateNew()
        {
            return new SudokuCellsChromosome(_targetSudoku, ExtendedMask);
        }


        public List<Sudoku> GetSudokus()
        {
	      var sudoku = new Sudoku(GetGenes().Select(g => (int)g.Value));
            return new List<Sudoku>(new[] { sudoku });
        }
    }
}