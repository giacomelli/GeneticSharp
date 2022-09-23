using System.ComponentModel;
using GeneticSharp.Extensions;

namespace GeneticSharp.Runner.MauiApp.Samples
{
    /// <summary>
    /// TSP (Travelling Salesman Problem) sample controller.
    /// </summary>
    [DisplayName("TSP")]
    public class TspSampleController : SampleControllerBase
    {
        private TspFitness _fitness;
        private int _numberOfCities = 50; 
        private bool _showIndexes = true;
        private TspChromosome _bestChromosome;
        public override IView CreateConfigView()
        {
            var view = new Grid();
            view.AddColumnDefinition(new ColumnDefinition(GridLength.Star));
            view.AddColumnDefinition(new ColumnDefinition(GridLength.Star));
            view.AddRowDefinition(new RowDefinition(GridLength.Auto));
            view.AddRowDefinition(new RowDefinition(GridLength.Auto));
            view.AddRowDefinition(new RowDefinition(GridLength.Auto));
            view.AddRowDefinition(new RowDefinition(GridLength.Auto));

            var citiesNumberLabel = new Label();
            var citiesNumber = new Slider(2, 1000, 2);                       
            citiesNumber.ValueChanged += (object sender, ValueChangedEventArgs e) =>
            {
                _numberOfCities = Convert.ToInt32(e.NewValue);
                citiesNumber.Value = _numberOfCities;
                citiesNumberLabel.Text = $"Number of cities: {citiesNumber.Value}";
                OnReconfigured();
            };

            citiesNumber.Value = _numberOfCities;
            Grid.SetColumnSpan(citiesNumber, 2);

            var generateButton = new Button { Text = "Generate cities" };
            generateButton.Clicked += (e, args) =>
            {
                OnReconfigured();
            };
            Grid.SetColumnSpan(generateButton, 2);

            var showIndexesLabel = new Label { Text = "Show indexes" };
            var showIndexes = new CheckBox
            {
                IsChecked = _showIndexes
            };
            showIndexes.CheckedChanged += (e, args) =>
            {
                _showIndexes = args.Value;
            };


            view.Add(citiesNumberLabel, 0, 0);
            view.Add(citiesNumber, 0, 1);
            view.Add(generateButton, 0, 2);
            view.Add(showIndexesLabel, 0, 3);
            view.Add(showIndexes, 1, 3);
           

            return view;
      
        }

        /// <summary>
        /// Creates the fitness.
        /// </summary>
        /// <returns>The fitness.</returns>
        public override IFitness CreateFitness()
        {
            var r = Context.DrawingArea;
            _fitness = new TspFitness(_numberOfCities, 0, r.Width, 0, r.Height);

            return _fitness;
        }

        /// <summary>
        /// Creates the chromosome.
        /// </summary>
        /// <returns>The chromosome.</returns>
        public override IChromosome CreateChromosome()
        {
            return new TspChromosome(_numberOfCities);
        }

        public override ICrossover CreateCrossover()
        {
            return new OrderedCrossover();
        }

        public override IMutation CreateMutation()
        {
            return new ReverseSequenceMutation();
        }

        public override ISelection CreateSelection()
        {
            return new EliteSelection();
        }

        public override void ConfigGA(GeneticAlgorithm ga)
        {
            ga.TaskExecutor = new ParallelTaskExecutor
            {
                MinThreads = ga.Population.MinSize,
                MaxThreads = ga.Population.MaxSize
            };

            base.ConfigGA(ga);
        }
        /// <summary>
        /// Resets the sample.
        /// </summary>
        public override void Reset()
        {
            _bestChromosome = null;
        }

        /// <summary>
        /// Updates the sample.
        /// </summary>
        public override void Update()
        {
            var population = Context.Population;

            if (population != null && population.CurrentGeneration != null)
            {
                _bestChromosome = population.BestChromosome as TspChromosome;
            }
        }

        /// <summary>
        /// Draws the sample.
        /// </summary>
        public override void Draw(ICanvas canvas)
        {
            // Draw cities.
            foreach (var c in _fitness.Cities)
                canvas.DrawRectangle(c.X - 2, c.Y - 2, 4, 4);
            
            if (_bestChromosome != null)
            {
                var genes = _bestChromosome.GetGenes();

                for (int i = 0; i < genes.Length; i += 2)
                {
                    var cityOneIndex = Convert.ToInt32(genes[i].Value);
                    var cityTwoIndex = Convert.ToInt32(genes[i + 1].Value);
                    var cityOne = _fitness.Cities[cityOneIndex];
                    var cityTwo = _fitness.Cities[cityTwoIndex];

                    if (i > 0)
                    {
                        var previousCity = _fitness.Cities[Convert.ToInt32(genes[i - 1].Value)];
                        canvas.DrawLine(previousCity.X, previousCity.Y, cityOne.X, cityOne.Y);
                    }

                    canvas.DrawLine(cityOne.X, cityOne.Y, cityTwo.X, cityTwo.Y);

                    //if (_showIndexes)
                    //{
                    //    layout.SetMarkup("<span color='black'>{0}</span>".With(i));
                    //    buffer.DrawLayout(gc, cityOne.X, cityOne.Y, layout);

                    //    layout.SetMarkup("<span color='black'>{0}</span>".With(i + 1));
                    //    buffer.DrawLayout(gc, cityTwo.X, cityTwo.Y, layout);
                    //}
                }

                var lastCity = _fitness.Cities[Convert.ToInt32(genes[genes.Length - 1].Value)];
                var firstCity = _fitness.Cities[Convert.ToInt32(genes[0].Value)];
                canvas.DrawLine(lastCity.X, lastCity.Y, firstCity.X, firstCity.Y);

                //Context.WriteText("Distance: {0:n2}", _bestChromosome.Distance);
            }
        }        
    }
}
