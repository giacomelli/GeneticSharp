using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Extensions.Tsp;
using HelperSharp;

namespace GeneticSharp.Runner.WinApp
{
    public partial class MainForm : Form
    {
        #region Fields
        private Graphics m_graphics;
        private Population m_population;
        private TspFitness m_fitness;
        #endregion

        #region Constructors
        public MainForm()
        {
            InitializeComponent();
            m_graphics = pcbResult.CreateGraphics();

            int numberOfCities = 12;
            var selection = new EliteSelection();
            //var crossover = new OnePointCrossover(5);
            //var crossover = new TwoPointCrossover(3, 8);
            var crossover = new UniformCrossover(0.5f);
            var mutation = new UniformMutation(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11);
            var chromosome = new TspChromosome(numberOfCities);
            m_fitness = new TspFitness(numberOfCities, pcbResult.Width - 10, pcbResult.Height - 10);

            m_population = new Population(
                40,
                40,
                chromosome,
                m_fitness,
                selection, crossover, mutation);

            m_population.GenerationRan += delegate {
                UpdateMap();
            };
        }
        #endregion

        private void btnRunGeneration_Click(object sender, EventArgs e)
        {          
            m_population.RunGeneration();           
        }

        private void btnRunGenerations_Click(object sender, EventArgs e)
        {
            m_population.RunGenerations(1000);
        }

        private void UpdateMap()
        {            
            lblGeneration.Text = "Generation: {0}".With(m_population.CurrentGeneration.Number);

            m_graphics.Clear(Color.White);
            foreach (var c in m_fitness.Cities)
            {
                m_graphics.FillRectangle(Brushes.Red, c.Location.X - 2, c.Location.Y - 2, 4, 4);
            }

            var genes = m_population.BestChromosome.GetGenes();

            for (int i = 0; i < genes.Count; i += 2)
            {
                var cityOneIndex = Convert.ToInt32(genes[i].Value);
                var cityTwoIndex = Convert.ToInt32(genes[i + 1].Value);
                var cityOne = m_fitness.Cities[cityOneIndex];
                var cityTwo = m_fitness.Cities[cityTwoIndex];

                if (i > 0)
                {
                    var previousCity = m_fitness.Cities[Convert.ToInt32(genes[i - 1].Value)];
                    m_graphics.DrawLine(Pens.Black, previousCity.Location, cityOne.Location);
                }

                m_graphics.DrawLine(Pens.Black, cityOne.Location, cityTwo.Location);
            }

            m_graphics.DrawLine(Pens.Black, m_fitness.Cities[Convert.ToInt32(genes[genes.Count - 1].Value)].Location, m_fitness.Cities[Convert.ToInt32(genes[0].Value)].Location);

            Update();
        }
    }
}
