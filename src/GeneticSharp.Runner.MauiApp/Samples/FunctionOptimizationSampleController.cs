//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using GeneticSharp.Runner.MauiApp.Samples;

//namespace GeneticSharp.Runner.MauiApp
//{
//    [DisplayName("Function optimization")]
//    public class FunctionOptimizationSampleController : SampleControllerBase
//    {
//        private FloatingPointChromosome _bestChromosome;
//        private List<KeyValuePair<double, double[]>> _positions;

//        #region implemented abstract members of SampleControllerBase
//        public override Gtk.Widget CreateConfigWidget()
//        {
//            var container = new VBox();

//            return container;
//        }

//        public override IFitness CreateFitness()
//        {
//            return new FuncFitness((chromosome) =>
//            {
//                var c = chromosome as FloatingPointChromosome;

//                var values = c.ToFloatingPoints();
//                var x1 = values[0];
//                var y1 = values[1];
//                var x2 = values[2];
//                var y2 = values[3];

//                // Euclidean distance: https://en.wikipedia.org/wiki/Euclidean_distance
//                return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
//            });
//        }

//        public override IChromosome CreateChromosome()
//        {
//            var w = Context.DrawingArea.Size.Width;
//            var h = Context.DrawingArea.Size.Height;

//            return new FloatingPointChromosome(
//                new double[] { 0, 0, 0, 0 },
//                new double[] { w, h, w, h },
//                new int[] { 16, 16, 16, 16 },
//                new int[] { 0, 0, 0, 0 });
//        }

//        public override ICrossover CreateCrossover()
//        {
//            return new UniformCrossover();
//        }

//        public override IMutation CreateMutation()
//        {
//            return new FlipBitMutation();
//        }

//        public override ISelection CreateSelection()
//        {
//            return new EliteSelection();
//        }

//        public override ITermination CreateTermination()
//        {
//            return new FitnessStagnationTermination(100);
//        }

//        public override void ConfigGA(GeneticAlgorithm ga)
//        {
//            var latestFitness = 0.0;

//            Context.GA.GenerationRan += (object sender, EventArgs e) =>
//            {
//                _bestChromosome = Context.GA.BestChromosome as FloatingPointChromosome;

//                if (_bestChromosome != null)
//                {
//                    lock (_positions)
//                    {
//                        var fitness = _bestChromosome.Fitness.Value;
//                        var points = _bestChromosome.ToFloatingPoints();

//                        _positions.Add(new KeyValuePair<double, double[]>(fitness, points));


//                        if (fitness != latestFitness)
//                        {
//                            latestFitness = fitness;
//                            var phenotype = _bestChromosome.ToFloatingPoints();

//                            Console.WriteLine(
//                                "Generation {0,2}: ({1},{2}),({3},{4}) = {5}",
//                                ga.GenerationsNumber,
//                                phenotype[0],
//                                phenotype[1],
//                                phenotype[2],
//                                phenotype[3],
//                                fitness
//                            );
//                        }
//                    }
//                }
//            };
//        }

//        public override void Reset()
//        {
//            _positions = new List<KeyValuePair<double, double[]>>();
//            _bestChromosome = null;
//        }

//        public override void Update()
//        {
//        }

//        public override void Draw()
//        {
//            lock (_positions)
//            {
//                Plot(_positions);
//            }
//        }

//        void Plot(List<KeyValuePair<double, double[]>> positions)
//        {
//            var buffer = Context.Buffer;
//            var gc = Context.GC;
//            var layout = Context.Layout;
//            var plotMargin = 10;
//            var plotMinX = plotMargin;
//            var plotMinY = 60;

//            var width = Context.DrawingArea.Width;
//            var height = Context.DrawingArea.Height;
//            var plotWidth = width - (plotMinX + plotMargin);
//            var plotHeight = height - (plotMargin);

//            // Draw the rectangle area.
//            buffer.DrawRectangle(gc, false, plotMinX, plotMinY, plotWidth, plotHeight);
//            layout.SetMarkup("<span color='black'>{0}, {1}</span>".With(width, height));
//            buffer.DrawLayout(gc, plotWidth - plotMinX - 100, plotMinY + plotHeight, layout);

//            if (Context.GA == null || positions.Count == 0)
//            {
//                return;
//            }

//            var points = new List<Gdk.Point>();
//            var maxX1 = positions.Max(p => p.Value[0]);
//            var maxY1 = positions.Max(p => p.Value[1]);
//            var maxX2 = positions.Max(p => p.Value[2]);
//            var maxY2 = positions.Max(p => p.Value[3]);

//            for (int i = 0; i < positions.Count; i++)
//            {
//                var p = positions[i];

//                var x1 = plotMinX + Convert.ToInt32((plotWidth * p.Value[0]) / maxX1);
//                var y1 = plotMinY + Convert.ToInt32((plotHeight * p.Value[1]) / maxY1);
//                var point = new Gdk.Point(x1, y1);
//                buffer.DrawRectangle(gc, true, point.X, point.Y, 1, 1);
//                points.Add(point);

//                var x2 = plotMinX + Convert.ToInt32((plotWidth * p.Value[2]) / maxX2);
//                var y2 = plotMinY + Convert.ToInt32((plotHeight * p.Value[3]) / maxY2);
//                point = new Gdk.Point(x2, y2);
//                buffer.DrawRectangle(gc, true, point.X, point.Y, 1, 1);
//                points.Add(point);
//            }

//            // Draw all lines (black).
//            gc.RgbFgColor = new Gdk.Color(0, 0, 0);
//            gc.SetLineAttributes(1, Gdk.LineStyle.OnOffDash, Gdk.CapStyle.Butt, Gdk.JoinStyle.Round);
//            buffer.DrawLines(gc, points.ToArray());

//            // Draw the latest best chromossome line in highlight (red).
//            gc.RgbFgColor = new Gdk.Color(255, 0, 0);
//            gc.SetLineAttributes(2, Gdk.LineStyle.Solid, Gdk.CapStyle.Butt, Gdk.JoinStyle.Round);
//            buffer.DrawLines(gc, points.Skip(points.Count - 2).ToArray());
//        }

//        #endregion


//    }
//}