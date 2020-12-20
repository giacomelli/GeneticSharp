using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Crossovers.Geometric;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Metaheuristics;
using GeneticSharp.Domain.Metaheuristics.Primitives;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Extensions.Multiple;
using GeneticSharp.Extensions.Sudoku;
using GeneticSharp.Infrastructure.Framework.Commons;
using Gtk;

namespace GeneticSharp.Runner.GtkApp.Samples
{

    /// <summary>
    /// This enumeration represents the types of Genetics to represent a Sudoku solutions. There are 2 families: those with genes for Sudoku cells, and those with genes for permutations of a [1..9] row.
    /// </summary>
    public enum SudokuChromosomeType
    {
        Cells,
        CellsInitWithRowsPermutations,
        CellsWithEukaryoteMetaHeuristics,
        CellsWithoutMask,
        RowsPermutations,
        RandomRowsPermutations,
        RowsWithoutMask,
    }


    /// <summary>
    /// Sample controller for solving Sudokus.
    /// Includes 4 default games, and allows for loading additional ones from a file, supporting most file formats.
    /// Includes 2 different types of chromosome. One trivial with 81 genes each cell digit, and one with 9 genes for row permutations taking into account the original mask
    /// </summary>
    [DisplayName("Sudoku")]
    public class SudokuSampleController : SampleControllerBase
    {

        public SudokuSampleController()
        {
            // Super easy - Population 250 - generation 16 - 0.2s
            _sudokuList.Add(SudokuBoard.Parse("9.2..54.31...63.255.84.7.6..263.9..1.57.1.29..9.67.53.24.53.6..7.52..3.4.8..4195."));
            //Easy - Population 5000 - generation 24 - 10s
            _sudokuList.Add(SudokuBoard.Parse("..48...1767.9.....5.8.3...43..74.1...69...78...1.69..51...8.3.6.....6.9124...15.."));
            //Medium - Population 100000 - generation 30  - 10mn
            _sudokuList.Add(SudokuBoard.Parse("..6.......8..542...4..9..7...79..3......8.4..6.....1..2.3.67981...5...4.478319562"));
            // Hard - Population 300000 - generation 37 - 1h30mn
            _sudokuList.Add(SudokuBoard.Parse("....9.4.8.....2.7..1.7....32.4..156...........952..7.19....5.1..3.4.....1.2.7...."));

        }


        private SudokuChromosomeType _ChromosomeType = SudokuChromosomeType.RowsPermutations; // The selected type of chromosome
        private int _nbPermutations = 2; //The number of genes per permutation for random permutations
        private int _nbSudokus = 5; //The number of Sudokus to generate from random permutations
        private HBox _nbPermsHBox;
        private bool _multipleChromosome; // Do we evolve several sudokus/sub-chromosomes per individual solution
        private int _nbChromosomes = 2; //Nb of sudokus per individual if multiple
        private HBox _nbChromosomesHBox;





        private IList<SudokuBoard> _sudokuList = new List<SudokuBoard>();
        private int _sudokuIndex;



        private SudokuBoard GetTargetSudoku()
        {
            return _sudokuList[_sudokuIndex];
        }


        public override IChromosome CreateChromosome()
        {
            return CreateChromosome(_multipleChromosome, _nbChromosomes);
        }

        private IChromosome CreateChromosome(bool multi, int nbChromosomes = 5)
        {
            if (!multi)
            {
                switch (_ChromosomeType)
                {
                    case SudokuChromosomeType.RandomRowsPermutations:
                        return new SudokuRandomPermutationsChromosome(GetTargetSudoku(), _nbPermutations, _nbSudokus);
                    case SudokuChromosomeType.RowsPermutations:
                        return new SudokuPermutationsChromosome(GetTargetSudoku());
                    case SudokuChromosomeType.RowsWithoutMask:
                        return new SudokuPermutationsChromosome();
                    case SudokuChromosomeType.Cells:
                        return new SudokuCellsChromosome(GetTargetSudoku(), false);
                    case SudokuChromosomeType.CellsInitWithRowsPermutations:
                        return new SudokuCellsChromosome(GetTargetSudoku(), true);
                    case SudokuChromosomeType.CellsWithEukaryoteMetaHeuristics:
                        return new SudokuCellsChromosome(GetTargetSudoku(), true);
                    case SudokuChromosomeType.CellsWithoutMask:
                        return new SudokuCellsChromosome();
                }
            }
            else
            {
                return new MultipleChromosome(i => CreateChromosome(false), nbChromosomes);
            }


            return null;

        }



        public override Widget CreateConfigWidget()
        {
            var container = new VBox();




            var fileHBox = new HBox();
            container.Add(fileHBox);
            Box.BoxChild wfileHBox = (Box.BoxChild)container[fileHBox];
            wfileHBox.Expand = false;
            wfileHBox.Fill = false;


            // Sudoku index.
            var indexHBox = new HBox();
            fileHBox.Add(indexHBox);
            var indexLabel = new Label { Text = "Sudoku index" };
            indexHBox.Add(indexLabel);

            var indexButton = new SpinButton(1, _sudokuList.Count, 1) {Value = 1};
            indexButton.ValueChanged += delegate
            {
                _sudokuIndex = (int)indexButton.Value - 1;

                OnReconfigured();
            };
            indexHBox.Add(indexButton);

            // File support

            var selectImageButton = new Button { Label = "Load sudoku(s) file" };
            selectImageButton.Clicked += delegate
            {
                FileChooserDialog filechooser =
            new FileChooserDialog(
              "Select the sudoku to use",
              Context.GtkWindow,
              FileChooserAction.Open,
              "Cancel",
              ResponseType.Cancel,
              "Open",
              ResponseType.Accept);

                if (filechooser.Run() == (int)ResponseType.Accept)
                {
                    _sudokuList = SudokuBoard.ParseFile(filechooser.Filename);
                    indexButton.SetRange(1, _sudokuList.Count);
                }

                filechooser.Destroy();

                OnReconfigured();
            };
            fileHBox.Add(selectImageButton);
            var helpImageButton = new Button { Label = "?" };
            
            helpImageButton.Clicked += delegate
            {
                var msg = new MessageDialog(
                    Context.GtkWindow,
                    DialogFlags.Modal,
                    MessageType.Info,
                    ButtonsType.Ok,
                    "Accepted formats represent Sudokus on one or several lines," +
                    "\n with characters '.', '-', or 'X' for empty cells and digits otherwise." +
                    "\n Lines starting with other characters are ignored such as '#' for comments on the common sdk format.");
                msg.Run();

                msg.Destroy();
            };
            fileHBox.Add(helpImageButton);



            // Genetics selector.

            var geneticsHBox = new HBox();

            geneticsHBox.Spacing += 2;

            var geneticsLabel = new Label { Text = "Genetics" };
            geneticsHBox.Add(geneticsLabel);

            var chromosomeTypes = new[] {
        nameof(SudokuChromosomeType.RowsPermutations)
        ,nameof(SudokuChromosomeType.Cells)
        ,nameof(SudokuChromosomeType.CellsInitWithRowsPermutations)
        ,nameof(SudokuChromosomeType.CellsWithEukaryoteMetaHeuristics)
        ,nameof(SudokuChromosomeType.RandomRowsPermutations)
        ,nameof(SudokuChromosomeType.RowsWithoutMask)
        ,nameof(SudokuChromosomeType.CellsWithoutMask)
      };

            // Random Permutation chromosome: choosing Nb of permutations and Nb of Sudokus
            _nbPermsHBox = new HBox
            {
                Visible = _ChromosomeType == SudokuChromosomeType.RandomRowsPermutations
            };
            
            var nbPermsLabel = new Label { Text = "Nb Permutations" };
            _nbPermsHBox.Add(nbPermsLabel);

            var nbPermsButton = new SpinButton(1, 1000, 1);
            _nbPermsHBox.Add(nbPermsButton);
            nbPermsButton.Value = _nbPermutations;
            nbPermsButton.ValueChanged += delegate
            {
                _nbPermutations = (int)nbPermsButton.Value;

                OnReconfigured();
            };

            var nbSudokusLabel = new Label { Text = "Nb Sudokus" };
            _nbPermsHBox.Add(nbSudokusLabel);

            var nbSudokusButton = new SpinButton(1, 1000, 1);
            _nbPermsHBox.Add(nbSudokusButton);
            nbSudokusButton.Value = _nbSudokus;
            nbSudokusButton.ValueChanged += delegate
            {
                _nbSudokus = (int)nbSudokusButton.Value;

                OnReconfigured();
            };


            // Choosing Type of Sudoku Chromosome
            var selectorCombo = new ComboBox(chromosomeTypes) { Active = 0 };
            selectorCombo.Changed += delegate
            {
                Enum.TryParse( selectorCombo.ActiveText, out _ChromosomeType);
                _nbPermsHBox.Visible = _ChromosomeType == SudokuChromosomeType.RandomRowsPermutations;
                OnReconfigured();
            };
            geneticsHBox.Add(selectorCombo);
            container.Add(geneticsHBox);
            Box.BoxChild wgeneticsHBox = (Box.BoxChild)container[geneticsHBox];
            wgeneticsHBox.Expand = false;
            wgeneticsHBox.Fill = false;
            container.Add(_nbPermsHBox);
            Box.BoxChild wnbPermsHBox = (Box.BoxChild)container[_nbPermsHBox];
            wnbPermsHBox.Expand = false;
            wnbPermsHBox.Fill = false;

            //Multi-chromosome checkbox
            var multiHBox = new HBox();
            var multiCheck = new CheckButton("Multi-Solutions") {Active = _multipleChromosome};

            _nbChromosomesHBox = new HBox();
            _nbChromosomesHBox.Spacing += 2;
            _nbChromosomesHBox.Visible = _multipleChromosome;

            var nbChromosomesLabel = new Label { Text = "Nb Chrom." };
            _nbChromosomesHBox.Add(nbChromosomesLabel);

            var nbChromosomesButton = new SpinButton(1, 1000, 1);
            _nbChromosomesHBox.Add(nbChromosomesButton);
            nbChromosomesButton.Value = _nbChromosomes;
            nbChromosomesButton.ValueChanged += delegate
            {
                _nbChromosomes = (int)nbChromosomesButton.Value;

                OnReconfigured();
            };

            multiCheck.Toggled += delegate
            {
                _multipleChromosome = multiCheck.Active;
                _nbChromosomesHBox.Visible = _multipleChromosome;

                OnReconfigured();
            };

            multiHBox.Add(multiCheck);
            multiHBox.Add(_nbChromosomesHBox);

            container.Add(multiHBox);
            Box.BoxChild wmultiHBox = (Box.BoxChild)container[multiHBox];
            wmultiHBox.Expand = false;
            wmultiHBox.Fill = false;

            return container;
        }




        public override ICrossover CreateCrossover()
        {
            return new UniformCrossover();
        }

        public override IFitness CreateFitness()
        {
            return CreateFitness(_multipleChromosome);
        }


        private IFitness CreateFitness(bool multi)
        {
            if (multi)
            {
                return new MultipleFitness(CreateFitness(false));
            }

            if (_ChromosomeType == SudokuChromosomeType.RandomRowsPermutations)
            {
                return new SudokuFitness(GetTargetSudoku());
            }
            return new SudokuFitness(GetTargetSudoku());
        }

        public override IMutation CreateMutation()
        {
            return new UniformMutation();
        }

        public override ISelection CreateSelection()
        {
            return new EliteSelection();
        }

        public override void Draw()
        {
            var buffer = Context.Buffer;
            var layout = Context.Layout;
            var population = Context.Population;
            SudokuBoard sudokuBoardToDraw = null;
            if (population != null)
            {
                if (population.BestChromosome is ISudokuChromosome bestChromosome)
                {
                    if (population.CurrentGeneration != null)
                    {
                        var stats = population.CurrentGeneration.Chromosomes.GroupBy(c => c.Fitness).OrderByDescending(g => g.Key).Select(g => new { Fitness = g.Key, Count = g.Count(), First = ((ISudokuChromosome)g.First()).GetSudokus().First(), Last = ((ISudokuChromosome)g.Last()).GetSudokus().First() }).ToList();
                        Context.WriteText($"Fitness,Count:({stats[0].Fitness},{stats[0].Count})...({stats[stats.Count / 3].Fitness},{stats[stats.Count / 3].Count})...({stats[stats.Count * 2 / 3].Fitness},{stats[stats.Count * 2 / 3].Count})...({stats[stats.Count - 1].Fitness},{stats[stats.Count - 1].Count})");
                        Context.WriteText($"Top: [{string.Join(",", stats[0].First.Cells.Take(9).Select(i => i.ToString()).ToArray())}] [{string.Join(",", stats[0].Last.Cells.Take(9).Select(i => i.ToString()).ToArray())}]");
                        if (stats.Count > 1)
                        {
                            Context.WriteText($"Next: [{string.Join(",", stats[1].First.Cells.Take(9).Select(i => i.ToString()).ToArray())}] [{string.Join(",", stats[1].Last.Cells.Take(9).Select(i => i.ToString()).ToArray())}]");
                        }
                        

                    }
                    sudokuBoardToDraw = bestChromosome.GetSudokus().First();
                }
                else
                {
                    if (population.BestChromosome is MultipleChromosome multiChromosome)
                    {
                        var orderedSubChromosomes = multiChromosome.Chromosomes.OrderByDescending(c => c.Fitness).ToList();
                        bestChromosome = (ISudokuChromosome)orderedSubChromosomes.First();
                        var worstChromosome = (ISudokuChromosome)orderedSubChromosomes.Last();
                        sudokuBoardToDraw = bestChromosome.GetSudokus().First();
                        Context.WriteText($"Best Chromosome Best Sub-Fitness: {((IChromosome)bestChromosome).Fitness}");
                        Context.WriteText($"Best Chromosome Worst Sub-Fitness:: {((IChromosome)worstChromosome).Fitness}");
                    }

                }
            }
            else
            {
                sudokuBoardToDraw = GetTargetSudoku();
            }
            if (sudokuBoardToDraw != null)
            {
                layout.SetMarkup($"<span color='black'>{sudokuBoardToDraw}</span>");
                buffer.DrawLayout(Context.GC, 50, 120, layout);
            }
        }


        public override void Reset()
        {
            // Quick hack to force visibility not taken into account at creation (GTK#/MainWidow bug?)
            _nbPermsHBox.Visible = _ChromosomeType == SudokuChromosomeType.RandomRowsPermutations;
            _nbChromosomesHBox.Visible = _multipleChromosome;
        }

        public override void Update()
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// We handle the special case of using a <see cref="EukaryoteMetaHeuristic"/>, which makes use of <see cref="EukaryoteChromosome"/> to split the <see cref="SudokuCellsChromosome"/> into 9 smaller rows chromosomes.
        /// As demonstrated in <see href="https://www.researchgate.net/publication/224645744_Product_Geometric_Crossover_for_the_Sudoku_Puzzle">Product_Geometric_Crossover_for_the_Sudoku_Puzzle</see>, with rows permutations initialization this permits using ordered based crossovers and mutations preserving permutations, and in effect demonstrates better robustness together with those operators, in terms of preventing early collapse to a non solution, than simply using a cells based chromosome with same population size, initialization and non ordered based operators.     
        /// </summary>
        /// <param name="ga"></param>
        public override void ConfigGA(GeneticAlgorithm ga)
        {
            
            if (_ChromosomeType == SudokuChromosomeType.CellsWithEukaryoteMetaHeuristics)
            {
                if (ga is MetaGeneticAlgorithm mga)
                {
                    mga.Metaheuristic = new EukaryoteMetaHeuristic(9, 9, new DefaultMetaHeuristic()) { Scope = EvolutionStage.Crossover | EvolutionStage.Mutation };
                }
                else
                {
                    throw new InvalidOperationException("Metaheuristics should be enabled in order for the Eukaryote MetaHeuristic to be applied");
                }
                
            }
        }


        public override IGeometricConverter<object> GetGeometricConverters()
        {
            return new GeometricConverter<object> { DoubleToGeneConverter = DoubleToGene, GeneToDoubleConverter = GeneToDouble };
        }

        private object DoubleToGene(int geneIndex, double d)
        {
            switch (this._ChromosomeType)
            {
                case SudokuChromosomeType.Cells:
                case SudokuChromosomeType.CellsInitWithRowsPermutations:
                case SudokuChromosomeType.CellsWithEukaryoteMetaHeuristics:
                case SudokuChromosomeType.CellsWithoutMask:
                    return Math.Round(d-1).PositiveMod(9) + 1;
                case SudokuChromosomeType.RowsPermutations:
                case SudokuChromosomeType.RandomRowsPermutations:
                case SudokuChromosomeType.RowsWithoutMask:
                    return Math.Round(d)
                        .PositiveMod(this._sudokuList[_sudokuIndex].GetRowsPermutations()[geneIndex].Count);
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }

        private double GeneToDouble(int geneIndex, object geneValue) => Convert.ToDouble((int)geneValue);


    }
}
