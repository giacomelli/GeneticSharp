using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Extensions.Mathematic;
using GeneticSharp.Extensions.Mathematic.Functions;
using GeneticSharp.Infrastructure.Framework.Commons;
using GeneticSharp.Infrastructure.Framework.Images;
using GeneticSharp.Infrastructure.Framework.Threading;
using GeneticSharp.Runner.GtkApp.Samples;
using Gtk;
using Action = System.Action;
using Color = System.Drawing.Color;

namespace GeneticSharp.Runner.GtkApp
{

    public enum LandscapeMode
    {
        KnownFunction,
        KnownHeightMap,
        CustomImage
    }

    public enum KnownHeightMap
    {
        EverestMount,
        NepalBhoutan,
        TibetanPlateau,
        World
    }


    /// <summary>
    /// Sample controller to visualize and explore fitness landscapes.  
    /// </summary>
    [DisplayName("Landscape Explorer")]
    public class LandscapeExplorerSampleController : SampleControllerBase
    {
        private int mNbDimensions = 2;
        private int mNbSamples = 10;
        private readonly Color indColor = Color.BlueViolet;
        private readonly Color bestColor = Color.Aqua;
        private SpinButton _xMin;
        private SpinButton _xMax;
        private SpinButton _yMin;
        private SpinButton _yMax;

        private IKnownFunction mTargetFunction;
        private ((double min, double max) xRange, (double min, double max) yRange) mRange = ((-10,10),(-10,10));
        //private double coordsRange = 10;

        //private Dictionary<(int xDraw, int yDraw), double> _fValues = new Dictionary<(int xDraw, int yDraw), double>();
        private ((int xDraw, int yDraw) position, double fValue) _minPoint;
        private ((int xDraw, int yDraw) position, double fValue) _maxPoint;
        //private byte[] _fBitmapBytes;
        private DirectBitmap _fBitmap;
        private readonly ITaskExecutor _taskExecutor = new LinearTaskExecutor();

        private Action _hideAndShow;

        public override Widget CreateConfigWidget()
        {
            
            var container = new VBox();


           
            // Landscape type


            var landscapeHBox = new HBox();
            container.Add(landscapeHBox);
            Box.BoxChild wlandscapeHBox = (Box.BoxChild)container[landscapeHBox];
            wlandscapeHBox.Expand = false;
            wlandscapeHBox.Fill = false;

            var landscapeLabel = new Label { Text = "Landscape type" };
            landscapeHBox.Add(landscapeLabel);

            var modes = Enum.GetNames(typeof(LandscapeMode));
            var landscapeModeCombo = new ComboBox(modes) { Active = 0 };
            landscapeHBox.Add(landscapeModeCombo);
            Box.BoxChild wlandscapeModeCombo = (Box.BoxChild)landscapeHBox[landscapeModeCombo];
            wlandscapeModeCombo.Expand = false;
            wlandscapeModeCombo.Fill = false;

            //Nb Dimensions / sample number

            var nbDimensionsHBox = new HBox();
            container.Add(nbDimensionsHBox);
            Box.BoxChild wnbDimensionsHBox = (Box.BoxChild)container[nbDimensionsHBox];
            wnbDimensionsHBox.Expand = false;
            wnbDimensionsHBox.Fill = false;

            var nbDimensionsLabel = new Label { Text = "Nb of gene dimensions" };
            nbDimensionsHBox.Add(nbDimensionsLabel);

            var nbDimensionsButton = new SpinButton(2, 10000, 1) { Digits = 0 };
            nbDimensionsButton.ValueChanged += delegate
            {
                mNbDimensions = nbDimensionsButton.ValueAsInt;
                _hideAndShow();
            };
            nbDimensionsHBox.Add(nbDimensionsButton);
            Box.BoxChild wnbDimensionsButton = (Box.BoxChild)nbDimensionsHBox[nbDimensionsButton];
            wnbDimensionsButton.Expand = false;
            wnbDimensionsButton.Fill = false;

            var nbSamplesLabel = new Label { Text = "Nb of samples" };
            nbDimensionsHBox.Add(nbSamplesLabel);

            var nbSamplesButton = new SpinButton(1, 10000, 1) { Digits = 0 };
            nbSamplesButton.Value = mNbSamples;
            nbSamplesButton.ValueChanged += delegate
            {
                mNbSamples = nbDimensionsButton.ValueAsInt;
            };
            nbDimensionsHBox.Add(nbSamplesButton);
            Box.BoxChild wnbSamplesButton = (Box.BoxChild)nbDimensionsHBox[nbSamplesButton];
            wnbSamplesButton.Expand = false;
            wnbSamplesButton.Fill = false;



            //Height maps


            var heightMapHBox = new HBox();
            container.Add(heightMapHBox);
            Box.BoxChild wheightMapHBox = (Box.BoxChild)container[heightMapHBox];
            wheightMapHBox.Expand = false;
            wheightMapHBox.Fill = false;

            var heightMapHLabel = new Label { Text = "HeightMap to search" };
            heightMapHBox.Add(heightMapHLabel);
            var knownHeightMaps = Enum.GetNames(typeof(KnownHeightMap));

            // Choosing function to display
            var heightMapCombo = new ComboBox(knownHeightMaps) { Active = 0 };

            heightMapHBox.Add(heightMapCombo);

            void ConfigureHeightMap()
            {
                Enum.TryParse(heightMapCombo.ActiveText, out KnownHeightMap selectedHeightMap);
                var imageFunction = new ImageHeightMapFunction();
                switch (selectedHeightMap)
                {
                    case KnownHeightMap.EverestMount:
                        imageFunction.TargetImage = Properties.Resources.EverestMount;
                        break;
                    case KnownHeightMap.NepalBhoutan:
                        imageFunction.TargetImage = Properties.Resources.NepalBhoutan;
                        break;
                    case KnownHeightMap.TibetanPlateau:
                        imageFunction.TargetImage = Properties.Resources.TibetanPlateau;
                        break;
                    case KnownHeightMap.World:
                        imageFunction.TargetImage = Properties.Resources.World;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                mRange.xRange.min = 0;
                mRange.xRange.max = imageFunction.TargetImage.Width - 1;
                mRange.yRange.min = 0;
                mRange.yRange.max = imageFunction.TargetImage.Height - 1;

                mTargetFunction = imageFunction;
                
            }

            heightMapCombo.Changed += delegate
            {
                ConfigureHeightMap();
                Replot();
            };

            heightMapHBox.Add(heightMapCombo);

            // Custom image

            var imageHBox = new HBox();
            container.Add(imageHBox);
            Box.BoxChild wimageHBox = (Box.BoxChild)container[imageHBox];
            wimageHBox.Expand = false;
            wimageHBox.Fill = false;
            // Height map image picker
            var selectImageButton = new Button {Label = "Load an image"};
            selectImageButton.Clicked += delegate
            {
                Gtk.FileChooserDialog filechooser =
                    new Gtk.FileChooserDialog(
                        "Select image to use as an height map",
                        Context.GtkWindow,
                        FileChooserAction.Open,
                        "Cancel",
                        ResponseType.Cancel,
                        "Open",
                        ResponseType.Accept);

                if (filechooser.Run() == (int)ResponseType.Accept)
                {
                    var heightMapImage = System.Drawing.Image.FromFile(filechooser.Filename) as Bitmap;
                    var imageFunction = new ImageHeightMapFunction(){TargetImage = heightMapImage};
                    mTargetFunction = imageFunction;
                    mRange.xRange.min = 0;
                    mRange.xRange.max = heightMapImage.Width - 1;
                    mRange.yRange.min = 0;
                    mRange.yRange.max = heightMapImage.Height - 1;
                    Replot();
                }

                filechooser.Destroy();

                Replot();
            };
            imageHBox.Add(selectImageButton);

            //Known functions

            var functionHBox = new HBox();
            container.Add(functionHBox);
            Box.BoxChild wfunctionHBox = (Box.BoxChild)container[functionHBox];
            wfunctionHBox.Expand = false;
            wfunctionHBox.Fill = false;

            var functionsLabel = new Label {Text = "Function to search"};
            functionHBox.Add(functionsLabel);
            var knownFunctions = KnownFunctions.GetKnownFunctions();

            var functionList = knownFunctions.Select(m => m.Key).ToArray();

            // Choosing function to display
            var functionCombo = new ComboBox(functionList) {Active = 0};
            mTargetFunction = knownFunctions[functionCombo.ActiveText];
            mRange = ((mTargetFunction.Ranges(2)[0], mTargetFunction.Ranges(2)[1]));
           
            functionHBox.Add(functionCombo);

            _hideAndShow = delegate
            {

                Enum.TryParse(landscapeModeCombo.ActiveText, out LandscapeMode selectedMode);
                switch (selectedMode)
                {
                    case LandscapeMode.KnownFunction:
                        functionHBox.ShowAll();
                        imageHBox.HideAll();
                        heightMapHBox.HideAll();
                        ConfigureFunction();
                        break;
                    case LandscapeMode.KnownHeightMap:
                        heightMapHBox.ShowAll();
                        functionHBox.HideAll();
                        imageHBox.HideAll();
                        ConfigureHeightMap();
                        break;
                    case LandscapeMode.CustomImage:
                        imageHBox.ShowAll();
                        functionHBox.HideAll();
                        heightMapHBox.HideAll();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                var isMultidimensional = mNbDimensions > 2;
                nbSamplesButton.Visible = isMultidimensional;
                nbSamplesLabel.Visible = isMultidimensional;

            };

            landscapeModeCombo.Changed += delegate
            {

                _hideAndShow();
                Replot();
            };


            //Range
           

            var rangeHBox = new HBox();
            container.Add(rangeHBox);
            Box.BoxChild wrangeHBox = (Box.BoxChild)container[rangeHBox];
            wrangeHBox.Expand = false;
            wrangeHBox.Fill = false;
            var rangeLabel = new Label { Text = "Search Range" };
            rangeHBox.Add(rangeLabel);

            var rangeVBox = new VBox();
            rangeHBox.Add(rangeVBox);

            var xHBox = new HBox();
            rangeVBox.Add(xHBox);


            var xMinLabel = new Label { Text = "Xmin" };
            xHBox.Add(xMinLabel);
            _xMin = new SpinButton(-10000, 10000, 1) { Digits = 2};
            _xMin.ValueChanged += delegate
            {
                mRange.xRange.min = _xMin.Value;
            };
            xHBox.Add(_xMin);
            var xMaxLabel = new Label { Text = "Xmax" };
            xHBox.Add(xMaxLabel);
            _xMax = new SpinButton(-10000, 10000, 1) { Digits = 2 };
            _xMax.ValueChanged += delegate
            {
                mRange.xRange.max = _xMax.Value;
            };
            xHBox.Add(_xMax);

            var yHBox = new HBox();
            rangeVBox.Add(yHBox);

            var yMinLabel = new Label { Text = "Ymin" };
            yHBox.Add(yMinLabel);
            _yMin = new SpinButton(-10000, 10000, 1) { Digits = 2 };
            _yMin.ValueChanged += delegate
            {
                mRange.yRange.min = _yMin.Value;
            };
            yHBox.Add(_yMin);
            var yMaxLabel = new Label { Text = "Ymax" };
            yHBox.Add(yMaxLabel);
            _yMax = new SpinButton(-10000, 10000, 1) { Digits = 2 };
            _yMax.ValueChanged += delegate
            {
                mRange.yRange.max = _yMax.Value;
            };
            yHBox.Add(_yMax);


            void ConfigureFunction()
            {
                //var method = knownFunctions[functionCombo.ActiveText];
                //mTargetFunction = doubles => (double) method.Invoke(null, new[] {doubles});
                mTargetFunction = knownFunctions[functionCombo.ActiveText];
                //mRange = ((mTargetFunction.Ranges(2)[0], mTargetFunction.Ranges(2)[1]));
                mRange.xRange.min = mTargetFunction.Ranges(2)[0].min;
                mRange.xRange.max = mTargetFunction.Ranges(2)[0].max;
                mRange.yRange.min = mTargetFunction.Ranges(2)[1].min;
                mRange.yRange.max = mTargetFunction.Ranges(2)[1].max;
                
            }



            functionCombo.Changed += delegate
            {
                ConfigureFunction();
                Replot();
            };

            //Replot button

            var replotButton = new Button { Label = "Replot Landscape" };
            replotButton.Clicked += delegate
            {
                Replot();
            };
            container.Add(replotButton);
            Box.BoxChild wreplotButton = (Box.BoxChild)container[replotButton];
            wreplotButton.Expand = false;
            wreplotButton.Fill = false;


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
            var ranges = new List<(double min, double max)>(new[] {
                (mRange.xRange.min, mRange.xRange.max),
                (mRange.yRange.min,mRange.yRange.max)});
            for (int i = 2; i < mNbDimensions; i++)
            {
                ranges.Add((mRange.xRange.min, mRange.xRange.max));
            }

            var toReturn = new EquationChromosome<double>(mNbDimensions) {
                Ranges = ranges};
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
            _hideAndShow();
            _xMin.Value = mRange.xRange.min;
            _xMax.Value = mRange.xRange.max;
            _yMin.Value = mRange.yRange.min;
            _yMax.Value = mRange.yRange.max;
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



        private bool _plotting;
        private int samplingNb = 10;

        private void PlotFunction()
        {

            if (mTargetFunction != null)
            {

                if (_fBitmap == null)
                {

                    Context.WriteText("Plotting Landscape, please wait...");
                    if (_plotting) return;
                    _plotting = true;
                    _taskExecutor.Add(() =>
                    {
                        _fBitmap = BuildBitmap();
                        _plotting = false;
                        OnReconfigured();
                        _taskExecutor.Stop();
                        _taskExecutor.Clear();
                    });
                    _taskExecutor.Start();
                }
                else
                {

                    if (Context.Population != null)
                    {
                        using (var displayBitmap = _fBitmap.Clone())
                        {
                            PlotPopulation(displayBitmap);
                            Context.WriteText($"Max Value: {_maxPoint.fValue}");
                            Context.WriteText($"Min Value: {_minPoint.fValue}");
                            DrawBitmap(displayBitmap.Bitmap);
                        }

                    }
                    else
                    {
                        DrawBitmap(_fBitmap.Bitmap);
                    }
                }
            }
        }


        private void Replot()
        {
            _fBitmap = null;
            OnReconfigured();
        }


        private void PlotPopulation(DirectBitmap image)
        {

           

            foreach (var chromosome in Context.Population.CurrentGeneration.Chromosomes.Cast<EquationChromosome<double>>())
            {
                
                (double x, double y) = ((double)chromosome.GetGene(0).Value,
                    (double)chromosome.GetGene(1).Value);
                var cFitness = chromosome.Fitness; 
                var (xDraw, yDraw) = GetDrawingCoords(x, y);
                DrawFunctionPoint(image, xDraw, yDraw, indColor);

            }

            var best = (EquationChromosome<double>) Context.Population.CurrentGeneration.BestChromosome;
            if (best!=null)
            {
                (double xBest, double yBest) = ((double)best.GetGene(0).Value,
                    (double)best.GetGene(1).Value);
                var (xDraw, yDraw) = GetDrawingCoords(xBest, yBest);
                DrawFunctionPoint(image, xDraw, yDraw, bestColor);
                //layout.SetMarkup($"<span color='black'>({xBest},{yBest}){best.Fitness}</span>");
                //buffer.DrawLayout(gc, Context.DrawingArea.X + positionBest.xDraw, Context.DrawingArea.Y + positionBest.yDraw, layout);    
            }
        }

        private void DrawFunctionPoint(DirectBitmap image, int xDraw, int yDraw, Color indColor)
        {
            for (int i = -2; i < 3; i++)
            {
                for (int j = -2; j < 3; j++)
                {

                    var xDrawi = xDraw + i;
                    var yDrawj = yDraw + j;
                    if ((Math.Abs(i) + Math.Abs(j)) < 4 && xDrawi >= 0 && xDrawi < image.Width && yDrawj >= 0 && yDrawj < image.Height)
                    {
                        image.SetPixel(xDrawi, yDrawj, indColor);
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
            var (r, g, b) = (hue, sat, val).HsvToRgb();
            return Color.FromArgb((int)(r * 255), (int)(g * 255), (int)(b * 255));
        }



        /// <summary>
        /// Builds the bitmap from genes.
        /// </summary>
        /// <returns>The bitmap.</returns>
        public DirectBitmap BuildBitmap()
        {
            var width = Context.DrawingArea.Width; 
            var height = Context.DrawingArea.Height;
            var result = new DirectBitmap(width, height);
            var fValues = ComputeFunctionValues();
            foreach (var pixelValue in fValues)
            {
                var color = GetColor(pixelValue.Value, _minPoint.fValue, _maxPoint.fValue);
                result.SetPixel(pixelValue.Key.xDraw, pixelValue.Key.yDraw, color );
            }

            DrawFunctionPoint(result,_minPoint.position.xDraw, _minPoint.position.yDraw, Color.White);
            DrawFunctionPoint(result, _maxPoint.position.xDraw, _maxPoint.position.yDraw, Color.Black);
          

            return result;
        }


        /// <summary>
        /// Computes the currently selected function value for every pixel to draw
        /// </summary>
        private Dictionary<(int xDraw, int yDraw), double> ComputeFunctionValues()
        {
            var fValues = new Dictionary<(int xDraw, int yDraw), double>();
            var r = Context.DrawingArea;
            var width = Context.DrawingArea.Width;
            var height = Context.DrawingArea.Height;
            ((int xDraw, int yDraw), double fValue) minPoint = ((0, 0), double.MaxValue);
            ((int xDraw, int yDraw), double fValue) maxPoint = ((0, 0), double.MinValue);

            for (int xDraw = 0; xDraw < width; xDraw++)
            {
                for (int yDraw = 0; yDraw < height; yDraw++)
                {
                    var (x, y) = GetRealCoords(xDraw, yDraw);
                    var fValue = GetFunctionValue(x, y);
                    fValues.Add((xDraw, yDraw), fValue);
                    if (fValue > maxPoint.fValue)
                    {
                        maxPoint = ((xDraw, yDraw), fValue);
                    }
                    if (fValue < minPoint.fValue)
                    {
                        minPoint = ((xDraw, yDraw), fValue);
                    }
                }
            }

            if (maxPoint.fValue <= minPoint.fValue)
            {
                throw new InvalidOperationException("function has FMax <= FMin");
            }

            _maxPoint = maxPoint;
            _minPoint = minPoint;
            return fValues;
        }

        private double GetFunctionValue(double x, double y)
        {

            double fValue;
            if (mNbDimensions == 2)
            {
                fValue = ComputeFunctionValue(new[] { x, y });
            }
            else
            {
                fValue = double.MinValue;
                var rnd = RandomizationProvider.Current;
                var sampleCoords = new double[mNbDimensions];
                sampleCoords[0] =x;
                sampleCoords[1] = y;
                var coordsRange = mRange.xRange.max - mRange.xRange.min;
                for (int i = 0; i < samplingNb; i++)
                {
                    for (int extraCoord = 2; extraCoord < mNbDimensions; extraCoord++)
                    {
                        var coord = this.mRange.xRange.min + rnd.GetDouble() * coordsRange;
                        sampleCoords[extraCoord] = coord;
                    }
                    fValue = Math.Max(fValue, ComputeFunctionValue(sampleCoords));
                }
            }


            if (double.IsNaN(fValue))
            {
                throw new InvalidOperationException("landscape function returned a NaN value for a landscape point");
            }

            return fValue;
        }

        private double ComputeFunctionValue(double[] inputCoords)
        {
            return mTargetFunction.Fitness(mTargetFunction.Function(inputCoords));
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

            //pb = pb.ScaleSimple(width, height, Gdk.InterpType.Nearest);
            buffer.DrawPixbuf(gc, pb, 0, 0, 0, 100, width, height, Gdk.RgbDither.None, 0, 0);

        }
    }
}