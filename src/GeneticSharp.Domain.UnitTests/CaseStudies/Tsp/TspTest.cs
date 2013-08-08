using NUnit.Framework;
using System;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using System.Drawing;
using System.Drawing.Imaging;

namespace GeneticSharp.Domain.UnitTests
{
	[TestFixture()]
	public class TspTest
	{
		[Test()]
		public void RunGeneration_Generations_Optimization ()
		{
			int numberOfCities = 6;
			var selection = new EliteSelection();
            var crossover = new UniformCrossover(0.5f); //new TwoPointCrossover(2, 6);
			var mutation = new UniformMutation();
			var chromosome = new TspChromosome(numberOfCities);
			var fitness = new TspFitness ();

			var target = new Population (
				40, 
				40,
				chromosome,
				fitness, selection, crossover, mutation);

			target.RunGenerations (100);       

            using (var bitmap = new Bitmap(fitness.MaxX + 100, fitness.MaxY + 100))
            {
                var graphics = Graphics.FromImage(bitmap);

                foreach (var c in fitness.Cities)
                {
                    graphics.FillRectangle(Brushes.Red, c.Location.X, c.Location.Y, 5, 5);
                }

                var genes = target.BestChromosome.GetGenes();

                for(int i = 0; i < genes.Count; i += 2)
                {         
                    var cityOneIndex = Convert.ToInt32(genes[i].Value);
                    var cityTwoIndex = Convert.ToInt32(genes[i + 1].Value);
                    var cityOne = fitness.Cities[cityOneIndex];
                    var cityTwo = fitness.Cities[cityTwoIndex];

                    if (i > 0)
                    {
                        var previousCity = fitness.Cities[Convert.ToInt32(genes[i - 1].Value)];
                        graphics.DrawLine(Pens.Black, previousCity.Location, cityOne.Location);
                    }

                    graphics.DrawLine(Pens.Black, cityOne.Location, cityTwo.Location);
                }

                graphics.DrawLine(Pens.Black, fitness.Cities[Convert.ToInt32(genes[genes.Count - 1].Value)].Location, fitness.Cities[Convert.ToInt32(genes[0].Value)].Location);

                bitmap.Save("RunGeneration_Generations_Optimization.png", ImageFormat.Png);
            }


            var lastFitness = 0.0;

            foreach (var g in target.Generations)
            {
                Assert.GreaterOrEqual(g.BestChromosome.Fitness.Value, lastFitness);
                lastFitness = g.BestChromosome.Fitness.Value;
            }

            Assert.GreaterOrEqual(lastFitness, 0.9);     
		}
	}
}

