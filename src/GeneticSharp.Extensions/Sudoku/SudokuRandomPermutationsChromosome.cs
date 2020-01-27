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

        private readonly int _nbPermutations = 10;
        private readonly int _nbSudokus = 10;


        public SudokuRandomPermutationsChromosome(Sudoku targetSudoku, int nbPermutations, int nbSudokus) : base(targetSudoku, 9 * nbPermutations, null)
        {
            _nbPermutations = nbPermutations;
            _nbSudokus = nbSudokus;

        }

        public override Gene GenerateGene(int geneIndex)
        {
            var rnd = RandomizationProvider.Current;

            var rowIndex = geneIndex % 9;

            var permIdx = rnd.GetInt(0, TargetRowsPermutations[rowIndex].Count);
            return new Gene(permIdx);
        }

        public override List<Sudoku> GetSudokus()
        {
            var toReturn = new List<Sudoku>(_nbSudokus);
            for (int i = 0; i < _nbSudokus; i++)
            {
                toReturn.AddRange(base.GetSudokus());
            }

            return toReturn;
        }


        protected override int GetPermutationIndex(int rowIndex)
        {
            var rnd = RandomizationProvider.Current;
            var switchIdx = rnd.GetInt(0, _nbPermutations);
            var permGeneIdx = switchIdx * 9 + rowIndex;
            return (int)GetGene(permGeneIdx).Value;
        }

        public override IChromosome CreateNew()
        {
            return new SudokuRandomPermutationsChromosome(TargetSudoku, _nbPermutations, _nbSudokus);
        }
    }
}