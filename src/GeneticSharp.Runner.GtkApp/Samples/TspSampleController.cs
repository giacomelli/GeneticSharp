using System;
using System.ComponentModel;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Extensions.Tsp;
using Gtk;
using HelperSharp;

namespace GeneticSharp.Runner.GtkApp.Samples
{
    /// <summary>
    /// TSP (Travelling Salesman Problem) sample controller.
    /// </summary>
    [DisplayName("TSP")]
    public class TspSampleController : SampleControllerBase
    {
        #region Fields
        private TspFitness m_fitness;
        private int m_numberOfCities = 50;
        private bool m_showIndexes = true;
        private TspChromosome m_bestChromosome;
        #endregion

        #region Methods
        /// <summary>
        /// Creates the config widget.
        /// </summary>
        /// <returns>The config widget.</returns>
        public override Gtk.Widget CreateConfigWidget()
        {
            var container = new VBox();
            var citiesNumber = new SpinButton(2, 10000, 2);
            citiesNumber.Text = "Number of cities";
            citiesNumber.Value = m_numberOfCities;
            citiesNumber.ValueChanged += delegate
            {
                m_numberOfCities = citiesNumber.ValueAsInt - (citiesNumber.ValueAsInt % 2);
                citiesNumber.Value = m_numberOfCities;
                OnReconfigured();
            };
            container.Add(citiesNumber);

            var generateButton = new Button();
            generateButton.Label = "Generate cities";
            generateButton.Clicked += delegate
            {
                m_numberOfCities = citiesNumber.ValueAsInt;
                OnReconfigured();
            };
            container.Add(generateButton);

            var showIndexes = new CheckButton();
            showIndexes.Active = m_showIndexes;
            showIndexes.Label = "Show indexes";
            showIndexes.Toggled += delegate
            {
                m_showIndexes = showIndexes.Active;
            };

            container.Add(showIndexes);

            return container;
        }

        /// <summary>
        /// Creates the fitness.
        /// </summary>
        /// <returns>The fitness.</returns>
        public override IFitness CreateFitness()
        {
            var r = Context.DrawingArea;
            m_fitness = new TspFitness(m_numberOfCities, r.Left, r.Right, r.Top, r.Bottom);

            return m_fitness;
        }

        /// <summary>
        /// Creates the chromosome.
        /// </summary>
        /// <returns>The chromosome.</returns>
        public override IChromosome CreateChromosome()
        {
            return new TspChromosome(m_numberOfCities);
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

        /// <summary>
        /// Resets the sample.
        /// </summary>
        public override void Reset()
        {
            m_bestChromosome = null;
        }

        /// <summary>
        /// Updates the sample.
        /// </summary>
        public override void Update()
        {
            var population = Context.Population;

            if (population != null && population.CurrentGeneration != null)
            {
                m_bestChromosome = population.BestChromosome as TspChromosome;
            }
        }

        /// <summary>
        /// Draws the sample.
        /// </summary>
        public override void Draw()
        {
            var buffer = Context.Buffer;
            var gc = Context.GC;
            var layout = Context.Layout;

            // Draw cities.
            foreach (var c in m_fitness.Cities)
            {
                buffer.DrawRectangle(gc, true, c.X - 2, c.Y - 2, 4, 4);
            }

            if (m_bestChromosome != null)
            {
                var genes = m_bestChromosome.GetGenes();

                for (int i = 0; i < genes.Length; i += 2)
                {
                    var cityOneIndex = Convert.ToInt32(genes[i].Value);
                    var cityTwoIndex = Convert.ToInt32(genes[i + 1].Value);
                    var cityOne = m_fitness.Cities[cityOneIndex];
                    var cityTwo = m_fitness.Cities[cityTwoIndex];

                    if (i > 0)
                    {
                        var previousCity = m_fitness.Cities[Convert.ToInt32(genes[i - 1].Value)];
                        buffer.DrawLine(gc, previousCity.X, previousCity.Y, cityOne.X, cityOne.Y);
                    }

                    buffer.DrawLine(gc, cityOne.X, cityOne.Y, cityTwo.X, cityTwo.Y);

                    if (m_showIndexes)
                    {
                        layout.SetMarkup("<span color='black'>{0}</span>".With(i));
                        buffer.DrawLayout(gc, cityOne.X, cityOne.Y, layout);

                        layout.SetMarkup("<span color='black'>{0}</span>".With(i + 1));
                        buffer.DrawLayout(gc, cityTwo.X, cityTwo.Y, layout);
                    }
                }

                var lastCity = m_fitness.Cities[Convert.ToInt32(genes[genes.Length - 1].Value)];
                var firstCity = m_fitness.Cities[Convert.ToInt32(genes[0].Value)];
                buffer.DrawLine(gc, lastCity.X, lastCity.Y, firstCity.X, firstCity.Y);

                Context.WriteText("Distance: {0:n2}", m_bestChromosome.Distance);
            }
        }
        #endregion
    }
}
