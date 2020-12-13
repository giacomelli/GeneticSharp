using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using Gdk;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Extensions.Mathematic;
using GeneticSharp.Extensions.Mathematic.Functions;
using GeneticSharp.Infrastructure.Framework.Commons;
using GeneticSharp.Runner.GtkApp.Samples;
using Gtk;
using Color = System.Drawing.Color;

namespace GeneticSharp.Runner.GtkApp
{

    /// <summary>
    /// Sample controller to visualize and explore fitness landscapes.  
    /// </summary>
    [DisplayName("Landscape Explorer")]
    public class LandscapeExplorerSampleController : SampleControllerBase
    {

        private IKnownFunction mTargetFunction;
        private ((double min, double max) xRange, (double min, double max) yRange) mRange = ((-10,10),(-10,10));
        //private double coordsRange = 10;

        private Dictionary<(int xDraw, int yDraw), double> _fValues = new Dictionary<(int xDraw, int yDraw), double>();
        private double _fMin;
        private double _fMax;
        //private byte[] _fBitmapBytes;
        private Bitmap _fBitmap;



        public override Widget CreateConfigWidget()
        {
            var container = new VBox();

            var functionHBox = new HBox();
            container.Add(functionHBox);

            var functionsLabel = new Label {Text = "Function to search"};
            functionHBox.Add(functionsLabel);
            var knownFunctions = KnownFunctions.GetKnownFunctions();
            var functionList = knownFunctions.Select(m => m.Key).ToArray();

            // Choosing function to display
            var functionCombo = new ComboBox(functionList) {Active = 0};
            mTargetFunction = knownFunctions[functionCombo.ActiveText];
            mRange = ((mTargetFunction.Ranges(2)[0], mTargetFunction.Ranges(2)[1]));
           
            functionHBox.Add(functionCombo);

            var rangeHBox = new HBox();
            container.Add(rangeHBox);
            var rangeLabel = new Label { Text = "Search Range" };
            rangeHBox.Add(rangeLabel);

            var rangeVBox = new VBox();
            rangeHBox.Add(rangeVBox);

            var xHBox = new HBox();
            rangeVBox.Add(xHBox);


            var xMinLabel = new Label { Text = "Xmin" };
            xHBox.Add(xMinLabel);
            var xMin = new SpinButton(-10000, 10000, 1) { Value = mTargetFunction.Ranges(2)[0].min, Digits = 2};
            xMin.ValueChanged += delegate
            {
                mRange.xRange.min = xMin.Value;
                _fBitmap = null;
                _fValues.Clear();
                OnReconfigured();
            };
            xHBox.Add(xMin);
            var xMaxLabel = new Label { Text = "Xmax" };
            xHBox.Add(xMaxLabel);
            var xMax = new SpinButton(-10000, 10000, 1) { Value = mTargetFunction.Ranges(2)[0].max, Digits = 2 };
            xMax.ValueChanged += delegate
            {
                mRange.xRange.max = xMax.Value;
                _fBitmap = null;
                _fValues.Clear();
                OnReconfigured();
            };
            xHBox.Add(xMax);

            var yHBox = new HBox();
            rangeVBox.Add(yHBox);

            var yMinLabel = new Label { Text = "Ymin" };
            yHBox.Add(yMinLabel);
            var yMin = new SpinButton(-10000, 10000, 1) { Value = mTargetFunction.Ranges(2)[1].min, Digits = 2 };
            yMin.ValueChanged += delegate
            {
                mRange.yRange.min = yMin.Value;
                _fBitmap = null;
                _fValues.Clear();
                OnReconfigured();
            };
            yHBox.Add(yMin);
            var yMaxLabel = new Label { Text = "Ymax" };
            yHBox.Add(yMaxLabel);
            var yMax = new SpinButton(-10000, 10000, 1) { Value = mTargetFunction.Ranges(2)[1].max, Digits = 2 };
            yMax.ValueChanged += delegate
            {
                mRange.yRange.max = yMax.Value;
                _fBitmap = null;
                _fValues.Clear();
                OnReconfigured();
            };
            yHBox.Add(yMax);

            functionCombo.Changed += delegate
            {
                //var method = knownFunctions[functionCombo.ActiveText];
                //mTargetFunction = doubles => (double) method.Invoke(null, new[] {doubles});
                mTargetFunction = knownFunctions[functionCombo.ActiveText];
                //mRange = ((mTargetFunction.Ranges(2)[0], mTargetFunction.Ranges(2)[1]));
                xMin.Value = mTargetFunction.Ranges(2)[0].min;
                xMax.Value = mTargetFunction.Ranges(2)[0].max;
                yMin.Value = mTargetFunction.Ranges(2)[1].min;
                yMax.Value = mTargetFunction.Ranges(2)[1].max;
                xMin.Update(); 
                xMax.Update();
                yMin.Update();
                yMax.Update();
                _fValues.Clear();
                _fBitmap = null;
                OnReconfigured();
            };

            return container;
        }

        public override IFitness CreateFitness()
        {
            var toReturn = new FunctionFitness<double>(genes =>
                mTargetFunction.Fitness(mTargetFunction.Function(genes.Select(g => (double) g.Value).ToArray())));
            return toReturn;
        }

        public override IChromosome CreateChromosome()
        {
            var toReturn = new EquationChromosome<double>( 2) {
                Ranges = new List<(double min, double max)>(new [] {
                    (mRange.xRange.min, mRange.xRange.max),
                    (mRange.yRange.min,mRange.yRange.max)
                })};
            return toReturn;
        }

        public override ICrossover CreateCrossover()
        {
            return new UniformCrossover();
        }

        public override IMutation CreateMutation()
        {
            return new UniformMutation();
        }

        public override ISelection CreateSelection()
        {
            return new EliteSelection();
        }

        public override void Reset()
        {
            //throw new System.NotImplementedException();
        }

        public override void Update()
        {
            //throw new System.NotImplementedException();
        }

        public override void Draw()
        {
            try
            {
                PlotFunction();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        /// <summary>
        /// Computes the currently selected function value for every pixel to draw
        /// </summary>
        private void ComputeFunctionValues()
        {
            var fValues = new Dictionary<(int xDraw, int yDraw), double>();
            var r = Context.DrawingArea;
            var width = Context.DrawingArea.Width; 
            var height = Context.DrawingArea.Height;
            var fMin = double.MaxValue;
            var fMax = double.MinValue;

            for (int xDraw = 0; xDraw < width; xDraw++)
            {
                for (int yDraw = 0; yDraw < height; yDraw++)
                {
                    var (x, y) = GetRealCoords(xDraw, yDraw);
                    var fValue = mTargetFunction.Fitness(mTargetFunction.Function(new[] { x, y }));
                    fValues.Add((xDraw, yDraw), fValue);
                    if (fValue > fMax)
                    {
                        fMax = fValue;
                    }
                    if (fValue < fMin)
                    {
                        fMin = fValue;
                    }
                }
            }

            if (fMax<=fMin)
            {
                throw new InvalidOperationException("function has FMax <= FMin");
            }
            _fValues = fValues;
            _fMin = fMin;
            _fMax = fMax;
        }


        private void PlotFunction()
        {
             
            if (mTargetFunction!=null)
            {
                if (_fValues.Count == 0)
                {
                    ComputeFunctionValues();
                }
                if (_fBitmap == null)
                {
                    _fBitmap = BuildBitmap();
                }
                var  displayBitmap = _fBitmap;
                if (Context.GA != null)
                {
                    displayBitmap = (Bitmap)displayBitmap.Clone();
                    PlotPopulation(displayBitmap);
                }
                
                var converter = new ImageConverter();
                var imageBytes = (byte[]) converter.ConvertTo(displayBitmap, typeof(byte[]));
                var pb = new Pixbuf(imageBytes);
                var buffer = Context.Buffer;
                var gc = Context.GC;
                var width = Context.DrawingArea.Width;
                var height = Context.DrawingArea.Height;

                buffer.DrawPixbuf(gc, pb, 0, 0, 0, 100, width, height, RgbDither.None, 0, 0);
               
            }
        }


        private Color indColor = GetColorFromHSV(0.75);

        private void PlotPopulation(Bitmap image)
        {
            foreach (var chromosome in Context.GA.Population.CurrentGeneration.Chromosomes)
            {
                var equChromosome = (EquationChromosome<double>)chromosome;
                (double x, double y) = ((double)equChromosome.GetGene(0).Value,
                    (double)equChromosome.GetGene(1).Value);
                var cFitness = Context.GA.Fitness.Evaluate(chromosome);
                var position = GetDrawingCoords(x, y);
                for (int i = -2; i < 3; i++)
                {
                    for (int j = -2; j < 3; j++)
                    {

                        var xDrawi = position.xDraw + i;
                        var yDrawj = position.yDraw + j;
                        if ((Math.Abs(i)+Math.Abs(j)) < 4 && xDrawi>=0 && xDrawi<image.Width && yDrawj>=0 && yDrawj<image.Height)
                        {
                            image.SetPixel(xDrawi, yDrawj, indColor);
                        }
                    }
                }
                
            }
        }



        private Color GetColor(double fValue, double fMin, double fMax)
        {
            var ratio = 0.5 - ((fValue - fMin) / (2*(fMax - fMin)));
            //var scaledRatio = 2 - (2 / (1 + ratio));
            //var hue = Convert.ToInt32(Math.Round(ratio * 360));
            var hue = ratio;
            return GetColorFromHSV(hue);
        }

        private static Color GetColorFromHSV(double hue, double sat = 1.0, double val =1.0)
        {
            HSVToRGB(hue, sat, val, out var dr, out var dg, out var db);
            return Color.FromArgb((int)(dr * 255), (int)(dg * 255), (int)(db * 255));
        }



        /// <summary>
        /// Builds the bitmap from genes.
        /// </summary>
        /// <returns>The bitmap.</returns>
        public Bitmap BuildBitmap()
        {
            var width = Context.DrawingArea.Width; 
            var height = Context.DrawingArea.Height;
            var result = new Bitmap(width, height);

            foreach (var pixelValue in _fValues)
            {
                var color = GetColor(pixelValue.Value, _fMin, _fMax);
                result.SetPixel(pixelValue.Key.xDraw, pixelValue.Key.yDraw, color );
            }
            return result;
        }

       



        /// <summary>
        /// Gets the draw coordinates from the 
        /// </summary>
        private (int xDraw, int yDraw) GetDrawingCoords(double x, double y)
        {
            var xDraw = Convert.ToInt32(Math.Round((x - mRange.xRange.min) / (mRange.xRange.max - mRange.xRange.min) * Context.DrawingArea.Width));
            var yDraw = Convert.ToInt32(Math.Round((y - mRange.yRange.min) / (mRange.yRange.max - mRange.yRange.min) * Context.DrawingArea.Height));
            return (xDraw, yDraw);
        }

        /// <summary>
        /// Gets the draw coordinates from the 
        /// </summary>
        private (double x, double y) GetRealCoords(int xDraw, int yDraw)
        {
            double xFactor = xDraw / (double)Context.DrawingArea.Width;
            var x = mRange.xRange.min + xFactor * (mRange.xRange.max - mRange.xRange.min);
            double yFactor = yDraw / (double)Context.DrawingArea.Height;
            var y = mRange.yRange.min + yFactor * (mRange.yRange.max - mRange.yRange.min);
            return (x, y);
        }


        /// <summary>
        /// Draws an image in the gdk environment
        /// </summary>
        /// <param name="bitmap"></param>
        private void DrawBitmap(Bitmap bitmap)
        {

            var buffer = Context.Buffer;
            var gc = Context.GC;
            var converter = new ImageConverter();

            var imageBytes = (byte[])converter.ConvertTo(bitmap, typeof(byte[]));
            var pb = new Gdk.Pixbuf(imageBytes);
            var width = Context.DrawingArea.Width;
            var height = Context.DrawingArea.Height;

            pb = pb.ScaleSimple(width, height, Gdk.InterpType.Nearest);
            buffer.DrawPixbuf(gc, pb, 0, 0, 0, 100, width, height, Gdk.RgbDither.None, 0, 0);

        }


        /// <summary>
        /// Helper to build a RGB color from HSV definition
        /// </summary>
        /// <param name="h">Hue</param>
        /// <param name="s">Saturation</param>
        /// <param name="v"Value></param>
        /// <param name="r">Red</param>
        /// <param name="g">Green</param>
        /// <param name="b">Blue</param>
        public static void HSVToRGB(double h, double s, double v, out double r, out double g, out double b)
        {
            if (Math.Abs(h - 1.0) <= double.Epsilon)
            {
                h = 0.0;
            }

            double step = 1.0 / 6.0;
            double vh = h / step;

            int i = (int) Math.Floor(vh);

            double f = vh - i;
            double p = v * (1.0 - s);
            double q = v * (1.0 - s * f);
            double t = v * (1.0 - s * (1.0 - f));

            switch (i)
            {
                case 0:
                {
                    r = v;
                    g = t;
                    b = p;
                    break;
                }
                case 1:
                {
                    r = q;
                    g = v;
                    b = p;
                    break;
                }
                case 2:
                {
                    r = p;
                    g = v;
                    b = t;
                    break;
                }
                case 3:
                {
                    r = p;
                    g = q;
                    b = v;
                    break;
                }
                case 4:
                {
                    r = t;
                    g = p;
                    b = v;
                    break;
                }
                case 5:
                {
                    r = v;
                    g = p;
                    b = q;
                    break;
                }
                default:
                {
                    // not possible - if we get here it is an internal error
                    throw new ArgumentException();
                }
            }
        }


    }
}