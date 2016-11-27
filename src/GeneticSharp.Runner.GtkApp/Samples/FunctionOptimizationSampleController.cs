using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Runner.GtkApp.Samples;
using Gtk;
using HelperSharp;

namespace GeneticSharp.Runner.GtkApp
{
	[DisplayName("Function optimization")]
	public class FunctionOptimizationSampleController : SampleControllerBase
	{
		private FloatingPointChromosome m_bestChromosome;
		private List<KeyValuePair<double, double>> m_positions;
		private ComboBox m_cmbPlotType;  		#region implemented abstract members of SampleControllerBase  		/// <summary> 		/// Creates the config widget. 		/// </summary> 		/// <returns>The config widget.</returns> 		public override Gtk.Widget CreateConfigWidget() 		{
			var container = new VBox(); 			var label = new Label("Plot"); 			container.Add(label);  			m_cmbPlotType = new ComboBox(new string[] { "generation, fitness", "x, y" }); 			m_cmbPlotType.Active = 0; 			container.Add(m_cmbPlotType);  			return container; 		} 

		public override IFitness CreateFitness()
		{
			return new FuncFitness((chromosome) =>
			{
				var c = chromosome as FloatingPointChromosome;
				var x = c.ToFloatingPoint();

				// GRAMACY & LEE (2012) FUNCTION
				// http://www.sfu.ca/~ssurjano/grlee12.html
				return (Math.Sin(10.0 * Math.PI * x) / (2.0 * x)) + Math.Pow((x - 1.0), 4);
			});
		}

		public override IChromosome CreateChromosome()
		{
			return new FloatingPointChromosome(0.5, 2.5, 2);
		}

		public override ICrossover CreateCrossover()
		{
			return new ThreeParentCrossover();
		}

		public override IMutation CreateMutation()
		{
			return new FlipBitMutation();
		}

		public override ISelection CreateSelection()
		{
			return new EliteSelection();
		}

		public override void ConfigGA(GeneticSharp.Domain.GeneticAlgorithm ga)
		{
			Context.GA.GenerationRan += (object sender, EventArgs e) =>
			{
				m_bestChromosome = Context.GA.BestChromosome as FloatingPointChromosome;

				if (m_bestChromosome != null)
				{
					lock (m_positions)
					{
						var x = m_bestChromosome.ToFloatingPoint();
						var y = m_bestChromosome.Fitness.Value;

						m_positions.Add(new KeyValuePair<double, double>(x, y));
						Console.WriteLine("{0}\t{1}", x, y);
					}
				}
			};
		}

		public override void Reset()
		{
			m_positions = new List<KeyValuePair<double, double>>();
			m_bestChromosome = null;
		}

		public override void Update()
		{
		}

		public override void Draw()
		{
			if (Context.GA == null || m_positions.Count == 0)
			{
				return;
			}

			lock (m_positions)
			{
				Plot(m_positions);
			}
		}

		void Plot(List<KeyValuePair<double, double>> positions)
		{
			var buffer = Context.Buffer;
			var gc = Context.GC;
			var layout = Context.Layout;
			var plotMargin = 20;
			var plotMinX = plotMargin;
			var plotMinY = -50;
			var plotWidth = Context.DrawingArea.Width - (plotMinX + plotMargin);
			var plotHeight = Context.DrawingArea.Height - (plotMinY + plotMargin);
			var isXYPlot = m_cmbPlotType.Active == 1;

			var maxX = isXYPlot ? positions.Max(p => p.Key) : Context.GA.GenerationsNumber;
			var maxY = positions.Max(p => p.Value);

			var generationsNumber = Context.GA.GenerationsNumber;

			var points = new List<Gdk.Point>();

			for (int i = 0; i < positions.Count; i++)
			{
				var p = positions[i];
				var posX = isXYPlot ? p.Key : i;
				var x = plotMinX + Convert.ToInt32((plotWidth * posX) / maxX);
				var y = plotHeight - (plotMinY + Convert.ToInt32((plotHeight * p.Value) / maxY));
				var point = new Gdk.Point(x, y);

				buffer.DrawRectangle(gc, true, point.X, point.Y, 1, 1);
				points.Add(point);
			}

			buffer.DrawLines(gc, points.ToArray());
		}

		#endregion


	}
}