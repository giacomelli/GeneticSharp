//using System.ComponentModel;
//using System.Drawing;
//using GeneticSharp.Extensions;

//namespace GeneticSharp.Runner.MauiApp.Samples
//{
//    [DisplayName("Bitmap equality")]
//    public class BitmapEqualitySampleController : SampleControllerBase
//    {
//        #region Fields
//        private BitmapEqualityFitness _fitness = new BitmapEqualityFitness();
//        //// private IChromosome _lastBest;
//        private string _destFolder;
//        private double _resolution = 1;
//        private Bitmap _targetBitmap;
//        private Label _resolutionLabel;
//        #endregion

//        #region Methods
//        public override IChromosome CreateChromosome()
//        {
//            return new BitmapChromosome(_fitness.BitmapWidth, _fitness.BitmapHeight);
//        }

//        //public override Widget CreateConfigWidget()
//        //{
//        //    var container = new VBox();
//        //    var selectImageButton = new Button();
//        //    selectImageButton.Label = "Select the image";
//        //    selectImageButton.Clicked += delegate
//        //    {
//        //        Gtk.FileChooserDialog filechooser =
//        //        new Gtk.FileChooserDialog(
//        //            "Select the image to use",
//        //        Context.GtkWindow,
//        //        FileChooserAction.Open,
//        //        "Cancel",
//        //        ResponseType.Cancel,
//        //        "Open",
//        //        ResponseType.Accept);

//        //        if (filechooser.Run() == (int)ResponseType.Accept)
//        //        {
//        //            _targetBitmap = Bitmap.FromFile(filechooser.Filename) as Bitmap;
//        //            InitializeFitness();

//        //            var folder = Path.Combine(Path.GetDirectoryName(filechooser.Filename), "results");
//        //            _destFolder = "{0}_{1:yyyyMMdd_HHmmss}".With(folder, DateTime.Now);
//        //            Directory.CreateDirectory(_destFolder);
//        //        }

//        //        filechooser.Destroy();

//        //        OnReconfigured();
//        //    };
//        //    container.Add(selectImageButton);

//        //    // Resolution.
//        //    _resolutionLabel = new Label();
//        //    _resolutionLabel.Text = "Resolution";
//        //    container.Add(_resolutionLabel);

//        //    var resolutionButton = new SpinButton(0.01, 1, 0.01);
//        //    resolutionButton.Value = 100;
//        //    resolutionButton.ValueChanged += delegate
//        //    {
//        //        _resolution = resolutionButton.Value;
//        //        InitializeFitness();

//        //        OnReconfigured();
//        //    };
//        //    container.Add(resolutionButton);

//        //    return container;
//        //}

//        public override ICrossover CreateCrossover()
//        {
//            return new UniformCrossover();
//        }

//        public override IFitness CreateFitness()
//        {
//            return _fitness;
//        }

//        public override IMutation CreateMutation()
//        {
//            return new TworsMutation();
//        }

//        public override ISelection CreateSelection()
//        {
//            return new EliteSelection();
//        }

//        public override void ConfigGA(GeneticAlgorithm ga)
//        {
//            ga.TaskExecutor = new ParallelTaskExecutor();
//            base.ConfigGA(ga);
//        }

//        public override void Draw()
//        {
//            var ga = Context.GA;

//            if (ga != null)
//            {
//                var generationsNumber = ga.GenerationsNumber;
//                var bestChromosome = ga.BestChromosome as BitmapChromosome;

//                //// if (generationsNumber == 1 || (generationsNumber % 200 == 0 && _lastBest.Fitness != bestChromosome.Fitness))
//                if (bestChromosome != null)
//                {
//                    var buffer = Context.Buffer;
//                    var gc = Context.GC;
//                    var layout = Context.Layout;

//                    using (var bitmap = bestChromosome.BuildBitmap())
//                    {
//                        //// bitmap.Save("{0}/{1}_{2}.png".With(_destFolder, generationsNumber.ToString("D10"), best.Fitness));

//                        using (var ms = new MemoryStream())
//                        {

//                            bitmap.Save(ms, ImageFormat.Png);
//                            var imageBytes = ms.ToArray();
//                            var pb = new Gdk.Pixbuf(imageBytes);
//                            var width = Context.DrawingArea.Width;
//                            var height = Context.DrawingArea.Height;

//                            pb = pb.ScaleSimple(width, height, Gdk.InterpType.Nearest);
//                            buffer.DrawPixbuf(gc, pb, 0, 0, 0, 100, width, height,  Gdk.RgbDither.None, 0, 0);
//                        }
//                    }

//                    //// _lastBest = best;
//                }
//            }
//        }

//        public override void Reset()
//        {
//            ////var targetBitmap = Bitmap.FromFile(inputImageFile) as Bitmap;
//            ////_fitness = new BitmapEqualityFitness(targetBitmap);

//            ////var folder = Path.Combine(Path.GetDirectoryName(inputImageFile), "results");
//            ////_destFolder = "{0}_{1:yyyyMMdd_HHmmss}".With(folder, DateTime.Now);
//            ////Directory.CreateDirectory(_destFolder);
//            ////Console.WriteLine("Results images will be written to '{0}'.", _destFolder);

//            ////Console.WriteLine("Minutes to evolve:");
//            ////_minutesToEvolve = Convert.ToInt32(Console.ReadLine());
//        }

//        public override void Update()
//        {
//        }

//        private void InitializeFitness()
//        {
//            if (_targetBitmap != null)
//            {
//                var resolutionSize = new Size(Convert.ToInt32(_targetBitmap.Width * _resolution), Convert.ToInt32(_targetBitmap.Height * _resolution));
//                var resizedBitmap = new Bitmap(_targetBitmap, resolutionSize);
//                _fitness.Initialize(resizedBitmap);
//            }

//            Application.Invoke(delegate
//            {
//                _resolutionLabel.Text = "Resolution {0}x{1}".With(_fitness.BitmapWidth, _fitness.BitmapHeight);
//            });
//        }
//        #endregion
//    }
//}
